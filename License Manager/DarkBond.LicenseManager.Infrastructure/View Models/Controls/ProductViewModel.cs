// <copyright file="ProductViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DarkBond.ViewModels;

    /// <summary>
    /// View model for a product.
    /// </summary>
    public class ProductViewModel : ViewModel
    {
        /// <summary>
        /// The description of the product.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The name of the product.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> notifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// The unique identifier of the product.
        /// </summary>
        private Guid productIdField;

        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }

            set
            {
                if (this.descriptionField != value)
                {
                    this.descriptionField = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameField;
            }

            set
            {
                if (this.nameField != value)
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        public Guid ProductId
        {
            get
            {
                return this.productIdField;
            }

            set
            {
                if (this.productIdField != value)
                {
                    this.productIdField = value;
                    this.OnPropertyChanged("ProductId");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="productRow">A product row.</param>
        public void Map(ProductRow productRow)
        {
            // Validate the parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Initialize the object.
            this.productRowField = productRow;
            productRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Description", (p) => this.Description = p.Description);
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);
            this.notifyActions.Add("ProductId", (p) => this.ProductId = p.ProductId);

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
            // Disengage from the notifications from the data model.
            this.productRowField.PropertyChanged -= this.OnPropertyChanged;

            // Allow the base class to dispose.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the property of the underlying data model.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<ProductRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(sender as ProductRow);
            }
        }
    }
}