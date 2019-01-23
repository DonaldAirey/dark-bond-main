// <copyright file="DirectoryViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;
    using Input;

    /// <summary>
    /// View Model for a directory that shows items in several views (list, detail, content, small, medium, large icons).
    /// </summary>
    public abstract class DirectoryViewModel : ViewModel
    {
        /// <summary>
        /// A collection of AppBar items.
        /// </summary>
        private ObservableCollection<IDisposable> appBarItems;

        /// <summary>
        /// Command to clears the selection from an item.
        /// </summary>
        private DelegateCommand clearSelection;

        /// <summary>
        /// Command to clear the selection from all items.
        /// </summary>
        private DelegateCommand clearSelectionAll;

        /// <summary>
        /// The context button items in the ribbon.
        /// </summary>
        private ObservableCollection<IDisposable> contextButtonItems;

        /// <summary>
        /// The items in the context menu.
        /// </summary>
        private ObservableCollection<IDisposable> contextMenuViewItems;

        /// <summary>
        /// The command handler for filtering.
        /// </summary>
        private DelegateCommand<FilterItemViewModel> filterCommand;

        /// <summary>
        /// The items displayed in this directory.
        /// </summary>
        private FilteredCollection<ListItemViewModel> itemsField = new FilteredCollection<ListItemViewModel>();

        /// <summary>
        /// Command to select all items.
        /// </summary>
        private DelegateCommand selectAll;

        /// <summary>
        /// Command to selects an item.
        /// </summary>
        private DelegateCommand select;

        /// <summary>
        /// The command handler for sorting.
        /// </summary>
        private DelegateCommand<Collection<SortDescription>> sortCommand;

        /// <summary>
        /// A value indicating whether commands to update the AppBar items are allowed when the selection changes.
        /// </summary>
        private bool isUpdateProhibited;

        /// <summary>
        /// The current view.
        /// </summary>
        private string viewField;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryViewModel"/> class.
        /// </summary>
        protected DirectoryViewModel()
        {
            // These commands are handled by this view model.
            this.selectAll = new DelegateCommand(this.SelectAll, this.CanSelectAll);
            this.clearSelection = new DelegateCommand(this.ClearSelection);
            this.clearSelectionAll = new DelegateCommand(this.ClearSelectionAll, this.CanClearSelectionAll);
            this.select = new DelegateCommand(this.Select);
            this.sortCommand = new DelegateCommand<Collection<SortDescription>>(this.Sort);
            this.filterCommand = new DelegateCommand<FilterItemViewModel>(this.Filter);

            // This will dispose of items removed from the directory.
            this.itemsField.CollectionChanged += this.OnItemCollectionChanged;
        }

        /// <summary>
        /// Gets the collection of items displayed in the global section of the appBar.
        /// </summary>
        public ObservableCollection<IDisposable> AppBar
        {
            get
            {
                if (this.appBarItems == null)
                {
                    this.appBarItems = this.CreateAppBarItems();
                }

                return this.appBarItems;
            }
        }

        /// <summary>
        /// Gets the number of items in this directory.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Gets the view models for the context buttons.
        /// </summary>
        public virtual ObservableCollection<IDisposable> ContextButtons
        {
            get
            {
                // Because the context button items can have managed resources, we're going to get them on demand.
                if (this.contextButtonItems == null)
                {
                    this.contextButtonItems = this.CreateContextButtonItems();
                }

                return this.contextButtonItems;
            }
        }

        /// <summary>
        /// Gets the view models for every item in the context menu.
        /// </summary>
        public virtual ObservableCollection<IDisposable> ContextMenu
        {
            get
            {
                // Because the context menu items can have managed resources, we're going to get them on demand.
                if (this.contextMenuViewItems == null)
                {
                    this.contextMenuViewItems = this.CreateContextMenuItems();
                }

                return this.contextMenuViewItems;
            }
        }

        /// <summary>
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets the items that are displayed in the directory.
        /// </summary>
        public FilteredCollection<ListItemViewModel> Items
        {
            get
            {
                return this.itemsField;
            }
        }

        /// <summary>
        /// Gets or sets the name of this item.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the number of selected items in the directory.
        /// </summary>
        public int SelectedItemCount
        {
            get
            {
                // This counts the number of selected items.
                int selectedItemCount = 0;
                foreach (ListItemViewModel listItemViewModel in this.Items)
                {
                    if (listItemViewModel.IsSelected)
                    {
                        selectedItemCount++;
                    }
                }

                return selectedItemCount;
            }
        }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        public string View
        {
            get
            {
                return this.viewField;
            }

            set
            {
                if (this.viewField != value)
                {
                    this.viewField = value;
                    this.OnPropertyChanged("View");
                }
            }
        }

        /// <summary>
        /// Gets the URI of this directory.
        /// </summary>
        public abstract Uri Uri { get; }

        /// <summary>
        /// Gets or sets the root URI for a path to navigate to this object.
        /// </summary>
        protected Uri RootUri { get; set; }

        /// <summary>
        /// Loads the resources for the given path.
        /// </summary>
        /// <param name="path">The path to display in the directory.</param>
        public virtual void Load(string path)
        {
            // When the directory is loaded it will handle these composite commands.
            GlobalCommands.ClearSelection.RegisterCommand(this.clearSelection);
            GlobalCommands.SelectNone.RegisterCommand(this.clearSelectionAll);
            GlobalCommands.Filter.RegisterCommand(this.filterCommand);
            GlobalCommands.Sort.RegisterCommand(this.sortCommand);
            GlobalCommands.Select.RegisterCommand(this.select);
            GlobalCommands.SelectAll.RegisterCommand(this.selectAll);

            // Once the directory has been cleared and a new set of context buttons has been generated we can install the buttons that manage the new
            // directory.
            GlobalCommands.SetGlobalAppBar.Execute(this.AppBar);

            // This is used to set the context of the details pane.
            GlobalCommands.Select.Execute(this);

            // This will update the global buttons to reflect the state of the newly loaded directory.
            DirectoryViewModel.UpdateClearSelectionAllCommand();
            DirectoryViewModel.UpdateSelectAllCommand();
        }

        /// <summary>
        /// Returns a string that represents the current instance.
        /// </summary>
        /// <returns>a string that represents the current instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Hosting {0}: {1}", this.GetType(), this.Items.Count);
        }

        /// <summary>
        /// Unloads the resources for the current directory.
        /// </summary>
        public virtual void Unload()
        {
            // When the directory is unloaded it needs to remove itself from the composite commands that we registered when the directory loaded.
            GlobalCommands.ClearSelection.UnregisterCommand(this.clearSelection);
            GlobalCommands.SelectNone.UnregisterCommand(this.clearSelectionAll);
            GlobalCommands.Filter.UnregisterCommand(this.filterCommand);
            GlobalCommands.Select.UnregisterCommand(this.select);
            GlobalCommands.SelectAll.UnregisterCommand(this.selectAll);
            GlobalCommands.Sort.UnregisterCommand(this.sortCommand);

            // All the selections are cleared when a new directory cleared.  This is critical to clear gang commands like 'Delete' which register
            // each of the selected items.
            this.ClearSelectionAll();

            // This will remove the managed resources allocated for the directory.  Note that the items themselves need to be removed individually in
            // order for the trigger to catch the event.  A 'Clear' will reset the collection and we lose any chance to dispose of the individual
            // items.
            foreach (ListItemViewModel listViewViewModel in this.itemsField.ToArray())
            {
                // Note that because the refresh is deferred, there will only be a single collection changed event issued at the end of this block of
                // code and it will be a RESET event.
                this.itemsField.Remove(listViewViewModel);
            }
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected virtual ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected virtual ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Create a list for the context menu items.
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected virtual ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Create a list for the context menu items.
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Creates the metadata items.
        /// </summary>
        /// <returns>The collection of metadata items.</returns>
        protected virtual ObservableCollection<IDisposable> CreateMetadataItems()
        {
            // Create a list for the context menu items.
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Select or toggle the sort order on the given column.
        /// </summary>
        /// <param name="sortDescriptions">The column on which to sort.</param>
        protected void Sort(Collection<SortDescription> sortDescriptions)
        {
            // Validate the argument.
            if (sortDescriptions == null)
            {
                throw new ArgumentNullException(nameof(sortDescriptions));
            }

            // For multiple sort levels, this will prevent each addition of a sort level from triggering an update of the viewed data.
            using (this.Items.View.DeferRefresh())
            {
                // Clear out the existing sort order from the view.
                this.Items.View.SortDescriptions.Clear();
                foreach (SortDescription sortDescription in sortDescriptions)
                {
                    this.Items.View.SortDescriptions.Add(sortDescription);
                }
            }
        }

        /// <summary>
        /// Updates the enabled status of the ClearSelectionAll composite command.
        /// </summary>
        private static void UpdateClearSelectionAllCommand()
        {
            // This will ask each of the component commands in the 'ClearSelectionAll' composite command to update their enabled status.
            foreach (ICommand command in GlobalCommands.SelectNone.RegisteredCommands)
            {
                DelegateCommand delegateCommand = command as DelegateCommand;
                if (delegateCommand != null)
                {
                    delegateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Updates the enabled status of the SelectAll composite command.
        /// </summary>
        private static void UpdateSelectAllCommand()
        {
            // This will ask each of the component commands in the 'SelectAll' composite command to update their enabled status.
            foreach (ICommand command in GlobalCommands.SelectAll.RegisteredCommands)
            {
                DelegateCommand delegateCommand = command as DelegateCommand;
                if (delegateCommand != null)
                {
                    delegateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Returns an indication of whether the 'ClearSelectionAll' command should be enabled.
        /// </summary>
        /// <returns>true if the 'ClearSelectionAll' command should be enabled.</returns>
        private bool CanClearSelectionAll()
        {
            // If any of the items are selected, then the 'ClearSelectionAll' command should be enabled.
            foreach (ListItemViewModel gridViewItemViewModel in this.Items)
            {
                if (gridViewItemViewModel.IsSelected)
                {
                    return true;
                }
            }

            // None of the items are selected so the 'ClearSelectionAll' command should be disabled.
            return false;
        }

        /// <summary>
        /// Returns an indication of whether the 'SelectAll' command should be enabled.
        /// </summary>
        /// <returns>true if the 'SelectAll' command should be enabled.</returns>
        private bool CanSelectAll()
        {
            // If any of the items are not selected, then the 'SelectAll' command should be enabled.
            foreach (ListItemViewModel gridViewItemViewModel in this.Items)
            {
                if (!gridViewItemViewModel.IsSelected)
                {
                    return true;
                }
            }

            // None of the items are selected so the 'SelectiAll' command should be disabled.
            return false;
        }

        /// <summary>
        /// Handles a request to clear the selection on a single items in the directory.
        /// </summary>
        private void ClearSelection()
        {
            if (!this.isUpdateProhibited)
            {
                // Note that the individual item has already been cleared because it is bound to the view.  However, we still must update the status of
                // the buttons as they are sensitive to how many items are selected.
                this.UpdateContextButtons();

                // The global commands need to be updated after every change in selection.
                DirectoryViewModel.UpdateClearSelectionAllCommand();
                DirectoryViewModel.UpdateSelectAllCommand();
            }

            // When none of the items in the directory is selected, the directory should be selected.
            if (this.SelectedItemCount == 0)
            {
                GlobalCommands.Select.Execute(this);
            }
        }

        /// <summary>
        /// Handles a request to clear the selection on all the items in the directory.
        /// </summary>
        private void ClearSelectionAll()
        {
            // Use the internal command to clear the selection.  This will prevent a massive feedback of individual 'ClearSelection' commands from
            // being regurgitated at us.
            try
            {
                this.isUpdateProhibited = true;

                // Un-select everything in the model.
                foreach (ListItemViewModel listItemViewModel in this.Items)
                {
                    if (listItemViewModel.IsSelected)
                    {
                        listItemViewModel.IsSelected = false;
                    }
                }
            }
            finally
            {
                this.isUpdateProhibited = false;
            }

            // The status of the context buttons and the select composite commands needs to be updated after every change to the selection state.
            this.UpdateContextButtons();
            DirectoryViewModel.UpdateClearSelectionAllCommand();
            DirectoryViewModel.UpdateSelectAllCommand();
        }

        /// <summary>
        /// Handles a filter command.
        /// </summary>
        /// <param name="filterItemViewModel">The view model of the filter.</param>
        private void Filter(FilterItemViewModel filterItemViewModel)
        {
        }

        /// <summary>
        /// Handles the CollectionChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Information about the event.</param>
        private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (ListItemViewModel itemViewModel in notifyCollectionChangedEventArgs.NewItems)
                    {
                        itemViewModel.Parent = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    foreach (ListItemViewModel itemViewModel in notifyCollectionChangedEventArgs.OldItems)
                    {
                        itemViewModel.Parent = null;
                        itemViewModel.Dispose();
                    }

                    break;
            }

            // Make sure that the view model is updated when the count changes.
            this.OnPropertyChanged("Count");

            // Whenever the set of items has changed, re-calculate the state of the buttons.
            this.UpdateContextButtons();
            DirectoryViewModel.UpdateClearSelectionAllCommand();
            DirectoryViewModel.UpdateSelectAllCommand();
        }

        /// <summary>
        /// Handles a request to set the selection on a single items in the directory.
        /// </summary>
        private void Select()
        {
            if (!this.isUpdateProhibited)
            {
                // Note that the individual item has already been set because it is bound to the view.  However, we still must update the status of the
                // buttons as they are sensitive to how many items are selected.
                this.UpdateContextButtons();

                // The global commands need to be updated after every change in selection.
                DirectoryViewModel.UpdateClearSelectionAllCommand();
                DirectoryViewModel.UpdateSelectAllCommand();
            }
        }

        /// <summary>
        /// Handles a request to set the selection on all the items in the directory.
        /// </summary>
        private void SelectAll()
        {
            // Use the internal command to clear the selection.  This will prevent a massive feedback of individual 'Select' commands from being
            // regurgitated at us.
            try
            {
                this.isUpdateProhibited = true;

                // Select everything in the model.
                foreach (ListItemViewModel listItemViewModel in this.Items)
                {
                    if (!listItemViewModel.IsSelected)
                    {
                        listItemViewModel.IsSelected = true;
                    }
                }
            }
            finally
            {
                this.isUpdateProhibited = false;
            }

            // The status of the context buttons and the select composite commands needs to be updated after every change to the selection state.
            this.UpdateContextButtons();
            DirectoryViewModel.UpdateClearSelectionAllCommand();
            DirectoryViewModel.UpdateSelectAllCommand();
        }

        /// <summary>
        /// Updates the state of the context buttons.
        /// </summary>
        private void UpdateContextButtons()
        {
            // This is the list of buttons that are common to all selected items in the directory.
            ObservableCollection<IDisposable> commonAppBarItems = new ObservableCollection<IDisposable>();

            // The general idea is to create a set of AppBar items using the first selected item and then throw away the ones that are not common to
            // all the other selected items.  At the end of this loop, only those buttons that have common commands will be left.
            foreach (ListItemViewModel listItemsViewModel in this.Items)
            {
                // Only include buttons from the items that are selected.
                if (listItemsViewModel.IsSelected)
                {
                    // The first selected directory item provides the seed for the list.
                    if (commonAppBarItems.Count == 0)
                    {
                        foreach (IDisposable appBarItem in listItemsViewModel.AppBar)
                        {
                            commonAppBarItems.Add(appBarItem);
                        }
                    }
                    else
                    {
                        // If an item in the common set of AppBar items doesn't belong to the currently selected item then it is discarded.
                        for (int index = 0; index < commonAppBarItems.Count;)
                        {
                            // This is the item in the common set of AppBar items that we'll compare to the next selected item.
                            IDisposable commonAppBarItem = commonAppBarItems[index];

                            // We're going to discard anything that's not a button when there are multiple items selected.
                            ButtonViewModel commonButton = commonAppBarItem as ButtonViewModel;
                            if (commonButton == null)
                            {
                                // The AppBar can host a variety of controls.  However, if it's not a button then it can't belong in a multiple
                                // selection.
                                commonAppBarItems.Remove(commonAppBarItem);
                            }
                            else
                            {
                                // At this point, we've got a valid button and are going to look to see if our common button exists here.
                                bool found = false;
                                foreach (IDisposable appBarItem in listItemsViewModel.AppBar)
                                {
                                    ButtonViewModel itemButton = appBarItem as ButtonViewModel;
                                    if (itemButton != null)
                                    {
                                        if (commonButton.Command == itemButton.Command && commonButton.CommandParameter == itemButton.CommandParameter)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                // If the common button was found in the current item in the directory, then it can remain common.  If not, then it
                                // is removed from the collection.
                                if (found)
                                {
                                    index++;
                                }
                                else
                                {
                                    commonAppBarItems.Remove(commonAppBarItem);
                                }
                            }
                        }
                    }
                }
            }

            // At this point we have our list of common items (or no items at all).  This will instruct the frame to place them in the section of the
            // AppBar that hosts the context sensitive items.
            GlobalCommands.SetContextAppBar.Execute(commonAppBarItems);

            // This will open or close the AppBar depending on whether there are any context items to display.
            GlobalCommands.OpenAppBar.Execute(commonAppBarItems.Count != 0);
        }
    }
}