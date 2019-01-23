// <copyright file="ILicenseMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using DarkBond.LicenseManager;
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Interface for mapping license records.
    /// </summary>
    public interface ILicenseMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="licenseViewModel">The property view model.</param>
        void Clear(ViewModels.Forms.LicenseViewModel licenseViewModel);

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="licenseRow">The data model row.</param>
        /// <param name="licenseViewModel">The property view model record.</param>
        void Map(LicenseRow licenseRow, ViewModels.Forms.LicenseViewModel licenseViewModel);

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="licenseRow">The data model row.</param>
        /// <param name="licenseViewModel">The list view model.</param>
        void Map(LicenseRow licenseRow, ViewModels.ListViews.LicenseViewModel licenseViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="licenseViewModel">The property view model.</param>
        /// <param name="license">A License business entity.</param>
        /// <returns>A data model row.</returns>
        License Map(ViewModels.Forms.LicenseViewModel licenseViewModel, License license);
    }
}