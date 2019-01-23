// <copyright file="Program.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using ClientModel;
    using DarkBond.LicenseManager.Import.Properties;
    using ImportService;
    using LicenseManager;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Tool to load data into the data model.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The client id of the service application to which we want to connect.
        /// </summary>
#if PRODUCTION
        private const string ClientId = "0ca4a095-ec6d-4bed-afa8-6e3682d33158";
#elif STAGING
        private const string ClientId = "21d9374c-cbf9-466d-b3cc-599254c864fa";
#else
        private const string ClientId = "381ae805-9ca3-49d8-87a3-e2c4e6fe9729";
#endif

        /// <summary>
        /// The authority for authentication.
        /// </summary>
#if PRODUCTION
        private const string Authority = "https://login.microsoftonline.com/darkbondproduction.onmicrosoft.com";
#elif STAGING
        private const string Authority = "https://login.microsoftonline.com/darkbondstaging.onmicrosoft.com";
#else
        private const string Authority = "https://login.microsoftonline.com/darkbonddevelopment.onmicrosoft.com";
#endif

        /// <summary>
        /// The sign-in policy.
        /// </summary>
        private const string SignInPolicy = "B2C_1_Sign_In";

        /// <summary>
        /// Dictionary of command line parameter switches and the states they invoke in the parser.
        /// </summary>
        private static Dictionary<string, ArgumentState> argumentStates = new Dictionary<string, ArgumentState>()
        {
            { "-f", ArgumentState.ForceLoginParam },
            { "-i", ArgumentState.InputFileParam },
            { "-v", ArgumentState.VerbosityParam }
        };

        /// <summary>
        /// The composition host.
        /// </summary>
        private CompositionHost compositionHost;

        /// <summary>
        /// Indication that a login should be forced.
        /// </summary>
        private bool forceLogin;

        /// <summary>
        /// The file to load.
        /// </summary>
        private string fileNameField;

        /// <summary>
        /// Used to authenticate the client.
        /// </summary>
        private PublicClientApplication publicClientApplication;

        /// <summary>
        /// Indicates how much diagnostic information is output to the console.
        /// </summary>
        private Verbosity verbosityField;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The environment arguments.</param>
        /// <returns>0 to indicate success, otherwise an error code.</returns>
        [STAThread]
        private static int Main(string[] args)
        {
            Program program = new Program();
            return program.Run(args);
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The result of running the program.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Not possible to catch specific errors as we're using reflection.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Method used reflection to dynamically determine the executing method.")]
        private int Run(string[] args)
        {
            // This provides a context for authenticating the user.  ADAL implements an in-memory cache by default.  Since we want tokens to persist
            // when the user closes the app, we've extended the ADAL TokenCache and created a simple FileCache in this app.
            this.publicClientApplication = new PublicClientApplication(Program.ClientId)
            {
                UserTokenCache = new FileCache()
            };

            // Parse the operating parameters out of the command line.
            this.ParseCommandLine(args);

            // These are the assemblies that need to be registered with MEF.
            var assemblies = new Assembly[]
            {
                typeof(ClientSecurityToken).Assembly,
                typeof(EndpointIdentifier).Assembly
            };

            // This provides the conventions for exporting and importing types.
            ConventionBuilder conventions = new ConventionBuilder();
            conventions.ForType<ClientSecurityToken>().Export().Shared();
            conventions.ForType<ImporterHttpsEndpointAddress>().Shared().Export((ecb) => ecb.AsContractType<EndpointAddress>());
            conventions.ForType<ImporterHttpsBinding>().Shared().Export((ecb) => ecb.AsContractType<Binding>());
            conventions.ForType<Importer>().Export().Shared();

            // Create the IOC container host with the given assemblies and conventions.
            ContainerConfiguration containerConfigurartion = new ContainerConfiguration();
            containerConfigurartion.WithAssemblies(assemblies.ToArray(), conventions);
            this.compositionHost = containerConfigurartion.CreateContainer();

            // If the user changes their mind about logging in, then exit without executing the script.
            if (!this.SignIn())
            {
                return -1;
            }

            // This object will import a script into the data model service.
            Importer importer = this.compositionHost.GetExport<Importer>();

            try
            {
                // This will load the given file into the web service.
                importer.Load(this.fileNameField, this.verbosityField);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Trace.TraceError(fileNotFoundException.Message);
                importer.HasErrors = true;
            }
            catch (Exception exception)
            {
                // This will force an abnormal exit from the program.
                Trace.TraceError(exception.Message);
                importer.HasErrors = true;
            }

            // Write the final status of the load to the console.
            Console.WriteLine(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ExecutedMethodsMessage,
                    DateTime.Now.ToString("u", CultureInfo.CurrentCulture),
                    importer.ScriptName,
                    importer.MethodCount));

            // If an error happened anywhere, don't exit normally.
            if (importer.HasErrors)
            {
                return 1;
            }

            // If we reached here, the file was imported without issue.
            return 0;
        }

        /// <summary>
        /// Parse the operation parameters out of the command line.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private void ParseCommandLine(string[] args)
        {
            // These are the parameters that are parsed out of the command line.
            string fileName = null;
            Verbosity verbosity = Verbosity.Minimal;

            // The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has been parsed, the
            // command line parser assumes that it's reading the file name from the command line.
            ArgumentState argumentState = ArgumentState.InputFileName;

            // Parse the command line for arguments.
            foreach (string argument in args)
            {
                // Use the dictionary to transition from one state to the next based on the input parameters.
                ArgumentState nextArgumentState;
                if (Program.argumentStates.TryGetValue(argument, out nextArgumentState))
                {
                    argumentState = nextArgumentState;
                }

                // The parsing state will determine which variable is read next.
                switch (argumentState)
                {
                    case ArgumentState.ForceLoginParam:

                        // This will cause the script loader to prompt for login credentials.
                        this.forceLogin = true;
                        argumentState = ArgumentState.InputFileName;
                        break;

                    case ArgumentState.InputFileParam:

                        // The next command line argument will be the input file name.
                        argumentState = ArgumentState.InputFileName;
                        break;

                    case ArgumentState.InputFileName:

                        // Expand the environment variables so that paths don't need to be absolute.
                        fileName = Environment.ExpandEnvironmentVariables(argument);
                        break;

                    case ArgumentState.VerbosityParam:

                        // Verbose output.
                        argumentState = ArgumentState.Verbosity;
                        break;

                    case ArgumentState.Verbosity:

                        try
                        {
                            // Make sure any parsing errors are ignored.
                            verbosity = (Verbosity)Enum.Parse(typeof(Verbosity), argument);
                        }
                        catch (ArgumentNullException)
                        {
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (OverflowException)
                        {
                        }

                        break;

                    default:

                        // The parser will revert back to looking for an input file when it doesn't recognized the switch.
                        argumentState = ArgumentState.InputFileName;
                        break;
                }
            }

            // Throw a usage message back at the user if no file name was given.
            if (fileName == null)
            {
                Console.WriteLine(Resources.UsageMessage);
                Environment.Exit(-1);
            }

            // Make the rest of the operational parameters available to the class.
            this.verbosityField = verbosity;
            this.fileNameField = fileName;
        }

        /// <summary>
        /// Sign in to the server and start updating the data model.
        /// </summary>
        /// <returns>true indicates the user entered credentials, false if they cancelled.</returns>
        private bool SignIn()
        {
            // The end result of this method is to find a security token and put it here where any web method can use it for authentication.
            ClientSecurityToken clientSecurityToken = this.compositionHost.GetExport<ClientSecurityToken>();

            // Without the 'force login' parameter, we'll try to use cached tokens.
            if (!this.forceLogin)
            {
                try
                {
                    // If the user has has a token cached with any policy, we'll display them as signed-in.
                    AuthenticationResult result = this.publicClientApplication.AcquireTokenSilentAsync(
                        new string[] { Program.ClientId },
                        string.Empty,
                        Program.Authority,
                        Program.SignInPolicy,
                        false).Result;

                    // Use this as the security token when communicating with the service.
                    clientSecurityToken.Value = result.Token;
                    return true;
                }
                catch (AggregateException)
                {
                }
                catch (MsalServiceException)
                {
                }
            }

            try
            {
                // Get a security token from the active directory.  This token will be passed to the server to authenticate the users.  The advantage
                // to working with a token is that this application ever needs to know about the user's credentials or store them in any way.
                AuthenticationResult result = this.publicClientApplication.AcquireTokenAsync(
                    new string[] { Program.ClientId },
                    string.Empty,
                    UiOptions.ForceLogin,
                    null,
                    null,
                    Program.Authority,
                    Program.SignInPolicy).Result;

                // Use this as the security token when communicating with the service.
                clientSecurityToken.Value = result.Token;
                return true;
            }
            catch (MsalServiceException)
            {
            }

            // If we got here, we failed to authentica the user.
            return false;
        }
    }
}