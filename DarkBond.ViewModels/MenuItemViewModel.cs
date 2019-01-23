// <copyright file="MenuItemViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A view model for menu items.
    /// </summary>
    public class MenuItemViewModel : ButtonViewModel
    {
        /// <summary>
        /// The sub-menu items.
        /// </summary>
        private ObservableCollection<MenuItemViewModel> itemsField = new ObservableCollection<MenuItemViewModel>();

        /// <summary>
        /// Gets the view of the items.
        /// </summary>
        public ObservableCollection<MenuItemViewModel> Items
        {
            get
            {
                return this.itemsField;
            }
        }

        /// <summary>
        /// Gets or sets the key used for sorting.
        /// </summary>
        public IComparable SortKey { get; set; }
    }
}