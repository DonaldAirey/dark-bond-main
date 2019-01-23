// <copyright file="EventAggregator.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Implements <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        /// <summary>
        /// A dictionary of events.
        /// </summary>
        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        /// <summary>
        /// The synchronization context.
        /// </summary>
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        /// <summary>
        /// Gets the single instance of the event managed by this EventAggregator.  Multiple calls to this method with the same
        /// <typeparamref name="TEventType"/> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get.  This must inherit from <see cref="EventBase"/> .</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/> .</returns>
        public TEventType GetEvent<TEventType>()
                    where TEventType : EventBase, new()
                {
            lock (this.events)
            {
                EventBase existingEvent = null;

                if (!this.events.TryGetValue(typeof(TEventType), out existingEvent))
                {
                    TEventType newEvent = new TEventType();
                    newEvent.SynchronizationContext = this.synchronizationContext;
                    this.events[typeof(TEventType)] = newEvent;
                    return newEvent;
                }
                else
                {
                    return (TEventType)existingEvent;
                }
            }
        }
    }
}