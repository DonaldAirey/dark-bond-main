// <copyright file="Subscription.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Entities
{
    using System;

    /// <summary>
    /// A License business entity.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the unique identifier of the underwriter.
        /// </summary>
        public Guid UnderwriterId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public string ExternalId0 { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the subscription.
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the offerings.
        /// </summary>
        public Guid OfferingId { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public long RowVersion { get; set; }
    }
}