// <copyright file="NotifyRelationChangedAction.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// Describes the action that caused a CollectionChanged event.
    /// </summary>
    public enum NotifyRelationChangedAction
    {
        /// <summary>
        /// One or more items were added to the relation.
        /// </summary>
        Add = 0,

        /// <summary>
        /// One or more items were removed from the relation.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// One ore more items were moved from one relation to another.
        /// </summary>
        Change = 3,

        /// <summary>
        /// The relation was cleared.
        /// </summary>
        Reset = 4
    }
}