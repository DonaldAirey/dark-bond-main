// <copyright file="LicenseService.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Composition;
    using System.Globalization;
    using DarkBond.LicenseManager.Entities;
    using DarkBond.LicenseManager.Mappers;
    using DarkBond.LicenseManager.ViewModels.Forms;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Events;
    using Repositories;

    /// <summary>
    /// The view model for the frame of the application.
    /// </summary>
    public class LicenseService : ILicenseService
    {
        /// <summary>
        /// Mediates actions between the customer view model and the data model.
        /// </summary>
        private CustomerRepository customerRepository;

        /// <summary>
        /// Provides mapping functions for the customer records.
        /// </summary>
        private ICustomerMapper customerMapper;

        /// <summary>
        /// The data model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Provides conversions between the view models and the data models.
        /// </summary>
        private ILicenseMapper licenseMapper;

        /// <summary>
        /// Mediates actions between the license view model and the data model.
        /// </summary>
        private LicenseRepository licenseRepository;

        /// <summary>
        /// Provides data mappings between the product view models and data models.
        /// </summary>
        private IProductMapper productMapper;

        /// <summary>
        /// Mediates actions between the product view model and the data model.
        /// </summary>
        private ProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseService"/> class.
        /// </summary>
        /// <param name="customerMapper">A mapper for customer records.</param>
        /// <param name="customerRepository">The repository for customer records.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseRepository">The repository for license records.</param>
        /// <param name="licenseMapper">The mapper for license records.</param>
        /// <param name="productMapper">The mapper for product records.</param>
        /// <param name="productRepository">The repository for product records.</param>
        [ImportingConstructor]
        public LicenseService(
            ICustomerMapper customerMapper,
            CustomerRepository customerRepository,
            IEventAggregator eventAggregator,
            DataModel dataModel,
            LicenseRepository licenseRepository,
            ILicenseMapper licenseMapper,
            IProductMapper productMapper,
            ProductRepository productRepository)
        {
            // Validate the argument.
            if (customerMapper == null)
            {
                throw new ArgumentNullException(nameof(customerMapper));
            }

            // Validate the argument.
            if (customerRepository == null)
            {
                throw new ArgumentNullException(nameof(customerRepository));
            }

            // Validate the argument.
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            // Validate the argument.
            if (licenseMapper == null)
            {
                throw new ArgumentNullException(nameof(licenseMapper));
            }

            // Validate the argument.
            if (licenseRepository == null)
            {
                throw new ArgumentNullException(nameof(licenseRepository));
            }

            // Validate the argument.
            if (productMapper == null)
            {
                throw new ArgumentNullException(nameof(productMapper));
            }

            // Validate the argument.
            if (productRepository == null)
            {
                throw new ArgumentNullException(nameof(productRepository));
            }

            // Initialize the object.
            this.customerMapper = customerMapper;
            this.customerRepository = customerRepository;
            this.dataModel = dataModel;
            this.licenseRepository = licenseRepository;
            this.licenseMapper = licenseMapper;
            this.productMapper = productMapper;
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Gets an indication of whether the customer can be deleted or not.
        /// </summary>
        /// <param name="customerId">The customer to be deleted.</param>
        /// <returns>true indicates the customer can be deleted, false otherwise.</returns>
        public bool CanDeleteCustomer(Guid customerId)
        {
            // We can delete a customer that has no licenses.
            return this.dataModel.CustomerLicenseCustomerIdKey.GetLicenseRows(customerId).Count == 0;
        }

        /// <summary>
        /// Gets an indication of whether the product can be deleted or not.
        /// </summary>
        /// <param name="productId">The product to be deleted.</param>
        /// <returns>true indicates the product can be deleted, false otherwise.</returns>
        public bool CanDeleteProduct(Guid productId)
        {
            // We can delete a product that has no licenses.
            return this.dataModel.ProductLicenseProductIdKey.GetLicenseRows(productId).Count == 0;
        }

        /// <summary>
        /// Deletes a customer from the persistent store.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to be deleted.</param>
        public async void DeleteCustomer(Guid customerId)
        {
            // Use the repository to delete this customer and then make sure a valid URI is selected.
            await this.customerRepository.DeleteCustomer(customerId);
        }

        /// <summary>
        /// Gets an indication of whether the product can be deleted or not.
        /// </summary>
        /// <param name="licenseId">The product to be deleted.</param>
        /// <returns>true, we can always delete a license.</returns>
        public bool CanDeleteLicense(Guid licenseId)
        {
            // We can always delete a license
            return true;
        }

        /// <summary>
        /// Gets an indication of whether the customer properties can be edited.
        /// </summary>
        /// <param name="customerId">The customer to be edited.</param>
        /// <returns>true indicates the customer can be edited, false otherwise.</returns>
        public bool CanNavigateToCustomer(Guid customerId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Gets an indication of whether the license properties can be edited.
        /// </summary>
        /// <param name="licenseId">The license to be edited.</param>
        /// <returns>true indicates the license can be edited, false otherwise.</returns>
        public bool CanNavigateToLicense(Guid licenseId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Gets an indication of whether the product properties can be edited.
        /// </summary>
        /// <param name="productId">The product to be edited.</param>
        /// <returns>true indicates the product can be edited, false otherwise.</returns>
        public bool CanNavigateToProduct(Guid productId)
        {
            // We can edit the properties only when a single item is selected.
            return GlobalCommands.Properties.RegisteredCommands.Count == 1;
        }

        /// <summary>
        /// Deletes a license.
        /// </summary>
        /// <param name="licenseId">The unique identifier of the license to be deleted.</param>
        public async void DeleteLicense(Guid licenseId)
        {
            // Use the repository to delete this license.
            await this.licenseRepository.DeleteLicense(licenseId);
        }

        /// <summary>
        /// Deletes a product from the persistent store.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to be deleted.</param>
        public async void DeleteProduct(Guid productId)
        {
            // Use the repository to delete this product and then make sure a valid URI is selected.
            await this.productRepository.DeleteProduct(productId);
        }

        /// <summary>
        /// Adds the customer view model to the persistent store.
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        public async void InsertCustomer(CustomerViewModel customerViewModel)
        {
            // Validate the argument.
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            // Add the customer to the persistent store.
            await this.customerRepository.InsertCustomer(this.customerMapper.Map(customerViewModel, new Customer()));
        }

        /// <summary>
        /// Adds the license view model to the persistent store.
        /// </summary>
        /// <param name="licenseViewModel">The license view model.</param>
        public async void InsertLicense(LicenseViewModel licenseViewModel)
        {
            // Validate the argument.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Add the license to the persistent store.
            await this.licenseRepository.InsertLicense(this.licenseMapper.Map(licenseViewModel, new License()));
        }

        /// <summary>
        /// Adds the product view model to the persistent store.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        public async void InsertProduct(ProductViewModel productViewModel)
        {
            // Validate the argument.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            // Add the product to the persistent store.
            await this.productRepository.InsertProduct(this.productMapper.Map(productViewModel, new Product()));
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
        /// Navigate to the new customer page.
        /// </summary>
        public void NavigateToCustomer()
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.CustomerView");
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the customer page.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        public void NavigateToCustomer(Guid customerId)
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.CustomerView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "customerId={0}", customerId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new license page for a customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        public void NavigateToCustomerLicense(Guid customerId)
        {
            // Navigate to the Edit Customer view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.LicenseView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "customerId={0}", customerId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the license page.
        /// </summary>
        /// <param name="licenseId">The unique identifier of the license.</param>
        public void NavigateToLicense(Guid licenseId)
        {
            // Navigate to view an existing license.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.LicenseView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "licenseId={0}", licenseId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new product page.
        /// </summary>
        public void NavigateToProduct()
        {
            // Navigate to the Edit Product view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.ProductView");
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the product page.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        public void NavigateToProduct(Guid productId)
        {
            // Navigate to the Edit Product view.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.ProductView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "productId={0}", productId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Navigate to the new license page for a product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        public void NavigateToProductLicense(Guid productId)
        {
            // Navigate to view an existing license.
            UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.LicenseManager.Views;/DarkBond.LicenseManager.Views.Forms.LicenseView");
            uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "productId={0}", productId);
            this.NavigateTo(uriBuilder.Uri);
        }

        /// <summary>
        /// Modifies the customer view model in the persistent store.
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        public async void UpdateCustomer(CustomerViewModel customerViewModel)
        {
            // Update the customer in the repository.
            await this.customerRepository.UpdateCustomer(this.customerMapper.Map(customerViewModel, new Customer()));
        }

        /// <summary>
        /// Modifies the product view model in the persistent store.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        public async void UpdateProduct(ProductViewModel productViewModel)
        {
            // Update the product in the repository.
            await this.productRepository.UpdateProduct(this.productMapper.Map(productViewModel, new Product()));
        }

        /// <summary>
        /// Modifies the license view model in the persistent store.
        /// </summary>
        /// <param name="licenseViewModel">The license view model.</param>
        public async void UpdateLicense(LicenseViewModel licenseViewModel)
        {
            // Update the license in the repository.
            await this.licenseRepository.UpdateLicense(this.licenseMapper.Map(licenseViewModel, new License()));
        }
    }
}