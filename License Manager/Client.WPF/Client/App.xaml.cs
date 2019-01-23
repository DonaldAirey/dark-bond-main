// <copyright file="App.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Windows;
    using System.Windows.Threading;
    using DarkBond;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.ViewModels;
    using DarkBond.LicenseManager.Views;
    using DarkBond.Navigation;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Events;
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
            // This provides a context for authenticating the user.
            this.publicClientApplication = new PublicClientApplication(App.ClientId)
            {
                UserTokenCache = new FileCache()
            };

            // Get assemblies that will be providing imports and exports
            var assemblies = new Assembly[]
            {
                typeof(EventAggregator).GetTypeInfo().Assembly,
                typeof(ClientSecurityToken).GetTypeInfo().Assembly,
                typeof(FrameViewModel).GetTypeInfo().Assembly,
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(DataModel).GetTypeInfo().Assembly,
                typeof(FrameView).GetTypeInfo().Assembly,
                typeof(ShellView).GetTypeInfo().Assembly
            };

            // This provides the conventions for exporting and importing types.
            ConventionBuilder conventions = new ConventionBuilder();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("View", StringComparison.Ordinal)).Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("ViewModel", StringComparison.Ordinal)).Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("Repository", StringComparison.Ordinal)).Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("Collection", StringComparison.Ordinal)).Export();
            conventions.ForType<FrameViewModel>().Shared();
            conventions.ForType<FrameView>().Shared();
            conventions.ForType<BreadcrumbViewModel>().Shared();
            conventions.ForType<NavigationTreeViewModel>().Shared();
            conventions.ForType<ClientSecurityToken>().Shared().Export();
            conventions.ForType<NavigationService>().Shared().Export((ecb) => ecb.AsContractType<INavigationService>());
            conventions.ForType<EventAggregator>().Shared().Export((ecb) => ecb.AsContractType<IEventAggregator>());
            conventions.ForType<DataServiceHttpsEndpointAddress>().Shared().Export((ecb) => ecb.AsContractType<EndpointAddress>());
            conventions.ForType<DataServiceHttpsBinding>().Shared().Export((ecb) => ecb.AsContractType<Binding>());
            conventions.ForType<CommunicationExceptionHandler>().Shared().Export((ecb) => ecb.AsContractType<ICommunicationExceptionHandler>());
            conventions.ForType<LicenseService>().Shared().Export((ecb) => ecb.AsContractType<ILicenseService>());
            conventions.ForType<DataModel>().Shared().Export((ecb) => ecb.AsContractType<DataModel>());

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
                UriBuilder uriBuilder = new UriBuilder(new Uri(LicenseManager.Properties.Resources.FrameUri));
                uriBuilder.Query = @"path=\" + Strings.Resources.ApplicationName;
                return uriBuilder.Uri;
            }
        }

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
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
            this.GetSecurityToken();

            // Install a handler for navigation exceptions.  This generally happens when an item in the navigation journal has been deleted and the
            // user attempts to use the 'Go Back' operation and encounters the deleted item.
            INavigationService navigationService = this.compositionHost.GetExport<INavigationService>();
            navigationService.NavigationFailed += OnNavigationFailed;

            // This will navigate to the main application directory after the user is signed in.
            shellView.ViewModel.Source = App.ApplicationUri;

            // Ensure the current window is active
            Application.Current.MainWindow.Activate();
        }

        /// <summary>
        /// Get the token used to authenticate the user.
        /// </summary>
        private async void GetSecurityToken()
        {
            // The end result of this method is to find a security token and put it here where any web method can use it for authentication.
            ClientSecurityToken clientSecurityToken = this.compositionHost.GetExport<ClientSecurityToken>();

            // This indicates we got a security token from the cache or user.
            bool success = false;

            // Attempt to sign in using the cached credentials.
            try
            {
                // If the user has has a token cached with any policy, we'll display them as signed-in.
                AuthenticationResult result = await this.publicClientApplication.AcquireTokenSilentAsync(
                    new string[] { App.ClientId },
                    string.Empty,
                    App.Authority,
                    App.SignInPolicy,
                    false);

                // Use this as the security token when communicating with the service.
                clientSecurityToken.Value = result.Token;
                success = true;
            }
            catch (MsalSilentTokenAcquisitionException)
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
                        new string[] { App.ClientId },
                        string.Empty,
                        UiOptions.ForceLogin,
                        null,
                        null,
                        App.Authority,
                        App.SignInPolicy);

                    // Use this as the security token when communicating with the service.
                    clientSecurityToken.Value = result.Token;
                    success = true;
                }
                catch (MsalServiceException)
                {
                }
            }

            // Now that we have a security token, we can start the data model background reconcilliation.  Alternatively, if the token can't be
            // acquired, this will gracefully shut down any existing processes.
            DataModel dataModel = this.compositionHost.GetExport<DataModel>();
            dataModel.IsReading = success;

            // If a security token can't be obtained, then exit the application.
            if (!success)
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
            MessageBox.Show(
                navigationFailedEventArgs.Error.Message,
                Strings.Resources.ApplicationName,
                MessageBoxButton.OK);
        }

        /// <summary>
        /// Clears the cached credentials then forces the user to enter a new set of credentials.
        /// </summary>
        private void SignIn()
        {
            // Clear the token cache and prompt for the credentials.
            this.publicClientApplication.UserTokenCache.Clear(App.ClientId);
            this.GetSecurityToken();
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
    }
}