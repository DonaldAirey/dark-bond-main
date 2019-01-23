// <copyright file="ItemsView.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using DarkBond.ViewModels;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// An items control with assignable views.
    /// </summary>
    [ContentProperty(Name = "Views")]
    [Bindable]
    public class ItemsView : SelectableListView
    {
        /// <summary>
        /// The Current dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register(
            "Current",
            typeof(ViewDefinition),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The HeaderVisibility dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.RegisterAttached(
            "HeaderVisibility",
            typeof(Visibility),
            typeof(ItemsView),
            new PropertyMetadata(default(Visibility)));

        /// <summary>
        /// The ItemMargin dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin",
            typeof(Thickness),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The SortOrder dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(
            "SortOrder",
            typeof(Collection<SortDescription>),
            typeof(ItemsView),
            new PropertyMetadata(null));

        /// <summary>
        /// The Orientation dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty MaximumRowsOrColumnsProperty = DependencyProperty.Register(
            "MaximumRowsOrColumns",
            typeof(int),
            typeof(ItemsView),
            new PropertyMetadata(-1, ItemsView.OnMaximumRowsOrColumnsPropertyChanged));

        /// <summary>
        /// The Orientation dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(ItemsView),
            new PropertyMetadata(Orientation.Horizontal, ItemsView.OnOrientationPropertyChanged));

        /// <summary>
        /// The View dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof(string),
            typeof(ItemsView),
            new PropertyMetadata(null, ItemsView.OnViewPropertyChanged));

        /// <summary>
        /// The Views DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ViewsProperty = DependencyProperty.Register(
            "Views",
            typeof(ViewDefinitionCollection),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The property path for the style.
        /// </summary>
        private static PropertyPath stylePropertyPath = new PropertyPath("Style");

        /// <summary>
        /// The property path for the items margin.
        /// </summary>
        private static PropertyPath itemMarginPropertyPath = new PropertyPath("ItemMargin");

        /// <summary>
        /// The property path for the item template.
        /// </summary>
        private static PropertyPath itemTemplatePropertyPath = new PropertyPath("ItemTemplate");

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsView"/> class.
        /// </summary>
        public ItemsView()
        {
            // Initialize the object.
            this.SetValue(ItemsView.ViewsProperty, new ViewDefinitionCollection());
            this.DefaultStyleKey = typeof(ItemsView);
            ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Enabled);
            ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Disabled);

            // This provides a default sort order for the view.
            this.SetValue(ItemsView.SortOrderProperty, new Collection<SortDescription>());

            // This handler is needed to switch between a grid view and a list view.
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public ViewDefinition Current
        {
            get
            {
                return this.GetValue(ItemsView.CurrentProperty) as ViewDefinition;
            }
        }

        /// <summary>
        /// Gets or sets the margin used to separate items.
        /// </summary>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)this.GetValue(ItemsView.ItemMarginProperty);
            }

            set
            {
                this.SetValue(ItemsView.ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximumRowsOrColumns of the items panel.
        /// </summary>
        public int MaximumRowsOrColumns
        {
            get
            {
                return (int)this.GetValue(ItemsView.MaximumRowsOrColumnsProperty);
            }

            set
            {
                this.SetValue(ItemsView.MaximumRowsOrColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the items panel.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(ItemsView.OrientationProperty);
            }

            set
            {
                this.SetValue(ItemsView.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets the default sort order.
        /// </summary>
        public Collection<SortDescription> SortOrder
        {
            get
            {
                return this.GetValue(ItemsView.SortOrderProperty) as Collection<SortDescription>;
            }
        }

        /// <summary>
        /// Gets or sets the name of the current view.
        /// </summary>
        public string View
        {
            get
            {
                return this.GetValue(ItemsView.ViewProperty) as string;
            }

            set
            {
                this.SetValue(ItemsView.ViewProperty, value);
            }
        }

        /// <summary>
        /// Gets a collection views used to display the data in the ItemsSource.
        /// </summary>
        public ViewDefinitionCollection Views
        {
            get
            {
                return this.GetValue(ItemsView.ViewsProperty) as ViewDefinitionCollection;
            }
        }

        /// <summary>
        /// Sets the visibility of the header.
        /// </summary>
        /// <param name="target">The target of the operation.</param>
        /// <returns>The visibility of the target.</returns>
        public static Visibility GetHeaderVisibility(DependencyObject target)
        {
            // Validate the target argument.
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return (Visibility)target.GetValue(ItemsView.HeaderVisibilityProperty);
        }

        /// <summary>
        /// Sets the visibility of the header.
        /// </summary>
        /// <param name="target">The target of the operation.</param>
        /// <param name="value">The new value for the property.</param>
        public static void SetHeaderVisibility(DependencyObject target, Visibility value)
        {
            // Validate the target argument.
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.SetValue(ItemsView.HeaderVisibilityProperty, value);
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // Clear the margin binding when cleaning up the container.
            ItemsViewItem itemsViewItem = element as ItemsViewItem;
            itemsViewItem.ClearValue(ItemsViewItem.MarginProperty);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            // This provides a container for all items in the view.
            return new ItemsViewItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Only ItemsViewItems can be handled by this view.
            return item is ItemsViewItem;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element that's used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            // Ask the base class to start the preparation for us.
            base.PrepareContainerForItemOverride(element, item);

            // The spacing between items in the panel is controlled by the definition of the view.
            ItemsViewItem itemsViewItem = element as ItemsViewItem;
            Binding itemMarginBinding = new Binding
            {
                Path = ItemsView.itemMarginPropertyPath,
                Source = this
            };
            itemsViewItem.SetBinding(ItemsViewItem.MarginProperty, itemMarginBinding);
        }

        /// <summary>
        /// Handles a change to the maximumRowsOrColumns of the items panel.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that tracks changes to the effective value of this property.</param>
        private static void OnMaximumRowsOrColumnsPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Extract the specific arguments from the generic ones.
            ItemsView itemsView = dependencyObject as ItemsView;
            int maximumRowsOrColumns = (int)dependencyPropertyChangedEventArgs.NewValue;

            // This should be a simple template binding, but there's no such option available for a setter property, so we'll have to change the
            // value through code.  Reconcile the change in maximum rows or columns to the panel.
            ItemsWrapGrid itemsWrapGrid = VisualTreeExtensions.FindChild<ItemsWrapGrid>(itemsView);
            if (itemsWrapGrid != null)
            {
                itemsWrapGrid.MaximumRowsOrColumns = maximumRowsOrColumns;
            }
        }

        /// <summary>
        /// Handles a change to the orientation of the items panel.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that tracks changes to the effective value of this property.</param>
        private static void OnOrientationPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Extract the specific arguments from the generic ones.
            ItemsView itemsView = dependencyObject as ItemsView;
            Orientation orientation = (Orientation)dependencyPropertyChangedEventArgs.NewValue;

            // This should be a simple template binding, but there's no such option available in WinRT for a setter property, so we'll have to change
            // the value through code.  Reconcile the change in orientation with the items panel.
            ItemsWrapGrid itemsWrapGrid = VisualTreeExtensions.FindChild<ItemsWrapGrid>(itemsView);
            if (itemsWrapGrid != null)
            {
                itemsWrapGrid.Orientation = orientation;
                if (orientation == Orientation.Horizontal)
                {
                    ScrollViewer.SetVerticalScrollMode(itemsView, ScrollMode.Enabled);
                    ScrollViewer.SetHorizontalScrollMode(itemsView, ScrollMode.Disabled);
                }
                else
                {
                    ScrollViewer.SetVerticalScrollMode(itemsView, ScrollMode.Disabled);
                    ScrollViewer.SetHorizontalScrollMode(itemsView, ScrollMode.Enabled);
                }
            }
        }

        /// <summary>
        /// Handles a change to the state of the IsProductView property.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that tracks changes to the effective value of this property.</param>
        private static void OnViewPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Extract the specific arguments from the generic ones.
            ItemsView itemsView = dependencyObject as ItemsView;
            string view = dependencyPropertyChangedEventArgs.NewValue as string;

            // The general idea here is to keep all the items in place, but change everything about the way they're presented.  That is, swap out the
            // style and the item template.  The first step to switching the views is to find the new view in the collection of views.
            ViewDefinition newView = itemsView.Views.FirstOrDefault<ViewDefinition>(vd => vd.Name == view);

            // This provides an easy reference to the currently displayed view.
            itemsView.SetValue(ItemsView.CurrentProperty, newView);

            // If a new view was selected then we're going to swap the major elements that describe the view.
            if (newView != null)
            {
                // Bind to the new item margin.  The item margin provides the space in between the items; not every view wants the same spacing.
                Binding itemMarginBinding = new Binding();
                itemMarginBinding.Path = ItemsView.itemMarginPropertyPath;
                itemMarginBinding.Source = newView;
                itemMarginBinding.Mode = BindingMode.OneWay;
                itemsView.SetBinding(ItemsView.ItemMarginProperty, itemMarginBinding);

                // Bind to the new item template.
                Binding itemTemplateBinding = new Binding();
                itemTemplateBinding.Path = ItemsView.itemTemplatePropertyPath;
                itemTemplateBinding.Source = newView;
                itemTemplateBinding.Mode = BindingMode.OneWay;
                itemsView.SetBinding(ItemsView.ItemTemplateProperty, itemTemplateBinding);

                // Bind to the new style.
                Binding styleBinding = new Binding();
                styleBinding.Path = ItemsView.stylePropertyPath;
                styleBinding.Source = newView;
                styleBinding.Mode = BindingMode.OneWay;
                itemsView.SetBinding(ItemsView.StyleProperty, styleBinding);
            }
            else
            {
                // Make sure we clear the bindings out when an empty view is selected.
                itemsView.ClearValue(ItemsView.ItemMarginProperty);
                itemsView.ClearValue(ItemsView.ItemsPanelProperty);
                itemsView.ClearValue(ItemsView.ItemTemplateProperty);
                itemsView.ClearValue(ItemsView.StyleProperty);
            }
        }

        /// <summary>
        /// Handles the loading of the window.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // When the window is first loaded, initialize the properties that are managed through code.  These should be done through binding, but
            // because they are properties of the items panel, and because the items panel is specified through a property setter rather than a
            // template, we need to do this all in code.
            ItemsWrapGrid itemsWrapGrid = VisualTreeExtensions.FindChild<ItemsWrapGrid>(this);
            if (itemsWrapGrid != null)
            {
                itemsWrapGrid.MaximumRowsOrColumns = this.MaximumRowsOrColumns;
                itemsWrapGrid.Orientation = this.Orientation;
            }

            // This will broadcast the default sort order to the view model.
            if (GlobalCommands.Sort.CanExecute(this.SortOrder))
            {
                GlobalCommands.Sort.Execute(this.SortOrder);
            }
        }
    }
}