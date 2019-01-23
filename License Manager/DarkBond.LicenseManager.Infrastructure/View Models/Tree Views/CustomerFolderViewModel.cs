// <copyright file="CustomerFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A navigation tree view item for the folder that displays customers.
    /// </summary>
    public class CustomerFolderViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        public CustomerFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the properties of this node.
            this.Header = Resources.Customer;
            this.Identifier = Resources.Customer;
            this.ImageKey = ImageKeys.Folder;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // When customers are added to the data model we need to make children of them for this breadcrumb.
            CustomerTable customerTable = this.DataModel.Customer;
            customerTable.CollectionChanged += this.OnCollectionChanged;

            // Initialize the collection of items in this directory from the data model.
            foreach (CustomerRow customerRow in this.DataModel.Customer)
            {
                CustomerViewModel customerViewModel = this.CompositionContext.GetExport<CustomerViewModel>();
                customerViewModel.Map(customerRow);
                int index = this.Items.BinarySearch((mivm) => mivm.SortKey, customerRow.CustomerId);
                this.Items.Insert(~index, customerViewModel);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Release the event handlers that were attached to the data model.
            if (disposing)
            {
                this.DataModel.Customer.CollectionChanged -= this.OnCollectionChanged;
            }

            // Allow the base class to recover resources.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a context menu for this item.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // The common menu items.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();
            contextMenuViewItems.Add(this.ExpandMenuItem);
            contextMenuViewItems.Add(new SeparatorViewModel());

            // New Customer Menu Item
            MenuItemViewModel newCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newCustomerMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToCustomer());
            newCustomerMenuItem.Header = Resources.NewCustomer;
            newCustomerMenuItem.ImageKey = ImageKeys.Customer;
            contextMenuViewItems.Add(newCustomerMenuItem);

            // These are the context menu items.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Handle a change to the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // This will handle the different verbs.
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (CustomerViewModel customerViewModel in this.Items)
                    {
                        customerViewModel.Dispose();
                    }

                    this.Items.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // This will add the item as a child of this breadcrumb.
                    foreach (CustomerRow customerRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        CustomerViewModel customerViewModel = this.CompositionContext.GetExport<CustomerViewModel>();
                        customerViewModel.Map(customerRow);
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, customerRow.CustomerId);
                        this.Items.Insert(~index, customerViewModel);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // This remove the item as a child of this breadcrumb.
                    foreach (CustomerRow customerRow in notifyCollectionChangedEventArgs.OldItems)
                    {
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, customerRow[DataRowVersion.Original].CustomerId);
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