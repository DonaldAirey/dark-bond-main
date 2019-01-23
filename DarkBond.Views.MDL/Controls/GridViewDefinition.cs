// <copyright file="GridViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// A view that displays items in a list.
    /// </summary>
    [ContentProperty(Name = "ItemTemplate")]
    public class GridViewDefinition : ViewDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewDefinition"/> class.
        /// </summary>
        public GridViewDefinition()
        {
            // This is the predefined styles for this view.
            this.Style = Application.Current.Resources["GridViewStyle"] as Style;
        }
    }
}