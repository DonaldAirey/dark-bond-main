// <copyright file="CustomerCollection.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager;

    /// <summary>
    /// A collection of customers.
    /// </summary>
    public class CustomerCollection : ObservableCollection<CustomerViewModel>, IDisposable
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
        /// Initializes a new instance of the <see cref="CustomerCollection"/> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="compositionContext">The composition context.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        [ImportingConstructor]
        public CustomerCollection(DataModel dataModel, CompositionContext compositionContext)
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

            // This will populate the collection with the customers found in the data model.
            foreach (CustomerRow customerRow in this.dataModel.Customer)
            {
                CustomerViewModel customerViewModel = this.compositionContext.GetExport<CustomerViewModel>();
                customerViewModel.Map(customerRow);
                int index = this.BinarySearch((pvm) => pvm.CustomerId, customerViewModel.CustomerId);
                if (index < 0)
                {
                    this.Insert(~index, customerViewModel);
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
            this.dataModel.Customer.CollectionChanged += this.OnCollectionChanged;
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
                this.dataModel.Customer.CollectionChanged -= this.OnCollectionChanged;
                foreach (CustomerViewModel customerViewModel in this)
                {
                    customerViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Handle the RowChanged events of a <see cref="CustomerTable"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (CustomerViewModel customerViewModel in this)
                    {
                        customerViewModel.Dispose();
                    }

                    this.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // Add each item into the view model that was added to the data model.
                    foreach (CustomerRow customerRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        // This will add a new item to the directory.  Note that we use the data model object as a key into our dictionary which
                        // allows us to access the elements of the dictionary even after the view model record has been deleted and purged.
                        CustomerViewModel customerViewModel = this.compositionContext.GetExport<CustomerViewModel>();
                        customerViewModel.Map(customerRow);
                        int index = this.BinarySearch((pvm) => pvm.CustomerId, customerViewModel.CustomerId);
                        if (index < 0)
                        {
                            this.Insert(~index, customerViewModel);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // Remove each of the items from the view model that have been removed from the data model.
                    foreach (CustomerRow customerRow in notifyCollectionChangedEventArgs.OldItems)
                    {
                        // This will purge the view model of the record that corresponds to the data model record.
                        int index = this.BinarySearch((pvm) => pvm.CustomerId, customerRow[DataRowVersion.Previous].CustomerId);
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