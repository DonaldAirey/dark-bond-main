﻿// <copyright file="NavigationTreeViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels
{
    using System;
    using System.Composition;
    using DarkBond.LicenseManager.ViewModels.TreeViews;
    using DarkBond.Navigation;
    using DarkBond.ViewModels;

    /// <summary>
    /// The collection of breadcrumbs that define the application hierarchy.
    /// </summary>
    public class NavigationTreeViewModel : NavigationTreeViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTreeViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="navigationService">The navigation service.</param>
        public NavigationTreeViewModel(CompositionContext compositionContext, INavigationService navigationService)
            : base(navigationService)
        {
            // Validate the parameter
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // This is the root of the breadcrumb path.
            this.Items.Add(compositionContext.GetExport<ApplicationFolderViewModel>());
        }
    }
}