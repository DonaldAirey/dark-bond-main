// <copyright file="OfferingViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Forms
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Composition;
    using System.Windows.Input;
    using DarkBond.Navigation;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.SubscriptionManager.Mappers;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Product view model.
    /// </summary>
    public class OfferingViewModel : CommonFormViewModel
    {
        /// <summary>
        /// The text that appears in the banner of the dialog.
        /// </summary>
        private string bannerTextField;

        /// <summary>
        /// A description of the offering.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// Indicates that the offering is new.
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// The name of the offering.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The unique identifier of the offering.
        /// </summary>
        private Guid? offeringIdField;

        /// <summary>
        /// Maps underwriter records from one format to another.
        /// </summary>
        private IOfferingMapper offeringMapper;

        /// <summary>
        /// Command for submitting the form.
        /// </summary>
        private ICommand submitField;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        /// <param name="navigationService">The navigation services.</param>
        /// <param name="offeringMapper">Maps the offering data model to the view model.</param>
        public OfferingViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService,
            INavigationService navigationService,
            IOfferingMapper offeringMapper)
            : base(compositionContext, dataModel, subscriptionService, navigationService)
        {
            // Validate the parameter.
            if (offeringMapper == null)
            {
                throw new ArgumentNullException(nameof(offeringMapper));
            }

            // Initialize the object.
            this.offeringMapper = offeringMapper;

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
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
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
        /// Gets or sets the name of the offering.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
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
        /// Gets or sets the unique identifier of the offering.
        /// </summary>
        public Guid? OfferingId
        {
            get
            {
                return this.offeringIdField;
            }

            set
            {
                if (this.offeringIdField != value)
                {
                    this.offeringIdField = value;
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
            this.offeringMapper.Clear(this);

            // Extract the unique identifier for this view from the URI.
            this.OfferingId = navigationContext.Parameters.Count == 0 ? (Guid?)null : Guid.Parse(navigationContext.Parameters["offeringId"]);

            // There is a special, predefined identifier used to indicate that this is a view model for a new offering record.
            if (this.isUpdate = this.OfferingId.HasValue)
            {
                // Initialize the view model for an existing offering.
                this.BannerText = Resources.ProductProperties;
                OfferingRow offeringRow = this.DataModel.OfferingKey.Find(this.OfferingId.Value);
                this.offeringMapper.Map(offeringRow, this);
            }
            else
            {
                // Initialize the view model for a new offering.
                this.OfferingId = Guid.NewGuid();
                this.BannerText = Resources.NewProduct;
            }

            // This makes sure the navigation buttons reflect the current state.  Theoretically speaking, we can always navigate backwards from one
            // of these dialogs, this satisfies my instinct not to hard-code.
            this.GoBack.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Saves the offering view model to the persistent store.
        /// </summary>
        private void SubmitProduct()
        {
            // If the view model is valid then attempt to commit it to the persistent store.  If it isn't valid, then the field validation messages
            // will appear and give the user feedback about what fields need to be fixed.
            if (this.IsValid)
            {
                // The offering identifier indicates whether this is a new record or an existing one.
                if (this.isUpdate)
                {
                    // Ask the License Service to update the offering from this view model.
                    this.SubscriptionService.UpdateProductAsync(this);
                }
                else
                {
                    // Ask the License Service to create a offering from this view model.
                    this.SubscriptionService.InsertProductAsync(this);
                }

                // This will effectively dismiss the form by navigating back to the previous view.
                this.NavigationService.GoBack();
            }
        }
    }
}