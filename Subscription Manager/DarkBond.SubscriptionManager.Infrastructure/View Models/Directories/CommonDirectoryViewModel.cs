// <copyright file="CommonDirectoryViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Directories
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;

    /// <summary>
    /// View Model for the directory that shows the underwriters.
    /// </summary>
    public abstract class CommonDirectoryViewModel : DirectoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDirectoryViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        protected CommonDirectoryViewModel(CompositionContext compositionContext, DataModel dataModel, ISubscriptionService subscriptionService)
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
            if (subscriptionService == null)
            {
                throw new ArgumentNullException(nameof(subscriptionService));
            }

            // Initialize the object.
            this.CompositionContext = compositionContext;
            this.DataModel = dataModel;
            this.SubscriptionService = subscriptionService;
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
        /// Gets the subscription service.
        /// </summary>
        protected ISubscriptionService SubscriptionService { get; private set; }

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