// <copyright file="CommissionUnitCode.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// The units of elements in a commission schedule.
    /// </summary>
    public enum CommissionUnitCode
    {
        /// <summary>
        /// No units.
        /// </summary>
        Empty,

        /// <summary>
        /// In terms of face value.
        /// </summary>
        Face,

        /// <summary>
        /// In terms of market value.
        /// </summary>
        MarketValue,

        /// <summary>
        /// In terms of shares.
        /// </summary>
        Shares
    }
}
