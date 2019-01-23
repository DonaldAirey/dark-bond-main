// <copyright file="ArgumentState.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    /// <summary>
    /// These are the parsing states used to read the arguments on the command line.
    /// </summary>
    internal enum ArgumentState
    {
        /// <summary>
        /// The force-login parameter token.
        /// </summary>
        ForceLoginParam,

        /// <summary>
        /// The input file name parameter token.
        /// </summary>
        InputFileParam,

        /// <summary>
        /// The input file name parameter.
        /// </summary>
        InputFileName,

        /// <summary>
        /// The verbosity parameter.
        /// </summary>
        Verbosity,

        /// <summary>
        /// The verbosity parameter token.
        /// </summary>
        VerbosityParam
    }
}