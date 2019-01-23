// <copyright file="OfferingRepository.cs" company="Dark Bond, Inc.">
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
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Mediates the between the view model and the data model for Offerings.
    /// </summary>
    public class OfferingRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public OfferingRepository(
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
        /// Deletes a offering from the data model.
        /// </summary>
        /// <param name="offeringId">The unique identifier of the offering.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteProductAsync(Guid offeringId)
        {
            // Find the offering row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            OfferingRow offeringRow = this.dataModel.OfferingKey.Find(offeringId);
            if (offeringRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteOfferingAsync(offeringId, offeringRow.RowVersion);
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
        /// Inserts a offering into the data model.
        /// </summary>
        /// <param name="offering">A Product business entity.</param>
        /// <returns>True indicates it was successful stored, false otherwise.</returns>
        public async Task<bool> InsertProductAsync(Offering offering)
        {
            // Validate the parameter
            if (offering == null)
            {
                throw new ArgumentNullException(nameof(offering));
            }

            // Initialize the record.
            offering.OfferingId = Guid.NewGuid();
            offering.DateCreated = DateTime.Now;
            offering.DateModified = offering.DateCreated;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the offering.
                    await this.DataServiceClient.CreateOfferingAsync(
                        offering.Age,
                        offering.Coupon,
                        offering.DateCreated,
                        offering.DateModified,
                        offering.Description,
                        offering.ExternalId0,
                        offering.FaceValue,
                        offering.FicoScore,
                        offering.Maturity,
                        offering.Name,
                        offering.OfferingId);

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
        /// Updates a offering in the data model.
        /// </summary>
        /// <param name="offering">A Product business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateProductAsync(Offering offering)
        {
            // Validate the parameter
            if (offering == null)
            {
                throw new ArgumentNullException(nameof(offering));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing to do here.
            OfferingRow targetOfferingRow = this.dataModel.OfferingKey.Find(offering.OfferingId);
            if (targetOfferingRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            offering.DateCreated = targetOfferingRow.DateCreated;
            offering.DateModified = targetOfferingRow.DateModified;
            offering.DateModified = DateTime.Now;
            offering.RowVersion = targetOfferingRow.RowVersion;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the offering.
                    await this.DataServiceClient.UpdateOfferingAsync(
                        offering.Age,
                        offering.Coupon,
                        offering.DateCreated,
                        offering.DateModified,
                        offering.Description,
                        offering.ExternalId0,
                        offering.FaceValue,
                        offering.FicoScore,
                        offering.Maturity,
                        offering.Name,
                        offering.OfferingId,
                        offering.OfferingId,
                        offering.RowVersion);

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