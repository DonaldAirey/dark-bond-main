// <copyright file="ItemsView.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using ViewModels;

    /// <summary>
    /// An items control with assignable views.
    /// </summary>
    [ContentProperty("Views")]
    public class ItemsView : SelectableListBox
    {
        /// <summary>
        /// The Current dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register(
            "Current",
            typeof(ViewDefinition),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The ItemMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin",
            typeof(Thickness),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The SortOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(
            "SortOrder",
            typeof(Collection<SortDescription>),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The View dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof(string),
            typeof(ItemsView),
            new PropertyMetadata(null, ItemsView.OnViewPropertyChanged));

        /// <summary>
        /// The Views DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ViewsProperty = DependencyProperty.Register(
            "Views",
            typeof(ViewDefinitionCollection),
            typeof(ItemsView),
            null);

        /// <summary>
        /// The property path for the panel used to present the items.
        /// </summary>
        private static PropertyPath itemsPanelPropertyPath = new PropertyPath("ItemsPanel");

        /// <summary>
        /// The property path for the items margin.
        /// </summary>
        private static PropertyPath itemMarginPropertyPath = new PropertyPath("ItemMargin");

        /// <summary>
        /// The property path for the item template.
        /// </summary>
        private static PropertyPath itemTemplatePropertyPath = new PropertyPath("ItemTemplate");

        /// <summary>
        /// The property path for the style.
        /// </summary>
        private static PropertyPath stylePropertyPath = new PropertyPath("Style");

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsView"/> class.
        /// </summary>
        public ItemsView()
        {
            // Initialize the object.
            this.SetValue(ItemsView.ViewsProperty, new ViewDefinitionCollection());
            this.DefaultStyleKey = new ComponentResourceKey(typeof(ItemsView), "ItemsViewStyle");

            // Allow for multiple selections.
            this.SelectionMode = SelectionMode.Extended;

            // Initialize the sort order collection.  We need to be notified when the sort order changes.
            Collection<SortDescription> sortOrderList = new Collection<SortDescription>();
            this.SetValue(ItemsView.SortOrderProperty, sortOrderList);

            // This handler is needed to broadcast the sort order.
            this.Loaded += this.OnLoaded;

            // This handler is used to change the visual state of the realized children when the keyboard focus changes.
            this.IsKeyboardFocusWithinChanged += this.OnItemsViewIsKeyboardFocusWithinChanged;
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
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
        /// <value>
        /// The margin used to separate items.
        /// </value>
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
        /// <value>
        /// The name of the current view.
        /// </value>
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
        /// <value>
        /// A collection views used to display the data in the ItemsSource.
        /// </value>
        public ViewDefinitionCollection Views
        {
            get
            {
                return this.GetValue(ItemsView.ViewsProperty) as ViewDefinitionCollection;
            }
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
        /// Invoked when an unhandled <see cref="Keyboard.PreviewKeyDownEvent"/> attached event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Validate the argument.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // This view is kind of like Schrödinger's Cat: we don't know if an item is focused until it's realized.  If the view currently has the
            // focus, it means that either an item hasn't been selected yet, or that the selected item isn't realized.  So we wait for some activity.
            // If the user selects another view, then there's no need to realize the focused item and it remains more of a virtual focus.  However,
            // if the user hits a navigation key, then we need to realize the selected item by scrolling to it.  When a selected item is realized by
            // the ItemConainerGenerator, it will be focused.  Then we'll be able to navigate using the default logic for keyboard navigation.
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                this.ScrollIntoView(this.SelectedItem);
            }

            // Allow the base class to handle the rest of the event.
            base.OnPreviewKeyDown(e);
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

            // This will unload the previous view (if there was one).
            ViewDefinition previousView = itemsView.Current;
            if (previousView != null)
            {
                previousView.OnUnloaded();
            }

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
                Binding itemsPanelBinding = new Binding();
                itemsPanelBinding.Path = ItemsView.itemsPanelPropertyPath;
                itemsPanelBinding.Source = newView;
                itemsPanelBinding.Mode = BindingMode.OneWay;
                itemsView.SetBinding(ItemsView.ItemsPanelProperty, itemsPanelBinding);

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

                // This allows the view a chance to handle any additional initial processing before the view is displayed.
                newView.OnLoaded();
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
        /// Handles a change to the keyboard focus being within the control.
        /// </summary>
        /// <param name="sender">The object that created the event.</param>
        /// <param name="e">The event data.</param>
        private void OnItemsViewIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // This will tell each of the realized children that the keyboard focus has changed.  This, in turn, will allow them to update their
            // visual state, generally to provide a visual cue that the elements are actively selected or inactive.
            int children = this.ItemContainerGenerator.Items.Count;
            for (int index = 0; index < children; index++)
            {
                ItemsViewItem itemsViewItem = this.ItemContainerGenerator.ContainerFromIndex(index) as ItemsViewItem;
                if (itemsViewItem != null)
                {
                    itemsViewItem.UpdateState(this);
                }
            }
        }

        /// <summary>
        /// Handles the loading of the window.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // This will broadcast the default sort order to the view model.
            if (GlobalCommands.Sort.CanExecute(this.SortOrder))
            {
                GlobalCommands.Sort.Execute(this.SortOrder);
            }
        }
    }
}