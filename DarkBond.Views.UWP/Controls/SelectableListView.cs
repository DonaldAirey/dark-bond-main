// <copyright file="SelectableListView.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using DarkBond.ViewModels;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// A ListView that allows items to be selected.
    /// </summary>
    public abstract class SelectableListView : ListView
    {
        /// <summary>
        /// Binds to the 'IsSelected' property of an item in the view.
        /// </summary>
        private static Binding isSelectedBinding = new Binding()
        {
            Path = new PropertyPath("IsSelected"), Mode = BindingMode.TwoWay
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
            ListViewItem listViewItem = element as ListViewItem;
            listViewItem.ClearValue(ListViewItem.IsSelectedProperty);

            // This disengages the Tapped event from the view model.
            listViewItem.Tapped -= this.OnTapped;

            // Allow the base class to clear the item as well.
            base.ClearContainerForItemOverride(element, item);
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
            ListViewItem listViewItem = element as ListViewItem;
            BindingOperations.SetBinding(listViewItem, ListViewItem.IsSelectedProperty, SelectableListView.isSelectedBinding);

            // When the item is tapped we need to respond to it.  However, the action of tapping is an event and MVVM doesn't have a good method of
            // dealing with events.  This will turn the 'Tapped' event into a command to which the view model can respond.
            listViewItem.Tapped += this.OnTapped;
            listViewItem.DataContext = item;
        }

        /// <summary>
        /// Occurs when an item is tapped.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="tappedRoutedEventArgs">The event data.</param>
        private async void OnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            // This connects the Tapped event from the control with the view model which actually handles the action.
            ListItemViewModel itemViewModel = ((ListViewItem)sender).DataContext as ListItemViewModel;
            if (itemViewModel != null)
            {
                await itemViewModel.Tapped.Execute();
            }
        }
    }
}