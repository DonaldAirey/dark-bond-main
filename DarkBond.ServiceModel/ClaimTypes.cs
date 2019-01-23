// <copyright file="ClaimTypes.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System;

    /// <summary>
    /// Specific claims types for granting access to user roles.
    /// </summary>
    public static class ClaimTypes
    {
        /// <summary>
        /// Create permission required.
        /// </summary>
        public const string Create = "http://schemas.darkbond.com/identity/claims/create";

        /// <summary>
        /// Destroy permission required.
        /// </summary>
        public const string Delete = "http://schemas.darkbond.com/identity/claims/delete";

        /// <summary>
        /// Read permission required.
        /// </summary>
        public const string Read = "http://schemas.darkbond.com/identity/claims/read";

        /// <summary>
        /// Update permission required.
        /// </summary>
        public const string Update = "http://schemas.darkbond.com/identity/claims/update";
    }
}
