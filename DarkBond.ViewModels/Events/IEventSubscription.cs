// <copyright file="IEventSubscription.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines a contract for an event subscription to be used by <see cref="EventBase"/>.
    /// </summary>
    public interface IEventSubscription
    {
        /// <summary>
        /// Gets or sets a <see cref="Events.SubscriptionToken"/> that identifies this <see cref="IEventSubscription"/>.
        /// </summary>
        SubscriptionToken SubscriptionToken { get; set; }

        /// <summary>
        /// Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>An <see cref="Action{T}"/> with the execution strategy, or <see langword="null" /> if the <see cref="IEventSubscription"/> is no longer valid.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not enough time to fix")]
        Action<object[]> GetExecutionStrategy();
    }
}