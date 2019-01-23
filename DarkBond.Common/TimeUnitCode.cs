// <copyright file="TimeUnitCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Units of time.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Reviewed")]
    public enum TimeUnitCode
    {
        /// <summary>
        /// Days
        /// </summary>
        Days,

        /// <summary>
        /// Hours
        /// </summary>
        Hours,

        /// <summary>
        /// Minutes
        /// </summary>
        Minutes,

        /// <summary>
        /// Months
        /// </summary>
        Months,

        /// <summary>
        /// Seconds
        /// </summary>
        Seconds,

        /// <summary>
        /// Weeks
        /// </summary>
        Weeks,

        /// <summary>
        /// Years
        /// </summary>
        Years
    }
}
