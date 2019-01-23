// <copyright file="SubscriptionViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Composition;
    using System.Windows.Input;
    using DarkBond.Navigation;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.SubscriptionManager.Mappers;
    using DarkBond.SubscriptionManager.ViewModels.Controls;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// License view model.
    /// </summary>
    public class SubscriptionViewModel : CommonFormViewModel
    {
        /// <summary>
        /// The text that appears in the banner of the dialog.
        /// </summary>
        private string bannerTextField;

        /// <summary>
        /// The unique identifier of the underwriter.
        /// </summary>
        private Guid? underwriterIdField;

        /// <summary>
        /// The underwriter name.
        /// </summary>
        private string underwriterNameField;

        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// The view model for the underwriter ComboBox.
        /// </summary>
        private UnderwriterCollection underwriterCollection;

        /// <summary>
        /// A table that drives the notifications when underwriter row properties change.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> underwriterNotifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// Maps data model records to view models and back again.
        /// </summary>
        private ISubscriptionMapper subscriptionMapper;

        /// <summary>
        /// Indicates that the offering is new.
        /// </summary>
        private bool isUpdate;

        /// <summary>
        /// The unique identifier of the subscription.
        /// </summary>
        private Guid? subscriptionIdField;

        /// <summary>
        /// The current state of the view (whether it is showing a Customer License, Product License or general subscription).
        /// </summary>
        private SubscriptionViewState subscriptionViewState;

        /// <summary>
        /// The view model for the offering ComboBox.
        /// </summary>
        private OfferingCollection offeringCollection;

        /// <summary>
        /// The unique identifier of the offering.
        /// </summary>
        private Guid? offeringIdField;

        /// <summary>
        /// The offering name.
        /// </summary>
        private string offeringNameField;

        /// <summary>
        /// A table that drives the notifications when offering row properties change.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> offeringNotifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// The submit command.
        /// </summary>
        private ICommand submitField = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="underwriterCollection">The collection of underwriters.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionMapper">Maps subscriptions from the data model to the view model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        /// <param name="navigationService">The navigation services.</param>
        /// <param name="offeringCollection">The collection of offerings.</param>
        public SubscriptionViewModel(
            CompositionContext compositionContext,
            UnderwriterCollection underwriterCollection,
            DataModel dataModel,
            ISubscriptionMapper subscriptionMapper,
            ISubscriptionService subscriptionService,
            INavigationService navigationService,
            OfferingCollection offeringCollection)
            : base(compositionContext, dataModel, subscriptionService, navigationService)
        {
            // Initialize the object.
            this.underwriterCollection = underwriterCollection;
            this.subscriptionMapper = subscriptionMapper;
            this.offeringCollection = offeringCollection;

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
        /// Gets the collection of underwriters.
        /// </summary>
        public UnderwriterCollection Underwriters
        {
            get
            {
                return this.underwriterCollection;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the underwriter.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
        public Guid? UnderwriterId
        {
            get
            {
                return this.underwriterIdField;
            }

            set
            {
                if (this.underwriterIdField != value)
                {
                    this.underwriterIdField = value;
                    this.OnPropertyChanged("UnderwriterId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the underwriter.
        /// </summary>
        public string CustomerName
        {
            get
            {
                return this.underwriterNameField;
            }

            set
            {
                if (this.underwriterNameField != value)
                {
                    this.underwriterNameField = value;
                    this.OnPropertyChanged("CustomerName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the subscription.
        /// </summary>
        public Guid? SubscriptionId
        {
            get
            {
                return this.subscriptionIdField;
            }

            set
            {
                if (this.subscriptionIdField != value)
                {
                    this.subscriptionIdField = value;
                    this.OnPropertyChanged("LicensedId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the offerings.
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredFieldViolation", ErrorMessageResourceType = typeof(Errors))]
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
                    this.OnPropertyChanged("OfferingId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the offering.
        /// </summary>
        public string ProductName
        {
            get
            {
                return this.offeringNameField;
            }

            set
            {
                if (this.offeringNameField != value)
                {
                    this.offeringNameField = value;
                    this.OnPropertyChanged("ProductName");
                }
            }
        }

        /// <summary>
        /// Gets the collection of offerings.
        /// </summary>
        public OfferingCollection Offerings
        {
            get
            {
                return this.offeringCollection;
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
        public SubscriptionViewState ViewState
        {
            get
            {
                return this.subscriptionViewState;
            }

            set
            {
                if (this.subscriptionViewState != value)
                {
                    this.subscriptionViewState = value;
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
                case SubscriptionViewState.Customer:

                    this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;
                    break;

                case SubscriptionViewState.Product:

                    this.offeringRowField.PropertyChanged -= this.OnOfferingRowChanged;
                    break;

                case SubscriptionViewState.License:

                    this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;
                    this.offeringRowField.PropertyChanged -= this.OnOfferingRowChanged;
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
            this.subscriptionMapper.Clear(this);

            // Configure to create a subscription for a specific underwriter.
            string underwriterIdText = navigationContext.Parameters["underwriterId"];
            if (underwriterIdText != null)
            {
                this.ViewState = SubscriptionViewState.Customer;
                this.isUpdate = false;
                this.BannerText = Resources.NewLicense;
                this.SubscriptionId = Guid.NewGuid();
                this.UnderwriterId = Guid.Parse(underwriterIdText);

                // Bind the view model to the data model for the underwriters.
                this.InitializeCustomerBinding(this.UnderwriterId.Value);
            }

            // Configure to create a subscription for a specific underwriter.
            string offeringIdText = navigationContext.Parameters["offeringId"];
            if (offeringIdText != null)
            {
                this.ViewState = SubscriptionViewState.Product;
                this.isUpdate = false;
                this.BannerText = Resources.NewLicense;
                this.SubscriptionId = Guid.NewGuid();
                this.OfferingId = Guid.Parse(offeringIdText);

                // Bind the view model to the data model for the offerings.
                this.InitializeProductBinding(this.OfferingId.Value);
            }

            // Configure to display a subscription.
            string subscriptionIdText = navigationContext.Parameters["subscriptionId"];
            if (subscriptionIdText != null)
            {
                this.ViewState = SubscriptionViewState.License;
                this.isUpdate = true;
                this.BannerText = Resources.LicenseProperties;
                this.SubscriptionId = Guid.Parse(subscriptionIdText);
                SubscriptionRow subscriptionRow = this.DataModel.SubscriptionKey.Find(this.SubscriptionId.Value);
                this.subscriptionMapper.Map(subscriptionRow, this);

                // Bind the view model to the data model for the underwriters and offerings.
                this.InitializeCustomerBinding(subscriptionRow.UnderwriterId);
                this.InitializeProductBinding(subscriptionRow.OfferingId);
            }
        }

        /// <summary>
        /// Initialize the binding of the underwriter values to the data model.
        /// </summary>
        /// <param name="underwriterId">The underwriter id.</param>
        private void InitializeCustomerBinding(Guid underwriterId)
        {
            // Instruct the Customer row to notify this view model of relevant changes.
            this.underwriterRowField = this.DataModel.UnderwriterKey.Find(underwriterId);
            this.underwriterRowField.PropertyChanged += this.OnUnderwriterRowChanged;

            // Initialize the view model with the underwriter properties.
            foreach (string property in this.underwriterNotifyActions.Keys)
            {
                this.underwriterNotifyActions[property](this.underwriterRowField);
            }
        }

        /// <summary>
        /// Initialize the binding of the offering values to the data model.
        /// </summary>
        /// <param name="offeringId">The offering id.</param>
        private void InitializeProductBinding(Guid offeringId)
        {
            // Instruct the Product row to notify this view model of relevant changes.
            this.offeringRowField = this.DataModel.OfferingKey.Find(offeringId);
            this.offeringRowField.PropertyChanged += this.OnOfferingRowChanged;

            // This table drives the updating of the view model when the offering properties changes.
            this.offeringNotifyActions.Add("Name", (p) => this.ProductName = p.Name);

            // Initialize the view model with the offering properties.
            foreach (string property in this.offeringNotifyActions.Keys)
            {
                this.offeringNotifyActions[property](this.offeringRowField);
            }
        }

        /// <summary>
        /// Handles a change to the underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UnderwriterRow underwriterRow = sender as UnderwriterRow;
            Action<UnderwriterRow> underwriterNotifyAction;
            if (this.underwriterNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out underwriterNotifyAction))
            {
                underwriterNotifyAction(underwriterRow);
            }
        }

        /// <summary>
        /// Handles a change to the offering row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnOfferingRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OfferingRow offeringRow = sender as OfferingRow;
            Action<OfferingRow> offeringNotifyAction;
            if (this.offeringNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out offeringNotifyAction))
            {
                offeringNotifyAction(offeringRow);
            }
        }

        /// <summary>
        /// Saves the subscription view model to the persistent store.
        /// </summary>
        private void SubmitLicense()
        {
            // If the view model is valid then attempt to commit it to the persistent store.  If it isn't valid, then the field validation messages
            // will appear and give the user feedback about what fields need to be fixed.
            if (this.IsValid)
            {
                // The subscription identifier indicates whether this is a new record or an existing one.  Make sure to use the internal variable to set
                // the subscription identifier so as to not trigger the initialization of the view model.
                if (this.isUpdate)
                {
                    this.SubscriptionService.UpdateLicenseAsync(this);
                }
                else
                {
                    this.SubscriptionService.InsertLicenseAsync(this);
                }

                // Navigate to the previous page when we've successfully created or update the subscription.
                this.NavigationService.GoBack();
            }
        }
    }
}