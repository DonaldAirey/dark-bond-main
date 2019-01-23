// <copyright file="CommonDirectoryViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Directories
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.ViewModels;
    using Strings;

    /// <summary>
    /// View Model for the directory that shows the customers.
    /// </summary>
    public abstract class CommonDirectoryViewModel : DirectoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDirectoryViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        protected CommonDirectoryViewModel(CompositionContext compositionContext, DataModel dataModel, ILicenseService licenseService)
        {
            // Validate the compositionContext parameter.
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Validate the parameter.
            if (licenseService == null)
            {
                throw new ArgumentNullException(nameof(licenseService));
            }

            // Initialize the object.
            this.CompositionContext = compositionContext;
            this.DataModel = dataModel;
            this.LicenseService = licenseService;
        }

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        protected CompositionContext CompositionContext { get; private set; }

        /// <summary>
        /// Gets the data model.
        /// </summary>
        protected DataModel DataModel { get; private set; }

        /// <summary>
        /// Gets the license service.
        /// </summary>
        protected ILicenseService LicenseService { get; private set; }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // The base class is (sometimes) used to define common AppBar items.
            ObservableCollection<IDisposable> appBarItems = base.CreateAppBarItems();

            // The Clear Selection button.
            ButtonViewModel clearSelectionButton = this.CompositionContext.GetExport<ButtonViewModel>();
            clearSelectionButton.Command = GlobalCommands.SelectNone;
            clearSelectionButton.ImageKey = ImageKeys.ClearSelectionAll;
            clearSelectionButton.Label = Resources.ClearSelection;
            appBarItems.Add(clearSelectionButton);

            // The Select All button.
            ButtonViewModel selectAllButton = this.CompositionContext.GetExport<ButtonViewModel>();
            selectAllButton.Command = GlobalCommands.SelectAll;
            selectAllButton.ImageKey = ImageKeys.SelectAll;
            selectAllButton.Label = Resources.SelectAll;
            appBarItems.Add(selectAllButton);

            // The Sign-In button.
            ButtonViewModel signInButton = this.CompositionContext.GetExport<ButtonViewModel>();
            signInButton.Command = GlobalCommands.SignIn;
            signInButton.ImageKey = ImageKeys.SignIn;
            signInButton.Label = Resources.SignIn;
            appBarItems.Add(signInButton);

            // This is the set of appBar items for this ListView item.
            return appBarItems;
        }
    }
}