// <copyright file="ProductFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Directories
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
    /// View Model for the directory that shows the products.
    /// </summary>
    public class ProductFolderViewModel : CommonDirectoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        public ProductFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the object.
            this.ImageKey = ImageKeys.Folder;
            this.Name = Resources.Product;
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
                uriBuilder.Query = @"path=\" + Resources.ApplicationName + @"\" + Resources.Product;
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Loads the resources for this view model.
        /// </summary>
        /// <param name="path">The path for this view model.</param>
        public override void Load(string path)
        {
            // This will keep the view models of products reconciled to the data model.
            this.DataModel.Product.CollectionChanged += this.OnCollectionChanged;

            // This will initialize the collection of products in this directory from the data model.  Building a view can be expensive, so disable
            // the automatic refreshes during the bulk operation.
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.DataModel.Product));

            // Allow the base class to finish loading the view model.
            base.Load(path);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Unload()
        {
            // This will disengage from the data model and clear the managed resources.
            this.DataModel.Product.CollectionChanged -= this.OnCollectionChanged;

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

            // New Product
            ButtonViewModel newProductButton = this.CompositionContext.GetExport<ButtonViewModel>();
            newProductButton.Command = new DelegateCommand(() => { this.LicenseService.NavigateToProduct(); });
            newProductButton.Label = Resources.NewProduct;
            newProductButton.ImageKey = ImageKeys.Product;
            appBarItems.Insert(0, newProductButton);

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

            // New Product Menu Item
            DropDownButtonViewModel newProductButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newProductButton.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct());
            newProductButton.Header = Resources.NewProduct;
            newProductButton.ImageKey = ImageKeys.Product;
            newButtonItem.Items.Add(newProductButton);

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

            // New Product
            MenuItemViewModel newProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newProductMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct());
            newProductMenuItem.Header = Resources.NewProduct;
            newProductMenuItem.ImageKey = ImageKeys.Product;
            contextMenuViewItems.Add(newProductMenuItem);

            // These are the context menu items.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Handle the RowChanged events of a ProductDataSet.
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
                        foreach (ListViews.ProductViewModel countryViewModel in this.Items)
                        {
                            countryViewModel.Dispose();
                        }

                        this.Items.Clear();

                        break;

                    case NotifyCollectionChangedAction.Add:

                        // This will create a child view model for each of the rows in the data model and engage the event notification.  When the data model
                        // is updated from the service, the changes will cycle through to the child view models.
                        foreach (ProductRow productRow in notifyCollectionChangedEventArgs.NewItems)
                        {
                            // Create a new view model for the new record and hook it into the data model updates.
                            ListViews.ProductViewModel productViewModel = this.CompositionContext.GetExport<ListViews.ProductViewModel>();
                            productViewModel.Map(productRow);

                            // This will order the children so it's easy to find them individually and delete them.
                            int index = this.Items.BinarySearch((ivm) => ivm.SortKey, productViewModel.ProductId);
                            this.Items.Insert(~index, productViewModel);
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:

                        // This will disengage the child view models from the data model.
                        foreach (ProductRow productRow in notifyCollectionChangedEventArgs.OldItems)
                        {
                            // Find the item and disengage it from the data model updates before deleting it from the view model.
                            int index = this.Items.BinarySearch((ivm) => ivm.SortKey, productRow[DataRowVersion.Original].ProductId);
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