// <copyright file="ListViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A view that displays items in a list.
    /// </summary>
    public class ListViewDefinition : ViewDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewDefinition"/> class.
        /// </summary>
        public ListViewDefinition()
        {
            // This is the predefined styles for this view.
            this.Style = Application.Current.TryFindResource(new ComponentResourceKey(typeof(ItemsView), "ItemsViewStyle")) as Style;
            this.ItemsPanel = Application.Current.TryFindResource(new ComponentResourceKey(typeof(ItemsView), "VerticalStackPanel")) as ItemsPanelTemplate;
        }
    }
}