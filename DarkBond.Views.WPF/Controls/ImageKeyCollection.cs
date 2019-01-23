// <copyright file="ImageKeyCollection.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A collection of images and their keys.
    /// </summary>
    /// <remarks>This class exists primarily for the design surface that doesn't seem to process generic types very well.</remarks>
    public class ImageKeyCollection : ObservableCollection<ImageKeyPair>
    {
    }
}
