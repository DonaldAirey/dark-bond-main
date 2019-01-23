// <copyright file="LicenseMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using System;
    using System.Composition;
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Used to map license records.
    /// </summary>
    [Export(typeof(ILicenseMapper))]
    public class LicenseMapper : ILicenseMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="licenseViewModel">The property view model.</param>
        public void Clear(ViewModels.Forms.LicenseViewModel licenseViewModel)
        {
            // Validate the parameter.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Clear the view model.
            licenseViewModel.CustomerId = null;
            licenseViewModel.DeveloperLicenseTypeCode = null;
            licenseViewModel.LicenseId = default(Guid);
            licenseViewModel.ProductId = null;
            licenseViewModel.RuntimeLicenseTypeCode = null;
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="licenseRow">The data model row.</param>
        /// <param name="licenseViewModel">The property view model record.</param>
        public void Map(LicenseRow licenseRow, ViewModels.Forms.LicenseViewModel licenseViewModel)
        {
            // Validate the parameter.
            if (licenseRow == null)
            {
                throw new ArgumentNullException(nameof(licenseRow));
            }

            // Validate the parameter.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Copy the data model into the view model.
            licenseViewModel.CustomerId = licenseRow.CustomerId;
            licenseViewModel.DeveloperLicenseTypeCode = (LicenseTypeCode)licenseRow.DeveloperLicenseTypeCode;
            licenseViewModel.LicenseId = licenseRow.LicenseId;
            licenseViewModel.ProductId = licenseRow.ProductId;
            licenseViewModel.RuntimeLicenseTypeCode = (LicenseTypeCode)licenseRow.RuntimeLicenseTypeCode;
        }

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="licenseRow">The data model row.</param>
        /// <param name="licenseViewModel">The list view model.</param>
        public void Map(LicenseRow licenseRow, ViewModels.ListViews.LicenseViewModel licenseViewModel)
        {
            // Validate the parameter.
            if (licenseRow == null)
            {
                throw new ArgumentNullException(nameof(licenseRow));
            }

            // Validate the parameter.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Copy the data model into the view model.
            licenseViewModel.CustomerId = licenseRow.CustomerId;
            licenseViewModel.DateCreated = licenseRow.DateCreated;
            licenseViewModel.DateModified = licenseRow.DateModified;
            licenseViewModel.DeveloperLicenseTypeCode = licenseRow.DeveloperLicenseTypeCode;
            licenseViewModel.LicenseId = licenseRow.LicenseId;
            licenseViewModel.ProductId = licenseRow.ProductId;
            licenseViewModel.RuntimeLicenseTypeCode = licenseRow.RuntimeLicenseTypeCode;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="licenseViewModel">The property view model.</param>
        /// <param name="license">A License business entity.</param>
        /// <returns>A data model row.</returns>
        public License Map(ViewModels.Forms.LicenseViewModel licenseViewModel, License license)
        {
            // Validate the parameter.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Validate the parameter.
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            license.CustomerId = licenseViewModel.CustomerId.Value;
            license.DeveloperLicenseTypeCode = licenseViewModel.DeveloperLicenseTypeCode.Value;
            license.LicenseId = licenseViewModel.LicenseId.Value;
            license.ProductId = licenseViewModel.ProductId.Value;
            license.RuntimeLicenseTypeCode = licenseViewModel.RuntimeLicenseTypeCode.Value;
            return license;
        }
    }
}
