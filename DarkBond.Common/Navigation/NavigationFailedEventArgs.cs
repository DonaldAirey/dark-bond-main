// <copyright file="NavigationFailedEventArgs.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Navigation
{
    using System;

    /// <summary>
    /// EventArgs used with the NavigationFailed event.
    /// </summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationFailedEventArgs"/> class.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public NavigationFailedEventArgs(NavigationContext navigationContext)
        {
            if (navigationContext == null)
            {
                throw new ArgumentNullException(nameof(navigationContext));
            }

            this.NavigationContext = navigationContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationFailedEventArgs"/> class.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="error">The error.</param>
        public NavigationFailedEventArgs(NavigationContext navigationContext, Exception error)
            : this(navigationContext)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        public NavigationContext NavigationContext { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the navigation URI
        /// </summary>
        /// <remarks>
        /// This is a convenience accessor around NavigationContext.Uri.
        /// </remarks>
        public Uri Uri
        {
            get
            {
                if (this.NavigationContext != null)
                {
                    return this.NavigationContext.Uri;
                }

                return null;
            }
        }
    }
}