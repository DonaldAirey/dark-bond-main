// <copyright file="HttpHeaderHelper.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// Helps handle HTTP headers.
    /// </summary>
    public class HttpHeaderHelper
    {
        /// <summary>
        /// Extracts the ETag from the header.
        /// </summary>
        /// <param name="eTag">The tag.</param>
        /// <returns>The tag extracted from the header.</returns>
        public static long ConvertETagToLong(string eTag)
        {
            // The ETag is enclosed in quotes.  This strips the quotes and converts it to a long.
            long value = 0;
            string[] parts = eTag.Split('"');
            if (parts.Length == 3)
            {
                value = long.Parse(parts[1]);
            }

            return value;
        }
    }
}