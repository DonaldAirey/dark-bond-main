// <copyright file="ITemplateSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Used by a template selector to connect a view model to the template used to display it.
    /// </summary>
    public interface ITemplateSelector
    {
        /// <summary>
        /// Gets the name of the data template used to present the data.
        /// </summary>
        string Key { get; }
    }
}
