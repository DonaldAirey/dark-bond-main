// <copyright file="ProductView.xaml.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views.Directories
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Displays items in a variety of views: thumbnail, detail or columnar.
    /// </summary>
    public sealed partial class ProductView : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductView"/> class.
        /// </summary>
        public ProductView()
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();
        }
    }
}
