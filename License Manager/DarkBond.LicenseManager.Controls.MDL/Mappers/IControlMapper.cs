// <copyright file="IControlMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Controls
{
    using Controls;

    /// <summary>
    /// Interface for a class that maps the view models into the data models and back again.
    /// </summary>
    public interface IControlMapper
    {
        /// <summary>
        /// Map the fields from the <see cref="CountryRow"/> instance into those of a <see cref="CountryViewModel"/> instance.
        /// </summary>
        /// <param name="countryRow">The source <see cref="CountryRow"/> instance.</param>
        /// <param name="countryViewModel">The target <see cref="CountryViewModel"/> instance.</param>
        /// <returns>The <see cref="CountryViewModel"/> populated with the data from the <see cref="CountryRow"/>.</returns>
        CountryViewModel Map(CountryRow countryRow, CountryViewModel countryViewModel);

        /// <summary>
        /// Map the fields from the <see cref="CustomerRow"/> instance into those of a <see cref="CustomerViewModel"/> instance.
        /// </summary>
        /// <param name="customerRow">The source <see cref="CustomerRow"/> instance.</param>
        /// <param name="customerViewModel">The target <see cref="CustomerViewModel"/> instance.</param>
        /// <returns>The <see cref="CustomerViewModel"/> populated with the data from the <see cref="CustomerRow"/>.</returns>
        CustomerViewModel Map(CustomerRow customerRow, CustomerViewModel customerViewModel);

        /// <summary>
        /// Map the fields from the <see cref="LicenseTypeRow"/> instance into those of a <see cref="LicenseTypeViewModel"/> instance.
        /// </summary>
        /// <param name="licenseTypeRow">The source <see cref="LicenseTypeRow"/> instance.</param>
        /// <param name="licenseTypeViewModel">The target <see cref="LicenseTypeViewModel"/> instance.</param>
        /// <returns>The <see cref="LicenseTypeViewModel"/> populated with the data from the <see cref="LicenseTypeRow"/>.</returns>
        LicenseTypeViewModel Map(LicenseTypeRow licenseTypeRow, LicenseTypeViewModel licenseTypeViewModel);

        /// <summary>
        /// Map the fields from the <see cref="ProductRow"/> instance into those of a <see cref="ProductViewModel"/> instance.
        /// </summary>
        /// <param name="productRow">The source <see cref="ProductRow"/> instance.</param>
        /// <param name="productViewModel">The target <see cref="ProductViewModel"/> instance.</param>
        /// <returns>The <see cref="ProductViewModel"/> populated with the data from the <see cref="ProductRow"/>.</returns>
        ProductViewModel Map(ProductRow productRow, ProductViewModel productViewModel);
    }
}