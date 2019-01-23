// <copyright file="ProductViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Forms
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Composition;
    using System.Windows.Input;
    using DarkBond.LicenseManager.Mappers;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.Navigation;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Product view model.
    /// </summary>
    public class ProductViewModel : CommonFormViewModel
    {
        /// <summary>
        /// The text that appears in the banner of the dialog.
        /// </summary>
        private string bannerTextField;

        /// <summary>
        /// A description of the product.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// Indicates that the product is new.
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// The name of the product.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The unique identifier of the product.
        /// </summary>
        private Guid? productIdField;

        /// <summary>
        /// Maps customer records from one format to another.
        /// </summary>
        private IProductMapper productMapper;

        /// <summary>
        /// Command for submitting the form.
        /// </summary>
        private ICommand submitField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        /// <param name="navigationService">The navigation services.</param>
        /// <param name="productMapper">Maps the product data model to the view model.</param>
        public ProductViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService,
            INavigationService navigationService,
            IProductMapper productMapper)
            : base(compositionContext, dataModel, licenseService, navigationService)
        {
            // Validate the parameter.
            if (productMapper == null)
            {
                throw new ArgumentNullException(nameof(productMapper));
            }

            // Initialize the object.
            this.productMapper = productMapper;

            // These commands are handled by this view model.
            this.submitField = new DelegateCommand(this.SubmitProduct);
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
        /// Gets or sets the email address.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Strings.Errors))]
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
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Strings.Errors))]
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
                    this.OnPropertyChanged("ProductdId");
                }
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
        /// Occurs when the navigation is to this page.
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
            this.productMapper.Clear(this);

            // Extract the unique identifier for this view from the URI.
            this.ProductId = navigationContext.Parameters.Count == 0 ? (Guid?)null : Guid.Parse(navigationContext.Parameters["productId"]);

            // There is a special, predefined identifier used to indicate that this is a view model for a new product record.
            if (this.isUpdate = this.ProductId.HasValue)
            {
                // Initialize the view model for an existing product.
                this.BannerText = Resources.ProductProperties;
                ProductRow productRow = this.DataModel.ProductKey.Find(this.ProductId.Value);
                this.productMapper.Map(productRow, this);
            }
            else
            {
                // Initialize the view model for a new product.
                this.ProductId = Guid.NewGuid();
                this.BannerText = Resources.NewProduct;
            }

            // This makes sure the navigation buttons reflect the current state.  Theoretically speaking, we can always navigate backwards from one
            // of these dialogs, this satisfies my instinct not to hard-code.
            this.GoBack.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Saves the product view model to the persistent store.
        /// </summary>
        private void SubmitProduct()
        {
            // If the view model is valid then attempt to commit it to the persistent store.  If it isn't valid, then the field validation messages
            // will appear and give the user feedback about what fields need to be fixed.
            if (this.IsValid)
            {
                // The product identifier indicates whether this is a new record or an existing one.
                if (this.isUpdate)
                {
                    // Ask the License Service to update the product from this view model.
                    this.LicenseService.UpdateProduct(this);
                }
                else
                {
                    // Ask the License Service to create a product from this view model.
                    this.LicenseService.InsertProduct(this);
                }

                // This will effectively dismiss the form by navigating back to the previous view.
                this.NavigationService.GoBack();
            }
        }
    }
}