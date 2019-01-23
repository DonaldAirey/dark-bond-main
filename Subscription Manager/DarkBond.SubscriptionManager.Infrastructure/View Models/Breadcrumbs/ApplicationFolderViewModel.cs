// <copyright file="ApplicationFolderViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;

    /// <summary>
    /// A breadcrumb for the application.
    /// </summary>
    public class ApplicationFolderViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        public ApplicationFolderViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this node.
            this.Header = Resources.ApplicationName;
            this.ImageKey = ImageKeys.Application;
            this.Identifier = Resources.ApplicationName;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // Create the library nodes for underwriters and offering folders.
            this.Items.Add(compositionContext.GetExport<UnderwriterFolderViewModel>());
            this.Items.Add(compositionContext.GetExport<OfferingFolderViewModel>());
        }
    }
}