// <copyright file="ProvinceCollection.cs" company="Dark Bond, Inc.">
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
    /// A collection of provinces (states).
    /// </summary>
    public class ProvinceCollection : ObservableCollection<ProvinceViewModel>, IDisposable
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
        /// Initializes a new instance of the <see cref="ProvinceCollection"/> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="compositionContext">The composition context.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        [ImportingConstructor]
        public ProvinceCollection(DataModel dataModel, CompositionContext compositionContext)
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

            // This will populate the collection with the provinces found in the data model.
            foreach (ProvinceRow provinceRow in this.dataModel.Province)
            {
                ProvinceViewModel provinceViewModel = this.compositionContext.GetExport<ProvinceViewModel>();
                provinceViewModel.Map(provinceRow);
                int index = this.BinarySearch((pvm) => pvm.ProvinceId, provinceViewModel.ProvinceId);
                if (index < 0)
                {
                    this.Insert(~index, provinceViewModel);
                }
            }

            // When rows are added to or deleted from the data model we need reconcile the view model.
            this.dataModel.Province.CollectionChanged += this.OnCollectionChanged;
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
                this.dataModel.Province.CollectionChanged -= this.OnCollectionChanged;
                foreach (ProvinceViewModel provinceViewModel in this)
                {
                    provinceViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Handle the RowChanged events of a <see cref="ProvinceTable"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (ProvinceViewModel provinceViewModel in this)
                    {
                        provinceViewModel.Dispose();
                    }

                    this.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                // Add each item into the view model that was added to the data model.
                foreach (ProvinceRow provinceRow in notifyCollectionChangedEventArgs.NewItems)
                {
                    // This will add a new item to the directory.  Note that the child view models are ordered so they can be found and deleted
                    // without having to scan the entire list.
                    ProvinceViewModel provinceViewModel = this.compositionContext.GetExport<ProvinceViewModel>();
                    provinceViewModel.Map(provinceRow);
                    int index = this.BinarySearch((pvm) => pvm.ProvinceId, provinceRow.ProvinceId);
                    if (index < 0)
                    {
                        this.Insert(~index, provinceViewModel);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:

                // Remove each of the items from the view model that have been removed from the data model.
                foreach (ProvinceRow provinceRow in notifyCollectionChangedEventArgs.OldItems)
                {
                    // This will purge the view model of the record that corresponds to the data model record.
                    int index = this.BinarySearch((pvm) => pvm.ProvinceId, provinceRow[DataRowVersion.Previous].ProvinceId);
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