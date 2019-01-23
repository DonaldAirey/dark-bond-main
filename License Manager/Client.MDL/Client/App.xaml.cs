// <copyright file="App.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
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
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
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
            // Initialize the IDE maintained resources.
            this.InitializeComponent();

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
            conventions.ForType<NavigationService>().Shared().Export((ecb) => ecb.AsContractType<INavigationService>());
            conventions.ForType<EventAggregator>().Shared().Export((ecb) => ecb.AsContractType<IEventAggregator>());
            conventions.ForType<BreadcrumbViewModel>().Shared();
            conventions.ForType<ClientSecurityToken>().Export().Shared();
            conventions.ForType<DataServiceHttpsEndpointAddress>().Shared().Export((ecb) => ecb.AsContractType<EndpointAddress>());
            conventions.ForType<DataServiceHttpsBinding>().Shared().Export((ecb) => ecb.AsContractType<Binding>());
            conventions.ForType<CommunicationExceptionHandler>().Shared().Export((ecb) => ecb.AsContractType<ICommunicationExceptionHandler>());
            conventions.ForType<LicenseService>().Shared().Export((ecb) => ecb.AsContractType<ILicenseService>());
            conventions.ForType<DataModel>().Shared().Export((ecb) => ecb.AsContractType<DataModel>());

            // Create the composition container used by this application.
            ContainerConfiguration containerConfigurartion = new ContainerConfiguration();
            containerConfigurartion.WithAssemblies(assemblies, conventions);
            this.compositionHost = containerConfigurartion.CreateContainer();

            // This will handle the suspension of the application.
            this.Suspending += this.OnSuspending;

            // These commands are handled by the application.
            GlobalCommands.SignIn.RegisterCommand(new DelegateCommand(this.SignIn));
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            // Load or create the frame for the application.  If this is the first time the application has been loaded, then we'll initialize the
            // frame.  If we're waking from a suspension then we'll use the existing content.
            ShellView shellView = Window.Current.Content as ShellView;
            if (shellView == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                shellView = this.compositionHost.GetExport<ShellView>();

                // Place the newly created frame in the current Window.
                Window.Current.Content = shellView;
            }

            // Always log back in when coming back from a suspended state.
            this.GetSecurityToken();

            // This will navigate to the main application directory after the user is signed in.
            UriBuilder uriBuilder = new UriBuilder(@"urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.FrameView");
            uriBuilder.Query = @"path=\" + Strings.Resources.ApplicationName;
            shellView.ViewModel.Source = uriBuilder.Uri;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Get the token used to authenticate the user.
        /// </summary>
        private async void GetSecurityToken()
        {
            // The end result of this method is to find a security token and put it here where any web method can use it for authentication.
            ClientSecurityToken securityToken = this.compositionHost.GetExport<ClientSecurityToken>();

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
                securityToken.Value = result.Token;
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
                    securityToken.Value = result.Token;
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
                Application.Current.Exit();
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved without knowing whether the application will be
        /// terminated or resumed with the contents of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            DataModel dataModel = this.compositionHost.GetExport<DataModel>();
            dataModel.IsReading = false;
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        /// <summary>
        /// Clears the cached credentials then forces the user to enter a new set of credentials.
        /// </summary>
        private void SignIn()
        {
            // Clear the token cache and prompt for the credentials.
            this.publicClientApplication.UserTokenCache.Clear(string.Empty);
            this.GetSecurityToken();
        }
    }
}