// <copyright file="SubscriptionService.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using DarkBond.SubscriptionManager.Entities;
    using DarkBond.SubscriptionManager.Mappers;
    using DarkBond.SubscriptionManager.ViewModels.Forms;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Events;
    using Repositories;

    /// <summary>
    /// The view model for the frame of the application.
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        /// <summary>
        /// Mediates actions between the underwriter view model and the data model.
        /// </summary>
        private UnderwriterRepository underwriterRepository;

        /// <summary>
        /// Provides mapping functions for the underwriter records.
        /// </summary>
        private IUnderwriterMapper underwriterMapper;

        /// <summary>
        /// The data model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Provides conversions between the view models and the data models.
        /// </summary>
        private ISubscriptionMapper subscriptionMapper;

        /// <summary>
        /// Mediates actions between the subscription view model and the data model.
        /// </summary>
        private SubscriptionRepository subscriptionRepository;

        /// <summary>
        /// Provides data mappings between the offering view models and data models.
        /// </summary>
        private IOfferingMapper offeringMapper;

        /// <summary>
        /// Mediates actions between the offering view model and the data model.
        /// </summary>
        private OfferingRepository offeringRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
        /// </summary>
        /// <param name="underwriterMapper">A mapper for underwriter records.</param>
        /// <param name="underwriterRepository">The repository for underwriter records.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionRepository">The repository for subscription records.</param>
        /// <param name="subscriptionMapper">The mapper for subscription records.</param>
        /// <param name="offeringMapper">The mapper for offering records.</param>
        /// <param name="offeringRepository">The repository for offering records.</param>
        public SubscriptionService(
            IUnderwriterMapper underwriterMapper,
            UnderwriterRepository underwriterRepository,
            IEventAggregator eventAggregator,
            DataModel dataModel,
            SubscriptionRepository subscriptionRepository,
            ISubscriptionMapper subscriptionMapper,
            IOfferingMapper offeringMapper,
            OfferingRepository offeringRepository)
        {
            // Validate the argument.
            if (underwriterMapper == null)
            {
                throw new ArgumentNullException(nameof(underwriterMapper));
            }

            // Validate the argument.
            if (underwriterRepository == null)
            {
                throw new ArgumentNullException(nameof(underwriterRepository));
            }

            // Validate the argument.
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            // Validate the argument.
            if (subscriptionMapper == null)
            {
                throw new ArgumentNullException(nameof(subscriptionMapper));
            }

            // Validate the argument.
            if (subscriptionRepository == null)
            {
                throw new ArgumentNullException(nameof(subscriptionRepository));
            }

            // Validate the argument.
            if (offeringMapper == null)
            {
                throw new ArgumentNullException(nameof(offeringMapper));
            }

            // Validate the argument.
            if (offeringRepository == null)
            {
                throw new ArgumentNullException(nameof(offeringRepository));
            }

            // Initialize the object.
            this.underwriterMapper = underwriterMapper;
            this.underwriterRepository = underwriterRepository;
            this.dataModel = dataModel;
            this.subscriptionRepository = subscriptionRepository;
            this.subscriptionMapper = subscriptionMapper;
            this.offeringMapper = offeringMapper;
            this.offeringRepository = offeringRepository;
        }

        /// <summary>
        /// Gets an indication of whether the underwriter can be deleted or not.
        /// </summary>
        /// <param name="underwriterId">The underwriter to be deleted.</param>
        /// <returns>true indicates the underwriter can be deleted, false otherwise.</returns>
        public bool CanDeleteCustomer(Guid underwriterId)
        {
            // We can delete a underwriter that has no subscriptions.
            return this.dataModel.UnderwriterSubscriptionUnderwriterIdKey.GetSubscriptionRows(underwriterId).Count == 0;
        }

        /// <summary>
        /// Gets an indication of whether the offering can be deleted or not.
        /// </summary>
        /// <param name="offeringId">The offering to be deleted.</param>
        /// <returns>true indicates the offering can be deleted, false otherwise.</returns>
        public bool CanDeleteProduct(Guid offeringId)
        {
            // We can delete a offering that has no subscriptions.
            return this.dataModel.OfferingSubscriptionOfferingIdKey.GetSubscriptionRows(offeringId).Count == 0;
        }

        /// <summary>
        /// Deletes a underwriter from the persistent store.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter to be deleted.</param>
        /// <returns>A task.</returns>
        public async Task DeleteCustomerAsync(Guid underwriterId)
        {
            // Use the repository to delete this underwriter and then make sure a valid URI is selected.
            await this.underwriterRepository.DeleteCustomerAsync(underwriterId);
        }

        /// <summary>
        /// Gets an indication of whether the offering can be deleted or not.
        /// </summary>
        /// <param name="subscriptionId">The offering to be deleted.</param>
        /// <returns>true, we can always delete a subscription.</returns>
        public bool CanDeleteLicense(Guid subscriptionId)
        {
            // We can always delete a subscription
            return true;
        }

        /// <summary>
        /// Gets an indication of whether the underwriter properties can be edited.
        /// </summary>
        /// <param name="underwriterId">The underwriter to be edited.</param>
        /// <returns>true indicates the underwriter can be edited, false otherwise.</returns>
        public bool CanNavigateToCustomer(Guid underwriterId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Gets an indication of whether the subscription properties can be edited.
        /// </summary>
        /// <param name="subscriptionId">The subscription to be edited.</param>
        /// <returns>true indicates the subscription can be edited, false otherwise.</returns>
        public bool CanNavigateToLicense(Guid subscriptionId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Gets an indication of whether the offering properties can be edited.
        /// </summary>
        /// <param name="offeringId">The offering to be edited.</param>
        /// <returns>true indicates the offering can be edited, false otherwise.</returns>
        public bool CanNavigateToProduct(Guid offeringId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Deletes a subscription.
        /// </summary>
        /// <param name="subscriptionId">The unique identifier of the subscription to be deleted.</param>
        /// <returns>A task.</returns>
        public async Task DeleteLicenseAsync(Guid subscriptionId)
        {
            // Use the repository to delete this subscription.
            await this.subscriptionRepository.DeleteLicenseAsync(subscriptionId);
        }

        /// <summary>
        /// Deletes a offering from the persistent store.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering to be deleted.</param>
        /// <returns>A task.</returns>
        public async Task DeleteProductAsync(Guid offeringId)
        {
            // Use the repository to delete this offering and then make sure a valid URI is selected.
            await this.offeringRepository.DeleteProductAsync(offeringId);
        }

        /// <summary>
        /// Adds the underwriter view model to the persistent store.
        /// </summary>
        /// <param name="underwriterViewModel">The underwriter view model.</param>
        /// <returns>A task.</returns>
        public async Task InsertCustomerAsync(UnderwriterViewModel underwriterViewModel)
        {
            // Validate the argument.
            if (underwriterViewModel == null)
            {
                throw new ArgumentNullException(nameof(underwriterViewModel));
            }

            // Add the underwriter to the persistent store.
            await this.underwriterRepository.InsertCustomerAsync(this.underwriterMapper.Map(underwriterViewModel, new Underwriter()));
        }

        /// <summary>
        /// Adds the subscription view model to the persistent store.
        /// </summary>
        /// <param name="subscriptionViewModel">The subscription view model.</param>
        /// <returns>A task.</returns>
        public async Task InsertLicenseAsync(SubscriptionViewModel subscriptionViewModel)
        {
            // Validate the argument.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Add the subscription to the persistent store.
            await this.subscriptionRepository.InsertLicenseAsync(this.subscriptionMapper.Map(subscriptionViewModel, new Subscription()));
        }

        /// <summary>
        /// Adds the offering view model to the persistent store.
        /// </summary>
        /// <param name="offeringViewModel">The offering view model.</param>
        /// <returns>A task.</returns>
        public async Task InsertProductAsync(OfferingViewModel offeringViewModel)
        {
            // Validate the argument.
            if (offeringViewModel == null)
            {
                throw new ArgumentNullException(nameof(offeringViewModel));
            }

            // Add the offering to the persistent store.
            await this.offeringRepository.InsertProductAsync(this.offeringMapper.Map(offeringViewModel, new Offering()));
        }

        /// <summary>
        /// Navigate to the given URI.
        /// </summary>
        /// <param name="uri">The target URI.</param>
        public void NavigateTo(Uri uri)
        {
            // The global commands are probably not a feature that will work in Windows RT.  This should probably be replace with some other
            // mechanism.
            GlobalCommands.Locate.Execute(uri);
        }

        /// <summary>
        /// Navigate to the new underwriter page.
        /// </summary>
        public void NavigateToCustomer()
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.UnderwriterView");
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the underwriter page.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter.</param>
        public void NavigateToCustomer(Guid underwriterId)
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.UnderwriterView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "underwriterId={0}", underwriterId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new subscription page for a underwriter.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter.</param>
        public void NavigateToCustomerLicense(Guid underwriterId)
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.SubscriptionView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "underwriterId={0}", underwriterId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the subscription page.
        /// </summary>
        /// <param name="subscriptionId">The unique identifier of the subscription.</param>
        public void NavigateToLicense(Guid subscriptionId)
        {
            // Navigate to view an existing subscription.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.SubscriptionView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "subscriptionId={0}", subscriptionId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new offering page.
        /// </summary>
        public void NavigateToProduct()
        {
            // Navigate to the Edit Product view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.OfferingView");
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the offering page.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering.</param>
        public void NavigateToProduct(Guid offeringId)
        {
            // Navigate to the Edit Product view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.OfferingView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "offeringId={0}", offeringId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new subscription page for a offering.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering.</param>
        public void NavigateToProductLicense(Guid offeringId)
        {
            // Navigate to view an existing subscription.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.SubscriptionView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "offeringId={0}", offeringId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Modifies the underwriter view model in the persistent store.
        /// </summary>
        /// <param name="underwriterViewModel">The underwriter view model.</param>
        /// <returns>A task.</returns>
        public async Task UpdateCustomerAsync(UnderwriterViewModel underwriterViewModel)
        {
            // Update the underwriter in the repository.
            await this.underwriterRepository.UpdateCustomerAsync(this.underwriterMapper.Map(underwriterViewModel, new Underwriter()));
        }

        /// <summary>
        /// Modifies the offering view model in the persistent store.
        /// </summary>
        /// <param name="offeringViewModel">The offering view model.</param>
        /// <returns>A task.</returns>
        public async Task UpdateProductAsync(OfferingViewModel offeringViewModel)
        {
            // Update the offering in the repository.
            await this.offeringRepository.UpdateProductAsync(this.offeringMapper.Map(offeringViewModel, new Offering()));
        }

        /// <summary>
        /// Modifies the subscription view model in the persistent store.
        /// </summary>
        /// <param name="subscriptionViewModel">The subscription view model.</param>
        /// <returns>A task.</returns>
        public async Task UpdateLicenseAsync(SubscriptionViewModel subscriptionViewModel)
        {
            // Update the subscription in the repository.
            await this.subscriptionRepository.UpdateLicenseAsync(this.subscriptionMapper.Map(subscriptionViewModel, new Subscription()));
        }
    }
}