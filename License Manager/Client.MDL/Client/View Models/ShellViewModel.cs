// <copyright file="ShellViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels
{
    using DarkBond.Navigation;
    using DarkBond.ViewModels;

    /// <summary>
    /// The view model for the application shell.
    /// </summary>
    public class ShellViewModel : ShellViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public ShellViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        public static string Title
        {
            get
            {
                return Strings.Resources.ApplicationName;
            }
        }
    }
}