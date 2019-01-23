// <copyright file="DeferRefresh.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;

    /// <summary>
    /// Used to defers the updating of an observable collection and restore live updates when disposed.
    /// </summary>
    public class DeferRefresh : IDisposable
    {
        /// <summary>
        /// The collection that will have it's notifications disabled while this object is alive.
        /// </summary>
        private IDeferrable deferrable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferRefresh"/> class.
        /// </summary>
        /// <param name="deferRefresh">The collection that will have it's notifications disabled while this object is alive.</param>
        public DeferRefresh(IDeferrable deferRefresh)
        {
            // Validate the deferRefresh argument
            if (deferRefresh == null)
            {
                throw new ArgumentNullException(nameof(deferRefresh));
            }

            // Initialize the object and disable the collection changed notifications.
            this.deferrable = deferRefresh;
            this.deferrable.IsRefreshDisabled = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DeferRefresh"/> class.
        /// </summary>
        ~DeferRefresh()
        {
            // Call the virtual method to dispose of the resources.  This (recommended) design pattern gives any derived classes a chance to clean up
            // unmanaged resources even though this base class has none.
            this.Dispose(false);
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
            if (disposing)
            {
                // This will allow the collection to send out notifications as it is changed and sends out a message that the entire collection has
                // changed.
                this.deferrable.IsRefreshDisabled = false;
                this.deferrable.ResetCollection();
            }
        }
    }
}
