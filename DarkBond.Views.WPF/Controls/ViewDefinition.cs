// <copyright file="ViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    /// <summary>
    /// Base class for all views that can present items in an <see cref="ItemsView"/>.
    /// </summary>
    [ContentProperty("ItemTemplate")]
    public class ViewDefinition : DependencyObject
    {
        /// <summary>
        /// Identifies the ItemContainerStyle DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            "ItemContainerStyle",
            typeof(Style),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// The ItemMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin",
            typeof(Thickness),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the ItemsPanel DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty = DependencyProperty.Register(
            "ItemsPanel",
            typeof(ItemsPanelTemplate),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the ItemTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the Name DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof(string),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Identifies the Style DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register(
            "Style",
            typeof(Style),
            typeof(ViewDefinition),
            null);

        /// <summary>
        /// Gets or sets the style of the item containers.
        /// </summary>
        /// <value>
        /// The style of the item containers.
        /// </value>
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
        /// <value>
        /// The margin used to separate items.
        /// </value>
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
        /// Gets or sets the panel used to display the items.
        /// </summary>
        /// <value>
        /// The panel used to display the items.
        /// </value>
        public ItemsPanelTemplate ItemsPanel
        {
            get
            {
                return this.GetValue(ViewDefinition.ItemsPanelProperty) as ItemsPanelTemplate;
            }

            set
            {
                this.SetValue(ViewDefinition.ItemsPanelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template used to present the items.
        /// </summary>
        /// <value>
        /// The template used to present the items.
        /// </value>
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
        /// <value>
        /// The name of the view.
        /// </value>
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
        /// <value>
        /// The style of the view.
        /// </value>
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

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        public virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        public virtual void OnUnloaded()
        {
        }
    }
}