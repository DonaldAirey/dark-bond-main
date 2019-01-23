﻿// <copyright file="IEventAggregator.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines an interface to get instances of an event type.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets an instance of an event type.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get.</typeparam>
        /// <returns>An instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Reviewed")]
        TEventType GetEvent<TEventType>()
                    where TEventType : EventBase, new();
    }
}
