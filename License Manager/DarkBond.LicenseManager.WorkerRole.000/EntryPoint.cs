// <copyright file="EntryPoint.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Policy;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading;
    using System.Threading.Tasks;
    using DarkBond.LicenseManager.WorkerRole.Properties;
    using DarkBond.ServiceModel;
    using Microsoft.ApplicationInsights;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// License Manager Cloud Service Worker Role.
    /// </summary>
    public class EntryPoint : RoleEntryPoint, IDisposable
    {
        /// <summary>
        /// The application name.
        /// </summary>
        private const string ApplicationName = "license_manager";

        /// <summary>
        /// The name of the data service endpoint.
        /// </summary>
        private const string DataServiceEndpointName = "data_service";

        /// <summary>
        /// The name of the import endpoint.
        /// </summary>
        private const string ImportServiceEndpointName = "import_service";

        /// <summary>
        /// The name of the Metadata Exchange (MEX) endpoint.
        /// </summary>
        private const string MexEndpointName = "mex";

        /// <summary>
        /// Used to terminate the web service execution.
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Indicates that the execution of the service has finished.
        /// </summary>
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /// <summary>
        /// The composition host.
        /// </summary>
        private CompositionHost compositionHost;

        /// <summary>
        /// The data model service host.
        /// </summary>
        private ServiceHost dataHost = null;

        /// <summary>
        /// The import service host.
        /// </summary>
        private ServiceHost importHost = null;

        /// <summary>
        /// Finalizes an instance of the <see cref="EntryPoint"/> class.
        /// </summary>
        ~EntryPoint()
        {
            // Call the virtual method to dispose of the resources. This (recommended) design pattern gives any derived classes a chance to clean up
            // unmanaged resources even though this base class has none.
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the virtual method to allow derived classes to clean up resources.
            this.Dispose(true);

            // Since we took care of cleaning up the resources, there is no need to call the finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Runs code that is intended to be run for the life of the role instance.
        /// </summary>
        public override void Run()
        {
            try
            {
                // Loop until the cancellation token is raised.
                EntryPoint.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                // This signals that the worker role is finished.
                this.runCompleteEvent.Set();
            }
        }

        /// <summary>
        /// Runs code that initializes a role instance.
        /// </summary>
        /// <returns>true if initialization succeeds; otherwise, false. The default return value is true.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "MEF Requires large coupling")]
        public override bool OnStart()
        {
            // These are the assemblies that need to be registered with MEF.
            var assemblies = new Assembly[]
            {
                typeof(TelemetryClient).Assembly,
                typeof(ServerSettings).Assembly,
                typeof(MefServiceBehavior).Assembly,
                typeof(PersistentStore).Assembly,
                typeof(IAuthorizationPolicy).Assembly,
                typeof(RoleInfo).Assembly,
                typeof(DataModel).Assembly,
                typeof(DataService).Assembly,
                typeof(ImportService).Assembly
            };

            // This provides the conventions for exporting and importing types.
            ConventionBuilder conventions = new ConventionBuilder();
            conventions.ForType<ServerSettings>().Shared().Export<IServerSettings>();
            conventions.ForType<PersistentStore>().Shared().Export<IPersistentStore>();
            conventions.ForType<DataModel>().Shared().Export();
            conventions.ForType<RoleInfo>().Export<IRoleInfo>();
            conventions.ForType<ClaimsAuthorizationPolicy>().Shared().Export<IAuthorizationPolicy>();
            conventions.ForType<TelemetryClient>().SelectConstructor(pb => new TelemetryClient()).Shared().Export<TelemetryClient>();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("Service", StringComparison.Ordinal)).Shared().Export();
            conventions.ForTypesMatching((t) => t.FullName.EndsWith("Behavior", StringComparison.Ordinal)).Shared().Export();

            // Create the IOC container host with the given assemblies and conventions.
            ContainerConfiguration containerConfigurartion = new ContainerConfiguration();
            containerConfigurartion.WithAssemblies(assemblies.ToArray(), conventions);
            this.compositionHost = containerConfigurartion.CreateContainer();

            // Get the domain from the current configuration.
            IServerSettings serverSettings = this.compositionHost.GetExport<IServerSettings>();
            string domain = serverSettings.Domain;

            // This provides an instrumentation key which basically tells the Azure Application Insight where to deposit the information.
            TelemetryClient telemetryClient = this.compositionHost.GetExport<TelemetryClient>();
            telemetryClient.InstrumentationKey = serverSettings.InstrumentationKey;
            telemetryClient.TrackTrace(Resources.TelemetryInstalled);

            // These are the public facing port designations for HTTP and TCP.
            int httpPort = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WcfHttpPort"].IPEndpoint.Port;
            int httpsPort = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WcfHttpsPort"].IPEndpoint.Port;
            int tcpPort = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["WcfTcpPort"].IPEndpoint.Port;

            // These are the URLs for the services.
            string httpDataServiceUrl = "http://" + domain + ":" + httpPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.DataServiceEndpointName;
            string httpImportServiceUrl = "http://" + domain + ":" + httpPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.ImportServiceEndpointName;
            string httpsDataServiceUrl = "https://" + domain + ":" + httpsPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.DataServiceEndpointName;
            string httpsImportServiceUrl = "https://" + domain + ":" + httpsPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.ImportServiceEndpointName;
            string tcpDataServiceUrl = "net.tcp://" + domain + ":" + tcpPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.DataServiceEndpointName;
            string tcpImportServiceUrl = "net.tcp://" + domain + ":" + tcpPort + "/" + EntryPoint.ApplicationName + "/" + EntryPoint.ImportServiceEndpointName;

            // These are the bindings that will be used for the endpoints.
            NetTcpBinding netTcpBinding = new NetTcpBinding();
            netTcpBinding.Security.Mode = SecurityMode.Transport;
            netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            NetHttpBinding netHttpsBinding = new NetHttpBinding();
            netHttpsBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            netHttpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();

            // Create the Data Service.
            this.dataHost = new ServiceHost(typeof(DataService), new Uri[] { new Uri(httpDataServiceUrl) });

            // Add the common behaviors to this host.
            this.AddBehaviors(this.dataHost);

            // A TCP endpoint for the Data Service.
            this.dataHost.AddServiceEndpoint(typeof(IDataService), netTcpBinding, tcpDataServiceUrl);

            // An HTTPS endpoint for the Data Service.
            this.dataHost.AddServiceEndpoint(typeof(IDataService), netHttpsBinding, httpsDataServiceUrl);

            // A MEX endpoint for the Data Service.
            this.dataHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, EntryPoint.MexEndpointName);

            // The host(s) are now open for business.
            this.dataHost.Open();

            // Create the Import Service
            this.importHost = new ServiceHost(typeof(ImportService), new Uri[] { new Uri(httpImportServiceUrl) });

            // Add the common behaviors to this host.
            this.AddBehaviors(this.importHost);

            // A TCP endpoint for the Import Service.
            this.importHost.AddServiceEndpoint(typeof(IImportService), netTcpBinding, tcpImportServiceUrl);

            // A HTTPS endpoint for the Import Service.
            this.importHost.AddServiceEndpoint(typeof(IImportService), netHttpsBinding, httpsImportServiceUrl);

            // A MEX endpoint for the Import Service.
            this.importHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, EntryPoint.MexEndpointName);

            // The host(s) are now open for business.
            this.importHost.Open();

            // We're done starting the worker role.
            return true;
        }

        /// <summary>
        /// Runs code when a role instance is to be stopped.
        /// </summary>
        public override void OnStop()
        {
            // The close the hosts.
            this.dataHost.Close();
            this.importHost.Close();

            // Shutdown the background thread and wait for it to complete.
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            // Dispose of the services.
            ((IDisposable)this.dataHost).Dispose();
            ((IDisposable)this.importHost).Dispose();

            // Allow the base class to complete the shutdown process.
            base.OnStop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Dispose of the managed resources.
            this.cancellationTokenSource.Dispose();
            this.dataHost.Close();
            this.importHost.Close();
            this.runCompleteEvent.Dispose();
        }

        /// <summary>
        /// Run until the service is canceled.
        /// </summary>
        /// <param name="cancellationToken">Token used to address the running task.</param>
        /// <returns>The task running the service.</returns>
        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            // Wake up every ten minutes to look for a cancellation request.
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(600000);
            }
        }

        /// <summary>
        /// Add the desired behaviors to the given host.
        /// </summary>
        /// <param name="serviceHost">The host to which the behaviors apply.</param>
        private void AddBehaviors(ServiceHost serviceHost)
        {
            // This behavior will inspect the incoming messages for security tokens which are used to authenticate the user.
            serviceHost.Description.Behaviors.Add(this.compositionHost.GetExport<SecurityTokenServiceBehavior>());

            // Use a custom authorization policy to determine the rights of each user based on their role.
            List<IAuthorizationPolicy> authorizationPolicies = new List<IAuthorizationPolicy>();
            authorizationPolicies.Add(this.compositionHost.GetExport<IAuthorizationPolicy>());
            serviceHost.Authorization.ExternalAuthorizationPolicies = authorizationPolicies.AsReadOnly();

            // Use a custom authentication method to validate each user against an active directory.
            serviceHost.Description.Behaviors.Add(this.compositionHost.GetExport<CredentialsBehavior>());

            // This allows for the metadata to be exchanged, even when in production.
            serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            serviceHost.Description.Behaviors.Add(new UseRequestHeadersForMetadataAddressBehavior());

#if DEBUG
            // Allow the service to return debug information back only during development.
            ServiceDebugBehavior serviceDebugBehavior = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
#endif

            // This behavior creates an IOC for the service.  Incoming requests at the endpoints will use MEF to resolve/create objects.
            serviceHost.Description.Behaviors.Add(this.compositionHost.GetExport<MefServiceBehavior>());
        }
    }
}