// <copyright file="FilteredCollection{T}.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// A collection that can be filtered and sorted.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class FilteredCollection<T> : ObservableCollection<T>, IDeferrable
    {
        /// <summary>
        /// This provides the ability to have multiple filters on a view.
        /// </summary>
        private FilterCollection<T> filtersField = new FilterCollection<T>();

        /// <summary>
        /// Used to disable the collection changed update during a bulk insert.
        /// </summary>
        private bool isRefreshDisabledField;

        /// <summary>
        /// The sorted and filtered view of the collection.
        /// </summary>
        private ListCollectionView viewField;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredCollection{T}"/> class.
        /// </summary>
        public FilteredCollection()
        {
            // This is the view that provides filtering and sorting for this collection.
            this.viewField = new ListCollectionView(this);
            this.viewField.Filter = this.ProcessFilterList;

            // This lets us know that a new filter has been added to the collection so we can refresh the view.
            this.filtersField.CollectionChanged += this.OnFilterListChanged;
        }

        /// <summary>
        /// Gets a collection of filter groups.
        /// </summary>
        public FilterCollection<T> Filters
        {
            get
            {
                return this.filtersField;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether refreshing the list is deferred.
        /// </summary>
        public bool IsRefreshDisabled
        {
            get
            {
                return this.isRefreshDisabledField;
            }

            set
            {
                this.isRefreshDisabledField = value;
            }
        }

        /// <summary>
        /// Gets the view that provides sorting and filtering.
        /// </summary>
        public ListCollectionView View
        {
            get
            {
                return this.viewField;
            }
        }

        /// <summary>
        /// Resets the observable collection after a deferment.
        /// </summary>
        public void ResetCollection()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Raises the CollectionChanged event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Don't update the collection during a deferral.  When deferring is finished, a RESET event should be sent out to consumers of the collection.
            if (!this.isRefreshDisabledField)
            {
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Determine whether an item is suitable for inclusion in the view.
        /// </summary>
        /// <param name="item">The item to be tested.</param>
        /// <returns>true if the item is to be include in the view, false if not.</returns>
        protected virtual bool ProcessFilterList(object item)
        {
            // Only strongly typed objects can be tested against the strongly typed filters.
            T filteredItem = (T)item;
            if (filteredItem == null)
            {
                return false;
            }

            // If no filters are active then every item is available.  Otherwise the filters act like a logical 'Or' by including any item that
            // passes any of the filters.
            if (this.Filters.Count == 0)
            {
                return true;
            }

            // This will test the item against each of the filters.  If it passes any one of the filters it can be included in the view.  In this way
            // the combined filters act like a logical 'Or' by include any of the rows that pass this filter or that filter or any other filter.
            foreach (ObservableCollection<Predicate<T>> filterGroup in this.Filters)
            {
                // If there are no items in this group, then look at the next one.
                if (filterGroup.Count == 0)
                {
                    continue;
                }

                // Test the sample item against each of the predicates in the filter group.  When it passes just one of the tests it can be included
                // in the view.  The elements of each group act like an 'Or' statement.
                bool passedFilterGroup = false;
                foreach (Predicate<T> filter in filterGroup)
                {
                    if (filter(filteredItem))
                    {
                        passedFilterGroup = true;
                        break;
                    }
                }

                // If we failed to match any of the tests in this group the sample item is excluded.
                if (!passedFilterGroup)
                {
                    return false;
                }
            }

            // At this point the row has not passed any of the filters defined for this view.
            return true;
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Information about the event.</param>
        private void OnFilterListChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
            case NotifyCollectionChangedAction.Add:

                // Each group that is added to the collection needs to refresh the view when the collection changes.
                foreach (ObservableCollection<Predicate<T>> filterGroup in notifyCollectionChangedEventArgs.NewItems)
                {
                    filterGroup.CollectionChanged += this.OnFilterGroupCollectionChanged;
                }

                break;

            case NotifyCollectionChangedAction.Remove:

                // This will extract the refresh event from the group when it is pulled out of the filter.
                foreach (ObservableCollection<Predicate<T>> filterGroup in notifyCollectionChangedEventArgs.OldItems)
                {
                    filterGroup.CollectionChanged -= this.OnFilterGroupCollectionChanged;
                }

                break;
            }

            // Refresh the view after any of the groups have been added or removed.
            this.viewField.Refresh();
        }

        /// <summary>
        /// Handles a change to any one of the filter groups.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Information about the event.</param>
        private void OnFilterGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // Refresh the view when any of the filter groups contents have changed.
            this.viewField.Refresh();
        }
    }
}