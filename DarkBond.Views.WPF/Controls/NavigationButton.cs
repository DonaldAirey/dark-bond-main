// <copyright file="NavigationButton.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a lightweight control for displaying small amounts of flow content.
    /// </summary>
    public class NavigationButton : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationButton"/> class.
        /// </summary>
        public NavigationButton()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(NavigationButton);
        }
    }
}