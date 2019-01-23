// <copyright file="UnderwriterFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;

    /// <summary>
    /// A breadcrumb for the folder that displays underwriters.
    /// </summary>
    public class UnderwriterFolderViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        public UnderwriterFolderViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this node.
            this.Header = Resources.Customer;
            this.ImageKey = ImageKeys.Folder;
            this.Identifier = Resources.Customer;
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