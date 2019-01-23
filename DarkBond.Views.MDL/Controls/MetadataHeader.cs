// <copyright file="MetadataHeader.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides a lightweight control for displaying small amounts of flow content.
    /// </summary>
    public class MetadataHeader : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataHeader"/> class.
        /// </summary>
        public MetadataHeader()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(MetadataHeader);
        }
    }
}