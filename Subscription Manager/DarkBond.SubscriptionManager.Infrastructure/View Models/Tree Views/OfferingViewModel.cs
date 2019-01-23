// <copyright file="OfferingViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A navigation tree view item for the offering.
    /// </summary>
    public class OfferingViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> notifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription service.</param>
        public OfferingViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Product;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Maps the data model to the view model.
        /// </summary>
        /// <param name="offeringRow">The offering row.</param>
        public void Map(OfferingRow offeringRow)
        {
            // Validate the parameter.
            if (offeringRow == null)
            {
                throw new ArgumentNullException(nameof(offeringRow));
            }

            // Instruct the data model to notify this view model of relevant changes.
            this.offeringRowField = offeringRow;
            this.offeringRowField.PropertyChanged += this.OnOfferingRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("OfferingId", this.UpdateIdentifier);
            this.notifyActions.Add("Name", (c) => this.Header = c.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.offeringRowField);
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
            newLicenseMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProductLicense(this.offeringRowField.OfferingId));
            newLicenseMenuItem.Header = Resources.NewLicense;
            newLicenseMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newLicenseMenuItem);

            // Delete Product Menu Item
            MenuItemViewModel deleteProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteProductMenuItem.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteProductAsync(this.offeringRowField.OfferingId),
                () => this.SubscriptionService.CanDeleteProduct(this.offeringRowField.OfferingId));
            deleteProductMenuItem.Header = Resources.Delete;
            deleteProductMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteProductMenuItem);

            // Product Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.offeringRowField.OfferingId));
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
            this.offeringRowField.PropertyChanged -= this.OnOfferingRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
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

        /// <summary>
        /// Update the unique identifier.
        /// </summary>
        /// <param name="offeringRow">The offering row.</param>
        private void UpdateIdentifier(OfferingRow offeringRow)
        {
            // This is used to uniquely identify the object in a URL.
            this.Identifier = offeringRow.OfferingId.ToString("N");

            // This is used to uniquely identify the object in a ordered list.
            this.SortKey = offeringRow.OfferingId;
        }
    }
}