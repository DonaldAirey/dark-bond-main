// <copyright file="NavigationTreeViewModelBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Navigation;

    /// <summary>
    /// Used to define the navigational hierarchy of an application.
    /// </summary>
    public class NavigationTreeViewModelBase
    {
        /// <summary>
        /// The navigation service.
        /// </summary>
        private INavigationService navigationService;

        /// <summary>
        /// The list of navigation tree items.
        /// </summary>
        private ObservableCollection<NavigationTreeItemViewModel> navigationTreeViewItems = new ObservableCollection<NavigationTreeItemViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTreeViewModelBase"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public NavigationTreeViewModelBase(INavigationService navigationService)
        {
            // Validate the 'navigationService' argument.
            if (navigationService == null)
            {
                throw new ArgumentNullException(nameof(navigationService));
            }

            // Initialize the object
            this.navigationService = navigationService;

            // This is used to reconcile the tree view to the currently viewed URI.
            this.navigationService.Navigated += this.OnNavigated;
        }

        /// <summary>
        /// Gets the list of navigation tree items at the root.
        /// </summary>
        public ObservableCollection<NavigationTreeItemViewModel> Items
        {
            get
            {
                return this.navigationTreeViewItems;
            }
        }

        /// <summary>
        /// Reconciles the <see cref="NavigationTreeViewModelBase"/> with the current URI.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="navigationEventArgs">The navigation event data.</param>
        private void OnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            // This will take the source URI, extract the path and break the path up into the different hierarchy levels.
            UriQueryCollection uriQueryCollection = new UriQueryCollection(navigationEventArgs.Uri.Query);
            string path = uriQueryCollection["path"];
            if (path != null)
            {
                // This will split the path into the components that we'll use to construct the breadcrumb list.  Note that paths are absolute and
                // the opening path separator character is removed because the root is an implied directory level.
                string[] elements = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                // This will cycle through all the levels in the current path constructing tree view for each level.  Note that it's possible to
                // provide a path that doesn't exist in the hierarchy (temporary directories, dialog boxes, etc.).  The strategy here is to display
                // it if you can but don't cry about it if you can't.
                for (int level = 0; level < elements.Length; level++)
                {
                    // The general idea here is that when the treeview elements match the path elements, leave them in their place.  When we do find
                    // a breadcrumb item that doesn't match the current path, we'll remove the mismatched breadcrumb and install a new breadcrumb
                    // that matches the given path.
                    if (this.Items.Count - 1 < level || !this.Items[level].Identifier.Equals(elements[level]))
                    {
                    }
                }
            }
        }
    }
}