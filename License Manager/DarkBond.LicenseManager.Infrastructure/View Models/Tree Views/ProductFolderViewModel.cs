// <copyright file="ProductFolderViewModel.cs" company="Dark Bond, Inc.">
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
    /// A navigation tree view item for the folder that displays products.
    /// </summary>
    public class ProductFolderViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        public ProductFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the properties of this node.
            this.Header = Resources.Product;
            this.Identifier = Resources.Product;
            this.ImageKey = ImageKeys.Folder;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // When products are added to the data model we need to make children of them for this breadcrumb.
            ProductTable productTable = this.DataModel.Product;
            productTable.CollectionChanged += this.OnCollectionChanged;

            // Initialize the collection of items in this directory from the data model.
            foreach (ProductRow productRow in this.DataModel.Product)
            {
                ProductViewModel productViewModel = this.CompositionContext.GetExport<ProductViewModel>();
                productViewModel.Map(productRow);
                int index = this.Items.BinarySearch((mivm) => mivm.SortKey, productRow.ProductId);
                this.Items.Insert(~index, productViewModel);
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
                this.DataModel.Product.CollectionChanged -= this.OnCollectionChanged;
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

            // New Product Menu Item
            MenuItemViewModel newProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newProductMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct());
            newProductMenuItem.Header = Resources.NewProduct;
            newProductMenuItem.ImageKey = ImageKeys.Product;
            contextMenuViewItems.Add(newProductMenuItem);

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
                    foreach (ProductViewModel productViewModel in this.Items)
                    {
                        productViewModel.Dispose();
                    }

                    this.Items.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // This will add the item as a child of this breadcrumb.
                    foreach (ProductRow productRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        ProductViewModel productViewModel = this.CompositionContext.GetExport<ProductViewModel>();
                        productViewModel.Map(productRow);
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, productRow.ProductId);
                        this.Items.Insert(~index, productViewModel);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // This remove the item as a child of this breadcrumb.
                    foreach (ProductRow productRow in notifyCollectionChangedEventArgs.OldItems)
                    {
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, productRow[DataRowVersion.Original].ProductId);
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