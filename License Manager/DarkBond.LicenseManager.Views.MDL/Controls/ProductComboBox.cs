// <copyright file="ProductComboBox.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views.Controls
{
    using System;
    using DarkBond.Views.Controls;

    /// <summary>
    /// ComboBox used to select a customer.
    /// </summary>
    public class ProductComboBox : ComboBox<Guid?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductComboBox"/> class.
        /// </summary>
        public ProductComboBox()
        {
            // This ComboBox is designed to be paired to a Product view model.
            this.DisplayMemberPath = "Name";
            this.SelectedValuePath = "ProductId";
        }
    }
}
