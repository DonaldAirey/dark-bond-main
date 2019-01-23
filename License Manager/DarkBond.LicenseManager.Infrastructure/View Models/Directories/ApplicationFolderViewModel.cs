// <copyright file="ApplicationFolderViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Directories
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for a node in the hierarchy that contains customer items.
    /// </summary>
    public class ApplicationFolderViewModel : CommonDirectoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        public ApplicationFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the object.
            this.ImageKey = ImageKeys.Application;
            this.Name = Resources.ApplicationName;
            this.RootUri = new Uri(Properties.Resources.FrameUri);
        }

        /// <summary>
        /// Gets the URI of this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder(this.RootUri);
                uriBuilder.Query = @"path=\" + Resources.ApplicationName;
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Loads the resources of the directory.
        /// </summary>
        /// <param name="path">The path to be displayed in the directory.</param>
        public override void Load(string path)
        {
            // This will prevent updates to the view until all the items are added.
            using (this.Items.View.DeferRefresh())
            {
                // Create the view model for the customer folder.
                ListViews.CustomerFolderViewModel customerFolderViewModel = this.CompositionContext.GetExport<ListViews.CustomerFolderViewModel>();
                this.Items.Add(customerFolderViewModel);

                // Create the view model for the product folder.
                ListViews.ProductFolderViewModel productFolderViewModel = this.CompositionContext.GetExport<ListViews.ProductFolderViewModel>();
                this.Items.Add(productFolderViewModel);
            }

            // Allow the base class to finish loading the view model.
            base.Load(path);
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextButtonViewItems = base.CreateContextButtonItems();

            // Open Button
            ButtonViewModel openButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            openButtonItem.Command = new DelegateCommand(() => { }, () => false);
            openButtonItem.Header = Resources.Open;
            openButtonItem.ImageKey = ImageKeys.Open;
            contextButtonViewItems.Add(openButtonItem);

            // New Button
            ButtonViewModel newButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            newButtonItem.Command = new DelegateCommand(() => { }, () => false);
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = new DelegateCommand(() => { }, () => false);
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButton = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButton.Command = new DelegateCommand(() => { }, () => false);
            propertiesButton.Header = Resources.Properties;
            propertiesButton.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButton);

            // This is the context menu.
            return contextButtonViewItems;
        }
    }
}