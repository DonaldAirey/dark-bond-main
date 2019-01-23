// <copyright file="Service.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ServiceLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.IdentityModel.Policy;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceProcess;
    using DarkBond.ServiceModel;
    using Microsoft.ApplicationInsights;
    using Properties;

    /// <summary>
    /// Windows Service that provides a shared, in-memory data model.
    /// </summary>
    public class Service : ServiceBase
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
        /// The telemetry client for trace messages.
        /// </summary>
        private TelemetryClient telemetryClient = new TelemetryClient();

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        protected override void OnStart(string[] args)
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

            // Domain should be "localhost" when running in development and should be the fully-qualified domain name of the cloud service when
            // running in the cloud.
            IServerSettings serverSettings = this.compositionHost.GetExport<IServerSettings>();
            string domain = serverSettings.Domain;

            // This provides an instrumentation key which basically tells the Azure Application Insight where to deposit the information.
            TelemetryClient telemetryClient = this.compositionHost.GetExport<TelemetryClient>();
            telemetryClient.InstrumentationKey = serverSettings.InstrumentationKey;

            // These are the public facing port designations for HTTP and TCP.
            int httpPort = 80;
            int httpsPort = 443;
            int tcpPort = 808;

            // These are the URLs for the services.
            string httpDataServiceUrl = "http://" + domain + ":" + httpPort + "/" + Service.ApplicationName + "/" + Service.DataServiceEndpointName;
            string httpImportServiceUrl = "http://" + domain + ":" + httpPort + "/" + Service.ApplicationName + "/" + Service.ImportServiceEndpointName;
            string httpsDataServiceUrl = "https://" + domain + ":" + httpsPort + "/" + Service.ApplicationName + "/" + Service.DataServiceEndpointName;
            string httpsImportServiceUrl = "https://" + domain + ":" + httpsPort + "/" + Service.ApplicationName + "/" + Service.ImportServiceEndpointName;
            string tcpDataServiceUrl = "net.tcp://" + domain + ":" + tcpPort + "/" + Service.ApplicationName + "/" + Service.DataServiceEndpointName;
            string tcpImportServiceUrl = "net.tcp://" + domain + ":" + tcpPort + "/" + Service.ApplicationName + "/" + Service.ImportServiceEndpointName;

            // These are the bindings that will be used for the endpoints.
            NetTcpBinding netTcpBinding = new NetTcpBinding();
            netTcpBinding.Security.Mode = SecurityMode.Transport;
            netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            NetHttpBinding netHttpsBinding = new NetHttpBinding();
            netHttpsBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            netHttpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();

            // Create the Data Model Service.
            this.dataHost = new ServiceHost(typeof(DataService), new Uri[] { new Uri(httpDataServiceUrl)});

            // Add the common behaviors to this host.
            this.AddBehaviors(this.dataHost);

            // A TCP endpoint for the Data Model Service.
            this.dataHost.AddServiceEndpoint(typeof(IDataService), netTcpBinding, tcpDataServiceUrl);

            // An HTTPS endpoint for the Data Model Service.
            this.dataHost.AddServiceEndpoint(typeof(IDataService), netHttpsBinding, httpsDataServiceUrl);

            // A MEX endpoint for the Data Model Service.
            this.dataHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, Service.MexEndpointName);

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
            this.importHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, Service.MexEndpointName);

            // The host(s) are now open for business.
            this.importHost.Open();
        }

        /// <summary>
        /// Called when the service is stopped.
        /// </summary>
        protected override void OnStop()
        {
            // Dispose of the services.
            ((IDisposable)this.dataHost).Dispose();
            ((IDisposable)this.importHost).Dispose();

            // Allow the base class to complete the shutdown process.
            base.OnStop();
        }

        /// <summary>
        /// The main entry point for the service.
        /// </summary>
        private static void Main()
        {
#if START_SERVICE
            // This will run the project as an executable rather than as a service when debugging.
            using (Service service = new Service())
            {
                service.OnStart(new string[] { });
                Console.WriteLine(Resources.HitAnyKey);
                Console.ReadKey();
                service.Stop();
            }
#else
            // This will run the Web Service as Windows Service.
            ServiceBase.Run(new ServiceBase[] { new Service() });
#endif
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

            // This behavior creates an IOC for the service.  Incoming requests at the endpoints will use MEF to resolve/create objects.
            serviceHost.Description.Behaviors.Add(this.compositionHost.GetExport<MefServiceBehavior>());
        }
    }
}