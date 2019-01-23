// <copyright file="BitmapSourceSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Provides an image from a store.
    /// </summary>
    public class BitmapSourceSelector : BitmapSource
    {
        /// <summary>
        /// The Category DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification ="The field is immutable")]
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
            "Category",
            typeof(string),
            typeof(BitmapSourceSelector),
            new PropertyMetadata(default(string), BitmapSourceSelector.OnCategoryPropertyChanged));

        /// <summary>
        /// The Dictionary DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DictionaryProperty = DependencyProperty.Register(
            "Dictionary",
            typeof(UriDictionary),
            typeof(BitmapSourceSelector),
            new PropertyMetadata(default(UriDictionary), BitmapSourceSelector.OnDictionaryPropertyChanged));

        /// <summary>
        /// The Key DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(BitmapSourceSelector),
            new PropertyMetadata(default(string), BitmapSourceSelector.OnKeyPropertyChanged));

        /// <summary>
        /// Gets or sets the category used to select the source for the icon.
        /// </summary>
        public string Category
        {
            get
            {
                return this.GetValue(BitmapSourceSelector.CategoryProperty) as string;
            }

            set
            {
                this.SetValue(BitmapSourceSelector.CategoryProperty, value);
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
                return this.GetValue(BitmapSourceSelector.DictionaryProperty) as UriDictionary;
            }

            set
            {
                this.SetValue(BitmapSourceSelector.DictionaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key used to select the source for the icon.
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetValue(BitmapSourceSelector.KeyProperty) as string;
            }

            set
            {
                this.SetValue(BitmapSourceSelector.KeyProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Source property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static async void OnDictionaryPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the dictionary.
            BitmapSourceSelector bitmapSourceSelector = dependencyObject as BitmapSourceSelector;
            UriDictionary resourceDictionary = dependencyPropertyChangedEventArgs.NewValue as UriDictionary;
            if (bitmapSourceSelector.Key != null && resourceDictionary != null)
            {
                Uri uri = resourceDictionary.GetUri(bitmapSourceSelector.Category, bitmapSourceSelector.Key);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmapSourceSelector.SetSourceAsync(stream);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Category property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static async void OnCategoryPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new category.
            BitmapSourceSelector bitmapSourceSelector = dependencyObject as BitmapSourceSelector;
            string category = dependencyPropertyChangedEventArgs.NewValue as string;
            if (bitmapSourceSelector.Key != null && bitmapSourceSelector.Dictionary != null)
            {
                Uri uri = bitmapSourceSelector.Dictionary.GetUri(category, bitmapSourceSelector.Key);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmapSourceSelector.SetSourceAsync(stream);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Key property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static async void OnKeyPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new key.
            BitmapSourceSelector bitmapSourceSelector = dependencyObject as BitmapSourceSelector;
            string key = dependencyPropertyChangedEventArgs.NewValue as string;
            if (!string.IsNullOrEmpty(key) && bitmapSourceSelector.Dictionary != null)
            {
                Uri uri = bitmapSourceSelector.Dictionary.GetUri(bitmapSourceSelector.Category, key);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmapSourceSelector.SetSourceAsync(stream);
            }
        }
    }
}