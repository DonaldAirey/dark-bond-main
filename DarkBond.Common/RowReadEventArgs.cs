// <copyright file="RowReadEventArgs.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Event arguments for the reading a row from a data source.
    /// </summary>
    public class RowReadEventArgs : EventArgs
    {
        /// <summary>
        /// The raw row data.
        /// </summary>
        private object[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="RowReadEventArgs"/> class.
        /// </summary>
        /// <param name="data">The raw row data.</param>
        public RowReadEventArgs(object[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the raw row data.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The data at the given index.</returns>
        public object GetData(int index)
        {
            return this.data[index];
        }
    }
}