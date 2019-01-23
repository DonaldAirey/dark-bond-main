// <copyright file="FilterItemsControl.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows.Controls;

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
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(FilterItemsControl);
        }
    }
}