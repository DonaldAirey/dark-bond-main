// <copyright file="GridViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A view that displays items in a grid.
    /// </summary>
    public class GridViewDefinition : ViewDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewDefinition"/> class.
        /// </summary>
        public GridViewDefinition()
        {
            // This is the predefined styles for this view.
            this.Style = Application.Current.TryFindResource(new ComponentResourceKey(typeof(ItemsView), "ItemsViewStyle")) as Style;
            this.ItemsPanel = Application.Current.TryFindResource(new ComponentResourceKey(typeof(ItemsView), "VerticalWrapPanel")) as ItemsPanelTemplate;
        }
    }
}