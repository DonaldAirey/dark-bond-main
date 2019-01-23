// <copyright file="ViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// Base class for all views that can present items in an <see cref="ItemsView"/>.
    /// </summary>
    [ContentProperty(Name = "ItemTemplate")]
    public class ViewDefinition : DependencyObject
    {
        /// <summary>
        /// Identifies the ItemContainerStyle DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            "ItemContainerStyle",
            typeof(Style),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// The ItemMargin dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin",
            typeof(Thickness),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the ItemTemplate DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the Name DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof(string),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the Style DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register(
            "Style",
            typeof(Style),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Gets or sets the style of the item containers.
        /// </summary>
        public Style ItemContainerStyle
        {
            get
            {
                return this.GetValue(ViewDefinition.ItemContainerStyleProperty) as Style;
            }

            set
            {
                this.SetValue(ViewDefinition.ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the margin used to separate items.
        /// </summary>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)this.GetValue(ViewDefinition.ItemMarginProperty);
            }

            set
            {
                this.SetValue(ViewDefinition.ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template used to present the items.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return this.GetValue(ViewDefinition.ItemTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(ViewDefinition.ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValue(ViewDefinition.NameProperty) as string;
            }

            set
            {
                this.SetValue(ViewDefinition.NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the view.
        /// </summary>
        public Style Style
        {
            get
            {
                return this.GetValue(ViewDefinition.StyleProperty) as Style;
            }

            set
            {
                this.SetValue(ViewDefinition.StyleProperty, value);
            }
        }
    }
}