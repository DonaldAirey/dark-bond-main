// <copyright file="EventSubscription.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides a way to retrieve a <see cref="Delegate"/> to execute an action depending
    /// on the value of a second filter predicate that returns true if the action should execute.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="System.Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class EventSubscription<TPayload> : IEventSubscription
    {
        /// <summary>
        /// The actions to be taken.
        /// </summary>
        private readonly IDelegateReference actionReference;

        /// <summary>
        /// The filters.
        /// </summary>
        private readonly IDelegateReference filterReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSubscription{TPayload}"/> class.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        public EventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            if (actionReference == null)
            {
                throw new ArgumentNullException(nameof(actionReference));
            }

            if (!(actionReference.Target is Action<TPayload>))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Invalid Delegate Reference {0}", typeof(Action<TPayload>).FullName),
                    "actionReference");
            }

            if (filterReference == null)
            {
                throw new ArgumentNullException(nameof(filterReference));
            }

            if (!(filterReference.Target is Predicate<TPayload>))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid Delegate Reference {0}", typeof(Predicate<TPayload>).FullName), "filterReference");
            }

            // Initialize the object.
            this.actionReference = actionReference;
            this.filterReference = filterReference;
        }

        /// <summary>
        /// Gets the target <see cref="System.Action{T}"/> that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        public Action<TPayload> Action
        {
            get
            {
                return (Action<TPayload>)this.actionReference.Target;
            }
        }

        /// <summary>
        /// Gets the target <see cref="Predicate{T}"/> that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        public Predicate<TPayload> Filter
        {
            get
            {
                return (Predicate<TPayload>)this.filterReference.Target;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Events.SubscriptionToken"/> that identifies this <see cref="IEventSubscription"/>.
        /// </summary>
        public SubscriptionToken SubscriptionToken { get; set; }

        /// <summary>
        /// Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>An <see cref="System.Action{T}"/> with the execution strategy, or <see langword="null" /> if the <see cref="IEventSubscription"/> is no longer valid.</returns>
        /// <remarks>
        /// If <see cref="Action"/> or <see cref="Filter"/> are no longer valid because they were
        /// garbage collected, this method will return <see langword="null" />.
        /// Otherwise it will return a delegate that evaluates the <see cref="Filter"/> and if it
        /// returns <see langword="true" /> will then call <see cref="InvokeAction"/>. The returned
        /// delegate holds hard references to the <see cref="Action"/> and <see cref="Filter"/> target
        /// <see cref="Delegate">delegates</see>. As long as the returned delegate is not garbage collected,
        /// the <see cref="Action"/> and <see cref="Filter"/> references delegates won't get collected either.
        /// </remarks>
        public virtual Action<object[]> GetExecutionStrategy()
        {
            Action<TPayload> action = this.Action;
            Predicate<TPayload> filter = this.Filter;
            if (action != null && filter != null)
            {
                return arguments =>
                {
                    TPayload argument = default(TPayload);
                    if (arguments != null && arguments.Length > 0 && arguments[0] != null)
                    {
                        argument = (TPayload)arguments[0];
                    }

                    if (filter(argument))
                    {
                        this.InvokeAction(action, argument);
                    }
                };
            }

            return null;
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> synchronously when not overridden.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="action"/> is null.</exception>
        public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            if (action == null)
            {
                throw new System.ArgumentNullException(nameof(action));
            }

            action(argument);
        }
    }
}