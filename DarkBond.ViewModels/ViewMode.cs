// <copyright file="ViewMode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// The various flavors of viewing the content.
    /// </summary>
    internal enum ViewMode
    {
        /// <summary>
        /// Views the metadata associated with the objects.
        /// </summary>
        Content,

        /// <summary>
        /// Views the details (size, dates, type, etc.) of the objects.
        /// </summary>
        Details,

        /// <summary>
        /// Views the extra large icon representation of the objects.
        /// </summary>
        ExtraLargeIcons,

        /// <summary>
        /// Views the large icon representation of the objects.
        /// </summary>
        LargeIcons,

        /// <summary>
        /// Views a simple list of the objects.
        /// </summary>
        List,

        /// <summary>
        /// Views the medium icon representation of the objects.
        /// </summary>
        MediumIcons,

        /// <summary>
        /// Views the small icon representation of the objects.
        /// </summary>
        SmallIcons,

        /// <summary>
        /// Views the thumbnail of the object.
        /// </summary>
        Tiles
    }
}
