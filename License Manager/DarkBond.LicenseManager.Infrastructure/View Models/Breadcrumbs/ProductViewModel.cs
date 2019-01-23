// <copyright file="ProductViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition;
    using Strings;

    /// <summary>
    /// A breadcrumb for products.
    /// </summary>
    public class ProductViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> notifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
         public ProductViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Product;
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