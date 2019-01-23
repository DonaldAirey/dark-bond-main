// <copyright file="ColumnDragParameter.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;

    /// <summary>
    /// Used to describe a column drag operation.
    /// </summary>
    internal class ColumnDragParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDragParameter"/> class.
        /// </summary>
        /// <param name="delta">The change in position.</param>
        /// <param name="mousePosition">The horizontal mouse position from the left edge of the column header.</param>
        internal ColumnDragParameter(double delta, double mousePosition)
        {
            // Initialize the object.
            this.Delta = delta;
            this.MousePosition = mousePosition;
        }

        /// <summary>
        /// Gets the mouse movement change.
        /// </summary>
        internal double Delta
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        internal double MousePosition
        {
            get;
            private set;
        }
    }
}