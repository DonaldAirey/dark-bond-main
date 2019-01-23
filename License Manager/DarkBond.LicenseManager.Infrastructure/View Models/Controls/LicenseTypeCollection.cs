// <copyright file="LicenseTypeCollection.cs" company="Dark Bond, Inc.">
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
    /// A collection of licenseTypes.
    /// </summary>
    public class LicenseTypeCollection : ObservableCollection<LicenseTypeViewModel>, IDisposable
    {
        /// <summary>
        /// The data model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// The sorted and filtered view of the collection.
        /// </summary>
        private ListCollectionView viewField;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseTypeCollection"/> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        [ImportingConstructor]
        public LicenseTypeCollection(DataModel dataModel)
        {
            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Initialize the object.
            this.dataModel = dataModel;

            // This will populate the collection with the licenseTypes found in the data model.
            foreach (LicenseTypeRow licenseTypeRow in this.dataModel.LicenseType)
            {
                LicenseTypeViewModel licenseTypeViewModel = new LicenseTypeViewModel(licenseTypeRow);
                int index = this.BinarySearch((ltvm) => ltvm.LicenseTypeCode, licenseTypeViewModel.LicenseTypeCode);
                if (index < 0)
                {
                    this.Insert(~index, licenseTypeViewModel);
                }
            }

            // When rows are added to or deleted from the data model we need reconcile the view model.
            this.dataModel.LicenseType.CollectionChanged += this.OnCollectionChanged;

            // This will sort the view alphabetically by name.
            this.viewField = new ListCollectionView(this);
            this.viewField.SortDescriptions.Add(
                new SortDescription
                {
                    PropertyName = "LicenseTypeCode",
                    Direction = SortDirection.Ascending
                });
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
                this.dataModel.LicenseType.CollectionChanged -= this.OnCollectionChanged;
                foreach (LicenseTypeViewModel licenseTypeViewModel in this)
                {
                    licenseTypeViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Handle the RowChanged events of a <see cref="LicenseTypeTable"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // This will dispose of all the children.
                    foreach (LicenseTypeViewModel licenseTypeViewModel in this)
                    {
                        licenseTypeViewModel.Dispose();
                    }

                    this.Clear();

                    break;

                case NotifyCollectionChangedAction.Add:

                    // Add each item into the view model that was added to the data model.
                    foreach (LicenseTypeRow licenseTypeRow in notifyCollectionChangedEventArgs.NewItems)
                    {
                        // This will add a new item to the directory.  Note that we use the data model object as a key into our dictionary which allows
                        // us to access the elements of the dictionary even after the view model record has been deleted and purged.
                        LicenseTypeViewModel licenseTypeViewModel = new LicenseTypeViewModel(licenseTypeRow);
                        int index = this.BinarySearch((ltvm) => ltvm.LicenseTypeCode, licenseTypeRow.LicenseTypeCode);
                        if (index < 0)
                        {
                            this.Insert(~index, licenseTypeViewModel);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                // Remove each of the items from the view model that have been removed from the data model.
                foreach (LicenseTypeRow licenseTypeRow in notifyCollectionChangedEventArgs.OldItems)
                {
                    // This will purge the view model of the record that corresponds to the data model record.  Note that we use the data model
                    // record as the key for the hash.  Once a DataRow has been deleted and detached, there is no useful information left behind
                    // except for the object itself, which can still serve as a key into the dictionary.  Also note that the base table will take
                    // care of disposing the managed resources of the deleted item.
                    int index = this.BinarySearch((ltvm) => ltvm.LicenseTypeCode, licenseTypeRow[DataRowVersion.Previous].LicenseTypeCode);
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