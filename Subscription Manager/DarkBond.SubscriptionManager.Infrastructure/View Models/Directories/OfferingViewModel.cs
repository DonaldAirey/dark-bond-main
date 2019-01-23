// <copyright file="OfferingViewModel.cs" company="Dark Bond, Inc.">
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
    /// View model for managing the properties of a offering.
    /// </summary>
    public class OfferingViewModel : CommonDirectoryViewModel
    {
        /// <summary>
        /// The name of the offering.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The unique identifier of the offering that owns the items in this directory.
        /// </summary>
        private Guid offeringIdField;

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> notifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        public OfferingViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object.
            this.ImageKey = ImageKeys.Product;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Gets the unique identifier of the offering that owns the item in this directory.
        /// </summary>
        public Guid OfferingId
        {
            get
            {
                return this.offeringIdField;
            }
        }

        /// <summary>
        /// Gets or sets the name of the offering.
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
                uriBuilder.Query = @"path=\" + Resources.ApplicationName + @"\" + Resources.Product;
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

            // This will keep the list of subscriptions reconciled to the data model.
            this.DataModel.Subscription.CollectionChanged += this.OnCollectionChanged;

            // Pull the offering ID out of the path.
            string[] parts = path.Split('\\');
            this.offeringIdField = Guid.Parse(parts[3]);

            // Reject the navigation operation if the user doesn't exist.
            this.offeringRowField = this.DataModel.OfferingKey.Find(this.offeringIdField);
            if (this.offeringRowField == null)
            {
                throw new ArgumentException(Errors.ProductNotFound);
            }

            // Hook into the changes for this row.
            this.offeringRowField.PropertyChanged += this.OnOfferingRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.offeringRowField);
            }

            // This initialize the view model with the existing subscriptions.
            var newSubscriptions = this.DataModel.OfferingSubscriptionOfferingIdKey.GetSubscriptionRows(this.offeringIdField);
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
            newLicenseButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProductLicense(this.OfferingId));
            newLicenseButton.Label = Resources.NewLicense;
            newLicenseButton.ImageKey = ImageKeys.License;
            appBarItems.Insert(0, newLicenseButton);

            // The Update Product button.
            ButtonViewModel updateProductButton = this.CompositionContext.GetExport<ButtonViewModel>();
            updateProductButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.OfferingId));
            updateProductButton.Label = Resources.UpdateProduct;
            updateProductButton.ImageKey = ImageKeys.Product;
            appBarItems.Insert(0, updateProductButton);

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
            newLicenseButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProductLicense(this.OfferingId));
            newLicenseButton.Header = Resources.NewLicense;
            newLicenseButton.ImageKey = ImageKeys.License;
            newButtonItem.Items.Add(newLicenseButton);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteProductAsync(this.OfferingId),
                () => this.SubscriptionService.CanDeleteProduct(this.OfferingId));
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButton = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.OfferingId));
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
            // Create a context menu for this element.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // New License Menu Item
            MenuItemViewModel newLicenseMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newLicenseMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProductLicense(this.OfferingId));
            newLicenseMenuItem.Header = Resources.NewLicense;
            newLicenseMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newLicenseMenuItem);

            // Delete Menu Item
            MenuItemViewModel deleteProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteProductMenuItem.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteProductAsync(this.OfferingId),
                () => this.SubscriptionService.CanDeleteProduct(this.OfferingId));
            deleteProductMenuItem.Header = Resources.Delete;
            deleteProductMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteProductMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.OfferingId));
            propertiesMenuItem.Header = Resources.Properties;
            propertiesMenuItem.ImageKey = ImageKeys.Properties;
            contextMenuViewItems.Add(propertiesMenuItem);

            // The context menu.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Handle the RowChanged events of a DataModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">A DataModel that contains the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
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
                            // Only add subscriptions belonging to this offering.
                            if (subscriptionRow.OfferingId == this.OfferingId)
                            {
                                // Create a new view model for the new record and copy the data from the data model.
                                SubscriptionViewModel subscriptionViewModel = this.CompositionContext.GetExport<SubscriptionViewModel>();
                                subscriptionViewModel.Map(subscriptionRow);
                                subscriptionViewModel.IsOfferingView = true;

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
                                this.Items[index].Dispose();
                                this.Items.RemoveAt(index);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Handles a change to the data model offering row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnOfferingRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<OfferingRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.offeringRowField);
            }
        }
    }
}