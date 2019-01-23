﻿// <copyright file="RowVersion.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The different versions of a row.
    /// </summary>
    public enum RowVersion
    {
        /// <summary>
        /// The current version of a row.
        /// </summary>
        Current,

        /// <summary>
        /// The original version of a row.
        /// </summary>
        Original,

        /// <summary>
        /// The previous version of a row.
        /// </summary>
        Previous
    }
}
