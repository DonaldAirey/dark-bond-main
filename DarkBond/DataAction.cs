// <copyright file="DataAction.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// An action on a data model.
    /// </summary>
    public enum DataAction
    {
        /// <summary>
        /// Add an item.
        /// </summary>
        Add,

        /// <summary>
        /// Delete an item.
        /// </summary>
        Delete,

        /// <summary>
        /// Rollback an item.
        /// </summary>
        Rollback,

        /// <summary>
        /// Update an item.
        /// </summary>
        Update
    }
}
