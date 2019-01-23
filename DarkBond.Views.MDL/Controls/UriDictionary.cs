// <copyright file="UriDictionary.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A dictionary of URIs organized by category.
    /// </summary>
    public class UriDictionary : Dictionary<string, UriCategory>
    {
        /// <summary>
        /// Gets the URI associated with the category, key keys.
        /// </summary>
        /// <param name="category">The category of the URIs.</param>
        /// <param name="key">The key for the URI.</param>
        /// <returns>The URI entered into the dictionary with the category, key index.</returns>
        internal Uri GetUri(string category, string key)
        {
            // This URI is returned if there's no matching entry for the keys.
            Uri uri = default(Uri);

            // Use the two dictionary levels to find the URI.
            UriCategory resourceCategory;
            if (this.TryGetValue(category, out resourceCategory))
            {
                UriSource resourceSource;
                if (resourceCategory.TryGetValue(key, out resourceSource))
                {
                    uri = resourceSource.Uri;
                }
            }

            // The URI belonging to the compound key.
            return uri;
        }
    }
}