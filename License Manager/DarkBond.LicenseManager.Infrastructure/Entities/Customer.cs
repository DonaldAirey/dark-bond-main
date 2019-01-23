// <copyright file="Customer.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Entities
{
    using System;

    /// <summary>
    /// A Customer business entity.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second line of the address.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the country.
        /// </summary>
        public Guid CountryId { get; set; }

        /// <summary>
        /// Gets or sets the date the record was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public string ExternalId0 { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the postal (zip) code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the province.
        /// </summary>
        public Guid? ProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }
    }
}