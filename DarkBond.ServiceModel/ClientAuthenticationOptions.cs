// <copyright file="ClientAuthenticationOptions.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// Authentication options.
    /// </summary>
    public class ClientAuthenticationOptions
    {
        /// <summary>
        /// The base URL of the Azure AD B2C authentication service.
        /// </summary>
        public const string AzureAdInstance = "https://login.microsoftonline.com/";

        /// <summary>
        /// The version of OAuth used for authentication.
        /// </summary>
        public const string OAuthVersion = "v2.0";

        /// <summary>
        /// Gets or sets A Unique URI identifying the API (e.g. https://darkbonddemo.onmicrosoft.com/api).  Can be anything so long as it matches the
        /// configuration on the Azure AD tenant.
        /// </summary>
        public string ApiIdentifier { get; set; }

        /// <summary>
        /// Gets the full URI for the authority for this tentant.
        /// </summary>
        public string Authority => $"{ClientAuthenticationOptions.AzureAdInstance}{this.Tenant}/{ClientAuthenticationOptions.OAuthVersion}";

        /// <summary>
        /// Gets or sets the client identifier from Azure (e.g. 34634ad5-6c33-4568-9b8c-c44d2252deef)
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets a password used to gain access to the authority.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the tenant name (e.g. darkbonddemo.onmicrosoft.com).
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets where the client application is redirected after logging out.
        /// </summary>
        public string SignedOutRedirectUri { get; set; }

        /// <summary>
        /// Gets a full URI to the policy endpoint.
        /// </summary>
        /// <param name="policy">The policy (sign_up_sign_in, edit_profile, reset, etc.)</param>
        /// <returns>The full URI to the policy endpoint.</returns>
        public string GetAuthority(string policy) => $"{ClientAuthenticationOptions.AzureAdInstance}tfp/{this.Tenant}/{policy}/{ClientAuthenticationOptions.OAuthVersion}";
    }
}