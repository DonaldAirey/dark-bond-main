// <copyright file="ShellView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using DarkBond.LicenseManager.ViewModels;
    using DarkBond.Views.Controls;

    /// <summary>
    /// The view model for the application shell.
    /// </summary>
    internal sealed partial class ShellView : ShellViewBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView"/> class.
        /// </summary>
        /// <param name="shellViewModel">The view model for the shell.</param>
        public ShellView(ShellViewModel shellViewModel)
        {
            // Validate the compositionContext parameter.
            if (shellViewModel == null)
            {
                throw new ArgumentNullException(nameof(shellViewModel));
            }

            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // Set the default language for the user.
            this.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

            // Get the view model from the container and use it as a data context.
            this.DataContext = shellViewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public ShellViewModel ViewModel
        {
            get
            {
                return this.DataContext as ShellViewModel;
            }
        }
    }
}