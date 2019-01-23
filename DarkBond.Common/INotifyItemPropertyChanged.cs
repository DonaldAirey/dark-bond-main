// <copyright file="INotifyItemPropertyChanged.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Notifies clients that a property value has changed on an item in the collection.
    /// </summary>
    public interface INotifyItemPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes on an item in the collection.
        /// </summary>
        event EventHandler<ItemPropertyChangedEventArgs> ItemPropertyChanged;
    }
}
