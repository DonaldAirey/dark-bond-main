// <copyright file="IRoleInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    /// A collection of claims granted to a role.
    /// </summary>
    public interface IRoleInfo
    {
        /// <summary>
        /// Gets the claims for a given role.
        /// </summary>
        Dictionary<string, Claim[]> Claims { get; }
    }
}