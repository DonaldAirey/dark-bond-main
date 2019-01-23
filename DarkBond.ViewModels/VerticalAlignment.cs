// <copyright file="VerticalAlignment.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// Describes how a child element is vertically positioned or stretched within a parent's layout slot.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// The element is aligned to the top of the parent's layout slot.
        /// </summary>
        Top = 0,

        /// <summary>
        /// The element is aligned to the center of the parent's layout slot.
        /// </summary>
        Center,

        /// <summary>
        /// The element is aligned to the bottom of the parent's layout slot.
        /// </summary>
        Bottom,

        /// <summary>
        /// The element is stretched to fill the entire layout slot of the parent element.
        /// </summary>
        Stretch = 3
    }
}