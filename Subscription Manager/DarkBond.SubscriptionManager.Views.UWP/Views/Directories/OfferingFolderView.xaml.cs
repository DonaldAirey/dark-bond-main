// <copyright file="OfferingFolderView.xaml.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Directories
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Displays items in a variety of views: thumbnail, detail or columnar.
    /// </summary>
    public sealed partial class OfferingFolderView : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingFolderView"/> class.
        /// </summary>
        public OfferingFolderView()
        {
            // Initialize the IDE managed components.
            this.InitializeComponent();
        }
    }
}
