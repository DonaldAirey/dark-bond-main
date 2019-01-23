// <copyright file="UnderwriterViewModel.cs" company="Dark Bond, Inc.">
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
    /// A navigation tree view item for the underwriter.
    /// </summary>
    public class UnderwriterViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> notifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription service.</param>
        public UnderwriterViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Customer;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Maps the data model to the view model.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        public void Map(UnderwriterRow underwriterRow)
        {
            // Validate the parameter.
            if (underwriterRow == null)
            {
                throw new ArgumentNullException(nameof(underwriterRow));
            }

            // Instruct the data model to notify this view model of relevant changes.
            this.underwriterRowField = underwriterRow;
            this.underwriterRowField.PropertyChanged += this.OnUnderwriterRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("UnderwriterId", this.UpdateIdentifier);
            this.notifyActions.Add("FirstName", this.UpdateName);
            this.notifyActions.Add("LastName", this.UpdateName);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.underwriterRowField);
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
            newLicenseMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomerLicense(this.underwriterRowField.UnderwriterId));
            newLicenseMenuItem.Header = Resources.NewLicense;
            newLicenseMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newLicenseMenuItem);

            // Delete Customer Menu Item
            MenuItemViewModel deleteCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteCustomerMenuItem.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteCustomerAsync(this.underwriterRowField.UnderwriterId),
                () => this.SubscriptionService.CanDeleteCustomer(this.underwriterRowField.UnderwriterId));
            deleteCustomerMenuItem.Header = Resources.Delete;
            deleteCustomerMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteCustomerMenuItem);

            // Customer Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer(this.underwriterRowField.UnderwriterId));
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
            this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the data model underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<UnderwriterRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.underwriterRowField);
            }
        }

        /// <summary>
        /// Update the unique identifier.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        private void UpdateIdentifier(UnderwriterRow underwriterRow)
        {
            // This is used to uniquely identify the object in a URL.
            this.Identifier = underwriterRow.UnderwriterId.ToString("N");

            // This is used to uniquely identify the object in a ordered list.
            this.SortKey = underwriterRow.UnderwriterId;
        }

        /// <summary>
        /// Update the name property.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        private void UpdateName(UnderwriterRow underwriterRow)
        {
            // Format the name from the components.
            this.Header = underwriterRow.Name;
        }
    }
}