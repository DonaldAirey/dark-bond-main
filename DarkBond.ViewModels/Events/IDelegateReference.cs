// <copyright file="IDelegateReference.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;

    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/>.
    /// </summary>
    public interface IDelegateReference
    {
        /// <summary>
        /// Gets the referenced <see cref="Delegate" /> object.
        /// </summary>
        Delegate Target { get; }
    }
}