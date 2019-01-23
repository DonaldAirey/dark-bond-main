// <copyright file="RoleInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.OrderManagementSystem
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using DarkBond;
    using DarkBond.CoreModel;

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
                            new Claim(CoreModel.ClaimTypes.Create, ClaimResources.Application),
                            new Claim(CoreModel.ClaimTypes.Delete, ClaimResources.Application),
                            new Claim(CoreModel.ClaimTypes.Update, ClaimResources.Application),
                            new Claim(CoreModel.ClaimTypes.Read, ClaimResources.Application)
                        }
                    },
                    {
                        "Users",
                        new Claim[]
                        {
                            new Claim(CoreModel.ClaimTypes.Create, ClaimResources.Application),
                            new Claim(CoreModel.ClaimTypes.Update, ClaimResources.Application),
                            new Claim(CoreModel.ClaimTypes.Read, ClaimResources.Application)
                        }
                    },
                    {
                        "Guest",
                        new Claim[]
                        {
                            new Claim(CoreModel.ClaimTypes.Read, ClaimResources.Application),
                        }
                    }
                };
            }
        }
    }
}