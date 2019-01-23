// <copyright file="ProductRepository.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Repositories
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Mediates the between the view model and the data model for Products.
    /// </summary>
    public class ProductRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public ProductRepository(
            Binding binding,
            ClientSecurityToken clientSecurityToken,
            ICommunicationExceptionHandler communicationExceptionHandler,
            EndpointAddress endpointAddress,
            DataModel dataModel)
            : base(binding, clientSecurityToken, communicationExceptionHandler, endpointAddress)
        {
            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Initialize the object.
            this.dataModel = dataModel;
        }

        /// <summary>
        /// Deletes a product from the data model.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteProduct(Guid productId)
        {
            // Find the product row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            ProductRow productRow = this.dataModel.ProductKey.Find(productId);
            if (productRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteProductAsync(productId, productRow.RowVersion);
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateProductOperation"))
                    {
                        break;
                    }
                }
                catch (TimeoutException)
                {
                }
            }

            // If we reached here there was a handled error.
            return false;
        }

        /// <summary>
        /// Inserts a product into the data model.
        /// </summary>
        /// <param name="product">A Product business entity.</param>
        /// <returns>True indicates it was successful stored, false otherwise.</returns>
        public async Task<bool> InsertProduct(Product product)
        {
            // Validate the parameter
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            // Initialize the record.
            product.ProductId = Guid.NewGuid();
            product.DateCreated = DateTime.Now;
            product.DateModified = product.DateCreated;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the product.
                    await this.DataServiceClient.CreateProductAsync(
                       product.DateCreated,
                       product.DateModified,
                       product.Description,
                       product.ExternalId0,
                       product.Name,
                       product.ProductId);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateProductOperation"))
                    {
                        break;
                    }
                }
                catch (TimeoutException)
                {
                }
            }

            // If we reached here the operation failed.
            return false;
        }

        /// <summary>
        /// Updates a product in the data model.
        /// </summary>
        /// <param name="product">A Product business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateProduct(Product product)
        {
            // Validate the parameter
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing to do here.
            ProductRow targetProductRow = this.dataModel.ProductKey.Find(product.ProductId);
            if (targetProductRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            product.DateCreated = targetProductRow.DateCreated;
            product.DateModified = targetProductRow.DateModified;
            product.DateModified = DateTime.Now;
            product.RowVersion = targetProductRow.RowVersion;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the product.
                    await this.DataServiceClient.UpdateProductAsync(
                       product.DateCreated,
                       product.DateModified,
                       product.Description,
                       product.ExternalId0,
                       product.Name,
                       product.ProductId,
                       product.ProductId,
                       product.RowVersion);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateProductOperation"))
                    {
                        break;
                    }
                }
                catch (TimeoutException)
                {
                }
            }

            // If we reached here the operation failed.
            return false;
        }
    }
}