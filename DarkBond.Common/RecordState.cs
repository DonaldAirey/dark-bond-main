// <copyright file="RecordState.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// The various states of a record during a transaction.
    /// </summary>
    public static class RecordState
    {
        /// <summary>
        /// The record has been added.
        /// </summary>
        public const int Added = 0;

        /// <summary>
        /// The record has been deleted.
        /// </summary>
        public const int Deleted = 1;

        /// <summary>
        /// The record has been modified.
        /// </summary>
        public const int Modified = 2;
    }
}
