// <copyright file="IProductMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Interface for mapping product records.
    /// </summary>
    public interface IProductMapper
    {
        /// <summary>
        /// Clears the view model.
        /// </summary>
        /// <param name="productViewModel">The view model.</param>
        void Clear(ViewModels.Forms.ProductViewModel productViewModel);

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="productRow">The data model row.</param>
        /// <param name="productViewModel">The property view model record.</param>
        void Map(ProductRow productRow, ViewModels.Forms.ProductViewModel productViewModel);

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="productRow">The data model row.</param>
        /// <param name="productViewModel">The list view model.</param>
        void Map(ProductRow productRow, ViewModels.ListViews.ProductViewModel productViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="productViewModel">The property view model.</param>
        /// <param name="product">A product business entity.</param>
        /// <returns>A data model row.</returns>
        Product Map(ViewModels.Forms.ProductViewModel productViewModel, Product product);
    }
}