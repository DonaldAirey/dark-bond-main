// <copyright file="ImageSelector.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Provides an image from a store.
    /// </summary>
    public class ImageSelector : Image
    {
        /// <summary>
        /// The Dictionary DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty DictionaryProperty = DependencyProperty.Register(
            "Dictionary",
            typeof(UriDictionary),
            typeof(ImageSelector),
            new PropertyMetadata(null, ImageSelector.OnDictionaryPropertyChanged));

        /// <summary>
        /// The Category DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
            "Category",
            typeof(string),
            typeof(ImageSelector),
            new PropertyMetadata(default(string), ImageSelector.OnCategoryPropertyChanged));

        /// <summary>
        /// The Key DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(ImageSelector),
            new PropertyMetadata(default(string), ImageSelector.OnKeyPropertyChanged));

        /// <summary>
        /// Gets or sets the category used to select the source for the icon.
        /// </summary>
        public string Category
        {
            get
            {
                return this.GetValue(ImageSelector.CategoryProperty) as string;
            }

            set
            {
                this.SetValue(ImageSelector.CategoryProperty, value);
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
                return this.GetValue(ImageSelector.DictionaryProperty) as UriDictionary;
            }

            set
            {
                this.SetValue(ImageSelector.DictionaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key used to select the source for the icon.
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetValue(ImageSelector.KeyProperty) as string;
            }

            set
            {
                this.SetValue(ImageSelector.KeyProperty, value);
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
            ImageSelector imageSelector = dependencyObject as ImageSelector;
            UriDictionary uriDictionary = dependencyPropertyChangedEventArgs.NewValue as UriDictionary;
            if (imageSelector.Key != null && uriDictionary != null)
            {
                imageSelector.LoadImage();
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
            ImageSelector imageSelector = dependencyObject as ImageSelector;
            if (imageSelector.Key != null && imageSelector.Dictionary != null)
            {
                imageSelector.LoadImage();
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
            ImageSelector imageSelector = dependencyObject as ImageSelector;
            string key = dependencyPropertyChangedEventArgs.NewValue as string;
            if (key != null && imageSelector.Dictionary != null)
            {
                imageSelector.LoadImage();
            }
        }

        /// <summary>
        /// Uses the dictionary to determine the source to load into the image.
        /// </summary>
        private void LoadImage()
        {
            Uri uriSource = this.Dictionary.GetUri(this.Category, this.Key);
            if (uriSource == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to find BitmapImage with key of " + this.Key);
            }
            else
            {
                this.Source = new BitmapImage(uriSource);
            }
        }
    }
}