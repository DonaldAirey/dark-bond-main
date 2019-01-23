// <copyright file="ISubscriptionService.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager
{
    using System;
    using System.Threading.Tasks;
    using DarkBond.SubscriptionManager.ViewModels.Forms;

    /// <summary>
    /// The view model for the frame of the application.
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Gets an indication of whether the underwriter can be deleted or not.
        /// </summary>
        /// <param name="underwriterId">The underwriter to be deleted.</param>
        /// <returns>true indicates the underwriter can be deleted, false otherwise.</returns>
        bool CanDeleteCustomer(Guid underwriterId);

        /// <summary>
        /// Gets an indication of whether the offering can be deleted or not.
        /// </summary>
        /// <param name="offeringId">The offering to be deleted.</param>
        /// <returns>true indicates the offering can be deleted, false otherwise.</returns>
        bool CanDeleteProduct(Guid offeringId);

        /// <summary>
        /// Deletes a underwriter from the persistent store.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter to be deleted.</param>
        /// <returns>A task.</returns>
        Task DeleteCustomerAsync(Guid underwriterId);

        /// <summary>
        /// Gets an indication of whether the subscription can be deleted or not.
        /// </summary>
        /// <param name="subscriptionId">The subscription to be deleted.</param>
        /// <returns>true indicates the subscription can be deleted, false otherwise.</returns>
        bool CanDeleteLicense(Guid subscriptionId);

        /// <summary>
        /// Gets an indication of whether the underwriter properties can be edited.
        /// </summary>
        /// <param name="underwriterId">The underwriter to be edited.</param>
        /// <returns>true indicates the underwriter can be edited, false otherwise.</returns>
        bool CanNavigateToCustomer(Guid underwriterId);

        /// <summary>
        /// Gets an indication of whether the subscription properties can be edited.
        /// </summary>
        /// <param name="subscriptionId">The subscription to be edited.</param>
        /// <returns>true indicates the subscription can be edited, false otherwise.</returns>
        bool CanNavigateToLicense(Guid subscriptionId);

        /// <summary>
        /// Gets an indication of whether the offering properties can be edited.
        /// </summary>
        /// <param name="offeringId">The offering to be edited.</param>
        /// <returns>true indicates the offering can be edited, false otherwise.</returns>
        bool CanNavigateToProduct(Guid offeringId);

        /// <summary>
        /// Deletes a subscription.
        /// </summary>
        /// <param name="subscriptionId">The unique identifier of the subscription to be deleted.</param>
        /// <returns>A task.</returns>
        Task DeleteLicenseAsync(Guid subscriptionId);

        /// <summary>
        /// Deletes a offering from the persistent store.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering to be deleted.</param>
        /// <returns>A task.</returns>
        Task DeleteProductAsync(Guid offeringId);

        /// <summary>
        /// Adds the underwriter view model to the persistent store.
        /// </summary>
        /// <param name="underwriterViewModel">The underwriter view model.</param>
        /// <returns>A task.</returns>
        Task InsertCustomerAsync(UnderwriterViewModel underwriterViewModel);

        /// <summary>
        /// Adds the subscription view model to the persistent store.
        /// </summary>
        /// <param name="subscriptionViewModel">The subscription view model.</param>
        /// <returns>A task.</returns>
        Task InsertLicenseAsync(SubscriptionViewModel subscriptionViewModel);

        /// <summary>
        /// Adds the offering view model to the persistent store.
        /// </summary>
        /// <param name="offeringViewModel">The offering view model.</param>
        /// <returns>A task.</returns>
        Task InsertProductAsync(OfferingViewModel offeringViewModel);

        /// <summary>
        /// Navigate to the given URI.
        /// </summary>
        /// <param name="uri">The target URI.</param>
        void NavigateTo(Uri uri);

        /// <summary>
        /// Navigate to the new underwriter page.
        /// </summary>
        void NavigateToCustomer();

        /// <summary>
        /// Navigate to the underwriter page.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter.</param>
        void NavigateToCustomer(Guid underwriterId);

        /// <summary>
        /// Navigate to the new subscription page for a underwriter.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter.</param>
        void NavigateToCustomerLicense(Guid underwriterId);

        /// <summary>
        /// Navigate to the subscription page.
        /// </summary>
        /// <param name="subscriptionId">The unique identifier of the subscription.</param>
        void NavigateToLicense(Guid subscriptionId);

        /// <summary>
        /// Navigate to the new offering page.
        /// </summary>
        void NavigateToProduct();

        /// <summary>
        /// Navigate to the offering page.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering.</param>
        void NavigateToProduct(Guid offeringId);

        /// <summary>
        /// Navigate to the new subscription page for a offering.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering.</param>
        void NavigateToProductLicense(Guid offeringId);

        /// <summary>
        /// Modifies the underwriter view model in the persistent store.
        /// </summary>
        /// <param name="underwriterViewModel">The underwriter view model.</param>
        /// <returns>A task.</returns>
        Task UpdateCustomerAsync(UnderwriterViewModel underwriterViewModel);

        /// <summary>
        /// Modifies the offering view model in the persistent store.
        /// </summary>
        /// <param name="offeringViewModel">The offering view model.</param>
        /// <returns>A task.</returns>
        Task UpdateProductAsync(OfferingViewModel offeringViewModel);

        /// <summary>
        /// Modifies the subscription view model in the persistent store.
        /// </summary>
        /// <param name="subscriptionViewModel">The subscription view model.</param>
        /// <returns>A task.</returns>
        Task UpdateLicenseAsync(SubscriptionViewModel subscriptionViewModel);
    }
}