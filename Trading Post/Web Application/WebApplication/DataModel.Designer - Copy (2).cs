﻿// <auto-generated />
#pragma warning disable SA1402
#pragma warning disable SA1649
#pragma warning disable CS1591
namespace DarkBond.OrderManagementSystem
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using DarkBond.TradingPost;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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

    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(DataModel dataModel)
        {
            // Initialize the object.
            this.dataModel = dataModel;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        public IActionResult Get()
        {
            List<Product> products = new List<Product>();
            using (TransactionScope transactionScope = new TransactionScope())
            {
                foreach (ProductRow productRow in this.dataModel.Product)
                {
                    Product product = new Product();
                    product.Description = productRow.Description;
                    product.Name = productRow.Name;
                    product.ProductId = productRow.ProductId;
                    products.Add(product);
                }

                transactionScope.Complete();
            }

            return Ok(products);
        }

        [Authorize]
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Get(Guid productId)
        {
            Product product = null;
            using (TransactionScope transactionScope = new TransactionScope())
            {
                ProductRow productRow = this.dataModel.ProductKey.Find(productId);
                if (productRow == null)
                {
                    return NotFound();
                }

                product = new Product();
                product.Description = productRow.Description;
                product.Name = productRow.Name;
                product.ProductId = productRow.ProductId;
                product.RowVersion = productRow.RowVersion;
                transactionScope.Complete();
            }

            return Ok(product);
        }

        [Authorize]
        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 409)]
        public IActionResult Put(Guid productId, [FromBody]Product product)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    this.dataModel.CreateProduct(DateTime.Now, DateTime.Now, product.Description, null, product.Name, productId);
                    transactionScope.Complete();
                }
            }
            catch
            {
                return StatusCode(409);
            }

            return Ok();
        }

        [Authorize(Policy = "CanDelete")]
        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 409)]
        public IActionResult Delete(Guid productId, long rowVersion)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    this.dataModel.DeleteProduct(productId, rowVersion);
                }
            }
            catch
            {
                return StatusCode(409);
            }

            return Ok();
        }
    }
}
#pragma warning restore CS1591
#pragma warning restore SA1402
#pragma warning restore SA1649
