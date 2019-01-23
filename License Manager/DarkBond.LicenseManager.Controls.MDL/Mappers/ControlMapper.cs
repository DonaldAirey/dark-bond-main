// <copyright file="ControlMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Controls
{
    using System;
    using System.Composition;
    using Controls;

    /// <summary>
    /// Maps the view models into the data models and back again.
    /// </summary>
    public class ControlMapper : IControlMapper
    {
        /// <summary>
        /// Map the fields from the <see cref="CountryRow"/> instance into those of a <see cref="CountryViewModel"/> instance.
        /// </summary>
        /// <param name="countryRow">The source <see cref="CountryRow"/> instance.</param>
        /// <param name="countryViewModel">The target <see cref="CountryViewModel"/> instance.</param>
        /// <returns>The <see cref="CountryViewModel"/> populated with the data from the <see cref="CountryRow"/>.</returns>
        public CountryViewModel Map(CountryRow countryRow, CountryViewModel countryViewModel)
        {
            // Validate the countryRow parameter.
            if (countryRow == null)
            {
                throw new ArgumentNullException(nameof(countryRow));
            }

            // Validate the countryViewModel parameter.
            if (countryViewModel == null)
            {
                throw new ArgumentNullException(nameof(countryViewModel));
            }

            // Map the fields from the data model to the view model.
            countryViewModel.CountryId = countryRow.CountryId;
            countryViewModel.Name = countryRow.Name;
            return countryViewModel;
        }

        /// <summary>
        /// Map the fields from the <see cref="CustomerRow"/> instance into those of a <see cref="CustomerViewModel"/> instance.
        /// </summary>
        /// <param name="customerRow">The source <see cref="CustomerRow"/> instance.</param>
        /// <param name="customerViewModel">The target <see cref="CustomerViewModel"/> instance.</param>
        /// <returns>The <see cref="CustomerViewModel"/> populated with the data from the <see cref="CustomerRow"/>.</returns>
        public CustomerViewModel Map(CustomerRow customerRow, CustomerViewModel customerViewModel)
        {
            // Validate the customerRow parameter.
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            // Validate the customerViewModel parameter.
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            // Map the fields from the data model to the view model.
            customerViewModel.Address1 = customerRow.Address1;
            customerViewModel.Address2 = customerRow.Address2;
            customerViewModel.City = customerRow.City;
            customerViewModel.CustomerId = customerRow.CustomerId;
            customerViewModel.CountryId = customerRow.CountryId;
            customerViewModel.Email = customerRow.Email;
            customerViewModel.FirstName = customerRow.FirstName;
            customerViewModel.LastName = customerRow.LastName;
            customerViewModel.Phone = customerRow.Phone;
            customerViewModel.PostalCode = customerRow.PostalCode;
            customerViewModel.ProvinceId = customerRow.ProvinceId;
            return customerViewModel;
        }

        /// <summary>
        /// Map the fields from the <see cref="LicenseTypeRow"/> instance into those of a <see cref="LicenseTypeViewModel"/> instance.
        /// </summary>
        /// <param name="licenseTypeRow">The source <see cref="LicenseTypeRow"/> instance.</param>
        /// <param name="licenseTypeViewModel">The target <see cref="LicenseTypeViewModel"/> instance.</param>
        /// <returns>The <see cref="LicenseTypeViewModel"/> populated with the data from the <see cref="LicenseTypeRow"/>.</returns>
        public LicenseTypeViewModel Map(LicenseTypeRow licenseTypeRow, LicenseTypeViewModel licenseTypeViewModel)
        {
            // Validate the licenseTypeRow parameter.
            if (licenseTypeRow == null)
            {
                throw new ArgumentNullException(nameof(licenseTypeRow));
            }

            // Validate the licenseTypeViewModel parameter.
            if (licenseTypeViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseTypeViewModel));
            }

            // Map the fields from the data model to the view model.
            licenseTypeViewModel.Description = licenseTypeRow.Description;
            licenseTypeViewModel.LicenseTypeCode = (LicenseTypeCode)licenseTypeRow.LicenseTypeCode;
            return licenseTypeViewModel;
        }

        /// <summary>
        /// Map the fields from the <see cref="ProductRow"/> instance into those of a <see cref="ProductViewModel"/> instance.
        /// </summary>
        /// <param name="productRow">The source <see cref="ProductRow"/> instance.</param>
        /// <param name="productViewModel">The target <see cref="ProductViewModel"/> instance.</param>
        /// <returns>The <see cref="ProductViewModel"/> populated with the data from the <see cref="ProductRow"/>.</returns>
        public ProductViewModel Map(ProductRow productRow, ProductViewModel productViewModel)
        {
            // Validate the productRow parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Validate the productViewModel parameter.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            // Map the fields from the data model to the view model.
            productViewModel.Description = productRow.Description;
            productViewModel.Name = productRow.Name;
            productViewModel.ProductId = productRow.ProductId;
            return productViewModel;
        }
    }
}