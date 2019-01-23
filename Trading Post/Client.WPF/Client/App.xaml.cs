// <copyright file="App.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using DarkBond.Navigation;
    using DarkBond.ServiceModel;
    using DarkBond.TradingPost.Common.Strings;
    using DarkBond.TradingPost.Views;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;
    using DarkBond.Views;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
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
        /// Used to authenticate the client.
        /// </summary>
        private PublicClientApplication publicClientApplication;

        /// <summary>
        /// The scopes used for logging in.
        /// </summary>
        private static string[] apiScopes = { "https://fabrikamb2c.onmicrosoft.com/demoapi/demo.read" };

        /// <summary>
        /// The composition context.
        /// </summary>
        private CompositionHost compositionHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Configuring IOC requires excessive complexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Configuring IOC requires excessive coupling")]
        public App()
        {
            string BaseAuthority = "https://login.microsoftonline.com/tfp/{tenant}/{policy}/oauth2/v2.0/authorize";
            string Authority = BaseAuthority.Replace("{tenant}", "darkbonddevelopment.onmicrosoft.com").Replace("{policy}", SignInPolicy);

            // This provides a context for authenticating the user.
            this.publicClientApplication = new PublicClientApplication(App.ClientId, Authority, TokenCacheHelper.GetTokenCache());

            // Get assemblies that will be providing imports and exports
            var assemblies = new Assembly[]
            {
                typeof(ShellView).GetTypeInfo().Assembly,
                typeof(ClientSecurityToken).GetTypeInfo().Assembly,
                typeof(NavigationService).GetTypeInfo().Assembly
            };

            // This provides the conventions for exporting and importing types.
            ConventionBuilder conventions = new ConventionBuilder();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("View", StringComparison.Ordinal)).Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("ViewModel", StringComparison.Ordinal)).Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("Repository", StringComparison.Ordinal)).Export();
            conventions.ForType<ClientSecurityToken>().Shared().Export();
            conventions.ForType<NavigationService>().Shared().Export((ecb) => ecb.AsContractType<INavigationService>());

            // Create the composition container used by this application.
            ContainerConfiguration containerConfigurartion = new ContainerConfiguration();
            containerConfigurartion.WithAssemblies(assemblies, conventions);
            this.compositionHost = containerConfigurartion.CreateContainer();

            // These commands are handled by the application.
            GlobalCommands.SignIn.RegisterCommand(new DelegateCommand(this.SignIn));
        }

        /// <summary>
        /// Gets the application root URI.
        /// </summary>
        private static Uri ApplicationUri
        {
            get
            {
                // Build the root URI from the frame root and the name of the application.
                UriBuilder uriBuilder = new UriBuilder(new Uri(DarkBond.TradingPost.Properties.Resources.FrameUri));
                uriBuilder.Query = @"path=\" + Labels.ApplicationName;
                return uriBuilder.Uri;
            }
        }

        /// <inheritdoc/>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Provide a place to dump the unhandled exception messages.
            Application.Current.DispatcherUnhandledException += UnhandledExceptionHandler;

            // Load or create the frame for the application.  If this is the first time the application has been loaded, then we'll initialize the
            // frame.  If we're waking from a suspension then we'll use the existing content.  Create a Frame to act as the navigation context and
            // navigate to the first page
            ShellView shellView = this.compositionHost.GetExport<ShellView>();
            Application.Current.MainWindow = shellView;
            Application.Current.MainWindow.Visibility = Visibility.Visible;

            // Always log back in when coming back from a suspended state.
            await this.GetSecurityToken();

            // Install a handler for navigation exceptions.  This generally happens when an item in the navigation journal has been deleted and the
            // user attempts to use the 'Go Back' operation and encounters the deleted item.
            INavigationService navigationService = this.compositionHost.GetExport<INavigationService>();
            navigationService.NavigationFailed += OnNavigationFailed;

            // This will navigate to the main application directory after the user is signed in.
            // shellView.ViewModel.Source = App.ApplicationUri;

            // Ensure the current window is active
            Application.Current.MainWindow.Activate();
        }

        /// <summary>
        /// Get the token used to authenticate the user.
        /// </summary>
        private async Task GetSecurityToken()
        {
            // The end result of this method is to find a security token and put it here where any web method can use it for authentication.
            ClientSecurityToken clientSecurityToken = this.compositionHost.GetExport<ClientSecurityToken>();

            // This indicates we got a security token from the cache or user.
            bool success = false;

            // Attempt to sign in using the cached credentials.
            try
            {
                // If the user has a token cached with any policy, we'll display them as signed-in.
                AuthenticationResult result = await this.publicClientApplication.AcquireTokenSilentAsync(
                    App.apiScopes,
                    this.GetUserByPolicy(this.publicClientApplication.Users, App.SignInPolicy),
                    App.Authority,
                    false);

                // Use this as the security token when communicating with the service.
                clientSecurityToken.Value = result.IdToken;
                success = true;
            }
            catch (MsalException)
            {
            }

            // If the attempt to use an existing token fails, then prompt the user for their credentials.
            if (!success)
            {
                try
                {
                    // Get a security token from the active directory.  This token will be passed to the server to authenticate the users.  The
                    // advantage to working with a token is that this application ever needs to know about the user's credentials or store them in
                    // any way.
                    AuthenticationResult result = await this.publicClientApplication.AcquireTokenAsync(
                        App.apiScopes,
                        GetUserByPolicy(this.publicClientApplication.Users, App.SignInPolicy));

                    // Use this as the security token when communicating with the service.
                    clientSecurityToken.Value = result.IdToken;
                    success = true;
                }
                catch (MsalServiceException)
                {
                }
            }

            // Now that we have a security token, we can start the data model background reconciliation.  Alternatively, if the token can't be
            // acquired, this will gracefully shut down any existing processes.
            // DataModel dataModel = this.compositionHost.GetExport<DataModel>();
            // dataModel.IsReading = success;

            // If a security token can't be obtained, then exit the application.
            if (success)
            {
                // Indicate that new credentials are available.
                GlobalCommands.SignedIn.Execute(null);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Handles a failure to navigate to a destination.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="navigationFailedEventArgs">The event data.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs navigationFailedEventArgs)
        {
            // Let the user know that the location isn't available.  This generally occurs when an item has been displayed, is placed in the journal,
            // subsequently deleted and then a navigation operation takes us back to the deleted item.
            MessageBox.Show(navigationFailedEventArgs.Error.Message, Labels.ApplicationName, MessageBoxButton.OK);
        }

        /// <summary>
        /// Clears the cached credentials then forces the user to enter a new set of credentials.
        /// </summary>
        private async void SignIn()
        {
            // Clear the token cache and prompt for the credentials.
            TokenCacheHelper.Clear();
            await this.GetSecurityToken();
        }

        /// <summary>
        /// Handles an unhandled exception.
        /// </summary>
        /// <param name="sender">The object that originated the unhandled exception.</param>
        /// <param name="dispatcherUnhandledExceptionEventArgs">The unhandled exception data.</param>
        private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            // This handler isn't for the user; it's for the developer.  Some XAML exceptions can only be displayed here after the failure.
            Debug.WriteLine("Unhandled Exception Caught:");
            if (dispatcherUnhandledExceptionEventArgs.Exception.InnerException != null)
            {
                Debug.WriteLine(dispatcherUnhandledExceptionEventArgs.Exception.InnerException.Message);
            }
            else
            {
                Debug.WriteLine(dispatcherUnhandledExceptionEventArgs.Exception.Message);
            }
        }


        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower()))
                {
                    return user;
                }
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }
    }
}