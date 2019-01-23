// <copyright file="BackgroundEventSubscription.cs" company="DarkBond, Inc.">
//     Copyright © 2015 - DarkBond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.View.Events
{
    using System;
    using Windows.System.Threading;

    /// <summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate in a background thread.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="System.Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class BackgroundEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundEventSubscription{TPayload}"/> class.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        public BackgroundEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
            : base(actionReference, filterReference)
        {
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> in an asynchronous thread by using a <see cref="ThreadPool"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public async override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            await ThreadPool.RunAsync((o) => action(argument));
        }
    }
}