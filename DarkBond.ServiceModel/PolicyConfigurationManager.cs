// <copyright file="PolicyConfigurationManager.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Handles the configuration of the login policies.
    /// </summary>
    public class PolicyConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
    {
        /// <summary>
        /// Used to construct the metadata address for the policy configuration data.
        /// </summary>
        private const string PolicyParameter = "/.well-known/openid-configuration?p";

        /// <summary>
        /// Dictionary of policies and the metadata where the configuration can be found.
        /// </summary>
        private readonly Dictionary<string, IConfigurationManager<OpenIdConnectConfiguration>> managers =
            new Dictionary<string, IConfigurationManager<OpenIdConnectConfiguration>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyConfigurationManager"/> class.
        /// </summary>
        /// <param name="authority">The authority that grants access.</param>
        /// <param name="policies">The policy this application wants to use.</param>
        public PolicyConfigurationManager(string authority, IEnumerable<string> policies)
        {
            // Create a dictionary of all the policies and the URL where that metadata for that policy can be found.
            foreach (var policy in policies)
            {
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}{PolicyParameter}={policy}",
                    new OpenIdConnectConfigurationRetriever());
                this.managers.Add(policy.ToLowerInvariant(), configurationManager);
            }
        }

        /// <summary>
        /// Gets the configuration of the policy.
        /// </summary>
        /// <param name="cancel">Used to cancel the requeasted operation.</param>
        /// <returns>The configuration of the policy at the endpoint.</returns>
        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            // This gets all the policy configurations from the authority and merges them into a single structure.
            OpenIdConnectConfiguration mergedConfiguration = null;
            foreach (var manager in this.managers)
            {
                var configuration = await manager.Value.GetConfigurationAsync(cancel);
                if (mergedConfiguration == null)
                {
                    mergedConfiguration = Clone(configuration);
                }
                else
                {
                    MergeConfig(mergedConfiguration, configuration);
                }
            }

            // A single structure containing all the configurations.
            return mergedConfiguration;
        }

        /// <summary>
        /// Gets the configuration by the policy name.
        /// </summary>
        /// <param name="cancel">Use to cancel the request.</param>
        /// <param name="policy">The policy name use to find the configuration.</param>
        /// <returns>The authentication configuration.</returns>
        public Task<OpenIdConnectConfiguration> GetConfigurationByPolicyAsync(CancellationToken cancel, string policy)
        {
            // Validate the arguments.
            if (string.IsNullOrEmpty(policy))
            {
                throw new ArgumentNullException(nameof(policy));
            }

            // Find the policy in the managed configuration and query the authority for the configuration information.
            var policyKey = policy.ToLowerInvariant();
            IConfigurationManager<OpenIdConnectConfiguration> policyConfiguration;
            if (this.managers.TryGetValue(policyKey, out policyConfiguration))
            {
                return policyConfiguration.GetConfigurationAsync(cancel);
            }

            // We didn't have the requested policy registered on this end.
            throw new InvalidOperationException($"Invalid policy: {policy}");
        }

        /// <summary>
        /// Request a refresh of the policies.
        /// </summary>
        public void RequestRefresh()
        {
            // Request a refresh of each of the policies in the manager.
            foreach (var manager in this.managers)
            {
                manager.Value.RequestRefresh();
            }
        }

        /// <summary>
        /// Create a copy of the policy manager.
        /// </summary>
        /// <param name="openIdConnectConfiguration">The configuration to be copied.</param>
        /// <returns>A copy of the authentication configuration.</returns>
        private static OpenIdConnectConfiguration Clone(OpenIdConnectConfiguration openIdConnectConfiguration)
        {
            var signingKeys = new List<SecurityKey>(openIdConnectConfiguration.SigningKeys);
            openIdConnectConfiguration.SigningKeys.Clear();

            var keySet = openIdConnectConfiguration.JsonWebKeySet;
            openIdConnectConfiguration.JsonWebKeySet = null;

            var json = OpenIdConnectConfiguration.Write(openIdConnectConfiguration);
            var clone = OpenIdConnectConfiguration.Create(json);

            foreach (var key in signingKeys)
            {
                openIdConnectConfiguration.SigningKeys.Add(key);
                clone.SigningKeys.Add(key);
            }

            openIdConnectConfiguration.JsonWebKeySet = keySet;
            clone.JsonWebKeySet = keySet;

            return clone;
        }

        /// <summary>
        /// Merges one OpenID Connect configuration into another.
        /// </summary>
        /// <param name="result">The destination for the merged results.</param>
        /// <param name="source">The source configuration.</param>
        private static void MergeConfig(OpenIdConnectConfiguration result, OpenIdConnectConfiguration source)
        {
            // This creates the common set of token signing algorithms.
            foreach (var tokenSigningAlg in source.IdTokenSigningAlgValuesSupported)
            {
                if (!result.IdTokenSigningAlgValuesSupported.Contains(tokenSigningAlg))
                {
                    result.IdTokenSigningAlgValuesSupported.Add(tokenSigningAlg);
                }
            }

            // This creates the common set of response types supported.
            foreach (var type in source.ResponseTypesSupported)
            {
                if (!result.ResponseTypesSupported.Contains(type))
                {
                    result.ResponseTypesSupported.Add(type);
                }
            }

            // This creates the common set of subject types.
            foreach (var subjectType in source.SubjectTypesSupported)
            {
                if (!result.ResponseTypesSupported.Contains(subjectType))
                {
                    result.SubjectTypesSupported.Add(subjectType);
                }
            }

            // This creates the common set of signing keys.
            foreach (var signingKeys in source.SigningKeys)
            {
                if (result.SigningKeys.All(k => k.KeyId != signingKeys.KeyId))
                {
                    result.SigningKeys.Add(signingKeys);
                }
            }
        }
    }
}