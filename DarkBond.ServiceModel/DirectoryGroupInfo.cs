// <copyright file="DirectoryGroupInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON structure describing a collection of groups.
    /// </summary>
    public class DirectoryGroupInfo
    {
        /// <summary>
        /// Gets or sets a collection of groups.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The set operation may be needed by JSON.  Try removing the set operation later.")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Using a list for JSON compatibility.")]
        [JsonProperty("value")]
        public List<DirectoryGroup> Groups { get; set; }
    }
}