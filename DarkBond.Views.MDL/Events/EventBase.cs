// <copyright file="EventBase.cs" company="DarkBond, Inc.">
//     Copyright © 2015 - DarkBond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.View.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Defines a base class to publish and subscribe to events.
    /// </summary>
    public abstract class EventBase
    {
        /// <summary>
        /// A collection of subscriptions.
        /// </summary>
        private readonly List<IEventSubscription> subscriptionsField = new List<IEventSubscription>();

        /// <summary>
        /// Gets or sets the SynchronizationContext to be set by the EventAggregator for UI Thread Dispatching.
        /// </summary>
        /// <value>
        /// The SynchronizationContext to be set by the EventAggregator for UI Thread Dispatching.
        /// </value>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Gets the list of current subscriptions.
        /// </summary>
        /// <value>The current subscribers.</value>
        protected ICollection<IEventSubscription> Subscriptions
        {
            get
            {
                return this.subscriptionsField;
            }
        }

        /// <summary>
        /// Removes the subscriber matching the <seealso cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (this.Subscriptions)
            {
                IEventSubscription eventSubscription = this.Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                if (eventSubscription != null)
                {
                    this.Subscriptions.Remove(eventSubscription);
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscriber matching <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is a <see cref="SubscriptionToken"/> that matches; otherwise <see langword="false"/>.</returns>
        public virtual bool Contains(SubscriptionToken token)
        {
            lock (this.Subscriptions)
            {
                IEventSubscription eventSubscription = this.Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                return eventSubscription != null;
            }
        }

        /// <summary>
        /// Adds the specified <see cref="IEventSubscription"/> to the subscribers' collection.
        /// </summary>
        /// <param name="eventSubscription">The subscriber.</param>
        /// <returns>The <see cref="SubscriptionToken"/> that uniquely identifies every subscriber.</returns>
        /// <remarks>
        /// Adds the subscription to the internal list and assigns it a new <see cref="SubscriptionToken"/>.
        /// </remarks>
        protected virtual SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
        {
            // Validate the 'eventSubscription' argument.
            if (eventSubscription == null)
            {
                throw new ArgumentNullException("eventSubscription");
            }

            eventSubscription.SubscriptionToken = new SubscriptionToken(this.Unsubscribe);

            lock (this.Subscriptions)
            {
                this.Subscriptions.Add(eventSubscription);
            }

            return eventSubscription.SubscriptionToken;
        }

        /// <summary>
        /// Calls all the execution strategies exposed by the list of <see cref="IEventSubscription"/>.
        /// </summary>
        /// <param name="arguments">The arguments that will be passed to the listeners.</param>
        /// <remarks>Before executing the strategies, this class will prune all the subscribers from the
        /// list that return a <see langword="null" /> <see cref="Action{T}"/> when calling the
        /// <see cref="IEventSubscription.GetExecutionStrategy"/> method.</remarks>
        protected virtual void InternalPublish(params object[] arguments)
        {
            List<Action<object[]>> executionStrategies = this.PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        /// <summary>
        /// Prunes and returns strategies.
        /// </summary>
        /// <returns>The pruned and returned strategies.</returns>
        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            List<Action<object[]>> returnList = new List<Action<object[]>>();

            lock (this.Subscriptions)
            {
                for (var i = this.Subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> listItem = this.subscriptionsField[i].GetExecutionStrategy();

                    if (listItem == null)
                    {
                        this.subscriptionsField.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }
    }
}