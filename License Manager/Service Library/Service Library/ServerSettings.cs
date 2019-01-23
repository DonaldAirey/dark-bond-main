// <copyright file="ServerSettings.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ServiceLibrary
{
    using DarkBond.LicenseManager.ServiceLibrary.Properties;
    using DarkBond.ServiceModel;

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
                return Settings.Default.Audience;
            }
        }

        /// <summary>
        /// Gets the ConnectionStringsSection data for the current application's default configuration.
        /// </summary>
        public string SqlConnectionString
        {
            get
            {
                return Settings.Default.SqlConnectionString;
            }
        }

        /// <summary>
        /// Gets the domain name.
        /// </summary>
        public string Domain
        {
            get
            {
                return Settings.Default.Domain;
            }
        }

        /// <summary>
        /// Gets the AppPrincipalId for the directory manager (used to query directory for group affiliations).
        /// </summary>
        public string DirectoryManagerAppPrincipalId
        {
            get
            {
                return Settings.Default.DirectoryManagerAppPrincipalId;
            }
        }

        /// <summary>
        /// Gets the secret (password) for the directory manager.
        /// </summary>
        public string DirectoryManagerSecret
        {
            get
            {
                return Settings.Default.DirectoryManagerSecret;
            }
        }

        /// <summary>
        /// Gets the telemetry instrumentation key.
        /// </summary>
        public string InstrumentationKey
        {
            get
            {
                return Settings.Default.InstrumentationKey;
            }
        }

        /// <summary>
        /// Gets the sign in policy (used to construct the metadata endpoint URL).
        /// </summary>
        public string SignInPolicy
        {
            get
            {
                return Settings.Default.SignInPolicy;
            }
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string TenantId
        {
            get
            {
                return Settings.Default.TenantId;
            }
        }

        /// <summary>
        /// Gets the service certificate's thumbprint
        /// </summary>
        public string Thumbprint
        {
            get
            {
                return Settings.Default.Thumbprint;
            }
        }
    }
}