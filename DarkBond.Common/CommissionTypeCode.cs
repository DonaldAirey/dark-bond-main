// <copyright file="CommissionTypeCode.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// The different ways that commission is assessed.
    /// </summary>
    public enum CommissionTypeCode
    {
        /// <summary>
        /// A hundredth of a percent (BIP) of the market value.
        /// </summary>
        BasisPoint,

        /// <summary>
        /// No commission.
        /// </summary>
        Empty,

        /// <summary>
        /// A flat fee.
        /// </summary>
        Fee,

        /// <summary>
        /// A percent of the market value.
        /// </summary>
        Percent,

        /// <summary>
        /// The commission is calculated based on a schedule.
        /// </summary>
        Schedule
    }
}
