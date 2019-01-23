// <copyright file="App.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager
{
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using DarkBond;
    using DarkBond.Navigation;
    using DarkBond.ServiceModel;
    using DarkBond.SubscriptionManager.ViewModels;
    using DarkBond.SubscriptionManager.Views;
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
        private const string ClientId = "2c2fb9e7-a056-4506-9638-ab5bf9e3ad2c";
#endif

        /// <summary>
        /// The authority for authentication.
        /// </summary>
#if PRODUCTION
        private const string Authority = "https://login.microsoftonline.com/tfp/darkbondofferingion.onmicrosoft.com/B2C_1_Sign_In/oauth2/v2.0/authorize";
#elif STAGING
        private const string Authority = "https://login.microsoftonline.com/tfp/darkbondstaging.onmicrosoft.com/B2C_1_Sign_In/oauth2/v2.0/authorize";
#else
        private const string Authority = "https://login.microsoftonline.com/tfp/darkbonddevelopment.onmicrosoft.com/B2C_1_Sign_In/oauth2/v2.0/authorize";
#endif

        /// <summary>
        /// The security scope.
        /// </summary>
        private static string[] scopes = new string[] { "https://darkbonddevelopment.onmicrosoft.com/api/user_impersonation" };

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
            this.publicClientApplication = new PublicClientApplication(App.ClientId, App.Authority);

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
            conventions.ForType<SubscriptionService>().Shared().Export((ecb) => ecb.AsContractType<ISubscriptionService>());
            conventions.ForType<DataModel>().Shared().Export((ecb) => ecb.AsContractType<DataModel>());

            // Create the composition container used by this application.
            ContainerConfiguration containerConfigurartion = new ContainerConfiguration();
            containerConfigurartion.WithAssemblies(assemblies, conventions);
            this.compositionHost = containerConfigurartion.CreateContainer();

            // This will handle the suspension of the application.
            this.Suspending += this.OnSuspending;

            // These commands are handled by the application.
            GlobalCommands.SignIn.RegisterCommand(DelegateCommand.FromAsyncHandler(this.SignInAsync));
        }

#pragma warning disable AvoidAsyncVoid
        /// <inheritdoc/>
        protected override async void OnLaunched(LaunchActivatedEventArgs launchActivatedEventArgs)
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
            await this.GetSecurityTokenAsync(false);

            // This will navigate to the main application directory after the user is signed in.
            UriBuilder uriBuilder = new UriBuilder(@"urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.FrameView");
            uriBuilder.Query = @"path=\" + Common.Strings.Resources.ApplicationName;
            shellView.ViewModel.Source = uriBuilder.Uri;

            // Ensure the current window is active
            Window.Current.Activate();
        }
#pragma warning restore AvoidAsyncVoid

        /// <summary>
        /// Get the token used to authenticate the user.
        /// </summary>
        /// <param name="isSilent">True if we should use persistence, false if a dialog should be displayed.</param>
        /// <returns>A task.</returns>
        private async Task GetSecurityTokenAsync(bool isSilent)
        {
            // The end result of this method is to find a security token and put it here where any web method can use it for authentication.
            ClientSecurityToken securityToken = this.compositionHost.GetExport<ClientSecurityToken>();

            // This indicates we got a security token from the cache or user.
            bool success = false;

            if (isSilent)
            {
                // Attempt to sign in using the cached credentials.
                try
                {
                    // If the user has has a token cached with any policy, we'll display them as signed-in.
                    AuthenticationResult result = await this.publicClientApplication.AcquireTokenSilentAsync(scopes, this.publicClientApplication.Users.FirstOrDefault());

                    // Use this as the security token when communicating with the service.
                    securityToken.Value = result.AccessToken;
                    success = true;
                }
                catch (MsalUiRequiredException)
                {
                }
            }

            // If the attempt to use an existing token fails, then prompt the user for their credentials.
            if (!success)
            {
                try
                {
                    // Get a security token from the active directory.  This token will be passed to the server to authenticate the users.  The
                    // advantage to working with a token is that this application ever needs to know about the user's credentials or store them in
                    // any way.
                    AuthenticationResult result = await this.publicClientApplication.AcquireTokenAsync(scopes);

                    // Use this as the security token when communicating with the service.
                    securityToken.Value = result.AccessToken;
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
        /// <returns>A task.</returns>
        private async Task SignInAsync()
        {
            // Clear the token cache and prompt for the credentials.
            await this.GetSecurityTokenAsync(false);
        }
    }
}