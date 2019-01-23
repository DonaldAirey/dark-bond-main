// <copyright file="ListItemViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Composition;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// The view model for an item that appears in a directory.
    /// </summary>
    public abstract class ListItemViewModel : ViewModel
    {
        /// <summary>
        /// The AppBar items.
        /// </summary>
        private ObservableCollection<IDisposable> appBarItems;

        /// <summary>
        /// The items in the toolbar.
        /// </summary>
        private ObservableCollection<IDisposable> contextButtonItems;

        /// <summary>
        /// The items in the context menu.
        /// </summary>
        private ObservableCollection<IDisposable> contextMenuItems;

        /// <summary>
        /// A value indicating whether the node is selected or not.  Part of the Model View.
        /// </summary>
        private bool isSelectedField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemViewModel"/> class.
        /// </summary>
        protected ListItemViewModel()
        {
            // Initialize the object.
            this.Tapped = new DelegateCommand(() =>
            {
                this.OnTapped();
            });

            // Handles changes to the properties.
            this.PropertyChangedActions["IsSelected"] = this.OnIsSelectedChanged;
        }

        /// <summary>
        /// Gets the appBar items for this object.
        /// </summary>
        public virtual ObservableCollection<IDisposable> AppBar
        {
            get
            {
                // Because the context menu items can have managed resources, we're going to get them on demand.
                if (this.appBarItems == null)
                {
                    this.appBarItems = this.CreateAppBarItems();
                }

                return this.appBarItems;
            }
        }

        /// <summary>
        /// Gets the view models for the context buttons.
        /// </summary>
        public virtual ObservableCollection<IDisposable> ContextButtons
        {
            get
            {
                // Because the context button items can have managed resources, we're going to get them on demand.
                if (this.contextButtonItems == null)
                {
                    this.contextButtonItems = this.CreateContextButtonItems();
                }

                return this.contextButtonItems;
            }
        }

        /// <summary>
        /// Gets the view models for every item in the context menu.
        /// </summary>
        public virtual ObservableCollection<IDisposable> ContextMenu
        {
            get
            {
                // Because the context menu items can have managed resources, we're going to get them on demand.
                if (this.contextMenuItems == null)
                {
                    this.contextMenuItems = this.CreateContextMenuItems();
                }

                return this.contextMenuItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelectedField;
            }

            set
            {
                if (value != this.isSelectedField)
                {
                    this.isSelectedField = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public DirectoryViewModel Parent { get; set; }

        /// <summary>
        /// Gets or sets the key used for sorting.
        /// </summary>
        public IComparable SortKey { get; set; }

        /// <summary>
        /// Gets the Tapped command.
        /// </summary>
        public DelegateCommand Tapped { get; private set; }

        /// <summary>
        /// Gets the URI of this object.
        /// </summary>
        public abstract Uri Uri { get; }

        /// <summary>
        /// Handles a tapped event.
        /// </summary>
        public virtual void OnTapped()
        {
            // If this object has a viewer associated with it then browse to the viewer.
            if (this.Uri != null)
            {
                GlobalCommands.Locate.Execute(this.Uri);
            }
        }

        /// <summary>
        /// Creates the AppBar items.
        /// </summary>
        /// <returns>The AppBar items.</returns>
        protected virtual ObservableCollection<IDisposable> CreateAppBarItems()
        {
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected virtual ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Create a list for the context menu items.
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected virtual ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Create a list for the context menu items.
            return new ObservableCollection<IDisposable>();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // Release the resources used by each of the app bar items.
            if (this.appBarItems != null)
            {
                foreach (IDisposable disposable in this.appBarItems)
                {
                    disposable.Dispose();
                }
            }

            // Release the resources used by each of the context menu items.
            if (this.contextMenuItems != null)
            {
                foreach (IDisposable disposable in this.contextMenuItems)
                {
                    disposable.Dispose();
                }
            }

            // Allow the base class to dispose.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the IsSelected property.
        /// </summary>
        protected virtual void OnIsSelectedChanged()
        {
            if (this.IsSelected)
            {
                GlobalCommands.Select.Execute(this);
            }
            else
            {
                GlobalCommands.ClearSelection.Execute(this);
            }
        }
    }
}