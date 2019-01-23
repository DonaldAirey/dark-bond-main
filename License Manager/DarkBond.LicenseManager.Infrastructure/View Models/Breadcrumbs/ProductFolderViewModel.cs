// <copyright file="ProductFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.Strings;

    /// <summary>
    /// A breadcrumb for the folder that displays products.
    /// </summary>
    public class ProductFolderViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        public ProductFolderViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this node.
            this.Header = Resources.Product;
            this.ImageKey = ImageKeys.Folder;
            this.Identifier = Resources.Product;
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