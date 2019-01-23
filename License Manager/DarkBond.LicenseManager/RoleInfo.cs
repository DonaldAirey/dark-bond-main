// <copyright file="RoleInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System.Collections.Generic;
    using System.IdentityModel.Claims;
    using DarkBond;
    using DarkBond.ServiceModel;

    /// <summary>
    /// Application specific set of rules for authorizing a user, given a set of claims.
    /// </summary>
    public class RoleInfo : IRoleInfo
    {
        /// <summary>
        /// Gets the claims for a given role.
        /// </summary>
        public Dictionary<string, Claim[]> Claims
        {
            get
            {
                return new Dictionary<string, Claim[]>
                {
                    {
                        "Administrators",
                        new Claim[]
                        {
                            new Claim(ServiceModel.ClaimTypes.Create, ClaimResources.Application, Rights.PossessProperty),
                            new Claim(ServiceModel.ClaimTypes.Delete, ClaimResources.Application, Rights.PossessProperty),
                            new Claim(ServiceModel.ClaimTypes.Update, ClaimResources.Application, Rights.PossessProperty),
                            new Claim(ServiceModel.ClaimTypes.Read, ClaimResources.Application, Rights.PossessProperty)
                        }
                    },
                    {
                        "Users",
                        new Claim[]
                        {
                            new Claim(ServiceModel.ClaimTypes.Create, ClaimResources.Application, Rights.PossessProperty),
                            new Claim(ServiceModel.ClaimTypes.Update, ClaimResources.Application, Rights.PossessProperty),
                            new Claim(ServiceModel.ClaimTypes.Read, ClaimResources.Application, Rights.PossessProperty)
                        }
                    },
                    {
                        "Guest",
                        new Claim[]
                        {
                            new Claim(ServiceModel.ClaimTypes.Read, ClaimResources.Application, Rights.PossessProperty),
                        }
                    }
                };
            }
        }
    }
}