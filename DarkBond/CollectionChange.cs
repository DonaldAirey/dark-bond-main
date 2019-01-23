// <copyright file="CollectionChange.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Describes the type of change on the collection.
    /// </summary>
    public enum CollectionChange
    {
        /// <summary>
        /// The collection has been reset.
        /// </summary>
        Reset = 0,

        /// <summary>
        /// An item has been inserted.
        /// </summary>
        ItemInserted = 1,

        /// <summary>
        /// An item has been removed.
        /// </summary>
        ItemRemoved = 2,

        /// <summary>
        /// An item has been changed.
        /// </summary>
        ItemChanged = 3
    }
}