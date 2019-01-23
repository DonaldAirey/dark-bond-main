// <copyright file="SubscriptionRepository.cs" company="Dark Bond, Inc.">
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
    /// Mediates the between the view model and the data model for Subscriptions.
    /// </summary>
    public class SubscriptionRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public SubscriptionRepository(
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
        /// Deletes a subscription from the data model.
        /// </summary>
        /// <param name="subscriptionId">The unique identifier of the subscription.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteLicenseAsync(Guid subscriptionId)
        {
            // Find the subscription row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            SubscriptionRow subscriptionRow = this.dataModel.SubscriptionKey.Find(subscriptionId);
            if (subscriptionRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteSubscriptionAsync(subscriptionRow.RowVersion, subscriptionId);
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateLicenseOperation"))
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
        /// Inserts a subscription into the data model.
        /// </summary>
        /// <param name="subscription">A License business entity.</param>
        /// <returns>True indicates it was successful stored, false otherwise.</returns>
        public async Task<bool> InsertLicenseAsync(Subscription subscription)
        {
            // Validate the parameter
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            // Initialize the record.
            subscription.DateModified = DateTime.Now;
            subscription.DateCreated = DateTime.Now;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the subscription.
                    await this.DataServiceClient.CreateSubscriptionAsync(
                        subscription.DateCreated,
                        subscription.DateModified,
                        subscription.ExternalId0,
                        subscription.FaceValue,
                        subscription.OfferingId,
                        subscription.SubscriptionId,
                        subscription.UnderwriterId);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateLicenseOperation"))
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
        /// Updates a subscription in the data model.
        /// </summary>
        /// <param name="subscription">A License business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateLicenseAsync(Subscription subscription)
        {
            // Validate the parameter
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing do here.
            SubscriptionRow targetSubscriptionRow = this.dataModel.SubscriptionKey.Find(subscription.SubscriptionId);
            if (targetSubscriptionRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            subscription.DateCreated = targetSubscriptionRow.DateCreated;
            subscription.DateModified = DateTime.Now;
            subscription.RowVersion = targetSubscriptionRow.RowVersion;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the subscription.
                    await this.DataServiceClient.UpdateSubscriptionAsync(
                        subscription.DateCreated,
                        subscription.DateModified,
                        subscription.ExternalId0,
                        subscription.FaceValue,
                        subscription.OfferingId,
                        subscription.RowVersion,
                        subscription.SubscriptionId,
                        subscription.SubscriptionId,
                        subscription.UnderwriterId);

                    // This indicates the operation was successful.
                    return true;
                }
                catch (CommunicationException communicationException)
                {
                    // If the communication exception can't be handled, then break out of the retry loop.
                    if (!this.CommunicationExceptionHandler.HandleException(communicationException, "UpdateLicenseOperation"))
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