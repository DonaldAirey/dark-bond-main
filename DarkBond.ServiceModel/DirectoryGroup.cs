// <copyright file="DirectoryGroup.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON structure describing a group in the AAD directory.
    /// </summary>
    public class DirectoryGroup
    {
        /// <summary>
        /// Gets or sets the JSON object type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Name chosen to be consistent with server structure.")]
        [JsonProperty("odata.type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a Guid that is the unique identifier for the object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets a string that identifies the directory object type.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets an optional description for the group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name for the group.
        /// </summary>
        public string DisplayName { get; set; }
    }
}