// <copyright file="DataEventArgs.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Events
{
    using System;

    /// <summary>
    /// Generic arguments class to pass to event handlers that need to receive data.
    /// </summary>
    /// <typeparam name="TData">The type of data to pass.</typeparam>
    public class DataEventArgs<TData> : EventArgs
    {
        /// <summary>
        /// The value of the event payload.
        /// </summary>
        private readonly TData value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataEventArgs{TData}"/> class.
        /// </summary>
        /// <param name="value">Information related to the event.</param>
        public DataEventArgs(TData value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the information related to the event.
        /// </summary>
        public TData Value
        {
            get
            {
                return this.value;
            }
        }
    }
}