// <copyright file="ApplicationFolderViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;

    /// <summary>
    /// A navigation tree view item for the folder the application.
    /// </summary>
    public class ApplicationFolderViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        public ApplicationFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the properties of this node.
            this.Header = Resources.ApplicationName;
            this.Identifier = Resources.ApplicationName;
            this.ImageKey = ImageKeys.Application;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // Create the library nodes for customers and product folders.
            this.Items.Add(this.CompositionContext.GetExport<CustomerFolderViewModel>());
            this.Items.Add(this.CompositionContext.GetExport<ProductFolderViewModel>());
        }

        /// <summary>
        /// Creates a context menu for this item.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();
            contextMenuViewItems.Add(this.ExpandMenuItem);
            return contextMenuViewItems;
        }
    }
}