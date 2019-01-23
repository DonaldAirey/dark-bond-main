// <copyright file="ItemsViewItem.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides a container for any object presented in an <see cref="ItemsView"/>.
    /// </summary>
    public class ItemsViewItem : ListViewItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsViewItem"/> class.
        /// </summary>
        public ItemsViewItem()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(ItemsViewItem);
        }
    }
}