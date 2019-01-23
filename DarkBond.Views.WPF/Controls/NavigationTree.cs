// <copyright file="NavigationTree.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Allows a user to select from a hierarchy of items.
    /// </summary>
    public class NavigationTree : TreeView
    {
        /// <summary>
        /// Identifies the ExpanderOpacity dependency property key.
        /// </summary>
        public static readonly DependencyProperty ExpanderOpacityProperty = DependencyProperty.Register(
            "ExpanderOpacity",
            typeof(double),
            typeof(NavigationTree),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the Indent dependency property.
        /// </summary>
        public static readonly DependencyProperty IndentProperty = DependencyProperty.Register(
            "Indent",
            typeof(double),
            typeof(NavigationTree),
            new FrameworkPropertyMetadata(10.0));

        /// <summary>
        /// Initializes static members of the <see cref="NavigationTree"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "No other way to initialize metadata.")]
        static NavigationTree()
        {
            // Override the default style used for this control class.
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationTree), new FrameworkPropertyMetadata(typeof(NavigationTree)));

            // The horizontal and vertical scroll bars are automatic for this control class.
            ScrollViewer.HorizontalScrollBarVisibilityProperty.OverrideMetadata(
                typeof(NavigationTree),
                new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
            ScrollViewer.VerticalScrollBarVisibilityProperty.OverrideMetadata(
                typeof(NavigationTree),
                new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTree"/> class.
        /// </summary>
        public NavigationTree()
        {
            // Register the command handlers that take care of expanding and collapsing the hierarchy nodes.
            GlobalCommands.Collapse.RegisterCommand(new DelegateCommand<NavigationTreeItemViewModel>(this.OnCollapseNode));
            GlobalCommands.Expand.RegisterCommand(new DelegateCommand<NavigationTreeItemViewModel>(this.OnExpandNode));
        }

        /// <summary>
        /// Gets the opacity for the expander buttons on the child TreeViewItem elements.
        /// </summary>
        public double ExpanderOpacity
        {
            get
            {
                return (double)this.GetValue(NavigationTree.ExpanderOpacityProperty);
            }
        }

        /// <summary>
        /// Gets or sets the amount of space that each child is indented from the parent.
        /// </summary>
        public double Indent
        {
            get
            {
                return (double)this.GetValue(NavigationTree.IndentProperty);
            }

            set
            {
                this.SetValue(NavigationTree.IndentProperty, value);
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            // Only NavigationTreeItem are displayed in the selector.
            return new NavigationTreeItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Only NavigationTreeItem can be considered containers for items in this selector.
            return item is NavigationTreeItem;
        }

        /// <summary>
        /// Provides class handling for the KeyDown event for a HierarchyView.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // This will open the item that generated the event.
            if (e.Key == Key.Enter)
            {
                FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
                NavigationTreeItemViewModel treeViewItemViewModel = frameworkElement.DataContext as NavigationTreeItemViewModel;
                GlobalCommands.Locate.Execute(treeViewItemViewModel.Uri);
            }

            // The base class does a good job with the rest of the navigation keys.
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the MousedoubleClick routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // A single click will open the item.
            if (e.ClickCount == 1)
            {
                // Extract the view model from the element generating the event.
                FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
                NavigationTreeItemViewModel treeViewItemViewModel = frameworkElement.DataContext as NavigationTreeItemViewModel;

                // Open the selected view model.
                if (treeViewItemViewModel != null)
                {
                    GlobalCommands.Locate.Execute(treeViewItemViewModel.Uri);
                }
            }

            // Allow the base class to handle the rest of the event.
            base.OnMouseLeftButtonUp(e);
        }

        /// <inheritdoc/>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Validate the parameter
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // When this control gets the focus, select the first item in the tree.
            if (e.NewFocus == this && this.ItemContainerGenerator.Items.Count > 0)
            {
                NavigationTreeItem navigationTreeItem = this.ItemContainerGenerator.ContainerFromIndex(0) as NavigationTreeItem;
                navigationTreeItem.Focus();
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this FrameworkElement has been updated.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // If either the keyboard focus is moved into the navigator or the mouse is over it the expander buttons on the TreeViewItems will light
            // up.  That is, the opacity of the expander buttons is controlled by these two properties.  This does not directly control the expander
            // buttons but provides a property that they can bind to through the visual tree hierarchy.  It also saves on Storyboards as only one
            // animation needs to be created for all the buttons that might appear in a heavily populated navigation hierarchy.
            bool turnOn = false;
            bool turnOff = false;

            // Evaluate whether the focus change will make the expander buttons visible or not.
            if (e.Property == UIElement.IsKeyboardFocusWithinProperty)
            {
                turnOn = (bool)e.NewValue;
                turnOff = !(bool)e.NewValue && !this.IsMouseOver;
            }

            // Evaluate whether the mouse hovering over the control will make the expander buttons visible or not.
            if (e.Property == UIElement.IsMouseOverProperty)
            {
                turnOn = (bool)e.NewValue;
                turnOff = !(bool)e.NewValue && !this.IsKeyboardFocusWithin;
            }

            // This animation will make the expander buttons visible.
            if (turnOn)
            {
                this.BeginAnimation(NavigationTree.ExpanderOpacityProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250)));
            }

            // This animation will make the expander buttons invisible.
            if (turnOff)
            {
                this.BeginAnimation(NavigationTree.ExpanderOpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000)));
            }

            // Allow the base class to handle the rest of the property changes.
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Handles a command to collapse a node.
        /// </summary>
        /// <param name="treeViewItemViewModel">The target node.</param>
        private void OnCollapseNode(NavigationTreeItemViewModel treeViewItemViewModel)
        {
            // Collapse the node.
            treeViewItemViewModel.IsExpanded = false;
        }

        /// <summary>
        /// Handles a command to expand a node.
        /// </summary>
        /// <param name="treeViewItemViewModel">The target node.</param>
        private void OnExpandNode(NavigationTreeItemViewModel treeViewItemViewModel)
        {
            // Expand the node.
            treeViewItemViewModel.IsExpanded = true;
        }
    }
}