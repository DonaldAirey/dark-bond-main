// <copyright file="Offering.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Entities
{
    using System;

    /// <summary>
    /// A Product business entity.
    /// </summary>
    public class Offering
    {
        /// <summary>
        /// Gets or sets the description of the offering.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the description of the offering.
        /// </summary>
        public decimal Coupon { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the description of the offering.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public string ExternalId0 { get; set; }

        /// <summary>
        /// Gets or sets the name of the offering.
        /// </summary>
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the offering.
        /// </summary>
        public decimal FicoScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the offering.
        /// </summary>
        public DateTime Maturity { get; set; }

        /// <summary>
        /// Gets or sets the name of the offering.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the offering.
        /// </summary>
        public Guid OfferingId { get; set; }
    }
}