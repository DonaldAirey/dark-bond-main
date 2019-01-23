// <copyright file="LicenseViewState.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    /// <summary>
    /// The various states of the LicenseView dialog.
    /// </summary>
    public enum LicenseViewState
    {
        /// <summary>
        /// The customer is fixed.
        /// </summary>
        Customer,

        /// <summary>
        /// The product is fixed.
        /// </summary>
        Product,

        /// <summary>
        /// Both the product and customer are fixed.
        /// </summary>
        License
    }
}
