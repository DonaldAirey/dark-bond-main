// <copyright file="ToolbarViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Navigation;

    /// <summary>
    /// View Model for the directory that shows a collection of <see cref="ToolBarViewModel"/> items.
    /// </summary>
    public abstract class ToolBarViewModel : ViewModel
    {
        /// <summary>
        /// The items displayed in this directory.
        /// </summary>
        private ObservableCollection<ToolBarViewModel> childrenField = new ObservableCollection<ToolBarViewModel>();

        /// <summary>
        /// The display name of the item.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The URI of the breadcrumb.
        /// </summary>
        private Uri uriField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarViewModel"/> class.
        /// </summary>
        protected ToolBarViewModel()
        {
            // These property changed actions are handled by the base class.
            this.PropertyChangedActions["Uri"] = this.OnUriChanged;

            // ToolBarViewModel items can have managed resources (generally this means event handlers connected to more permanent objects such as the
            // data model).  When an item is removed from hierarchy of toolBars, this handler will take care of calling the 'Dispose' method so any
            // item can clean up after itself.
            this.childrenField.CollectionChanged += this.OnChildrenCollectionChanged;
        }

        /// <summary>
        /// Gets the child items of this view model.
        /// </summary>
        public ObservableCollection<ToolBarViewModel> Children
        {
            get
            {
                return this.childrenField;
            }
        }

        /// <summary>
        /// Gets the items that are displayed in this toolBar.
        /// </summary>
        public abstract ObservableCollection<IDisposable> Items
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameField;
            }

            set
            {
                if (this.nameField != value)
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the URI of this view model.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uriField;
            }

            set
            {
                if (this.uriField != value)
                {
                    this.uriField = value;
                    this.OnPropertyChanged("Uri");
                }
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Information about the event.</param>
        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // When items are added to the hierarchy they inherit the directory's base URI.
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ToolBarViewModel listViewItemViewModel in notifyCollectionChangedEventArgs.NewItems)
                {
                    this.UpdateChildUri(listViewItemViewModel);
                }
            }
        }

        /// <summary>
        /// Handles a change to the Uri property.
        /// </summary>
        private void OnUriChanged()
        {
            // When the URI of this node has changed, the change will be cascaded to all of the items in the directory.  In order for navigation to
            // be brutally simple, we want each node to contain an absolute URI that reflects its place in the hierarchy.  I had considered a method
            // by which the URIs would be constructed "On Demand", but that made it difficult to create an MVVM version of the commands.
            foreach (ToolBarViewModel listViewItemViewModel in this.Items)
            {
                this.UpdateChildUri(listViewItemViewModel);
            }
        }

        /// <summary>
        /// Updates the child breadcrumb's URI with the parent's URI.
        /// </summary>
        /// <param name="toolBarViewModel">The child breadcrumb.</param>
        private void UpdateChildUri(ToolBarViewModel toolBarViewModel)
        {
            // If the parent has a URI (this is not always guaranteed as the hierarchy is constructed), then use it as the base part of the child's
            // URI.
            if (this.Uri != null)
            {
                UriQueryCollection uriQueryCollection = new UriQueryCollection(this.Uri.Query);
                string currentPath = uriQueryCollection["path"];
                UriBuilder uriBuilder = new UriBuilder(this.Uri);
                uriBuilder.Query = string.Format(CultureInfo.CurrentCulture, @"path={0}\{1}", currentPath, toolBarViewModel.Name);
                toolBarViewModel.Uri = uriBuilder.Uri;
            }
        }
    }
}