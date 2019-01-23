// <copyright file="IDirectory.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// Interface for directories.
    /// </summary>
    public interface IDirectory
    {
        /// <summary>
        /// Gets the items in the directory.
        /// </summary>
        FilteredCollection<ListItemViewModel> Items
        {
            get;
        }

        /// <summary>
        /// Loads the resources of the directory.
        /// </summary>
        /// <param name="path">The path to the directory to be loaded.</param>
        void Load(string path);

        /// <summary>
        /// Unloads the resources of the directory.
        /// </summary>
        void Unload();
    }
}