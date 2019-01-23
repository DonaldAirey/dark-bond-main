// <copyright file="FrameViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.LicenseManager.ViewModels.Directories;
    using DarkBond.Navigation;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Events;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// The view model for the frame of the application.
    /// </summary>
    public class FrameViewModel : FrameViewModelBase
    {
        /// <summary>
        /// The items in the breadcrumb bar.
        /// </summary>
        private BreadcrumbViewModelBase breadcrumbCollection;

        /// <summary>
        /// Maps the target path to a directory that can be use to view that object.
        /// </summary>
        private Dictionary<DirectoryTypes, Func<DirectoryViewModel>> directoryMap;

        /// <summary>
        /// A value indicating whether the details pane is visible.
        /// </summary>
        private bool isDetailsPaneVisibleField;

        /// <summary>
        /// A value indicating whether the navigation pane is visible.
        /// </summary>
        private bool isNavigationPaneVisibleField;

        /// <summary>
        /// The items in the navigation panel.
        /// </summary>
        private NavigationTreeViewModelBase navigationTreeViewModelBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameViewModel"/> class.
        /// </summary>
        /// <param name="breadcrumbCollection">The collection of breadcrumbs defining the application hierarchy.</param>
        /// <param name="navigationTreeViewModel">The application hierarchy.</param>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="navigationService">The navigation service.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed.")]
        public FrameViewModel(
            BreadcrumbViewModel breadcrumbCollection,
            NavigationTreeViewModel navigationTreeViewModel,
            CompositionContext compositionContext,
            IEventAggregator eventAggregator,
            INavigationService navigationService)
            : base(compositionContext, eventAggregator, navigationService)
        {
            // Initialize the object.
            this.breadcrumbCollection = breadcrumbCollection;
            this.navigationTreeViewModelBase = navigationTreeViewModel;

            // Each breadcrumb has a type associated with it.  When a breadcrumb is selected, the type is used to create a view model for objects of
            // that type.  This maps the type to the function that creates the view model.
            this.directoryMap = new Dictionary<DirectoryTypes, Func<DirectoryViewModel>>
            {
                { DirectoryTypes.Application, () => this.CompositionContext.GetExport<ApplicationFolderViewModel>() },
                { DirectoryTypes.CustomerFolder, () => this.CompositionContext.GetExport<CustomerFolderViewModel>() },
                { DirectoryTypes.Customer, () => this.CompositionContext.GetExport<CustomerViewModel>() },
                { DirectoryTypes.ProductFolder, () => this.CompositionContext.GetExport<ProductFolderViewModel>() },
                { DirectoryTypes.Product, () => this.CompositionContext.GetExport<ProductViewModel>() }
            };

            // Initialize the metadata.
            this.Status = new TextViewModel()
            {
                Text = Resources.FilesUpToDate
            };

            // The navigation and details pane are initially visible.
            this.IsDetailsPaneVisible = true;
            this.IsNavigationPaneVisible = true;

            // These property change events are handled by this view model.
            this.PropertyChangedActions["IsNavigationPaneVisible"] = () => GlobalCommands.ToggleNavigationPane.Execute(null);
            this.PropertyChangedActions["IsDetailsPaneVisible"] = () => GlobalCommands.ToggleDetailsPane.Execute(null);

            // This is the initial view.
            this.View = "ColumnsView";
        }

        /// <summary>
        /// These are the different directory types map to the paths provided in the URL.
        /// </summary>
        private enum DirectoryTypes
        {
            NotValid, Application, CustomerFolder, Customer, ProductFolder, Product
        }

        /// <summary>
        /// Gets the collection of breadcrumbs.
        /// </summary>
        public BreadcrumbViewModelBase BreadcrumbViewModel
        {
            get
            {
                return this.breadcrumbCollection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the details pane is visible.
        /// </summary>
        public bool IsDetailsPaneVisible
        {
            get
            {
                return this.isDetailsPaneVisibleField;
            }

            set
            {
                if (this.isDetailsPaneVisibleField != value)
                {
                    this.isDetailsPaneVisibleField = value;
                    this.OnPropertyChanged("IsDetailsPaneVisible");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation pane is visible.
        /// </summary>
        public bool IsNavigationPaneVisible
        {
            get
            {
                return this.isNavigationPaneVisibleField;
            }

            set
            {
                if (this.isNavigationPaneVisibleField != value)
                {
                    this.isNavigationPaneVisibleField = value;
                    this.OnPropertyChanged("IsNavigationPaneVisible");
                }
            }
        }

        /// <summary>
        /// Gets the collection of breadcrumbs.
        /// </summary>
        public NavigationTreeViewModelBase NavigationTreeViewModel
        {
            get
            {
                return this.navigationTreeViewModelBase;
            }
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Validate the navigationContext argument.
            if (navigationContext == null)
            {
                throw new ArgumentNullException(nameof(navigationContext));
            }

            // Extract the path from the URI.
            this.Path = navigationContext.Parameters["path"] ?? string.Empty;
            string[] elements = this.Path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            DirectoryTypes targetClass = DirectoryTypes.NotValid;

            // This will parse the URI and determine what class of directory to use to present the data.
            if (elements.Length >= 1)
            {
                if (elements[0] == Resources.ApplicationName)
                {
                    if (elements.Length == 1)
                    {
                        targetClass = DirectoryTypes.Application;
                    }
                    else
                    {
                        if (elements[1] == Resources.Customer)
                        {
                            targetClass = elements.Length == 2 ? DirectoryTypes.CustomerFolder : DirectoryTypes.Customer;
                        }

                        if (elements[1] == Resources.Product)
                        {
                            targetClass = elements.Length == 2 ? DirectoryTypes.ProductFolder : DirectoryTypes.Product;
                        }
                    }
                }
            }

            // If a URI doesn't resolve to a directory class, then we can't navigate to it.
            if (targetClass == DirectoryTypes.NotValid)
            {
                throw new InvalidOperationException("Unable to navigate to " + navigationContext.Uri);
            }

            DirectoryViewModel directoryViewModel = this.directoryMap[targetClass]();
            directoryViewModel.View = this.View;
            directoryViewModel.Load(this.Path);
            this.Directory = directoryViewModel;

            // Allow the base class to handle the rest of the function.
            base.OnNavigatedTo(navigationContext);
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Unload the managed resource allocated by the directory.
            if (this.Directory != null)
            {
                this.Directory.Unload();
            }

            // Allow the base class to handle the rest of the function.
            base.OnNavigatedFrom(navigationContext);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Remove any resources allocated for the status field.
            IDisposable disposable = this.Status as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            // Allow the base class to complete the clean-up.
            base.Dispose(disposing);
        }
    }
}