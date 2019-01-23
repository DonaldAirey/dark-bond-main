// <copyright file="FilterDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.ViewModels;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Definition of a filter item.
    /// </summary>
    public class FilterDefinition : DependencyObject
    {
        /// <summary>
        /// The Description DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(FilterDefinition),
            null);

        /// <summary>
        /// The GroupName DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(string),
            typeof(FilterDefinition),
            null);

        /// <summary>
        /// The Icon DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(ImageSource),
            typeof(FilterDefinition),
            null);

        /// <summary>
        /// The IsEnabled DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled",
            typeof(bool),
            typeof(FilterDefinition),
            new PropertyMetadata(false, FilterDefinition.OnIsEnabledPropertyChanged));

        /// <summary>
        /// The GroupName DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof(string),
            typeof(FilterDefinition),
            null);

        /// <summary>
        /// Gets or sets a description of the filter.
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetValue(FilterDefinition.DescriptionProperty) as string;
            }

            set
            {
                this.SetValue(FilterDefinition.DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of a group which an be used to combine filters for a column (or other logical grouping).
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.GetValue(FilterDefinition.GroupNameProperty) as string;
            }

            set
            {
                this.SetValue(FilterDefinition.GroupNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image to be displayed for the filter item.
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return this.GetValue(FilterDefinition.IconProperty) as ImageSource;
            }

            set
            {
                this.SetValue(FilterDefinition.IconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the filter is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(FilterDefinition.IsEnabledProperty);
            }

            set
            {
                this.SetValue(FilterDefinition.IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the filter.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValue(FilterDefinition.NameProperty) as string;
            }

            set
            {
                this.SetValue(FilterDefinition.NameProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the IsEnabled property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsEnabledPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // When the filter is enabled or disabled, invoke a command describing the new filter.
            FilterDefinition filterDefinition = dependencyObject as FilterDefinition;
            FilterDescription filterDescription = new FilterDescription
            {
                GroupName = filterDefinition.GroupName,
                IsEnabled = filterDefinition.IsEnabled,
                Name = filterDefinition.Name
            };
            GlobalCommands.Filter.Execute(filterDescription);
        }
    }
}