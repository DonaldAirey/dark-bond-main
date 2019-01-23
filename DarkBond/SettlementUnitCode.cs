// <copyright file="SettlementUnitCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Units for setting an order.
    /// </summary>
    public enum SettlementUnitCode
    {
        /// <summary>
        /// Settlement is specified in hundredth of a percentage point (BIP).
        /// </summary>
        BasisPoint,

        /// <summary>
        /// Not defined.
        /// </summary>
        Empty,

        /// <summary>
        /// Settlement is in terms of market value.
        /// </summary>
        MarketValue,

        /// <summary>
        /// Settlement is specified in terms of percent.
        /// </summary>
        Percent
    }
}
