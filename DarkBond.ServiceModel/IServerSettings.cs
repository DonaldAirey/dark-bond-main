// <copyright file="IServerSettings.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// A wrapper around the service model settings.
    /// </summary>
    public interface IServerSettings
    {
        /// <summary>
        /// Gets the audience for token validation.
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// Gets the AppPrincipalId for the directory manager (used to query directory for group affiliations).
        /// </summary>
        string DirectoryManagerAppPrincipalId { get; }

        /// <summary>
        /// Gets the secret (password) for the directory manager.
        /// </summary>
        string DirectoryManagerSecret { get; }

        /// <summary>
        /// Gets the domain name.
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Gets the telemetry instrumentation key.
        /// </summary>
        string InstrumentationKey { get; }

        /// <summary>
        /// Gets the sign in policy (used to construct the metadata endpoint URL).
        /// </summary>
        string SignInPolicy { get; }

        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        string SqlConnectionString { get; }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Gets the service certificate's thumbprint
        /// </summary>
        string Thumbprint { get; }
    }
}