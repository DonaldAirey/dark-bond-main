﻿// <copyright file="SubscriptionToken.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Subscription token returned from <see cref="EventBase"/> on subscribe.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Should never have a need for a finalizer, hence no need for Dispose(bool)")]
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        /// <summary>
        /// THe token.
        /// </summary>
        private readonly Guid token;

        /// <summary>
        /// The unsubscribe action.
        /// </summary>
        private Action<SubscriptionToken> unsubscribeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionToken"/> class.
        /// </summary>
        /// <param name="unsubscribeAction">The action to take when unsubscribing.</param>
        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            // Initialize the object.
            this.unsubscribeAction = unsubscribeAction;
            this.token = Guid.NewGuid();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SubscriptionToken other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(this.token, other.token);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.object" /> is equal to the current <see cref="T:System.object" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.object" /> is equal to the current <see cref="T:System.object" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.object" /> to compare with the current <see cref="T:System.object" />. </param>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is null.</exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return this.Equals(obj as SubscriptionToken);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.token.GetHashCode();
        }

        /// <summary>
        /// Disposes the SubscriptionToken, removing the subscription from the corresponding <see cref="EventBase"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Should never have need for a finalizer, hence no need for Dispose(bool).")]
        public virtual void Dispose()
        {
            // While the SubsctiptionToken class implements IDisposable, in the case of weak subscriptions
            // (i.e. keepSubscriberReferenceAlive set to false in the Subscribe method) it's not necessary to unsubscribe,
            // as no resources should be kept alive by the event subscription.
            // In such cases, if a warning is issued, it could be suppressed.
            if (this.unsubscribeAction != null)
            {
                this.unsubscribeAction(this);
                this.unsubscribeAction = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}