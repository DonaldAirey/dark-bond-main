﻿// <copyright file="IMailMerge.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Creates a merged Word document from a template and a dictionary of merge fields.
    /// </summary>
    public interface IMailMerge
    {
        /// <summary>
        /// Creates a document using the parameters provided.
        /// </summary>
        /// <param name="sourceDocument">The template for the document.</param>
        /// <param name="dictionary">The mail merge parameters.</param>
        /// <returns>The template document with the parameters substituted for the fields.</returns>
        MemoryStream CreateDocument(byte[] sourceDocument, Dictionary<string, object> dictionary);
    }
}
