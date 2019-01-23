// <copyright file="Product.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost
{
    using System;

    /// <summary>
    /// The product.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The productId.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The row version.
        /// </summary>
        public long RowVersion { get; set; }
    }
}