// <copyright file="ClaimsTransformer.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;

    /// <summary>
    /// Used for claims transformation.
    /// </summary>
    public class ClaimsTransformer : IClaimsTransformation
    {
        /// <summary>
        /// The Graph API URL.
        /// </summary>
        private const string GraphApiUrl = "https://graph.windows.net";

        /// <summary>
        /// The version of the Graph API to use.
        /// </summary>
        private const string GraphApiVersion = "?api-version=1.6";

        /// <summary>
        /// The Graph API MemberOf function.
        /// </summary>
        private const string MemberOfFunction = "/memberOf";

        /// <summary>
        /// The Microsoft authentication URL.
        /// </summary>
        private const string MicrosoftOnline = "https://login.microsoftonline.com/";

        /// <summary>
        /// The partial address of the OpenId configuration.
        /// </summary>
        private const string MicrosoftOnlineConfiguration = "/v2.0/.well-known/openid-configuration?p=";

        /// <summary>
        /// The User ID claim.
        /// </summary>
        private const string UserIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// The path the users functions in the Graph API .
        /// </summary>
        private const string UsersPath = "/users";

        /// <summary>
        /// Context for authenticating the Graph API calls.
        /// </summary>
        private AuthenticationContext authenticationContext;

        /// <summary>
        /// The Azure AD B2C options.
        /// </summary>
        private ServiceAuthenticationOptions serviceAuthenticationOptions;

        /// <summary>
        /// The client credentials for querying the AAD.
        /// </summary>
        private ClientCredential credential;

        /// <summary>
        /// The cached collection of groups to which a user belongs.
        /// </summary>
        private Dictionary<Guid, string[]> groupCache = new Dictionary<Guid, string[]>();

        /// <summary>
        /// Information about claims granted to users groups.
        /// </summary>
        private IRoleInfo roleInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsTransformer"/> class.
        /// </summary>
        /// <param name="serviceAuthenticationOptions">The Azure AD B2C options.</param>
        /// <param name="roleInfo">The roles associated with user groups.</param>
        public ClaimsTransformer(IOptions<ServiceAuthenticationOptions> serviceAuthenticationOptions, IRoleInfo roleInfo)
        {
            // Initialize the object.
            this.serviceAuthenticationOptions = serviceAuthenticationOptions.Value;
            this.roleInfo = roleInfo;

            // Create a set of credentials for querying the Azure Active Directory store for group memberships.
            string authority = ClaimsTransformer.MicrosoftOnline + this.serviceAuthenticationOptions.Tenant;
            this.authenticationContext = new AuthenticationContext(authority);
            this.credential = new ClientCredential(
                this.serviceAuthenticationOptions.DirectoryManagerClientId,
                this.serviceAuthenticationOptions.DirectoryManagerSecret);
        }

        /// <inheritdoc/>
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal claimsPrincipal)
        {
            // The general idea is to add claims to this user based on a role (group affiliation).
            ClaimsIdentity claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;

            // This will query the Azure B2C directory for the groups associated with this user.
            IEnumerable<string> groups = new List<string>();
            Claim userClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimsTransformer.UserIdClaim);
            if (userClaim != null)
            {
                groups = await this.GetGroups(Guid.Parse(userClaim.Value));
            }

            // This will create a distinct set of claims based on all the roles to which the current user has been assigned.  Keep in mind that a
            // given user can be assigned to one or more roles, so the final set must contain the distinct union of all the claims.
            HashSet<Claim> hashSetClaims = new HashSet<Claim>();
            foreach (string group in groups)
            {
                Claim[] groupClaims = null;
                if (this.roleInfo.Claims.TryGetValue(group, out groupClaims))
                {
                    foreach (Claim claim in groupClaims)
                    {
                        claimsIdentity.AddClaim(claim);
                    }
                }
            }

            // The user will now execute with this set of claims.
            return claimsPrincipal;
        }

        /// <summary>
        /// Gets the groups to which a user belongs.
        /// </summary>
        /// <param name="userId">The unique user id.</param>
        /// <returns>A list of groups to which this user belongs.</returns>
        private async Task<IEnumerable<string>> GetGroups(Guid userId)
        {
            // The general idea here is to see if we've cached this user's group affiliations yet.  If so, then return the groups already retrieved.
            // If not, use the Graph API to connect to the endpoint and query for the groups associated with the user.  This is an expensive
            // operation so we only do it once.  Eventually, this should be a set of claims that comes back with the security token in the
            // authentication (like it does in classic Azure Active Directory), or the cache should have an expiration date so we can pick up a
            // change to a user's group.
            string[] groups;
            if (!this.groupCache.TryGetValue(userId, out groups))
            {
                // Acquire a security token that gives us permission to query the directory.
                AuthenticationResult authenticationResult = await this.authenticationContext.AcquireTokenAsync(
                    ClaimsTransformer.GraphApiUrl,
                    this.credential);

                // Get the groups associated with the current user.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Construct an endpoint to the Graph API from the components and query the directory using the baked-in credentials.
                    string graphApiUrl = ClaimsTransformer.GraphApiUrl + "/" + this.serviceAuthenticationOptions.Tenant +
                        ClaimsTransformer.UsersPath + "/" + userId + ClaimsTransformer.MemberOfFunction +
                        ClaimsTransformer.GraphApiVersion;
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, graphApiUrl))
                    {
                        // Add the authentication token to the HTTP request header.
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                        // This will construct an array of group names from the JSON results of the query.
                        HttpResponseMessage response = httpClient.SendAsync(request).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            DirectoryGroupInfo directoryGroupInfo = JsonConvert.DeserializeObject<DirectoryGroupInfo>(
                                response.Content.ReadAsStringAsync().Result);
                            groups = new string[directoryGroupInfo.Groups.Count];
                            for (int index = 0; index < directoryGroupInfo.Groups.Count; index++)
                            {
                                groups[index] = directoryGroupInfo.Groups[index].DisplayName;
                            }
                        }
                        else
                        {
                            // In the event of an error, the user doesn't belong to any group.
                            groups = new string[0];
                        }
                    }
                }

                // This adds the user to the cache so we don't have to do this expensive operation each time we need to authenticate a user.
                this.groupCache.Add(userId, groups);
            }

            // These are the groups associated with the given user.
            return groups;
        }
    }
}