// <copyright file="HolidayTypeCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Holiday types for trading and settlement.
    /// </summary>
    public enum HolidayTypeCode
    {
        /// <summary>
        /// No trading or settlements on this day.
        /// </summary>
        Both,

        /// <summary>
        /// No trading on this day.
        /// </summary>
        Trading,

        /// <summary>
        /// No settlements on this day.
        /// </summary>
        Settlement
    }
}
