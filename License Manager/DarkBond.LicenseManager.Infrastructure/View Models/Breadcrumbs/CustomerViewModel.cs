// <copyright file="CustomerViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;

    /// <summary>
    /// A breadcrumb for the customer.
    /// </summary>
    public class CustomerViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// The customer row.
        /// </summary>
        private CustomerRow customerRowField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<CustomerRow>> notifyActions = new Dictionary<string, Action<CustomerRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        public CustomerViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Customer;
        }

        /// <summary>
        /// Maps the data model to the view model.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        public void Map(CustomerRow customerRow)
        {
            // Validate the parameter.
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            // Instruct the data model to notify this view model of relevant changes.
            this.customerRowField = customerRow;
            this.customerRowField.PropertyChanged += this.OnCustomerRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("CustomerId", this.UpdateIdentifier);
            this.notifyActions.Add("FirstName", this.UpdateName);
            this.notifyActions.Add("LastName", this.UpdateName);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.customerRowField);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disconnect from the data model.
            this.customerRowField.PropertyChanged -= this.OnCustomerRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the data model customer row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnCustomerRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<CustomerRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.customerRowField);
            }
        }

        /// <summary>
        /// Update the unique identifier.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        private void UpdateIdentifier(CustomerRow customerRow)
        {
            // This is used to uniquely identify the object in a URL.
            this.Identifier = customerRow.CustomerId.ToString("N");

            // This is used to uniquely identify the object in a ordered list.
            this.SortKey = customerRow.CustomerId;
        }

        /// <summary>
        /// Update the name property.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        private void UpdateName(CustomerRow customerRow)
        {
            // Format the name from the components.
            this.Header = string.IsNullOrEmpty(customerRow.FirstName) ? customerRow.LastName : customerRow.LastName + ", " + customerRow.FirstName;
        }
    }
}