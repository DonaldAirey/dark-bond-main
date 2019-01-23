// <copyright file="FilterItemsControl.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Generates the visual elements of a filter.
    /// </summary>
    public class FilterItemsControl : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterItemsControl"/> class.
        /// </summary>
        public FilterItemsControl()
        {
            // Provides a default style for the content of the FilterFlyout which allows us to style the check-boxes attached to the filters.  Note
            // that this class would have been completely unnecessary if the Flyout had just provided a method of creating a default style.
            this.DefaultStyleKey = typeof(FilterItemsControl);
        }
    }
}