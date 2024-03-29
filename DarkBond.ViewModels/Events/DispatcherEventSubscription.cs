﻿// <copyright file="DispatcherEventSubscription.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;
    using System.Threading;

    /// <summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate
    /// in a specific <see cref="SynchronizationContext"/>.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="System.Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class DispatcherEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        /// <summary>
        /// The synchronization context.
        /// </summary>
        private readonly SynchronizationContext synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherEventSubscription{TPayload}"/> class.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        /// <param name="synchronizationContext">The synchronization context to use for UI thread dispatching.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="System.Action{TPayload}"/>,
        /// or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        public DispatcherEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, SynchronizationContext synchronizationContext)
            : base(actionReference, filterReference)
        {
            // Initialize the object.
            this.synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> asynchronously in the specified <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            this.synchronizationContext.Post((o) => action((TPayload)o), argument);
        }
    }
}