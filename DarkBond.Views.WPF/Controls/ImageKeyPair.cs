// <copyright file="ImageKeyPair.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// An image with a key.
    /// </summary>
    [ContentProperty("Key")]
    public class ImageKeyPair : DependencyObject
    {
        /// <summary>
        /// The Key DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(object),
            typeof(ImageKeyPair),
            null);

        /// <summary>
        /// The Uri DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(
            "Uri",
            typeof(string),
            typeof(ImageKeyPair),
            null);

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public object Key
        {
            get
            {
                return this.GetValue(ImageKeyPair.KeyProperty);
            }

            set
            {
                this.SetValue(ImageKeyPair.KeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI for the image.
        /// </summary>
        /// <value>
        /// The URI for the image.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Bug in XAML Compiler prevents URIs from being parsed")]
        public string UriSource
        {
            get
            {
                return this.GetValue(ImageKeyPair.UriSourceProperty) as string;
            }

            set
            {
                this.SetValue(ImageKeyPair.UriSourceProperty, value);
            }
        }
    }
}