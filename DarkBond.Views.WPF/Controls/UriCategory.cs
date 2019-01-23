// <copyright file="UriCategory.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// A category of URIs.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is an element of a larger dictionary.")]
    [Serializable]
    public class UriCategory : Dictionary<string, UriSource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriCategory"/> class.
        /// </summary>
        public UriCategory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriCategory"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context for streaming.</param>
        protected UriCategory(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}