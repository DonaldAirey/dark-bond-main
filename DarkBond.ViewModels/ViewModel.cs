// <copyright file="ViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a common base class for all view models.
    /// </summary>
    public abstract class ViewModel : NotificationObject, IDisposable
    {
        /// <summary>
        /// A collection of handlers for property changed actions.
        /// </summary>
        private readonly Dictionary<string, Action> propertyChangedActions = new Dictionary<string, Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        protected ViewModel()
        {
            // A quick-and-dirty switch for delegating property change events.
            this.PropertyChanged += (s, e) =>
            {
                Action action;
                if (this.propertyChangedActions.TryGetValue(e.PropertyName, out action))
                {
                    action();
                }
            };
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ViewModel"/> class.
        /// </summary>
        ~ViewModel()
        {
            // Call the virtual method to dispose of the resources.  This (recommended) design pattern gives any derived classes a chance to clean up
            // unmanaged resources even though this base class has none.
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a dictionary of actions that are executed when a property changes.
        /// </summary>
        public Dictionary<string, Action> PropertyChangedActions
        {
            get
            {
                return this.propertyChangedActions;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the virtual method to allow derived classes to clean up resources.
            this.Dispose(true);

            // Since we took care of cleaning up the resources, there is no need to call the finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}