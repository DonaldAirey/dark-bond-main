// <copyright file="SelectableListBox.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using DarkBond.ViewModels;

    /// <summary>
    /// A ListView that allows items to be selected.
    /// </summary>
    public abstract class SelectableListBox : ListBox
    {
        /// <summary>
        /// Binds to the 'IsSelected' property of an item in the view.
        /// </summary>
        private static Binding isSelectedBinding = new Binding()
        {
            Path = new PropertyPath("IsSelected"),
            Mode = BindingMode.TwoWay
        };

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // Remove the connection binding between the control and it's view model.  Then remove the reference to the view model.  This just makes
            // it easier to garbage collect these items when they are no longer referenced.
            ListBoxItem listBoxItem = element as ListBoxItem;
            listBoxItem.ClearValue(ListBoxItem.IsSelectedProperty);

            // This disengages the Tapped event from the view model.
            listBoxItem.MouseDoubleClick -= this.OnMouseDoubleClick;

            // Allow the base class to clear the item as well.
            base.ClearContainerForItemOverride(element, item);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Validate the parameter
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // This sucks, but there doesn't seem to be another way to handle it.  We want to detect a single mouse click in the empty space of a
            // list box.  This is complicated by the fact that double-clicking on an item in the list box will navigate to that item.  The rub is
            // that the mouse button up can be either part of the single click (that we want to catch) or a double click (that we want to ignore).
            // The solution is to find out if an item in the list box has been selected (it will have a container).  If it doesn't have a container
            // we assume that it's part of the empty area in the list box.  But way, there's more.  The second click of a double click will no longer
            // reference the original source and the data context of this control will be empty because we're in the process of navigating.  So,
            // finally, check for a null data context.
            DependencyObject dependencyObject = ItemsControl.ContainerFromElement(this, e.OriginalSource as DependencyObject);
            if (dependencyObject == null && this.DataContext != null)
            {
                GlobalCommands.SelectNone.Execute(null);
                GlobalCommands.Select.Execute(this.DataContext);
            }

            // Allow the base class to handle the rest.
            base.OnMouseLeftButtonUp(e);
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

            // This will bind the 'IsSelected' of the view to the view model using two way binding.  The view model will be notified when the
            // selection changes and generally display or dismiss the AppBar.
            ListBoxItem listBoxItem = element as ListBoxItem;
            BindingOperations.SetBinding(listBoxItem, ListBoxItem.IsSelectedProperty, SelectableListBox.isSelectedBinding);

            // When the item is tapped we need to respond to it.  However, the action of tapping is an event and MVVM doesn't have a good method of
            // dealing with events.  This will turn the 'Tapped' event into a command to which the view model can respond.
            listBoxItem.MouseDoubleClick += this.OnMouseDoubleClick;
            listBoxItem.DataContext = item;
        }

        /// <summary>
        /// Occurs when an item is double clicked.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="mouseButtonEventArgs">The event data.</param>
        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // This connects the Tapped event from the control with the view model which actually handles the action.
            ListItemViewModel itemViewModel = ((ListBoxItem)sender).DataContext as ListItemViewModel;
            if (itemViewModel != null)
            {
                await itemViewModel.Tapped.Execute();
            }
        }
    }
}