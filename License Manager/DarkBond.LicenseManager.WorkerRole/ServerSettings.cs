// <copyright file="ServerSettings.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// A wrapper around the service model settings.
    /// </summary>
    public class ServerSettings : IServerSettings
    {
        /// <summary>
        /// Gets the audience for token validation.
        /// </summary>
        public string Audience
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("Audience");
            }
        }

        /// <summary>
        /// Gets the domain name.
        /// </summary>
        public string Domain
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("Domain");
            }
        }

        /// <summary>
        /// Gets the AppPrincipalId for the directory manager (used to query directory for group affiliations).
        /// </summary>
        public string DirectoryManagerAppPrincipalId
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("DirectoryManagerAppPrincipalId");
            }
        }

        /// <summary>
        /// Gets the secret (password) for the directory manager.
        /// </summary>
        public string DirectoryManagerSecret
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("DirectoryManagerSecret");
            }
        }

        /// <summary>
        /// Gets the telemetry instrumentation key.
        /// </summary>
        public string InstrumentationKey
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
        }

        /// <summary>
        /// Gets the sign in policy (used to construct the metadata endpoint URL).
        /// </summary>
        public string SignInPolicy
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("SignInPolicy");
            }
        }

        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        public string SqlConnectionString
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("SqlConnectionString");
            }
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string TenantId
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("TenantId");
            }
        }

        /// <summary>
        /// Gets the service certificate's thumbprint
        /// </summary>
        public string Thumbprint
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("Thumbprint");
            }
        }
    }
}