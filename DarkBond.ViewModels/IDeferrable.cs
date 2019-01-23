// <copyright file="IDeferrable.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;

    /// <summary>
    /// Allows refreshing to be deferred and then updates the collection when the deferment is over.
    /// </summary>
    public interface IDeferrable
    {
        /// <summary>
        /// Gets or sets a value indicating whether refreshing the collection is disabled.
        /// </summary>
        bool IsRefreshDisabled
        {
            get;
            set;
        }

        /// <summary>
        /// Resets the collection when deferment has ended.
        /// </summary>
        void ResetCollection();
    }
}
