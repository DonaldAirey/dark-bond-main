// <copyright file="BreadcrumbViewModelBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using DarkBond.ViewModels.Input;
    using Navigation;

    /// <summary>
    /// Used to define the navigational hierarchy of an application.
    /// </summary>
    public class BreadcrumbViewModelBase
    {
        /// <summary>
        /// The command to navigate backwards.
        /// </summary>
        private DelegateCommand goUpField;

        /// <summary>
        /// The navigation service.
        /// </summary>
        private INavigationService navigationService;

        /// <summary>
        /// The breadcrumb items.
        /// </summary>
        private ObservableCollection<BreadcrumbItemViewModel> breadcrumbItems = new ObservableCollection<BreadcrumbItemViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadcrumbViewModelBase"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public BreadcrumbViewModelBase(INavigationService navigationService)
        {
            // Validate the 'navigationService' argument.
            if (navigationService == null)
            {
                throw new ArgumentNullException(nameof(navigationService));
            }

            // Initialize the object
            this.navigationService = navigationService;

            // This is used to reconcile the breadcrumbs to the currently viewed URI.
            this.navigationService.Navigated += this.OnNavigated;

            // These commands are handled by this object.
            this.goUpField = new DelegateCommand(this.NavigateUp, this.CanNavigateUp);
        }

        /// <summary>
        /// Gets the command to navigate to the parent.
        /// </summary>
        public DelegateCommand GoUp
        {
            get
            {
                return this.goUpField;
            }
        }

        /// <summary>
        /// Gets the list of breadcrumb items.
        /// </summary>
        public ObservableCollection<BreadcrumbItemViewModel> Items
        {
            get
            {
                return this.breadcrumbItems;
            }
        }

        /// <summary>
        /// Gets an indication of whether navigation to the previous page is enabled.
        /// </summary>
        /// <returns>true if navigation backwards is allowed, false otherwise.</returns>
        private bool CanNavigateUp()
        {
            // If the current breadcrumb has a parent, then we can navigate to it.
            return this.breadcrumbItems.Count > 1;
        }

        /// <summary>
        /// Navigate to the previous page.
        /// </summary>
        private void NavigateUp()
        {
            // Get the parent item of the current breadcrumb and navigate to it.
            BreadcrumbItemViewModel parentViewModel = this.breadcrumbItems[this.breadcrumbItems.Count - 2];
            GlobalCommands.Locate.Execute(parentViewModel.Uri);

            // Make sure that the navigation buttons reflect the proper state.
            this.GoUp.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Reconciles the <see cref="BreadcrumbViewModelBase"/> with the current URI.
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

                // This will cycle through all the levels in the current path constructing breadcrumbs for each level.  Note that it's possible to
                // provide a path that doesn't exist in the hierarchy (temporary directories, dialog boxes, etc.).  The strategy here is to display
                // it if you can but don't cry about it if you can't.
                for (int level = 0; level < elements.Length; level++)
                {
                    // The general idea here is that when the breadcrumb elements match the path elements, leave them in their place.  When we do
                    // find a breadcrumb item that doesn't match the current path, we'll remove the mismatched breadcrumb and install a new
                    // breadcrumb that matches the given path.
                    if (this.breadcrumbItems.Count - 1 < level || !this.breadcrumbItems[level].Identifier.Equals(elements[level]))
                    {
                        // At this point we've found a breadcrumb that isn't a match for the current level of the path.  That means that none of this
                        // breadcrumb's children can be matches either, so we'll remove them.
                        while (this.breadcrumbItems.Count > level)
                        {
                            this.breadcrumbItems.RemoveAt(this.breadcrumbItems.Count - 1);
                        }

                        // Ask the parent breadcrumb which of it's children is a match for the current level of the path.  That item becomes the next
                        // breadcrumb in the list.
                        BreadcrumbItemViewModel previousBreadcrumb = this.breadcrumbItems[level - 1];
                        BreadcrumbItemViewModel childBreadcrumb = previousBreadcrumb.FindChild(elements[level]);
                        if (childBreadcrumb == null)
                        {
                            throw new InvalidOperationException("Unable to navigate to " + navigationEventArgs.Uri);
                        }

                        this.breadcrumbItems.Add(childBreadcrumb);
                    }
                }

                // If the old path had more elements than we matched in our current path, then they need to be removed from the breadcrumb bar.
                while (this.breadcrumbItems.Count > elements.Length)
                {
                    this.breadcrumbItems.RemoveAt(this.breadcrumbItems.Count - 1);
                }
            }

            // After adding or removing, make sure that the navigation buttons reflect the proper state.
            this.GoUp.RaiseCanExecuteChanged();
        }
    }
}