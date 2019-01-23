// <copyright file="UnderwriterViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Directories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.SubscriptionManager;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.SubscriptionManager.ViewModels.ListViews;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for managing the properties of a underwriter.
    /// </summary>
    public class UnderwriterViewModel : CommonDirectoryViewModel
    {
        /// <summary>
        /// The unique identifier of the underwriter that owns the items in this directory.
        /// </summary>
        private Guid underwriterIdField;

        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// The name of the underwriter.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> notifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        public UnderwriterViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object.
            this.ImageKey = ImageKeys.Customer;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Gets the unique identifier of the underwriter that owns the item in this directory.
        /// </summary>
        public Guid UnderwriterId
        {
            get
            {
                return this.underwriterIdField;
            }
        }

        /// <summary>
        /// Gets or sets the name of the underwriter.
        /// </summary>
        public new string Name
        {
            get
            {
                return this.nameField;
            }

            protected set
            {
                if (this.nameField != value)
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the URI of this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder(this.RootUri);
                uriBuilder.Query = @"path=\" + Resources.ApplicationName + @"\" + Resources.Customer;
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Loads the resources for this directory.
        /// </summary>
        /// <param name="path">The path to be displayed in the directory.</param>
        public override void Load(string path)
        {
            // Validate the parameter
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            // This will keep the count of subscriptions reconciled to the data model.
            this.DataModel.Subscription.CollectionChanged += this.OnCollectionChanged;

            // Pull the underwriter ID out of the path.
            string[] parts = path.Split('\\');
            this.underwriterIdField = Guid.Parse(parts[3]);

            // Reject the navigation operation if the user doesn't exist.
            this.underwriterRowField = this.DataModel.UnderwriterKey.Find(this.underwriterIdField);
            if (this.underwriterRowField == null)
            {
                throw new ArgumentException(Errors.CustomerNotFound);
            }

            // Hook into the changes for this row.
            this.underwriterRowField.PropertyChanged += this.OnUnderwriterRowChanged;

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.underwriterRowField);
            }

            // This initialize the view model with the existing subscriptions.
            var newSubscriptions = this.DataModel.UnderwriterSubscriptionUnderwriterIdKey.GetSubscriptionRows(this.underwriterIdField);
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newSubscriptions));

            // Allow the base class to finish loading the view model.
            base.Load(path);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Unload()
        {
            // This will disengage from the data model and clear the managed resources.
            this.DataModel.Subscription.CollectionChanged -= this.OnCollectionChanged;

            // Clear the view model of all the children.
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            // Allow the base class to dispose of the resources.
            base.Unload();
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            ObservableCollection<IDisposable> appBarItems = base.CreateAppBarItems();

            // The New License button.
            ButtonViewModel newLicenseButton = this.CompositionContext.GetExport<ButtonViewModel>();
            newLicenseButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomerLicense(this.UnderwriterId));
            newLicenseButton.Label = Resources.NewLicense;
            newLicenseButton.ImageKey = ImageKeys.License;
            appBarItems.Insert(0, newLicenseButton);

            // The Update Customer button.
            ButtonViewModel updateCustomerButton = this.CompositionContext.GetExport<ButtonViewModel>();
            updateCustomerButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer(this.UnderwriterId));
            updateCustomerButton.Label = Resources.UpdateCustomer;
            updateCustomerButton.ImageKey = ImageKeys.Customer;
            appBarItems.Insert(0, updateCustomerButton);

            // This is the set of appBar items for this ListView item.
            return appBarItems;
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextButtonViewItems = base.CreateContextButtonItems();

            // Open Button
            ButtonViewModel openButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            openButtonItem.Command = new DelegateCommand(() => { }, () => false);
            openButtonItem.Header = Resources.Open;
            openButtonItem.ImageKey = ImageKeys.Open;
            contextButtonViewItems.Add(openButtonItem);

            // New Button
            DropDownButtonViewModel newButtonItem = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // New License Menu Item
            DropDownButtonViewModel newLicenseButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newLicenseButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomerLicense(this.UnderwriterId));
            newLicenseButton.Header = Resources.NewLicense;
            newLicenseButton.ImageKey = ImageKeys.License;
            newButtonItem.Items.Add(newLicenseButton);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteCustomerAsync(this.UnderwriterId),
                () => this.SubscriptionService.CanDeleteCustomer(this.UnderwriterId));
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButton = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer(this.UnderwriterId));
            propertiesButton.Header = Resources.Properties;
            propertiesButton.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButton);

            // This is the context menu.
            return contextButtonViewItems;
        }

        /// <summary>
        /// Creates a context menu for this item.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // New License Menu Item
            MenuItemViewModel newLicenseMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newLicenseMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomerLicense(this.UnderwriterId));
            newLicenseMenuItem.Header = Resources.NewLicense;
            newLicenseMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newLicenseMenuItem);

            // Delete Menu Item
            MenuItemViewModel deleteCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteCustomerMenuItem.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteCustomerAsync(this.UnderwriterId),
                () => this.SubscriptionService.CanDeleteCustomer(this.UnderwriterId));
            deleteCustomerMenuItem.Header = Resources.Delete;
            deleteCustomerMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteCustomerMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer(this.UnderwriterId));
            propertiesMenuItem.Header = Resources.Properties;
            propertiesMenuItem.ImageKey = Resources.Properties;
            contextMenuViewItems.Add(propertiesMenuItem);

            // This is the context menu.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disconnect from the data model.
            this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle the RowChanged events of a DataModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">A DataModel that contains the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // If we remove a selected list item we want to select the directory so the details don't get messed up with a ghost object.
            bool resetSelection = false;

            // Deferring updates to the view will allow us to add a bulk of items without triggering a refresh for each item.
            using (this.Items.View.DeferRefresh())
            {
                // Handle the change actions.
                switch (notifyCollectionChangedEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Reset:

                        // This will dispose of all the children.
                        foreach (SubscriptionViewModel subscriptionViewModel in this.Items)
                        {
                            subscriptionViewModel.Dispose();
                        }

                        this.Items.Clear();

                        break;

                    case NotifyCollectionChangedAction.Add:

                        // Add a new SubscriptionViewModel for every new row in the data model.
                        foreach (SubscriptionRow subscriptionRow in notifyCollectionChangedEventArgs.NewItems)
                        {
                            // Only add subscriptions belonging to this underwriter.
                            if (subscriptionRow.UnderwriterId == this.UnderwriterId)
                            {
                                // Create a new view model for the new record and copy the data from the data model.
                                SubscriptionViewModel subscriptionViewModel = this.CompositionContext.GetExport<SubscriptionViewModel>();
                                subscriptionViewModel.Map(subscriptionRow);
                                subscriptionViewModel.IsUnderwriterView = true;

                                // This will order the children so it's easy to find them individually and delete them.
                                int index = this.Items.BinarySearch((ivm) => ivm.SortKey, subscriptionViewModel.SubscriptionId);
                                this.Items.Insert(~index, subscriptionViewModel);
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:

                        // This will disengage the child view models from the data model.
                        foreach (SubscriptionRow subscriptionRow in notifyCollectionChangedEventArgs.OldItems)
                        {
                            // Find the item and disengage it from the data model updates before deleting it from the view model.
                            int index = this.Items.BinarySearch((ivm) => ivm.SortKey, subscriptionRow[DataRowVersion.Original].SubscriptionId);
                            if (index >= 0)
                            {
                                // If the item is selected, then we want to reset the selection to the directory.
                                ListItemViewModel listItemViewModel = this.Items[index] as ListItemViewModel;
                                if (listItemViewModel.IsSelected)
                                {
                                    resetSelection = true;
                                }

                                listItemViewModel.Dispose();
                                this.Items.RemoveAt(index);
                            }
                        }

                        break;
                }

                // If all the items in the directory are deleted, then make sure that the directory is selected.
                if (resetSelection)
                {
                    GlobalCommands.Select.Execute(this);
                }
            }
        }

        /// <summary>
        /// Handles a change to the data model underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<UnderwriterRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.underwriterRowField);
            }
        }
    }
}