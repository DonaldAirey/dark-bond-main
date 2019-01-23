// <copyright file="LicenseViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.ListViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.LicenseManager.ViewModels.Controls;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A license who can own a license.
    /// </summary>
    public class LicenseViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        private Guid customerIdField;

        /// <summary>
        /// The customer name.
        /// </summary>
        private string customerNameField;

        /// <summary>
        /// The customer row.
        /// </summary>
        private CustomerRow customerRowField;

        /// <summary>
        /// A table that drives the notifications when customer row properties change.
        /// </summary>
        private Dictionary<string, Action<CustomerRow>> customerNotifyActions = new Dictionary<string, Action<CustomerRow>>();

        /// <summary>
        /// The description of the developer license.
        /// </summary>
        private string developerLicenseDescriptionField;

        /// <summary>
        /// The view model for the developer LicenseType ComboBox.
        /// </summary>
        private LicenseTypeCollection developerLicenseTypeCollection;

        /// <summary>
        /// The developer license type code.
        /// </summary>
        private LicenseTypeCode developerLicenseTypeCodeField;

        /// <summary>
        /// The command to delete a license.
        /// </summary>
        private DelegateCommand licenseDelete;

        /// <summary>
        /// The unique identifier of this license.
        /// </summary>
        private Guid licenseIdField;

        /// <summary>
        /// The command to update the properties.
        /// </summary>
        private DelegateCommand licenseProperties;

        /// <summary>
        /// The license row.
        /// </summary>
        private LicenseRow licenseRowField;

        /// <summary>
        /// A table that drives the notifications when license row properties change.
        /// </summary>
        private Dictionary<string, Action<LicenseRow>> notifyActions = new Dictionary<string, Action<LicenseRow>>();

        /// <summary>
        /// The unique identifier of the product.
        /// </summary>
        private Guid productIdField;

        /// <summary>
        /// The product name.
        /// </summary>
        private string productNameField;

        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// A table that drives the notifications when product row properties change.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> productNotifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// The description of the runtime license.
        /// </summary>
        private string runtimeLicenseDescriptionField;

        /// <summary>
        /// The runtime license type code.
        /// </summary>
        private LicenseTypeCode runtimeLicenseTypeCodeField;

        /// <summary>
        /// The view model for the runtime LicenseType ComboBox.
        /// </summary>
        private LicenseTypeCollection runtimeLicenseTypeCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="developerLicenseTypeCollection">The set of developer license types.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        /// <param name="runtimeLicenseTypeCollection">The set of runtime license types.</param>
        public LicenseViewModel(
            CompositionContext compositionContext,
            LicenseTypeCollection developerLicenseTypeCollection,
            DataModel dataModel,
            ILicenseService licenseService,
            LicenseTypeCollection runtimeLicenseTypeCollection)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the object
            this.developerLicenseTypeCollection = developerLicenseTypeCollection;
            this.runtimeLicenseTypeCollection = runtimeLicenseTypeCollection;
            this.ImageKey = ImageKeys.License;

            // These composite commands are handled by this view model.
            this.licenseDelete = new DelegateCommand(
                () => this.LicenseService.DeleteLicense(this.LicenseId),
                () => this.LicenseService.CanDeleteLicense(this.LicenseId));
            this.licenseProperties = new DelegateCommand(
                () => this.LicenseService.NavigateToLicense(this.LicenseId),
                () => this.LicenseService.CanNavigateToLicense(this.LicenseId));
        }

        /// <summary>
        /// Gets or sets the unique identifier of the customer.
        /// </summary>
        public Guid CustomerId
        {
            get
            {
                return this.customerIdField;
            }

            set
            {
                if (this.customerIdField != value)
                {
                    this.customerIdField = value;
                    this.OnPropertyChanged("CustomerId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }

            set
            {
                if (this.customerNameField != value)
                {
                    this.customerNameField = value;
                    this.OnPropertyChanged("CustomerName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the date the item was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the item was last modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the unique customer identifier.
        /// </summary>
        public LicenseTypeCode DeveloperLicenseTypeCode
        {
            get
            {
                return this.developerLicenseTypeCodeField;
            }

            set
            {
                if (this.developerLicenseTypeCodeField != value)
                {
                    this.developerLicenseTypeCodeField = value;
                    this.OnPropertyChanged("DeveloperLicenseTypeCode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the developer license.
        /// </summary>
        public string DeveloperLicenseDescription
        {
            get
            {
                return this.developerLicenseDescriptionField;
            }

            set
            {
                if (this.developerLicenseDescriptionField != value)
                {
                    this.developerLicenseDescriptionField = value;
                    this.OnPropertyChanged("DeveloperLicenseDescription");
                }
            }
        }

        /// <summary>
        /// Gets the view model for the Developer LicenseType ComboBox.
        /// </summary>
        public LicenseTypeCollection DeveloperLicenseTypes
        {
            get
            {
                return this.developerLicenseTypeCollection;
            }
        }

        /// <summary>
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the licenses are grouped by customer.
        /// </summary>
        public bool IsCustomerView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the licenses are grouped by product.
        /// </summary>
        public bool IsProductView { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this license.
        /// </summary>
        public Guid LicenseId
        {
            get
            {
                return this.licenseIdField;
            }

            set
            {
                if (this.licenseIdField != value)
                {
                    this.licenseIdField = value;
                    this.OnPropertyChanged("LicenseId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the products.
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
        /// Gets or sets the name of the product.
        /// </summary>
        public string ProductName
        {
            get
            {
                return this.productNameField;
            }

            set
            {
                if (this.productNameField != value)
                {
                    this.productNameField = value;
                    this.OnPropertyChanged("ProductName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the runtime license.
        /// </summary>
        public string RuntimeLicenseDescription
        {
            get
            {
                return this.runtimeLicenseDescriptionField;
            }

            set
            {
                if (this.runtimeLicenseDescriptionField != value)
                {
                    this.runtimeLicenseDescriptionField = value;
                    this.OnPropertyChanged("RuntimeLicenseDescription");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique customer identifier.
        /// </summary>
        public LicenseTypeCode RuntimeLicenseTypeCode
        {
            get
            {
                return this.runtimeLicenseTypeCodeField;
            }

            set
            {
                if (this.runtimeLicenseTypeCodeField != value)
                {
                    this.runtimeLicenseTypeCodeField = value;
                    this.OnPropertyChanged("RuntimeLicenseTypeCode");
                }
            }
        }

        /// <summary>
        /// Gets the view model for the Runtime LicenseType ComboBox.
        /// </summary>
        public LicenseTypeCollection RuntimeLicenseTypes
        {
            get
            {
                return this.runtimeLicenseTypeCollection;
            }
        }

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.LicenseId.ToString().Substring(0, 18) + "...";
            }
        }

        /// <summary>
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.LicenseView");
                uriBuilder.Query = "licenseId={" + this.LicenseId.ToString() + "}";
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Maps the properties of a LicenseRow in the data model to the properties of this view model.
        /// </summary>
        /// <param name="licenseRow">The customer row.</param>
        public void Map(LicenseRow licenseRow)
        {
            // Validate the parameter.
            if (licenseRow == null)
            {
                throw new ArgumentNullException(nameof(licenseRow));
            }

            // Instruct the License row to notify this view model of relevant changes.
            this.licenseRowField = licenseRow;
            this.licenseRowField.PropertyChanged += this.OnLicenseRowChanged;

            // This table drives the updating of the view model when the license properties changes.
            this.notifyActions.Add("CustomerId", (c) => this.CustomerId = c.CustomerId);
            this.notifyActions.Add("DeveloperLicenseTypeCode", this.UpdateDeveloperLicense);
            this.notifyActions.Add("LicenseId", this.UpdateLicenseId);
            this.notifyActions.Add("ProductId", (c) => this.ProductId = c.ProductId);
            this.notifyActions.Add("RuntimeLicenseTypeCode", this.UpdateRuntimeLicense);

            // Initialize the view model with the license properties.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.licenseRowField);
            }

            // Instruct the Customer row to notify this view model of relevant changes.
            this.customerRowField = this.DataModel.CustomerKey.Find(this.licenseRowField.CustomerId);
            this.customerRowField.PropertyChanged += this.OnCustomerRowChanged;

            // This table drives the updating of the view model when the customer properties changes.
            this.customerNotifyActions.Add("FirstName", this.UpdateCustomerName);
            this.customerNotifyActions.Add("LastName", this.UpdateCustomerName);

            // Initialize the view model with the customer properties.
            foreach (string property in this.customerNotifyActions.Keys)
            {
                this.customerNotifyActions[property](this.customerRowField);
            }

            // Instruct the Product row to notify this view model of relevant changes.
            this.productRowField = this.DataModel.ProductKey.Find(this.licenseRowField.ProductId);
            this.productRowField.PropertyChanged += this.OnProductRowChanged;

            // This table drives the updating of the view model when the product properties changes.
            this.productNotifyActions.Add("Name", (p) => this.ProductName = p.Name);

            // Initialize the view model with the product properties.
            foreach (string property in this.productNotifyActions.Keys)
            {
                this.productNotifyActions[property](this.productRowField);
            }
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            ObservableCollection<IDisposable> appBarItems = new ObservableCollection<IDisposable>();

            // The Delete command.
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Label = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            appBarItems.Add(deleteButton);

            // The Edit command.
            ButtonViewModel editButton = this.CompositionContext.GetExport<ButtonViewModel>();
            editButton.Command = new DelegateCommand(() => this.LicenseService.NavigateToLicense(this.LicenseId));
            editButton.Label = Resources.Edit;
            editButton.ImageKey = ImageKeys.Edit;
            appBarItems.Add(editButton);

            // This is the set of appBar items for this ListView item.
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
            ButtonViewModel newButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            newButtonItem.Command = new DelegateCommand(() => { }, () => false);
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButtonItem.Command = GlobalCommands.Properties;
            propertiesButtonItem.Header = Resources.Properties;
            propertiesButtonItem.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButtonItem);

            // This is the context menu.
            return contextButtonViewItems;
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // Delete Menu Item
            MenuItemViewModel deleteLicenseMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteLicenseMenuItem.Command = GlobalCommands.Delete;
            deleteLicenseMenuItem.Header = Resources.Delete;
            deleteLicenseMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteLicenseMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = GlobalCommands.Properties;
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
            // This will unhook us from the data model when the item is no longer displayed.
            if (disposing)
            {
                // Disengage from notifications from the data model.
                this.licenseRowField.PropertyChanged -= this.OnLicenseRowChanged;
                this.customerRowField.PropertyChanged -= this.OnCustomerRowChanged;
                this.productRowField.PropertyChanged -= this.OnProductRowChanged;

                // Remove all the actions that update this view model from the data model.
                this.notifyActions.Clear();

                // Make sure we unregister the composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Delete.UnregisterCommand(this.licenseDelete);
                    GlobalCommands.Properties.UnregisterCommand(this.licenseProperties);
                }
            }

            // Allow the base class to complete the method.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the IsSelected property.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            // In order for the 'gang' operations to work, the individual commands must be added to the composite command (and removed when no longer
            // needed).
            if (this.IsSelected)
            {
                GlobalCommands.Delete.RegisterCommand(this.licenseDelete);
                GlobalCommands.Properties.RegisterCommand(this.licenseProperties);
            }
            else
            {
                GlobalCommands.Delete.UnregisterCommand(this.licenseDelete);
                GlobalCommands.Properties.UnregisterCommand(this.licenseProperties);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles a change to the customer row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnCustomerRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            CustomerRow customerRow = sender as CustomerRow;
            Action<CustomerRow> customerNotifyAction;
            if (this.customerNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out customerNotifyAction))
            {
                customerNotifyAction(customerRow);
            }
        }

        /// <summary>
        /// Handles a change to the license row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnLicenseRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LicenseRow licenseRow = sender as LicenseRow;
            Action<LicenseRow> licenseNotifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out licenseNotifyAction))
            {
                licenseNotifyAction(licenseRow);
            }
        }

        /// <summary>
        /// Handles a change to the product row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnProductRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ProductRow productRow = sender as ProductRow;
            Action<ProductRow> productNotifyAction;
            if (this.productNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out productNotifyAction))
            {
                productNotifyAction(productRow);
            }
        }

        /// <summary>
        /// Update the name property.
        /// </summary>
        /// <param name="customerRow">The customer row.</param>
        private void UpdateCustomerName(CustomerRow customerRow)
        {
            // Format the name from the components.
            this.CustomerName = string.IsNullOrEmpty(customerRow.FirstName) ? customerRow.LastName : customerRow.LastName + ", " + customerRow.FirstName;
        }

        /// <summary>
        /// Update the developer license.
        /// </summary>
        /// <param name="licenseRow">The license row.</param>
        private void UpdateDeveloperLicense(LicenseRow licenseRow)
        {
            // Format the name from the components.
            LicenseTypeRow licenseTypeRow = this.DataModel.LicenseTypeKey.Find(licenseRow.DeveloperLicenseTypeCode);
            this.DeveloperLicenseDescription = licenseTypeRow.Description;
        }

        /// <summary>
        /// Updates the view model properties when the LicenseId column changes.
        /// </summary>
        /// <param name="licenseRow">The License row.</param>
        private void UpdateLicenseId(LicenseRow licenseRow)
        {
            this.SortKey = licenseRow.LicenseId;
            this.LicenseId = licenseRow.LicenseId;
        }

        /// <summary>
        /// Update the runtime license.
        /// </summary>
        /// <param name="licenseRow">The license row.</param>
        private void UpdateRuntimeLicense(LicenseRow licenseRow)
        {
            // Format the name from the components.
            LicenseTypeRow licenseTypeRow = this.DataModel.LicenseTypeKey.Find(licenseRow.RuntimeLicenseTypeCode);
            this.RuntimeLicenseDescription = licenseTypeRow.Description;
        }
    }
}