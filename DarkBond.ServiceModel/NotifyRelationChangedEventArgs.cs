// <copyright file="NotifyRelationChangedEventArgs.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System;

    /// <summary>
    /// Provides data for the relation changed event.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    public class NotifyRelationChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRelationChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This must be set to Reset.</param>
        public NotifyRelationChangedEventArgs(NotifyRelationChangedAction action)
        {
            // Reset is the only action allowed here.
            if (action != NotifyRelationChangedAction.Reset)
            {
                throw new ArgumentOutOfRangeException("action");
            }

            // Initialize the object.
            this.Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRelationChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to Add, or Remove.</param>
        /// <param name="key">The key that is affected by the change.</param>
        public NotifyRelationChangedEventArgs(NotifyRelationChangedAction action, T key)
        {
            // Create an 'Add' or 'Remove' action.
            switch (this.Action = action)
            {
                case NotifyRelationChangedAction.Add:

                    this.NewKey = key;
                    break;

                case NotifyRelationChangedAction.Remove:

                    this.OldKey = key;
                    break;

                default:

                    throw new ArgumentOutOfRangeException("action");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRelationChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to Add, or Remove.</param>
        /// <param name="newKey">The current key that is affected by the change.</param>
        /// <param name="oldKey">The previous key that is affected by the change.</param>
        public NotifyRelationChangedEventArgs(NotifyRelationChangedAction action, T newKey, T oldKey)
        {
            // Create a 'Change' action.
            switch (this.Action = action)
            {
                case NotifyRelationChangedAction.Change:

                    this.NewKey = newKey;
                    this.OldKey = oldKey;
                    break;

                default:

                    throw new ArgumentOutOfRangeException("action");
            }
        }

        /// <summary>
        /// Gets the action associated with the event.
        /// </summary>
        public NotifyRelationChangedAction Action { get; private set; }

        /// <summary>
        /// Gets the key that has had children added to it.
        /// </summary>
        public T NewKey { get; private set; }

        /// <summary>
        /// Gets the key that has had children removed from it.
        /// </summary>
        public T OldKey { get; private set; }
    }
}