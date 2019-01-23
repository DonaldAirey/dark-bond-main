// <copyright file="INavigate.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Navigation
{
    using System;

    /// <summary>
    /// Provides a method to navigate to a URI.
    /// </summary>
    public interface INavigate
    {
        /// <summary>
        /// Navigate to the given URI.
        /// </summary>
        /// <param name="target">The target URI.</param>
        void Navigate(Uri target);
    }
}
