// <copyright file="NavigationService.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Composition;
    using System.Composition.Hosting.Core;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using DarkBond.Navigation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// Regular expression used to pull apart the URI into the assembly and type.
        /// </summary>
        private static Regex assemblyRegEx = new Regex("/(?<assembly>[^;]*);/(?<type>.*)");

        /// <summary>
        /// The currently active view.
        /// </summary>
        private object activeView;

        /// <summary>
        /// The composition context.
        /// </summary>
        private CompositionContext compositionContext;

        /// <summary>
        /// The current navigation context.
        /// </summary>
        private NavigationContext currentNavigationContext;

        /// <summary>
        /// The navigation journal.
        /// </summary>
        private NavigationJournal navigationJournal = new NavigationJournal();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        public NavigationService(CompositionContext compositionContext)
        {
            // Initialize the object.
            this.compositionContext = compositionContext;
        }

        /// <summary>
        /// Raised when the active view has changed.
        /// </summary>
        public event EventHandler<ActiveViewChangedEventArgs> ActiveViewChanged;

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in the back navigation history.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return this.navigationJournal.CanGoBack;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in the forward navigation history.
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                return this.navigationJournal.CanGoForward;
            }
        }

        /// <summary>
        /// Resets the navigation service.
        /// </summary>
        public void Clear()
        {
            // Clear out the journal.
            this.navigationJournal.Clear();
        }

        /// <summary>
        /// Navigates to the previous entry in the journal.
        /// </summary>
        public void GoBack()
        {
            // Attempt to navigate backwards.
            if (this.NavigateCore(this.navigationJournal.BackEntry))
            {
                // If there's no problem, then synchroize the stack with the new position.
                this.navigationJournal.MoveBack();

                // This is generally used to allow the UI to reconcile itself to the stack location (forward, backward buttons, etc.)
                this.Navigated?.Invoke(this, new NavigationEventArgs(this.currentNavigationContext));
            }
        }

        /// <summary>
        /// Navigates to the given URI.
        /// </summary>
        /// <param name="target">A URI specifying the new page.</param>
        public void Navigate(Uri target)
        {
            // Call the core method to attempt the navigation.
            if (this.NavigateCore(target))
            {
                // Update the navigation journal before notifying others of navigation.
                this.navigationJournal.MoveTo(this.currentNavigationContext.Uri);

                // This is generally used to allow the UI to reconcile itself to the stack location (forward, backward buttons, etc.)
                this.Navigated?.Invoke(this, new NavigationEventArgs(this.currentNavigationContext));
            }
        }

        /// <summary>
        /// Navigates to the next entry in the journal.
        /// </summary>
        public void GoForward()
        {
            // Attempt to navigate forward.
            if (this.NavigateCore(this.navigationJournal.ForwardEntry))
            {
                // If there's no problem, then synchroize the stack with the new position.
                this.navigationJournal.MoveForward();

                // This is generally used to allow the UI to reconcile itself to the stack location (forward, backward buttons, etc.)
                this.Navigated?.Invoke(this, new NavigationEventArgs(this.currentNavigationContext));
            }
        }

        /// <summary>
        /// Invokes an action on an ActivationAware view.
        /// </summary>
        /// <param name="item">The view.</param>
        /// <param name="invocation">The action to be invoked on the view.</param>
        private static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
        {
            // See if the view itself is aware of activation events.
            INavigationAware navigationAware = item as INavigationAware;
            if (navigationAware != null)
            {
                invocation(navigationAware);
            }

            // Alternatively, the view model associated with the item can be aware of navigation events.
            FrameworkElement frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                navigationAware = frameworkElement.DataContext as INavigationAware;
                if (navigationAware != null)
                {
                    invocation(navigationAware);
                }
            }
        }

        /// <summary>
        /// Navigates to the given URI.
        /// </summary>
        /// <param name="target">A URI specifying the new page.</param>
        /// <returns>true to indicate that the navigation was successful, false otherwise.</returns>
        private bool NavigateCore(Uri target)
        {
            // Validate the argument
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Decompose the URI into the important parts.
            Match match = NavigationService.assemblyRegEx.Match(target.LocalPath);
            string uriAssemblyName = match.Groups["assembly"].Value;
            string uriTypeName = match.Groups["type"].Value;

            // This is the context for navigating.
            this.currentNavigationContext = new NavigationContext(this, target);

            // Used to detect if the navigation was successful.
            bool result = false;

            try
            {
                // Load the assembly and then load the type specified in the URI.
                Assembly assembly = Assembly.Load(new AssemblyName(uriAssemblyName));
                Type type = assembly.GetType(uriTypeName);

                // If an active view exists, inform it we're about to navigate way from it.
                if (this.activeView != null)
                {
                    NavigationService.InvokeOnNavigationAwareElement(this.activeView, (n) => n.OnNavigatedFrom(this.currentNavigationContext));
                }

                // If the type was loaded successfully then we'll compose an instance of it using MEF and make it the content window of the frame,
                // thus navigation is complete.
                this.activeView = this.compositionContext.GetExport(new CompositionContract(type));
                this.ActiveViewChanged?.Invoke(this, new ActiveViewChangedEventArgs(this.activeView, target));

                // The view can be informed of navigation
                NavigationService.InvokeOnNavigationAwareElement(this.activeView, (n) => n.OnNavigatedTo(this.currentNavigationContext));

                // At this point we successfully navigated to the URI.
                result = true;
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                this.NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(this.currentNavigationContext, fileNotFoundException));
            }
            catch (ArgumentNullException argumentNullException)
            {
                this.NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(this.currentNavigationContext, argumentNullException));
            }

            // This tell the caller if the navigation was successful.
            return result;
        }
    }
}