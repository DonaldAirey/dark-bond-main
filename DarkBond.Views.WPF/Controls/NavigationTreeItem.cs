// <copyright file="NavigationTreeItem.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// An element in a HierarchyView.
    /// </summary>
    public class NavigationTreeItem : TreeViewItem
    {
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            // Only HighlightElements are displayed in the selector.
            return new NavigationTreeItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Only HighlightElements can be considered containers for items in this selector.
            return item is NavigationTreeItem;
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="FrameworkElement"/> has been updated.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // When an element has been selected, make sure that it's visible within the scrollable region.
            if (e.Property == NavigationTreeItem.IsSelectedProperty && this.IsSelected)
            {
                this.BringIntoView();
            }

            // Allow the base class to finish the function.
            base.OnPropertyChanged(e);
        }
    }
}