// <copyright file="IDelegateReference.cs" company="DarkBond, Inc.">
//     Copyright © 2015 - DarkBond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.View.Events
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
        /// <value>
        /// The referenced <see cref="Delegate" /> object.
        /// </value>
        Delegate Target { get; }
    }
}