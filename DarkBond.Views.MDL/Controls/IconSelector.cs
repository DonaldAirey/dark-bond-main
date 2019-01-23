// <copyright file="IconSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides an image from a store.
    /// </summary>
    public class IconSelector : BitmapIcon
    {
        /// <summary>
        /// The Dictionary DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DictionaryProperty = DependencyProperty.Register(
            "Dictionary",
            typeof(UriDictionary),
            typeof(IconSelector),
            new PropertyMetadata(null, IconSelector.OnDictionaryPropertyChanged));

        /// <summary>
        /// The Category DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
            "Category",
            typeof(string),
            typeof(IconSelector),
            new PropertyMetadata(default(string), IconSelector.OnCategoryPropertyChanged));

        /// <summary>
        /// The Key DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(IconSelector),
            new PropertyMetadata(default(string), IconSelector.OnKeyPropertyChanged));

        /// <summary>
        /// Gets or sets the category used to select the source for the icon.
        /// </summary>
        public string Category
        {
            get
            {
                return this.GetValue(IconSelector.CategoryProperty) as string;
            }

            set
            {
                this.SetValue(IconSelector.CategoryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dictionary of URIs.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "No other way to use a dictionary in XAML")]
        public UriDictionary Dictionary
        {
            get
            {
                return this.GetValue(IconSelector.DictionaryProperty) as UriDictionary;
            }

            set
            {
                this.SetValue(IconSelector.DictionaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key used to select the source for the icon.
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetValue(IconSelector.KeyProperty) as string;
            }

            set
            {
                this.SetValue(IconSelector.KeyProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Source property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnDictionaryPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new key.
            IconSelector iconSelector = dependencyObject as IconSelector;
            UriDictionary resourceDictionary = dependencyPropertyChangedEventArgs.NewValue as UriDictionary;
            if (iconSelector.Key != null && resourceDictionary != null)
            {
                iconSelector.UriSource = resourceDictionary.GetUri(iconSelector.Category, iconSelector.Key);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Category property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnCategoryPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new image size.
            IconSelector iconSelector = dependencyObject as IconSelector;
            string category = dependencyPropertyChangedEventArgs.NewValue as string;
            if (iconSelector.Key != null && iconSelector.Dictionary != null)
            {
                iconSelector.UriSource = iconSelector.Dictionary.GetUri(category, iconSelector.Key);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Key property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnKeyPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new key.
            IconSelector iconSelector = dependencyObject as IconSelector;
            string key = dependencyPropertyChangedEventArgs.NewValue as string;
            if (key != null && iconSelector.Dictionary != null)
            {
                iconSelector.UriSource = iconSelector.Dictionary.GetUri(iconSelector.Category, key);
            }
        }
    }
}