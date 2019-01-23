// <copyright file="LicenseViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Composition;
    using System.Windows.Input;
    using DarkBond.LicenseManager.Mappers;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.LicenseManager.ViewModels.Controls;
    using DarkBond.Navigation;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// License view model.
    /// </summary>
    public class LicenseViewModel : CommonFormViewModel
    {
        /// <summary>
        /// The text that appears in the banner of the dialog.
        /// </summary>
        private string bannerTextField;

        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        private Guid? customerIdField;

        /// <summary>
        /// The customer name.
        /// </summary>
        private string customerNameField;

        /// <summary>
        /// The customer row.
        /// </summary>
        private CustomerRow customerRowField;

        /// <summary>
        /// The view model for the customer ComboBox.
        /// </summary>
        private CustomerCollection customerCollection;

        /// <summary>
        /// A table that drives the notifications when customer row properties change.
        /// </summary>
        private Dictionary<string, Action<CustomerRow>> customerNotifyActions = new Dictionary<string, Action<CustomerRow>>();

        /// <summary>
        /// Maps data model records to view models and back again.
        /// </summary>
        private ILicenseMapper licenseMapper;

        /// <summary>
        /// The view model for the developer LicenseType ComboBox.
        /// </summary>
        private LicenseTypeCollection developerLicenseTypeCollection;

        /// <summary>
        /// The developer license type code.
        /// </summary>
        private LicenseTypeCode? developerLicenseTypeCodeField;

        /// <summary>
        /// Indicates that the product is new.
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// The unique identifier of the license.
        /// </summary>
        private Guid? licenseIdField;

        /// <summary>
        /// The current state of the view (whether it is showing a Customer License, Product License or general license).
        /// </summary>
        private LicenseViewState licenseViewState;

        /// <summary>
        /// The view model for the product ComboBox.
        /// </summary>
        private ProductCollection productCollection;

        /// <summary>
        /// The unique identifier of the product.
        /// </summary>
        private Guid? productIdField;

        /// <summary>
        /// The product name.
        /// </summary>
        private string productNameField;

        /// <summary>
        /// A table that drives the notifications when product row properties change.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> productNotifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// The runtime license type code.
        /// </summary>
        private LicenseTypeCode? runtimeLicenseTypeCodeField;

        /// <summary>
        /// The view model for the runtime LicenseType ComboBox.
        /// </summary>
        private LicenseTypeCollection runtimeLicenseTypeCollection;

        /// <summary>
        /// The submit command.
        /// </summary>
        private ICommand submitField = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="customerCollection">The collection of customers.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseMapper">Maps licenses from the data model to the view model.</param>
        /// <param name="licenseService">The license services.</param>
        /// <param name="runtimeLicenseTypeCollection">The collection of runtime license types.</param>
        /// <param name="navigationService">The navigation services.</param>
        /// <param name="developerLicenseTypeCollection">The collection of developer license types.</param>
        /// <param name="productCollection">The collection of products.</param>
        public LicenseViewModel(
            CompositionContext compositionContext,
            CustomerCollection customerCollection,
            DataModel dataModel,
            ILicenseMapper licenseMapper,
            ILicenseService licenseService,
            LicenseTypeCollection runtimeLicenseTypeCollection,
            LicenseTypeCollection developerLicenseTypeCollection,
            INavigationService navigationService,
            ProductCollection productCollection)
            : base(compositionContext, dataModel, licenseService, navigationService)
        {
            // Initialize the object.
            this.customerCollection = customerCollection;
            this.developerLicenseTypeCollection = developerLicenseTypeCollection;
            this.licenseMapper = licenseMapper;
            this.productCollection = productCollection;
            this.runtimeLicenseTypeCollection = runtimeLicenseTypeCollection;

            // These commands are handled by this view model.
            this.submitField = new DelegateCommand(this.SubmitLicense);
        }

        /// <summary>
        /// Gets or sets the text that appears in the banner of this dialog.
        /// </summary>
        public string BannerText
        {
            get
            {
                return this.bannerTextField;
            }

            set
            {
                if (this.bannerTextField != value)
                {
                    this.bannerTextField = value;
                    this.OnPropertyChanged("BannerText");
                }
            }
        }

        /// <summary>
        /// Gets the collection of customers.
        /// </summary>
        public CustomerCollection Customers
        {
            get
            {
                return this.customerCollection;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the customer.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public Guid? CustomerId
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
        /// Gets or sets the unique customer identifier.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public LicenseTypeCode? DeveloperLicenseTypeCode
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
        /// Gets or sets the unique identifier of the license.
        /// </summary>
        public Guid? LicenseId
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
                    this.OnPropertyChanged("LicensedId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the products.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public Guid? ProductId
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
        /// Gets the collection of products.
        /// </summary>
        public ProductCollection Products
        {
            get
            {
                return this.productCollection;
            }
        }

        /// <summary>
        /// Gets or sets the unique customer identifier.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public LicenseTypeCode? RuntimeLicenseTypeCode
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
        /// Gets the view model for the Developer LicenseType ComboBox.
        /// </summary>
        public LicenseTypeCollection RuntimeLicenseTypes
        {
            get
            {
                return this.runtimeLicenseTypeCollection;
            }
        }

        /// <summary>
        /// Gets the command used to submit the form.
        /// </summary>
        public ICommand Submit
        {
            get
            {
                return this.submitField;
            }
        }

        /// <summary>
        /// Gets or sets an indication of the visual state of the form.
        /// </summary>
        public LicenseViewState ViewState
        {
            get
            {
                return this.licenseViewState;
            }

            set
            {
                if (this.licenseViewState != value)
                {
                    this.licenseViewState = value;
                    this.OnPropertyChanged("ViewState");
                }
            }
        }

        /// <summary>
        /// Occurs when the navigation is away from this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Unbind the view model from the data model.
            switch (this.ViewState)
            {
                case LicenseViewState.Customer:

                    this.customerRowField.PropertyChanged -= this.OnCustomerRowChanged;
                    break;

                case LicenseViewState.Product:

                    this.productRowField.PropertyChanged -= this.OnProductRowChanged;
                    break;

                case LicenseViewState.License:

                    this.customerRowField.PropertyChanged -= this.OnCustomerRowChanged;
                    this.productRowField.PropertyChanged -= this.OnProductRowChanged;
                    break;
            }
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Validate the parameter.
            if (navigationContext == null)
            {
                throw new ArgumentNullException(nameof(navigationContext));
            }

            // The view to which this view model is attached is a singleton, so we won't make any assumptions about the initial state of this view
            // model.  This will initialize it whenever it is the target of a navigation.
            this.IsValid = false;
            this.licenseMapper.Clear(this);

            // Configure to create a license for a specific customer.
            string customerIdText = navigationContext.Parameters["customerId"];
            if (customerIdText != null)
            {
                this.ViewState = LicenseViewState.Customer;
                this.isUpdate = false;
                this.BannerText = Resources.NewLicense;
                this.LicenseId = Guid.NewGuid();
                this.CustomerId = Guid.Parse(customerIdText);

                // Bind the view model to the data model for the customers.
                this.InitializeCustomerBinding(this.CustomerId.Value);
            }

            // Configure to create a license for a specific customer.
            string productIdText = navigationContext.Parameters["productId"];
            if (productIdText != null)
            {
                this.ViewState = LicenseViewState.Product;
                this.isUpdate = false;
                this.BannerText = Resources.NewLicense;
                this.LicenseId = Guid.NewGuid();
                this.ProductId = Guid.Parse(productIdText);

                // Bind the view model to the data model for the products.
                this.InitializeProductBinding(this.ProductId.Value);
            }

            // Configure to display a license.
            string licenseIdText = navigationContext.Parameters["licenseId"];
            if (licenseIdText != null)
            {
                this.ViewState = LicenseViewState.License;
                this.isUpdate = true;
                this.BannerText = Resources.LicenseProperties;
                this.LicenseId = Guid.Parse(licenseIdText);
                LicenseRow licenseRow = this.DataModel.LicenseKey.Find(this.LicenseId.Value);
                this.licenseMapper.Map(licenseRow, this);

                // Bind the view model to the data model for the customers and products.
                this.InitializeCustomerBinding(licenseRow.CustomerId);
                this.InitializeProductBinding(licenseRow.ProductId);
            }
        }

        /// <summary>
        /// Initialize the binding of the customer values to the data model.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        private void InitializeCustomerBinding(Guid customerId)
        {
            // Instruct the Customer row to notify this view model of relevant changes.
            this.customerRowField = this.DataModel.CustomerKey.Find(customerId);
            this.customerRowField.PropertyChanged += this.OnCustomerRowChanged;

            // This table drives the updating of the view model when the customer properties changes.
            this.customerNotifyActions.Add("FirstName", this.UpdateCustomerName);
            this.customerNotifyActions.Add("LastName", this.UpdateCustomerName);

            // Initialize the view model with the customer properties.
            foreach (string property in this.customerNotifyActions.Keys)
            {
                this.customerNotifyActions[property](this.customerRowField);
            }
        }

        /// <summary>
        /// Initialize the binding of the product values to the data model.
        /// </summary>
        /// <param name="productId">The product id.</param>
        private void InitializeProductBinding(Guid productId)
        {
            // Instruct the Product row to notify this view model of relevant changes.
            this.productRowField = this.DataModel.ProductKey.Find(productId);
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
        /// Saves the license view model to the persistent store.
        /// </summary>
        private void SubmitLicense()
        {
            // If the view model is valid then attempt to commit it to the persistent store.  If it isn't valid, then the field validation messages
            // will appear and give the user feedback about what fields need to be fixed.
            if (this.IsValid)
            {
                // The license identifier indicates whether this is a new record or an existing one.  Make sure to use the internal variable to set
                // the license identifier so as to not trigger the initialization of the view model.
                if (this.isUpdate)
                {
                    this.LicenseService.UpdateLicense(this);
                }
                else
                {
                    this.LicenseService.InsertLicense(this);
                }

                // Navigate to the previous page when we've successfully created or update the license.
                this.NavigationService.GoBack();
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
    }
}