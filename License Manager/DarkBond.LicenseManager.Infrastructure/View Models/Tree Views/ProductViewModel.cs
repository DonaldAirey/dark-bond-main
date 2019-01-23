// <copyright file="ProductViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A navigation tree view item for the product.
    /// </summary>
    public class ProductViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> notifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        public ProductViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Product;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Maps the data model to the view model.
        /// </summary>
        /// <param name="productRow">The product row.</param>
        public void Map(ProductRow productRow)
        {
            // Validate the parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Instruct the data model to notify this view model of relevant changes.
            this.productRowField = productRow;
            this.productRowField.PropertyChanged += this.OnProductRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("ProductId", this.UpdateIdentifier);
            this.notifyActions.Add("Name", (c) => this.Header = c.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.productRowField);
            }
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
            newLicenseMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProductLicense(this.productRowField.ProductId));
            newLicenseMenuItem.Header = Resources.NewLicense;
            newLicenseMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newLicenseMenuItem);

            // Delete Product Menu Item
            MenuItemViewModel deleteProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteProductMenuItem.Command = new DelegateCommand(
                () => this.LicenseService.DeleteProduct(this.productRowField.ProductId),
                () => this.LicenseService.CanDeleteProduct(this.productRowField.ProductId));
            deleteProductMenuItem.Header = Resources.Delete;
            deleteProductMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteProductMenuItem);

            // Product Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct(this.productRowField.ProductId));
            propertiesMenuItem.Header = Resources.Properties;
            propertiesMenuItem.ImageKey = ImageKeys.Properties;
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
            this.productRowField.PropertyChanged -= this.OnProductRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the data model product row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnProductRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<ProductRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.productRowField);
            }
        }

        /// <summary>
        /// Update the unique identifier.
        /// </summary>
        /// <param name="productRow">The product row.</param>
        private void UpdateIdentifier(ProductRow productRow)
        {
            // This is used to uniquely identify the object in a URL.
            this.Identifier = productRow.ProductId.ToString("N");

            // This is used to uniquely identify the object in a ordered list.
            this.SortKey = productRow.ProductId;
        }
    }
}