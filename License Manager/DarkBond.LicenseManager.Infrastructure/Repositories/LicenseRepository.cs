// <copyright file="LicenseRepository.cs" company="Dark Bond, Inc.">
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
    /// Mediates the between the view model and the data model for Licenses.
    /// </summary>
    public class LicenseRepository : BaseRepository
    {
        /// <summary>
        /// The Data Model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        /// <param name="dataModel">The data model.</param>
        public LicenseRepository(
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
        /// Deletes a license from the data model.
        /// </summary>
        /// <param name="licenseId">The unique identifier of the license.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> DeleteLicense(Guid licenseId)
        {
            // Find the license row.  Note that it may have been deleted while we were working on it.  If so, "Mission Accomplished".
            LicenseRow licenseRow = this.dataModel.LicenseKey.Find(licenseId);
            if (licenseRow == null)
            {
                return false;
            }

            // This will keep on trying the operation until it is successful or is the error is handled.
            while (true)
            {
                try
                {
                    await this.DataServiceClient.DeleteLicenseAsync(licenseId, licenseRow.RowVersion);
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
        /// Inserts a license into the data model.
        /// </summary>
        /// <param name="license">A License business entity.</param>
        /// <returns>True indicates it was successful stored, false otherwise.</returns>
        public async Task<bool> InsertLicense(License license)
        {
            // Validate the parameter
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            // Initialize the record.
            license.DateModified = DateTime.Now;
            license.DateCreated = DateTime.Now;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the license.
                    await this.DataServiceClient.CreateLicenseAsync(
                       license.CustomerId,
                       license.DateCreated,
                       license.DateModified,
                       license.DeveloperLicenseTypeCode,
                       license.ExternalId0,
                       license.LicenseId,
                       license.ProductId,
                       license.RuntimeLicenseTypeCode);

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
        /// Updates a license in the data model.
        /// </summary>
        /// <param name="license">A License business entity.</param>
        /// <returns>True indicates it was successfully stored, false otherwise.</returns>
        public async Task<bool> UpdateLicense(License license)
        {
            // Validate the parameter
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            // Attempt to find the existing row.  Note that it's possible the record may have been deleted while we were working on it.  If it was,
            // then there's nothing do here.
            LicenseRow targetLicenseRow = this.dataModel.LicenseKey.Find(license.LicenseId);
            if (targetLicenseRow == null)
            {
                return false;
            }

            // This will populate the record with the values that are not part of the view model.
            license.DateCreated = targetLicenseRow.DateCreated;
            license.DateModified = DateTime.Now;
            license.RowVersion = targetLicenseRow.RowVersion;

            // The repository will keep on trying the operation until it succeeds or the error is handled.
            while (true)
            {
                try
                {
                    // Call the service to update the license.
                    await this.DataServiceClient.UpdateLicenseAsync(
                       license.CustomerId,
                       license.DateCreated,
                       license.DateModified,
                       license.DeveloperLicenseTypeCode,
                       license.ExternalId0,
                       license.LicenseId,
                       license.LicenseId,
                       license.ProductId,
                       license.RowVersion,
                       license.RuntimeLicenseTypeCode);

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