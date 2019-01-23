// <copyright file="ColumnDragEventArgs.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;

    /// <summary>
    /// Used to communicate the change in a column's position during a drag operation.
    /// </summary>
    internal class ColumnDragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDragEventArgs"/> class.
        /// </summary>
        /// <param name="delta">The change in the column's position.</param>
        /// <param name="mousePosition">The horizontal position of the mouse with respect to the column's left edge.</param>
        public ColumnDragEventArgs(double delta, double mousePosition)
        {
            this.Delta = delta;
            this.MousePosition = mousePosition;
        }

        /// <summary>
        /// Gets the change in the column's position.
        /// </summary>
        public double Delta
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the location of the mouse with respect to the column.
        /// </summary>
        public double MousePosition
        {
            get;
            private set;
        }
    }
}