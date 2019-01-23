// <copyright file="PersistentStoreOptions.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The options for the persistent store.
    /// </summary>
    public class PersistentStoreOptions
    {
        /// <summary>
        /// Gets or sets the SQL connection string.
        /// </summary>
        public string SqlConnectionString { get; set; }
    }
}