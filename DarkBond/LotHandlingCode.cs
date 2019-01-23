// <copyright file="LotHandlingCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Describes the handling of the order.
    /// </summary>
    public enum LotHandlingCode
    {
        /// <summary>
        /// First In/First Out
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fifo", Justification = "Reviewed")]
        Fifo,

        /// <summary>
        /// Last In/First Out
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lifo", Justification = "Reviewed")]
        Lifo,

        /// <summary>
        /// Minimize the taxes
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mintax", Justification = "Reviewed")]
        Mintax
    }
}
