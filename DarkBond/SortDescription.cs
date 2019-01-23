﻿// <copyright file="SortDescription.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Describes a sorting operation.
    /// </summary>
    public class SortDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortDescription"/> class.
        /// </summary>
        public SortDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortDescription"/> class.
        /// </summary>
        /// <param name="propertyName">The property on which to sort.</param>
        /// <param name="direction">The sorting direction.</param>
        public SortDescription(string propertyName, SortDirection direction)
        {
            this.PropertyName = propertyName;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        public SortDirection Direction
        {
            get;
            set;
        }
    }
}