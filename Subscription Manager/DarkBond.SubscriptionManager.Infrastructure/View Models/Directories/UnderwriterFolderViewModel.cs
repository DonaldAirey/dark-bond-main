// <copyright file="UnderwriterFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Directories
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View Model for the directory that shows the underwriters.
    /// </summary>
    public class UnderwriterFolderViewModel : CommonDirectoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        public UnderwriterFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object.
            this.ImageKey = ImageKeys.Folder;
            this.Name = Resources.Customer;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
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
        /// Loads the resources for this view model.
        /// </summary>
        /// <param name="path">The path for this view model.</param>
        public override void Load(string path)
        {
            // This will keep the view models of underwriters reconciled to the data model.
            this.DataModel.Underwriter.CollectionChanged += this.OnCollectionChanged;

            // This will initialize the collection of underwriters in this directory from the data model.  Building a view can be expensive, so disable
            // the automatic refreshes during the bulk operation.
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.DataModel.Underwriter));

            // Allow the base class to finish loading the view model.
            base.Load(path);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Unload()
        {
            // This will disengage from the data model and clear the managed resources.
            this.DataModel.Underwriter.CollectionChanged -= this.OnCollectionChanged;

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
            // Create the AppBar from the common set of items.
            ObservableCollection<IDisposable> appBarItems = base.CreateAppBarItems();

            // New Customer
            ButtonViewModel newCustomerButton = this.CompositionContext.GetExport<ButtonViewModel>();
            newCustomerButton.Command = new DelegateCommand(() => { this.SubscriptionService.NavigateToCustomer(); });
            newCustomerButton.Label = Resources.NewCustomer;
            newCustomerButton.ImageKey = ImageKeys.Customer;
            appBarItems.Insert(0, newCustomerButton);

            // These are the AppBar items.
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

            // New Customer Menu Item
            DropDownButtonViewModel newCustomerButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newCustomerButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer());
            newCustomerButton.Header = Resources.Customer;
            newCustomerButton.ImageKey = ImageKeys.Customer;
            newButtonItem.Items.Add(newCustomerButton);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = new DelegateCommand(() => { }, () => false);
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButton = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButton.Command = new DelegateCommand(() => { }, () => false);
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
            // Create the context menu from the common set of items.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // New Customer
            MenuItemViewModel newCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newCustomerMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer());
            newCustomerMenuItem.Header = Resources.NewCustomer;
            newCustomerMenuItem.ImageKey = ImageKeys.Customer;
            contextMenuViewItems.Add(newCustomerMenuItem);

            // These are the context menu items.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Handle the RowChanged events of a CustomerDataSet.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
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
                        foreach (ListViews.UnderwriterViewModel countryViewModel in this.Items)
                        {
                            countryViewModel.Dispose();
                        }

                        this.Items.Clear();

                        break;

                    case NotifyCollectionChangedAction.Add:

                        // This will create a child view model for each of the rows in the data model and engage the event notification.  When the data model
                        // is updated from the service, the changes will cycle through to the child view models.
                        foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.NewItems)
                        {
                            // Create a new view model for the new record and hook it into the data model updates.
                            ListViews.UnderwriterViewModel underwriterViewModel = this.CompositionContext.GetExport<ListViews.UnderwriterViewModel>();
                            underwriterViewModel.Map(underwriterRow);

                            // This will order the children so it's easy to find them individually and delete them.
                            int index = this.Items.BinarySearch((ivm) => ivm.SortKey, underwriterViewModel.UnderwriterId);
                            this.Items.Insert(~index, underwriterViewModel);
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:

                        // This will disengage the child view models from the data model.
                        foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.OldItems)
                        {
                            // Find the item and disengage it from the data model updates before deleting it from the view model.
                            int index = this.Items.BinarySearch((ivm) => ivm.SortKey, underwriterRow[DataRowVersion.Original].UnderwriterId);
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
    }
}