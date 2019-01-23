// <copyright file="ILicenseService.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using DarkBond.LicenseManager.ViewModels.Forms;

    /// <summary>
    /// The view model for the frame of the application.
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>
        /// Gets an indication of whether the customer can be deleted or not.
        /// </summary>
        /// <param name="customerId">The customer to be deleted.</param>
        /// <returns>true indicates the customer can be deleted, false otherwise.</returns>
        bool CanDeleteCustomer(Guid customerId);

        /// <summary>
        /// Gets an indication of whether the product can be deleted or not.
        /// </summary>
        /// <param name="productId">The product to be deleted.</param>
        /// <returns>true indicates the product can be deleted, false otherwise.</returns>
        bool CanDeleteProduct(Guid productId);

        /// <summary>
        /// Deletes a customer from the persistent store.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to be deleted.</param>
        void DeleteCustomer(Guid customerId);

        /// <summary>
        /// Gets an indication of whether the license can be deleted or not.
        /// </summary>
        /// <param name="licenseId">The license to be deleted.</param>
        /// <returns>true indicates the license can be deleted, false otherwise.</returns>
        bool CanDeleteLicense(Guid licenseId);

        /// <summary>
        /// Gets an indication of whether the customer properties can be edited.
        /// </summary>
        /// <param name="customerId">The customer to be edited.</param>
        /// <returns>true indicates the customer can be edited, false otherwise.</returns>
        bool CanNavigateToCustomer(Guid customerId);

        /// <summary>
        /// Gets an indication of whether the license properties can be edited.
        /// </summary>
        /// <param name="licenseId">The license to be edited.</param>
        /// <returns>true indicates the license can be edited, false otherwise.</returns>
        bool CanNavigateToLicense(Guid licenseId);

        /// <summary>
        /// Gets an indication of whether the product properties can be edited.
        /// </summary>
        /// <param name="productId">The product to be edited.</param>
        /// <returns>true indicates the product can be edited, false otherwise.</returns>
        bool CanNavigateToProduct(Guid productId);

        /// <summary>
        /// Deletes a license.
        /// </summary>
        /// <param name="licenseId">The unique identifier of the license to be deleted.</param>
        void DeleteLicense(Guid licenseId);

        /// <summary>
        /// Deletes a product from the persistent store.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to be deleted.</param>
        void DeleteProduct(Guid productId);

        /// <summary>
        /// Adds the customer view model to the persistent store.
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        void InsertCustomer(CustomerViewModel customerViewModel);

        /// <summary>
        /// Adds the license view model to the persistent store.
        /// </summary>
        /// <param name="licenseViewModel">The license view model.</param>
        void InsertLicense(LicenseViewModel licenseViewModel);

        /// <summary>
        /// Adds the product view model to the persistent store.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        void InsertProduct(ProductViewModel productViewModel);

        /// <summary>
        /// Navigate to the given URI.
        /// </summary>
        /// <param name="uri">The target URI.</param>
        void NavigateTo(Uri uri);

        /// <summary>
        /// Navigate to the new customer page.
        /// </summary>
        void NavigateToCustomer();

        /// <summary>
        /// Navigate to the customer page.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        void NavigateToCustomer(Guid customerId);

        /// <summary>
        /// Navigate to the new license page for a customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        void NavigateToCustomerLicense(Guid customerId);

        /// <summary>
        /// Navigate to the license page.
        /// </summary>
        /// <param name="licenseId">The unique identifier of the license.</param>
        void NavigateToLicense(Guid licenseId);

        /// <summary>
        /// Navigate to the new product page.
        /// </summary>
        void NavigateToProduct();

        /// <summary>
        /// Navigate to the product page.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        void NavigateToProduct(Guid productId);

        /// <summary>
        /// Navigate to the new license page for a product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        void NavigateToProductLicense(Guid productId);

        /// <summary>
        /// Modifies the customer view model in the persistent store.
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        void UpdateCustomer(CustomerViewModel customerViewModel);

        /// <summary>
        /// Modifies the product view model in the persistent store.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        void UpdateProduct(ProductViewModel productViewModel);

        /// <summary>
        /// Modifies the license view model in the persistent store.
        /// </summary>
        /// <param name="licenseViewModel">The license view model.</param>
        void UpdateLicense(LicenseViewModel licenseViewModel);
    }
}