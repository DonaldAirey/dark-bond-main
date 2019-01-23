// <copyright file="ICustomerMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Interface for mapping customer records.
    /// </summary>
    public interface ICustomerMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="customerViewModel">The property view model.</param>
        void Clear(ViewModels.Forms.CustomerViewModel customerViewModel);

        /// <summary>
        /// Maps the data model row into the property view model.
        /// </summary>
        /// <param name="customerRow">The data model row.</param>
        /// <param name="customerViewModel">The property view model.</param>
        void Map(CustomerRow customerRow, ViewModels.Forms.CustomerViewModel customerViewModel);

        /// <summary>
        /// Maps the data model row into the list view model.
        /// </summary>
        /// <param name="customerRow">The data model row.</param>
        /// <param name="customerViewModel">The list view model.</param>
        void Map(CustomerRow customerRow, ViewModels.ListViews.CustomerViewModel customerViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="customerViewModel">The property view model.</param>
        /// <param name="customer">A customer business entity.</param>
        /// <returns>A data model row.</returns>
        Customer Map(ViewModels.Forms.CustomerViewModel customerViewModel, Customer customer);
    }
}