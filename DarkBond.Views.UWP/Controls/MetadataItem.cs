// <copyright file="MetadataItem.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Panel for arranging metadata.
    /// </summary>
    public class MetadataItem : ContentControl
    {
        /// <summary>
        /// Identifies the Text DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(MetadataItem),
            new PropertyMetadata(string.Empty, MetadataItem.OnHeaderPropertyChanged));

        /// <summary>
        /// Identifies the Format DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(MetadataItem),
            new PropertyMetadata(string.Empty));

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
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.GetValue(MetadataItem.TextProperty) as string;
            }
        }

        /// <summary>
        /// Handles a change to the text property.
        /// </summary>
        /// <param name="dependencyObject">The object on which the change has occurred.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The property change event arguments.</param>
        private static void OnHeaderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            MetadataItem metadataHeader = dependencyObject as MetadataItem;
            string newHeader = dependencyPropertyChangedEventArgs.NewValue as string;
            metadataHeader.SetValue(MetadataItem.TextProperty, newHeader + ":");
        }
    }
}