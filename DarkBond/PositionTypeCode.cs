// <copyright file="PositionTypeCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Indicates whether a position is owned or borrowed.
    /// </summary>
    public enum PositionTypeCode
    {
        /// <summary>
        /// The position is owned.
        /// </summary>
        Long,

        /// <summary>
        /// The position is borrowed.
        /// </summary>
        Short
    }
}
