// <copyright file="ProductMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using System;
    using System.Composition;
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Used to map product records.
    /// </summary>
    [Export(typeof(IProductMapper))]
    public class ProductMapper : IProductMapper
    {
        /// <summary>
        /// Clears the view model.
        /// </summary>
        /// <param name="productViewModel">The view model.</param>
        public void Clear(ViewModels.Forms.ProductViewModel productViewModel)
        {
            // Validate the parameter.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            productViewModel.Description = null;
            productViewModel.Name = null;
            productViewModel.ProductId = default(Guid);
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="productRow">The data model row.</param>
        /// <param name="productViewModel">The property view model record.</param>
        public void Map(ProductRow productRow, ViewModels.Forms.ProductViewModel productViewModel)
        {
            // Validate the parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Validate the parameter.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            // Map the data model into the view model.
            productViewModel.Description = productRow.Description;
            productViewModel.Name = productRow.Name;
            productViewModel.ProductId = productRow.ProductId;
        }

        /// <summary>
        /// Maps the data model row into the tree view model record.
        /// </summary>
        /// <param name="productRow">The data model row.</param>
        /// <param name="productViewModel">The tree view model record.</param>
        public void Map(ProductRow productRow, ViewModels.ListViews.ProductViewModel productViewModel)
        {
            // Validate the parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Validate the parameter.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            // Map the data model into the view model.
            productViewModel.DateCreated = productRow.DateCreated;
            productViewModel.DateModified = productRow.DateModified;
            productViewModel.Description = productRow.Description;
            productViewModel.Name = productRow.Name;
            productViewModel.ProductId = productRow.ProductId;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="productViewModel">The property view model.</param>
        /// <param name="product">A product business entity.</param>
        /// <returns>A data model row.</returns>
        public Product Map(ViewModels.Forms.ProductViewModel productViewModel, Product product)
        {
            // Validate the parameter.
            if (productViewModel == null)
            {
                throw new ArgumentNullException(nameof(productViewModel));
            }

            // Validate the parameter.
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.Description = string.IsNullOrEmpty(productViewModel.Description) ? null : productViewModel.Description;
            product.Name = productViewModel.Name.Trim();
            product.ProductId = productViewModel.ProductId.Value;
            return product;
        }
    }
}