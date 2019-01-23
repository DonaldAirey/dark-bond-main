// <copyright file="UnderwriterCollection.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.SubscriptionManager;

    /// <summary>
    /// A collection of underwriters.
    /// </summary>
    public class UnderwriterCollection : ObservableCollection<UnderwriterViewModel>, IDisposable
    {
        /// <summary>
        /// The composition context.
        /// </summary>
        private CompositionContext compositionContext;

        /// <summary>
        /// The data model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// The sorted and filtered view of the collection.
        /// </summary>
        private ListCollectionView viewField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterCollection"/> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="compositionContext">The composition context.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        [ImportingConstructor]
        public UnderwriterCollection(DataModel dataModel, CompositionContext compositionContext)
        {
            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Validate the parameter.
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // Initialize the object.
            this.dataModel = dataModel;
            this.compositionContext = compositionContext;

            // This will populate the collection with the underwriters found in the data model.
            foreach (UnderwriterRow underwriterRow in this.dataModel.Underwriter)
            {
                UnderwriterViewModel underwriterViewModel = this.compositionContext.GetExport<UnderwriterViewModel>();
                underwriterViewModel.Map(underwriterRow);
                int index = this.BinarySearch((pvm) => pvm.UnderwriterId, underwriterViewModel.UnderwriterId);
                if (index < 0)
                {
                    this.Insert(~index, underwriterViewModel);
                }
            }

            // This will sort the view alphabetically by name.
            this.viewField = new ListCollectionView(this);
            this.viewField.SortDescriptions.Add(
                new SortDescription
                {
                    PropertyName = "Name",
                    Direction = SortDirection.Ascending
                });

            // When rows are added to or deleted from the data model we need reconcile the view model.
            this.dataModel.Underwriter.CollectionChanged += this.OnCollectionChanged;
        }

        /// <summary>
        /// Gets the view for the collection.
        /// </summary>
        public ListCollectionView View
        {
            get
            {
                return this.viewField;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call our virtual method for disposing of resources and then inhibit the native garbage collection.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            // The data model must be disconnected from this object before it is garbage collected.
            if (disposing)
            {
                // Disengage from the notification updates from the data model.
                this.dataModel.Underwriter.CollectionChanged -= this.OnCollectionChanged;
                foreach (UnderwriterViewModel underwriterViewModel in this)
                {
                    underwriterViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Handle the RowChanged events of a <see cref="UnderwriterTable"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (UnderwriterViewModel underwriterViewModel in this)
                    {
                        underwriterViewModel.Dispose();
                    }

                    this.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // Add each item into the view model that was added to the data model.
                    foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        // This will add a new item to the directory.  Note that we use the data model object as a key into our dictionary which
                        // allows us to access the elements of the dictionary even after the view model record has been deleted and purged.
                        UnderwriterViewModel underwriterViewModel = this.compositionContext.GetExport<UnderwriterViewModel>();
                        underwriterViewModel.Map(underwriterRow);
                        int index = this.BinarySearch((pvm) => pvm.UnderwriterId, underwriterViewModel.UnderwriterId);
                        if (index < 0)
                        {
                            this.Insert(~index, underwriterViewModel);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // Remove each of the items from the view model that have been removed from the data model.
                    foreach (UnderwriterRow underwriterRow in notifyCollectionChangedEventArgs.OldItems)
                    {
                        // This will purge the view model of the record that corresponds to the data model record.
                        int index = this.BinarySearch((pvm) => pvm.UnderwriterId, underwriterRow[DataRowVersion.Previous].UnderwriterId);
                        if (index >= 0)
                        {
                            this[index].Dispose();
                            this.RemoveAt(index);
                        }
                    }

                    break;
            }
        }
    }
}