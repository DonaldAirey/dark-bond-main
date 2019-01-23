// <copyright file="NavigationTreeItemViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// A view model for items that are displayed in a hierarchy (e.g. file system).
    /// </summary>
    public class NavigationTreeItemViewModel : ButtonViewModel
    {
        /// <summary>
        /// The items in the context menu.
        /// </summary>
        private ObservableCollection<IDisposable> contextMenuViewItems;

        /// <summary>
        /// The sorted collection of hierarchy items.
        /// </summary>
        private ListCollectionView listCollectionView;

        /// <summary>
        /// View model for a menu item that allows expanding and collapsing the current node.
        /// </summary>
        private ExpandMenuItemViewModel expandMenuItemViewModel;

        /// <summary>
        /// Indication of whether the node has expanded items.
        /// </summary>
        private bool isExpandedField;

        /// <summary>
        /// Indicates that the current node is the root of the hierarchy.
        /// </summary>
        private bool isRootField;

        /// <summary>
        /// Indication of whether the node is selected.
        /// </summary>
        private bool isSelectedField;

        /// <summary>
        /// The child items belonging to this node.
        /// </summary>
        private ObservableCollection<NavigationTreeItemViewModel> itemsField = new ObservableCollection<NavigationTreeItemViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTreeItemViewModel"/> class.
        /// </summary>
        public NavigationTreeItemViewModel()
        {
            // This is a specialized menu item view model that controls expanding and collapsing of the nodes.  It isn't automatically added to the
            // context menu of each node, but this property is available and can be used to create a pre-defined menu item to be included in the
            // context menu that allows for the menu node to be expanded and collapsed.
            this.expandMenuItemViewModel = new ExpandMenuItemViewModel();
            this.expandMenuItemViewModel.CommandParameter = this;

            // These property changed actions are handled by the base class.
            this.PropertyChangedActions["IsExpanded"] = () => this.expandMenuItemViewModel.IsExpanded = this.IsExpanded;

            // This handler will link and unlink the children to and from the parent.
            this.Items.CollectionChanged += this.OnCollectionChanged;

            // The view of child nodes is always sorted alphabetically in ascending order by name.
            this.listCollectionView = new ListCollectionView(this.itemsField);
            this.listCollectionView.SortDescriptions.Add(new SortDescription
            {
                PropertyName = "Name",
                Direction = SortDirection.Ascending
            });
        }

        /// <summary>
        /// Gets or sets the unique identifier of this object.
        /// </summary>
        public string Identifier { get; protected set; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public ObservableCollection<NavigationTreeItemViewModel> Items
        {
            get
            {
                return this.itemsField;
            }
        }

        /// <summary>
        /// Gets the view models for every item in the context menu.
        /// </summary>
        public virtual ObservableCollection<IDisposable> ContextMenu
        {
            get
            {
                this.contextMenuViewItems = this.CreateContextMenuItems();
                return this.contextMenuViewItems;
            }
        }

        /// <summary>
        /// Gets the view model for the menu item that allows for expansion and collapsing of a node.
        /// </summary>
        public ExpandMenuItemViewModel ExpandMenuItem
        {
            get
            {
                return this.expandMenuItemViewModel;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is expanded in a hierarchy.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpandedField;
            }

            set
            {
                // Notify subscribers when the property has changed.
                if (value != this.isExpandedField)
                {
                    this.isExpandedField = value;
                    this.OnPropertyChanged("IsExpanded");
                }
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
        /// Gets a value indicating whether this is the top of a hierarchy tree.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this.isRootField;
            }
        }

        /// <summary>
        /// Gets the parent treeView item.
        /// </summary>
        public NavigationTreeItemViewModel Parent { get; private set; }

        /// <summary>
        /// Gets or sets the root URI for a path to navigate to this object.
        /// </summary>
        public Uri RootUri { get; protected set; }

        /// <summary>
        /// Gets or sets the key used for sorting.
        /// </summary>
        public IComparable SortKey { get; set; }

        /// <summary>
        /// Gets the viewer used to display this object.
        /// </summary>
        public Uri Uri
        {
            get
            {
                // Construct a path by recursing up through the hierarchy.
                string path = this.Identifier;
                NavigationTreeItemViewModel parent = this.Parent;
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

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // Release the resources used by each of the context menu items.
            if (this.contextMenuViewItems != null)
            {
                foreach (IDisposable disposable in this.contextMenuViewItems)
                {
                    disposable.Dispose();
                }
            }

            // Dispose of the common menu item(s).
            this.expandMenuItemViewModel.Dispose();

            // Allow the base class to dispose.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a context menu for this item.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected virtual ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            return new ObservableCollection<IDisposable>();
        }

        /// <summary>
        /// Sets an indication that this is the root node.
        /// </summary>
        protected void SetIsRoot()
        {
            // This allows the root element to be formatted differently than the child items.
            this.isRootField = true;
            this.OnPropertyChanged("IsRoot");
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
                    foreach (NavigationTreeItemViewModel treeViewItemViewModel in this.Items)
                    {
                        treeViewItemViewModel.Parent = null;
                    }

                    break;

                case NotifyCollectionChangedAction.Add:

                    // Add each of the items as a child of this parent.
                    foreach (NavigationTreeItemViewModel treeViewItemViewModel in notifyCollectionChangedEventArgs.NewItems)
                    {
                        treeViewItemViewModel.Parent = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    // Remove each of the items as a child of this parent.
                    foreach (NavigationTreeItemViewModel treeViewItemViewModel in notifyCollectionChangedEventArgs.OldItems)
                    {
                        treeViewItemViewModel.Parent = null;
                    }

                    break;
            }
        }
    }
}