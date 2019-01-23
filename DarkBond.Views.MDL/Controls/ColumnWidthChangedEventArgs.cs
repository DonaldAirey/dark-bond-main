// <copyright file="ColumnWidthChangedEventArgs.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;

    /// <summary>
    /// Event data for a change to the width of a column.
    /// </summary>
    internal class ColumnWidthChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnWidthChangedEventArgs"/> class.
        /// </summary>
        /// <param name="width">The new width of the column.</param>
        public ColumnWidthChangedEventArgs(double width)
        {
            this.Width = width;
        }

        /// <summary>
        /// Gets or sets the new column width.
        /// </summary>
        public double Width
        {
            get;
            set;
        }
    }
}