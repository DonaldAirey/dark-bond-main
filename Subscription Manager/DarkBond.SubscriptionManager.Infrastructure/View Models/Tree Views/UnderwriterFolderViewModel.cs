// <copyright file="UnderwriterFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.TreeViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A navigation tree view item for the folder that displays underwriters.
    /// </summary>
    public class UnderwriterFolderViewModel : CommonTreeViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription service.</param>
        public UnderwriterFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the properties of this node.
            this.Header = Resources.Customer;
            this.Identifier = Resources.Customer;
            this.ImageKey = ImageKeys.Folder;
            this.RootUri = new Uri(Properties.Resources.FrameUri);

            // When underwriters are added to the data model we need to make children of them for this breadcrumb.
            UnderwriterTable underwriterTable = this.DataModel.Underwriter;
            underwriterTable.CollectionChanged += this.OnCollectionChanged;

            // Initialize the collection of items in this directory from the data model.
            foreach (UnderwriterRow underwriterRow in this.DataModel.Underwriter)
            {
                UnderwriterViewModel underwriterViewModel = this.CompositionContext.GetExport<UnderwriterViewModel>();
                underwriterViewModel.Map(underwriterRow);
                int index = this.Items.BinarySearch((mivm) => mivm.SortKey, underwriterRow.UnderwriterId);
                this.Items.Insert(~index, underwriterViewModel);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Release the event handlers that were attached to the data model.
            if (disposing)
            {
                this.DataModel.Underwriter.CollectionChanged -= this.OnCollectionChanged;
            }

            // Allow the base class to recover resources.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a context menu for this item.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // The common menu items.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();
            contextMenuViewItems.Add(this.ExpandMenuItem);
            contextMenuViewItems.Add(new SeparatorViewModel());

            // New Customer Menu Item
            MenuItemViewModel newCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newCustomerMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer());
            newCustomerMenuItem.Header = Resources.NewCustomer;
            newCustomerMenuItem.ImageKey = ImageKeys.Customer;
            contextMenuViewItems.Add(newCustomerMenuItem);

            // These are the context menu items.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Handle a change to the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // This will handle the different verbs.
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (UnderwriterViewModel underwriterViewModel in this.Items)
                    {
                        underwriterViewModel.Dispose();
                    }

                    this.Items.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // This will add the item as a child of this breadcrumb.
                    foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        UnderwriterViewModel underwriterViewModel = this.CompositionContext.GetExport<UnderwriterViewModel>();
                        underwriterViewModel.Map(underwriterRow);
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, underwriterRow.UnderwriterId);
                        this.Items.Insert(~index, underwriterViewModel);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // This remove the item as a child of this breadcrumb.
                    foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.OldItems)
                    {
                        int index = this.Items.BinarySearch((mivm) => mivm.SortKey, underwriterRow[DataRowVersion.Original].UnderwriterId);
                        if (index >= 0)
                        {
                            this.Items[index].Dispose();
                            this.Items.RemoveAt(index);
                        }
                    }

                    break;
            }
        }
    }
}