// <copyright file="CustomerFolderView.xaml.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Directory
{
    using DarkBond.Views.Controls;

    /// <summary>
    /// Displays items in a variety of views: thumbnail, detail or columnar.
    /// </summary>
    public sealed partial class CustomerFolderView : ItemsView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerFolderView"/> class.
        /// </summary>
        public CustomerFolderView()
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();
        }
    }
}
