// <copyright file="Startup.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.OrderManagementSystem
{
    using System;
    using DarkBond.CoreModel;
    using DarkBond.ServiceModel;
    using DarkBond.TradingPost;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Used to initialize the web service.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Used to construct the metadata endpoint for querying the capabilities of the Azure B2C authentication.
        /// </summary>
        private const string AadInstance = "https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration?p={1}";

        /// <summary>
        /// Used for dependency injection.
        /// </summary>
        private IServiceCollection serviceCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The environment for the web host.</param>
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            // Build the configuration.
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = configurationBuilder.Build();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Add services to the container.
        /// </summary>
        /// <param name="serviceCollection">The service container.</param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            // This allows us to explicity create services.
            this.serviceCollection = serviceCollection;

            // Inject the dependencies.
            this.serviceCollection.AddMvc();
            this.serviceCollection.Configure<AzureAdB2COptions>(this.Configuration.GetSection("Authentication:AzureAdB2C"));
            this.serviceCollection.AddSingleton<IProductRepository, ProductRepository>();
            this.serviceCollection.AddSingleton<IPersistentStore, PersistentStore>();
            this.serviceCollection.AddSingleton<IRoleInfo, RoleInfo>();
            this.serviceCollection.AddSingleton<DataModel>();
            this.serviceCollection.AddSingleton<ClaimsTransformer>();

            // Configure the claims-based authentication.
            this.serviceCollection.AddAuthorization(
                options =>
                {
                    options.AddPolicy("CanDelete", policy => policy.RequireClaim(ClaimTypes.Delete));
                });
        }

        /// <summary>
        /// Configure the HTTP request pipeline.
        /// </summary>
        /// <param name="applicationBuilder">Not really sure what this is.</param>
        /// <param name="hostingEnvironment">The environment for the web service.</param>
        /// <param name="loggerFactory">The logging device factory.</param>
        /// <param name="serviceProvider">Used to resolve services.</param>
        public void Configure(
            IApplicationBuilder applicationBuilder,
            IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            // Create an output device for logging.
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add the OAuth 2.0 authentication to the service.
            IOptions<AzureAdB2COptions> azureAdB2COptions = serviceProvider.GetService<IOptions<AzureAdB2COptions>>();
            applicationBuilder.UseJwtBearerAuthentication(
                new JwtBearerOptions()
                {
                    MetadataAddress = string.Format(Startup.AadInstance, azureAdB2COptions.Value.Tenant, azureAdB2COptions.Value.DefaultPolicy),
                    Audience = azureAdB2COptions.Value.ClientId,
                });

            // Use the claims transformer to add claims based on the group.
            ClaimsTransformer claimsTransformer = serviceProvider.GetService<ClaimsTransformer>();
            applicationBuilder.UseClaimsTransformation(
                new ClaimsTransformationOptions
                {
                    Transformer = claimsTransformer
                });

            // Use MVC for implementing the API.
            applicationBuilder.UseMvc();
        }
    }
}