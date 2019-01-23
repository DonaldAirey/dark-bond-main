// <copyright file="Verbosity.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Describes how much information is emitted during the processing of the scripts.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// No progress messages.
        /// </summary>
        Quiet,

        /// <summary>
        /// Minimal progress messages.
        /// </summary>
        Minimal,

        /// <summary>
        /// Descriptive progress message.
        /// </summary>
        Verbose
    }
}
