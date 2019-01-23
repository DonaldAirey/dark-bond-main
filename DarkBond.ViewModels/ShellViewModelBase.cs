// <copyright file="ShellViewModelBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using Input;
    using Navigation;

    /// <summary>
    /// The view model for the shell.
    /// </summary>
    public class ShellViewModelBase : ViewModel
    {
        /// <summary>
        /// The content displayed in the shell.
        /// </summary>
        private object contentField;

        /// <summary>
        /// The navigation service.
        /// </summary>
        private INavigationService navigationService;

        /// <summary>
        /// The local source URI.
        /// </summary>
        private Uri sourceField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModelBase"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public ShellViewModelBase(INavigationService navigationService)
        {
            // Validate navigationService the argument.
            if (navigationService == null)
            {
                throw new ArgumentNullException(nameof(navigationService));
            }

            // Initialize the object.
            this.navigationService = navigationService;
            this.navigationService.ActiveViewChanged += this.OnActiveViewChanged;

            // These property events are handled by this view model.
            this.PropertyChangedActions["Source"] = this.OnSourceChanged;

            // These commands are handled by this view model.
            GlobalCommands.Locate.RegisterCommand(new DelegateCommand<Uri>((uri) => this.Source = uri));
        }

        /// <summary>
        /// Gets or sets the content of the shell (that is, the current view).
        /// </summary>
        public object Content
        {
            get
            {
                return this.contentField;
            }

            protected set
            {
                if (this.contentField != value)
                {
                    this.contentField = value;
                    this.OnPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// Gets or sets the source URI for the application.
        /// </summary>
        public Uri Source
        {
            get
            {
                return this.sourceField;
            }

            set
            {
                if (this.sourceField != value)
                {
                    this.sourceField = value;
                    this.OnPropertyChanged("Source");
                }
            }
        }

        /// <summary>
        /// Clears the navigation journal.
        /// </summary>
        public void ClearJournal()
        {
            this.navigationService.Clear();
        }

        /// <summary>
        /// Raised when the active view has changed as a result of a navigation event.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="activeViewChangedEventArgs">The event data.</param>
        protected virtual void OnActiveViewChanged(object sender, ActiveViewChangedEventArgs activeViewChangedEventArgs)
        {
            // Validate the activeViewChangedEventArgs argument
            if (activeViewChangedEventArgs == null)
            {
                throw new ArgumentNullException(nameof(activeViewChangedEventArgs));
            }

            // The content for the window is provided by the navigation service.
            this.Content = activeViewChangedEventArgs.ActiveView;

            // When a new source becomes the active view, we need to make sure that the shell is aware of the current source.  Note that we bypass
            // the property because that would cause a new navigation request when we just got through handling one.  This handles the changes that
            // come from using the navigation journal.
            this.sourceField = activeViewChangedEventArgs.Source;
        }

        /// <summary>
        /// Handles a change to the source field.
        /// </summary>
        private void OnSourceChanged()
        {
            // Changing the source field is the same as a request to navigate to the given URI.
            this.navigationService.Navigate(this.Source);
        }
    }
}