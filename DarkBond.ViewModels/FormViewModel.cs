// <copyright file="FormViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using DarkBond.Navigation;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Base class for view models behind validating forms that are displayed as pages.
    /// </summary>
    public class FormViewModel : ValidatingViewModelBase, INavigationAware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        protected FormViewModel(INavigationService navigationService)
        {
            // Initialize the object.
            this.NavigationService = navigationService;
            this.GoBack = new DelegateCommand(() => this.NavigationService.GoBack(), () => { return this.NavigationService.CanGoBack; });
            this.GoForward = new DelegateCommand(() => this.NavigationService.GoForward(), () => { return this.NavigationService.CanGoForward; });
        }

        /// <summary>
        /// Gets the command to navigate backwards.
        /// </summary>
        public DelegateCommand GoBack { get; private set; }

        /// <summary>
        /// Gets the command to navigate forward.
        /// </summary>
        public DelegateCommand GoForward { get; private set; }

        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        protected INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Occurs when the navigation is away from this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}