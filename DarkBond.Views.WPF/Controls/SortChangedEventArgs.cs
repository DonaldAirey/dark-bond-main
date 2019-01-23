// <copyright file="SortChangedEventArgs.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;

    /// <summary>
    /// Event arguments for a change in the sort order.
    /// </summary>
    public class SortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isExtended">Indicates whether multiple columns are used to describe a sort order.</param>
        public SortChangedEventArgs(bool isExtended)
        {
            // Initialize the object.
            this.IsExtended = isExtended;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sorting is simple or extended.
        /// </summary>
        /// <value>
        /// A value indicating whether the sorting is simple or extended.
        /// </value>
        public bool IsExtended
        {
            get;
            set;
        }
    }
}