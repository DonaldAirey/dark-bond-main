// <copyright file="CustomerViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Forms
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Composition;
    using System.Globalization;
    using System.Windows.Input;
    using DarkBond.LicenseManager;
    using DarkBond.LicenseManager.Mappers;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.LicenseManager.ViewModels.Controls;
    using DarkBond.Navigation;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Customer view model.
    /// </summary>
    public class CustomerViewModel : CommonFormViewModel
    {
        /// <summary>
        /// Regular Expression for an email address.
        /// </summary>
        private const string EmailRegEx = @"[0-9a-zA-Z.+_]+@[0-9a-zA-Z.+_\-]+\.[a-zA-Z]{2,4}";

        /// <summary>
        /// Regular Expression for a phone number.
        /// </summary>
        private const string PhoneRegEx = @"(\(\d{3}\) *|\d{3}-)\d{3}-\d{4}";

        /// <summary>
        /// Regular Expression for a zip code.
        /// </summary>
        private const string ZipRegEx = @"\d{5}([-\s]\d{4})?";

        /// <summary>
        /// The first address line.
        /// </summary>
        private string address1Field;

        /// <summary>
        /// The second address line.
        /// </summary>
        private string address2Field;

        /// <summary>
        /// The text that appears in the banner of the dialog.
        /// </summary>
        private string bannerTextField;

        /// <summary>
        /// The city.
        /// </summary>
        private string cityField;

        /// <summary>
        /// The customer's company.
        /// </summary>
        private string companyField;

        /// <summary>
        /// The unique identifier of the country.
        /// </summary>
        private Guid countryIdField;

        /// <summary>
        /// The view model for the Country ComboBox.
        /// </summary>
        private CountryCollection countryCollection;

        /// <summary>
        /// The unique identifier of this customer.
        /// </summary>
        private Guid? customerIdField;

        /// <summary>
        /// Maps customer records from one format to another.
        /// </summary>
        private ICustomerMapper customerMapper;

        /// <summary>
        /// The date of birth field.
        /// </summary>
        private DateTime dateOfBirthField;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailField;

        /// <summary>
        /// The first name.
        /// </summary>
        private string firstNameField;

        /// <summary>
        /// An indication of whether the ProvinceComboBox is enabled.
        /// </summary>
        private bool isProvinceEnabledField = true;

        /// <summary>
        /// Indicates that the product is new.
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// The last name.
        /// </summary>
        private string lastNameField;

        /// <summary>
        /// The middle name.
        /// </summary>
        private string middleNameField;

        /// <summary>
        /// The phone number.
        /// </summary>
        private string phoneNumberField;

        /// <summary>
        /// The postal code.
        /// </summary>
        private string postalCodeField;

        /// <summary>
        /// The unique identifier of the province.
        /// </summary>
        private Guid? provinceIdField;

        /// <summary>
        /// The view model for the province ComboBox.
        /// </summary>
        private ProvinceCollection provinceCollection;

        /// <summary>
        /// The unique identifier for the province ComboBox.
        /// </summary>
        private ProvinceCountryKey provinceCountryKeyField;

        /// <summary>
        /// The view of the province collection.
        /// </summary>
        private ListCollectionView provinceView;

        /// <summary>
        /// Command for submitting the form.
        /// </summary>
        private ICommand submitField;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="countryCollection">The collection of countries.</param>
        /// <param name="customerMapper">Maps customers from the data model to the view model.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        /// <param name="navigationService">The navigation services.</param>
        /// <param name="provinceCollection">The collection of provinces.</param>
        public CustomerViewModel(
            CompositionContext compositionContext,
            CountryCollection countryCollection,
            ICustomerMapper customerMapper,
            DataModel dataModel,
            ILicenseService licenseService,
            INavigationService navigationService,
            ProvinceCollection provinceCollection)
            : base(compositionContext, dataModel, licenseService, navigationService)
        {
            // Validate the parameter.
            if (countryCollection == null)
            {
                throw new ArgumentNullException(nameof(countryCollection));
            }

            // Validate the parameter.
            if (customerMapper == null)
            {
                throw new ArgumentNullException(nameof(customerMapper));
            }

            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Validate the parameter.
            if (licenseService == null)
            {
                throw new ArgumentNullException(nameof(licenseService));
            }

            // Validate the parameter.
            if (provinceCollection == null)
            {
                throw new ArgumentNullException(nameof(provinceCollection));
            }

            // Initialize the object.
            this.countryCollection = countryCollection;
            this.customerMapper = customerMapper;
            this.provinceCollection = provinceCollection;

            // Create an alphabetized view of the provinces/states.
            this.provinceView = new ListCollectionView(this.provinceCollection);
            this.provinceView.SortDescriptions.Add(new SortDescription
            {
                PropertyName = "Name",
                Direction = SortDirection.Ascending
            });

            // These property change events are handled by this view model.
            this.PropertyChangedActions["CountryId"] = this.OnCountryIdChanged;
            this.PropertyChangedActions["ProvinceId"] = () => this.ProvinceCountryKey = new ProvinceCountryKey(this.CountryId, this.ProvinceId);
            this.PropertyChangedActions["ProvinceCountryKey"] = () => this.ProvinceId = this.ProvinceCountryKey.ProvinceId;

            // These commands are handled by this view model.
            this.submitField = new DelegateCommand(this.SubmitCustomer);
        }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public string Address1
        {
            get
            {
                return this.address1Field;
            }

            set
            {
                if (this.address1Field != value)
                {
                    this.address1Field = value;
                    this.OnPropertyChanged("Address1");
                }
            }
        }

        /// <summary>
        /// Gets or sets the second line of the address.
        /// </summary>
        public string Address2
        {
            get
            {
                return this.address2Field;
            }

            set
            {
                if (this.address2Field != value)
                {
                    this.address2Field = value;
                    this.OnPropertyChanged("Address2");
                }
            }
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
        /// Gets or sets the city.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public string City
        {
            get
            {
                return this.cityField;
            }

            set
            {
                if (this.cityField != value)
                {
                    this.cityField = value;
                    this.OnPropertyChanged("City");
                }
            }
        }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string Company
        {
            get
            {
                return this.companyField;
            }

            set
            {
                if (this.companyField != value)
                {
                    this.companyField = value;
                    this.OnPropertyChanged("Company");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this customer.
        /// </summary>
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
        /// Gets the view model for the CountryComboBox control.
        /// </summary>
        public CountryCollection Countries
        {
            get
            {
                return this.countryCollection;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the country.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public Guid CountryId
        {
            get
            {
                return this.countryIdField;
            }

            set
            {
                if (this.countryIdField != value)
                {
                    this.countryIdField = value;
                    this.OnPropertyChanged("CountryId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this customer.
        /// </summary>
        [CustomValidation(typeof(CustomerViewModel), "ValidateDateOfBirth")]
        public DateTime DateOfBirth
        {
            get
            {
                return this.dateOfBirthField;
            }

            set
            {
                if (this.dateOfBirthField != value)
                {
                    this.dateOfBirthField = value;
                    this.OnPropertyChanged("DateOfBirth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        [RegularExpression(
            CustomerViewModel.EmailRegEx,
            ErrorMessageResourceName = "IncorrectFormat",
            ErrorMessageResourceType = typeof(Errors))]
        public string Email
        {
            get
            {
                return this.emailField;
            }

            set
            {
                if (this.emailField != value)
                {
                    this.emailField = value;
                    this.OnPropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }

            set
            {
                if (this.firstNameField != value)
                {
                    this.firstNameField = value;
                    this.OnPropertyChanged("FirstName");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the province field is enabled.
        /// </summary>
        public bool IsProvinceEnabled
        {
            get
            {
                return this.isProvinceEnabledField;
            }

            set
            {
                if (this.isProvinceEnabledField != value)
                {
                    this.isProvinceEnabledField = value;
                    this.OnPropertyChanged("IsProvinceEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }

            set
            {
                if (this.lastNameField != value)
                {
                    this.lastNameField = value;
                    this.OnPropertyChanged("LastName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get
            {
                return this.middleNameField;
            }

            set
            {
                if (this.middleNameField != value)
                {
                    this.middleNameField = value;
                    this.OnPropertyChanged("MiddleName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        [RegularExpression(
            CustomerViewModel.PhoneRegEx,
            ErrorMessageResourceName = "IncorrectFormat",
            ErrorMessageResourceType = typeof(Errors))]
        public string PhoneNumber
        {
            get
            {
                return this.phoneNumberField;
            }

            set
            {
                if (this.phoneNumberField != value)
                {
                    this.phoneNumberField = value;
                    this.OnPropertyChanged("PhoneNumber");
                }
            }
        }

        /// <summary>
        /// Gets or sets the postal (zip) code.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        [RegularExpression(
            CustomerViewModel.ZipRegEx,
            ErrorMessageResourceName = "IncorrectFormat",
            ErrorMessageResourceType = typeof(Errors))]
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }

            set
            {
                if (this.postalCodeField != value)
                {
                    this.postalCodeField = value;
                    this.OnPropertyChanged("PostalCode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the province.
        /// </summary>
        [CustomValidation(typeof(CustomerViewModel), "ValidateProvinceId")]
        public Guid? ProvinceId
        {
            get
            {
                return this.provinceIdField;
            }

            set
            {
                if (this.provinceIdField != value)
                {
                    this.provinceIdField = value;
                    this.OnPropertyChanged("ProvinceId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the province.
        /// </summary>
        public ProvinceCountryKey ProvinceCountryKey
        {
            get
            {
                return this.provinceCountryKeyField;
            }

            set
            {
                if (this.provinceCountryKeyField != value)
                {
                    this.provinceCountryKeyField = value;
                    this.OnPropertyChanged("ProvinceCountryKey");
                }
            }
        }

        /// <summary>
        /// Gets the collection of provinces.
        /// </summary>
        public ListCollectionView Provinces
        {
            get
            {
                return this.provinceView;
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
        /// Validates the date of birth.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        /// <param name="validationContext">The context for the validation.</param>
        /// <returns>ValidationResult.  Success if the date is in a valid range.</returns>
        public static ValidationResult ValidateDateOfBirth(object value, ValidationContext validationContext)
        {
            // Validate the parameter
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            // Extract the specific parameters from the generic ones.
            DateTime dateOfBirth = (DateTime)value;

            // First check that a date was entered (it will be the default value if not).
            ValidationResult validationResults = ValidationResult.Success;
            if (dateOfBirth == default(DateTime))
            {
                validationResults = new ValidationResult(Errors.RequiredFieldViolation);
            }
            else
            {
                // Then make sure that the date falls within a reasonable range.
                if (dateOfBirth == default(DateTime) || dateOfBirth.Year < 1900 || dateOfBirth.Year > 2100)
                {
                    validationResults = new ValidationResult(Errors.DateOfBirthViolation);
                }
            }

            // The result of the validation test.
            return validationResults;
        }

        /// <summary>
        /// Rule for validating the province (state) code.
        /// </summary>
        /// <param name="value">The new province code.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> containing the context data.</param>
        /// <returns>ValidationResult.Success if the data passes the validation rule, an error message otherwise.</returns>
        public static ValidationResult ValidateProvinceId(object value, ValidationContext validationContext)
        {
            // Validate the parameter
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            // Extract the specific parameters from the generic ones.
            CustomerViewModel customerViewModel = validationContext.ObjectInstance as CustomerViewModel;
            Guid? pronviceId = (Guid?)value;

            // The province is only required when there is a list of provinces associated with the country.  Some counties don't have or require a
            // province for an address.  It is only a validation error to have an empty state in countries that states.
            ValidationResult validationResults = ValidationResult.Success;
            if (customerViewModel.Provinces.Count != 0 && !pronviceId.HasValue)
            {
                validationResults = new ValidationResult(Errors.RequiredFieldViolation);
            }

            // The result of the validation test.
            return validationResults;
        }

        /// <summary>
        /// Occurs when the navigation is to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Validate the parameter
            if (navigationContext == null)
            {
                throw new ArgumentNullException(nameof(navigationContext));
            }

            // The view to which this view model is attached is a singleton, so we won't make any assumptions about the initial state of this view
            // model.  This will initialize it whenever it is the target of a navigation.
            this.IsValid = false;
            this.customerMapper.Clear(this);

            // Extract the unique identifier for this view from the URI.
            this.CustomerId = navigationContext.Parameters.Count == 0 ? (Guid?)null : Guid.Parse(navigationContext.Parameters["customerId"]);

            // There is a special, predefined identifier used to indicate that this is a view model for a new customer record.
            if (this.isUpdate = this.CustomerId.HasValue)
            {
                // Initialize the view model for an existing customer.
                this.BannerText = Resources.CustomerProperties;
                CustomerRow customerRow = this.DataModel.CustomerKey.Find(this.CustomerId.Value);
                this.customerMapper.Map(customerRow, this);
            }
            else
            {
                // Initialize the view model for a new customer.
                this.BannerText = Resources.NewCustomer;
                CountryRow countryRow = this.DataModel.CountryExternalId0Key.Find(RegionInfo.CurrentRegion.TwoLetterISORegionName);
                if (countryRow != null)
                {
                    this.CountryId = countryRow.CountryId;
                }
            }

            // This makes sure the navigation buttons reflect the current state.  Theoretically speaking, we can always navigate backwards from one
            // of these dialogs, this satisfies my instinct not to hard-code.
            this.GoBack.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Handles a change to the country.
        /// </summary>
        private void OnCountryIdChanged()
        {
            // Set the filter for the selector to only include provinces from the current country.
            this.provinceView.Filter = (p) => ((ProvinceViewModel)p).CountryId == this.CountryId;
            this.provinceView.Refresh();

            // If the current province isn't valid for the current country, then clear out the province code.
            if (this.ProvinceId.HasValue)
            {
                ProvinceRow provinceRow = this.DataModel.ProvinceKey.Find(this.ProvinceId.Value);
                if (provinceRow == null || provinceRow.CountryId != this.CountryId)
                {
                    this.ProvinceId = null;
                }
            }

            // The selector for the province isn't enabled if there are no values from which to choose.
            this.IsProvinceEnabled = this.provinceView.Count != 0;
        }

        /// <summary>
        /// Saves the view model to the persistent store.
        /// </summary>
        private void SubmitCustomer()
        {
            // If the view model is valid then attempt to commit it to the persistent store.  If it isn't valid, then the field validation messages
            // will appear and give the user feedback about what fields need to be fixed.
            if (this.IsValid)
            {
                // The customer identifier indicates whether this is a new record or an existing one.
                if (this.isUpdate)
                {
                    // Ask the License Service to update the customer from this view model.
                    this.LicenseService.UpdateCustomer(this);
                }
                else
                {
                    // Give this customer a unique identifier and insert them.
                    this.CustomerId = Guid.NewGuid();
                    this.LicenseService.InsertCustomer(this);
                }

                // This will effectively dismiss the form by navigating back to the previous view.
                this.NavigationService.GoBack();
            }
        }
    }
}