// <copyright file="ToolbarView.xaml.cs" company="Teraque, Inc.">
//     Copyright © 2014 - Teraque, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using DarkBond.View.Controls;

    /// <summary>
    /// The main toolbar for the application.
    /// </summary>
    public partial class ToolbarView : ToolbarViewBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarView"/> class.
        /// </summary>
        public ToolbarView()
        {
            // Initialize the IDE maintained components.
            this.InitializeComponent();
        }
    }
}
