// <copyright file="ApplicationFolderViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;

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
        /// <param name="subscriptionService">The subscription service.</param>
        public ApplicationFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the properties of this node.
            this.Header = Resources.ApplicationName;
            this.Identifier = Resources.ApplicationName;
            this.ImageKey = ImageKeys.Application;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // Create the library nodes for underwriters and offering folders.
            this.Items.Add(this.CompositionContext.GetExport<UnderwriterFolderViewModel>());
            this.Items.Add(this.CompositionContext.GetExport<OfferingFolderViewModel>());
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