// <copyright file="UnderwriterRepository.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Repositories
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using DarkBond.ServiceModel;

    /// <summary>
    /// Mediates the between the view model and the data model for Underwriters.
    /// </summary>
    public class UnderwriterRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public UnderwriterRepository(
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
        /// Inserts a underwriter.
        /// </summary>
        /// <param name="underwriter">The underwriter record.</param>
        /// <returns>true indicates success, false otherwise.</returns>
        public async Task<bool> InsertCustomerAsync(Entities.Underwriter underwriter)
        {
            // Validate the parameter
            if (underwriter == null)
            {
                throw new ArgumentNullException(nameof(underwriter));
            }

            // Initialize the record.
            underwriter.UnderwriterId = Guid.NewGuid();
            underwriter.DateModified = DateTime.Now;
            underwriter.DateCreated = DateTime.Now;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the underwriter.
                    await this.DataServiceClient.CreateUnderwriterAsync(
                        underwriter.Address1,
                        underwriter.Address2,
                        underwriter.City,
                        underwriter.CountryId,
                        underwriter.DateCreated,
                        underwriter.DateModified,
                        underwriter.DateOfBirth,
                        underwriter.Email,
                        underwriter.ExternalId0,
                        underwriter.Name,
                        underwriter.PhoneNumber,
                        underwriter.PostalCode,
                        underwriter.PrimaryContact,
                        underwriter.ProvinceId,
                        underwriter.UnderwriterId);

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
        /// Deletes a underwriter from the data model.
        /// </summary>
        /// <param name="underwriterId">The unique identifier of the underwriter.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteCustomerAsync(Guid underwriterId)
        {
            // Find the underwriter row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            UnderwriterRow underwriterRow = this.dataModel.UnderwriterKey.Find(underwriterId);
            if (underwriterRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteUnderwriterAsync(underwriterRow.RowVersion, underwriterId);
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
        /// Updates a underwriter in the data model.
        /// </summary>
        /// <param name="underwriter">A underwriter business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateCustomerAsync(Entities.Underwriter underwriter)
        {
            // Validate the parameter
            if (underwriter == null)
            {
                throw new ArgumentNullException(nameof(underwriter));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing to do here.
            UnderwriterRow underwriterRow = this.dataModel.UnderwriterKey.Find(underwriter.UnderwriterId);
            if (underwriterRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            underwriter.DateCreated = underwriterRow.DateCreated;
            underwriter.DateModified = DateTime.Now;
            underwriter.RowVersion = underwriterRow.RowVersion;
            underwriter.ExternalId0 = underwriterRow.ExternalId0;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the underwriter.
                    await this.DataServiceClient.UpdateUnderwriterAsync(
                        underwriter.Address1,
                        underwriter.Address2,
                        underwriter.City,
                        underwriter.CountryId,
                        underwriter.DateCreated,
                        underwriter.DateModified,
                        underwriter.DateOfBirth,
                        underwriter.Email,
                        underwriter.ExternalId0,
                        underwriter.Name,
                        underwriter.PhoneNumber,
                        underwriter.PostalCode,
                        underwriter.PrimaryContact,
                        underwriter.ProvinceId,
                        underwriter.RowVersion,
                        underwriter.UnderwriterId,
                        underwriter.UnderwriterId);

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