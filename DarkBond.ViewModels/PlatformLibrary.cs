// <copyright file="PlatformLibrary.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// The platform library.
    /// </summary>
    public enum PlatformLibrary
    {
        /// <summary>
        /// Windows .NET platform.
        /// </summary>
        Net,

        /// <summary>
        /// The platform has not been evaluated.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// Windows Silverlight platform.
        /// </summary>
        Silverlight,

        /// <summary>
        /// Unable to detect the current platform.
        /// </summary>
        Unknown,

        /// <summary>
        /// Windows Run Time platform.
        /// </summary>
        WinRT
    }
}