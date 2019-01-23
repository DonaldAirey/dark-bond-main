// <copyright file="ServiceAuthenticationOptions.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The options for authenticating with an Azure Active Directory B2C.
    /// </summary>
    public class ServiceAuthenticationOptions
    {
        /// <summary>
        /// The base URL of the Azure AD B2C authentication service.
        /// </summary>
        private const string AzureAdInstance = "https://login.microsoftonline.com/";

        /// <summary>
        /// The version of OAuth used for authentication.
        /// </summary>
        private const string OAuthVersion = "v2.0";

        /// <summary>
        /// Gets the URL of the Auth 2.0 endpoint.
        /// </summary>
        public string Authority => $"{ServiceAuthenticationOptions.AzureAdInstance}{this.Tenant}/{ServiceAuthenticationOptions.OAuthVersion}";

        /// <summary>
        /// Gets or sets the audience (also known as Application Id in Azure AD B2C, e.g. 'f833c677-d633-47f4-aa1d-c233a46313f9').
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the Client Identifier for the Directory Manager (used to query for roles).
        /// </summary>
        public string DirectoryManagerClientId { get; set; }

        /// <summary>
        /// Gets or sets the Client Secret for the Directory Manager (used to query for roles).
        /// </summary>
        public string DirectoryManagerSecret { get; set; }

        /// <summary>
        /// Gets the metadata address for the policy endpoint.
        /// </summary>
        public string MetadataAddress => $"{this.Authority}/.well-known/openid-configuration?p={this.SignInPolicy}";

        /// <summary>
        /// Gets or sets the sign-in/sign-up policy name.
        /// </summary>
        public string SignInPolicy { get; set; }

        /// <summary>
        /// Gets or sets the tenant (e.g. darkbonddemo.onmicrosoft.com)
        /// </summary>
        public string Tenant { get; set; }
    }
}