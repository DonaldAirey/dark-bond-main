// <copyright file="IProductId.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Provides a product code from the object.
    /// </summary>
    public interface IProductId
    {
        /// <summary>
        /// Gets a unique identifier of the product.
        /// </summary>
        Guid ProductId { get; }
    }
}
