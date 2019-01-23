// <copyright file="SubscriptionViewState.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager
{
    /// <summary>
    /// The various states of the SubscriptionView dialog.
    /// </summary>
    public enum SubscriptionViewState
    {
        /// <summary>
        /// The underwriter is fixed.
        /// </summary>
        Customer,

        /// <summary>
        /// The offering is fixed.
        /// </summary>
        Product,

        /// <summary>
        /// Both the offering and underwriter are fixed.
        /// </summary>
        License
    }
}
