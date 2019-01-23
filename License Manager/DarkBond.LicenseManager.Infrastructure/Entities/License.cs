// <copyright file="License.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Entities
{
    using System;

    /// <summary>
    /// A License business entity.
    /// </summary>
    public class License
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the unique customer identifier.
        /// </summary>
        public LicenseTypeCode DeveloperLicenseTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public string ExternalId0 { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the license.
        /// </summary>
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the products.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique customer identifier.
        /// </summary>
        public LicenseTypeCode RuntimeLicenseTypeCode { get; set; }
    }
}