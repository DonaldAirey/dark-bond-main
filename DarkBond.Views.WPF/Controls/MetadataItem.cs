// <copyright file="MetadataItem.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Panel for arranging metadata.
    /// </summary>
    public class MetadataItem : ContentControl
    {
        /// <summary>
        /// Identifies the Text DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(MetadataItem),
            null);

        /// <summary>
        /// Identifies the Text DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(
            "HeaderStyle",
            typeof(Style),
            typeof(MetadataItem),
            null);

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataItem"/> class.
        /// </summary>
        public MetadataItem()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(MetadataItem);
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return this.GetValue(MetadataItem.HeaderProperty) as string;
            }

            set
            {
                this.SetValue(MetadataItem.HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        public Style HeaderStyle
        {
            get
            {
                return this.GetValue(MetadataItem.HeaderStyleProperty) as Style;
            }

            set
            {
                this.SetValue(MetadataItem.HeaderStyleProperty, value);
            }
        }
    }
}