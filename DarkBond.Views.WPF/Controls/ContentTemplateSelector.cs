// <copyright file="ContentTemplateSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Used to select a template for the content of a control.
    /// </summary>
    public class ContentTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template used to display an image.
        /// </summary>
        /// <value>
        /// The template used to display an image.
        /// </value>
        public DataTemplate ImageTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the template used to display text.
        /// </summary>
        /// <value>
        /// The template used to display text.
        /// </value>
        public DataTemplate StringTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// returns a specific DataTemplate for a given item or container.
        /// </summary>
        /// <param name="item">The item to return a template for.</param>
        /// <param name="container">The parent container for the template item.</param>
        /// <returns>The template to use for the given item and/or container.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // If the item is an image, then return the image template.
            if (item is BitmapImage)
            {
                return this.ImageTemplate;
            }

            // All other types use the string template.
            return this.StringTemplate;
        }
    }
}