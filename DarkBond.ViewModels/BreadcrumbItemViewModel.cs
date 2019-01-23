// <copyright file="BreadcrumbItemViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// An element used to display and navigate a path.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Reviewed")]
    public abstract class BreadcrumbItemViewModel : DropDownButtonViewModel
    {
        /// <summary>
        /// The identifier of this breadcrumb.
        /// </summary>
        private string identifierField;

        /// <summary>
        /// A value indicating whether the node is selected or not.
        /// </summary>
        private bool isSelectedField;

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadcrumbItemViewModel"/> class.
        /// </summary>
        protected BreadcrumbItemViewModel()
        {
            // Every breadcrumb is capable of opening up a viewer.  The URI is the parameter to this command and won't be set until we've been
            // assigned a URI which is properly located in the hierarchy.
            this.Command = GlobalCommands.Locate;

            // This handler will link and unlink the children to and from the parent.
            this.Items.CollectionChanged += this.OnCollectionChanged;
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
        /// Gets or sets the unique identifier of this object.
        /// </summary>
        public string Identifier
        {
            get
            {
                return this.identifierField;
            }

            protected set
            {
                if (this.identifierField != value)
                {
                    this.identifierField = value;
                    this.OnPropertyChanged("Identifier");
                }
            }
        }

        /// <summary>
        /// Gets the parent breadcrumb item.
        /// </summary>
        public BreadcrumbItemViewModel Parent { get; private set; }

        /// <summary>
        /// Gets the viewer used to display this object.
        /// </summary>
        public Uri Uri
        {
            get
            {
                // Construct a path by recursing up through the hierarchy.
                string path = this.Identifier;
                BreadcrumbItemViewModel parent = this.Parent;
                while (parent != null)
                {
                    path = parent.Identifier + "\\" + path;
                    parent = parent.Parent;
                }

                // The URI of this node consists of the absolute path of the directory viewer plus the encoded path to the directory level.
                UriBuilder uriBuilder = new UriBuilder(this.RootUri);
                uriBuilder.Query = @"path=\" + path;
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Gets or sets the root URI for a path to navigate to this object.
        /// </summary>
        protected Uri RootUri { get; set; }

        /// <summary>
        /// Finds a child element with the given identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the child.</param>
        /// <returns>The item with the given name or null if there is no such item.</returns>
        public virtual BreadcrumbItemViewModel FindChild(object identifier)
        {
            return (from BreadcrumbItemViewModel b in this.Items
                    where b.Identifier.Equals(identifier)
                    select b).FirstOrDefault();
        }

        /// <summary>
        /// Handles a change to the collection.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Reset:

                    // Remove each of the children from this parent.
                    foreach (BreadcrumbItemViewModel breadcrumbItemViewModel in this.Items)
                    {
                        breadcrumbItemViewModel.Parent = null;
                    }

                    break;

                case NotifyCollectionChangedAction.Add:

                    // Add each of the items as a child of this parent.
                    foreach (BreadcrumbItemViewModel breadcrumbItemViewModel in notifyCollectionChangedEventArgs.NewItems)
                    {
                        breadcrumbItemViewModel.Parent = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // Remove each of the items as a child of this parent.
                    foreach (BreadcrumbItemViewModel breadcrumbItemViewModel in notifyCollectionChangedEventArgs.OldItems)
                    {
                        breadcrumbItemViewModel.Parent = null;
                    }

                    break;
            }
        }
    }
}