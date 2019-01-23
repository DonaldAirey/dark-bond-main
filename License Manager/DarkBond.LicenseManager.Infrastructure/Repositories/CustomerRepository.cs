// <copyright file="CustomerRepository.cs" company="Dark Bond, Inc.">
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

    /// <summary>
    /// Mediates the between the view model and the data model for Customers.
    /// </summary>
    public class CustomerRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public CustomerRepository(
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
        /// Inserts a customer.
        /// </summary>
        /// <param name="customer">The customer record.</param>
        /// <returns>true indicates success, false otherwise.</returns>
        public async Task<bool> InsertCustomer(Entities.Customer customer)
        {
            // Validate the parameter
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            // Initialize the record.
            customer.CustomerId = Guid.NewGuid();
            customer.DateModified = DateTime.Now;
            customer.DateCreated = DateTime.Now;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the customer.
                    await this.DataServiceClient.CreateCustomerAsync(
                       customer.Address1,
                       customer.Address2,
                       customer.City,
                       customer.Company,
                       customer.CountryId,
                       customer.CustomerId,
                       customer.DateCreated,
                       customer.DateModified,
                       customer.DateOfBirth,
                       customer.Email,
                       customer.ExternalId0,
                       customer.FirstName,
                       customer.LastName,
                       customer.MiddleName,
                       customer.PhoneNumber,
                       customer.PostalCode,
                       customer.ProvinceId);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateCustomerOperation"))
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
        /// Deletes a customer from the data model.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            // Find the customer row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            CustomerRow customerRow = this.dataModel.CustomerKey.Find(customerId);
            if (customerRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteCustomerAsync(customerId, customerRow.RowVersion);
                    break;
                }
                catch (CommunicationException communicationException)
                {
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateCustomerOperation"))
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
        /// Updates a customer in the data model.
        /// </summary>
        /// <param name="customer">A customer business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateCustomer(Entities.Customer customer)
        {
            // Validate the parameter
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing to do here.
            CustomerRow customerRow = this.dataModel.CustomerKey.Find(customer.CustomerId);
            if (customerRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            customer.DateCreated = customerRow.DateCreated;
            customer.DateModified = DateTime.Now;
            customer.RowVersion = customerRow.RowVersion;
            customer.ExternalId0 = customerRow.ExternalId0;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the customer.
                    await this.DataServiceClient.UpdateCustomerAsync(
                       customer.Address1,
                       customer.Address2,
                       customer.City,
                       customer.Company,
                       customer.CountryId,
                       customer.CustomerId,
                       customer.CustomerId,
                       customer.DateCreated,
                       customer.DateModified,
                       customer.DateOfBirth,
                       customer.Email,
                       customer.ExternalId0,
                       customer.FirstName,
                       customer.LastName,
                       customer.MiddleName,
                       customer.PhoneNumber,
                       customer.PostalCode,
                       customer.ProvinceId,
                       customer.RowVersion);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateCustomerOperation"))
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