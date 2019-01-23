// <copyright file="FrameViewModelBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Composition;
    using Events;
    using Input;
    using Navigation;

    /// <summary>
    /// The base view model for the frame of the application.
    /// </summary>
    public abstract class FrameViewModelBase : ViewModel, INavigationAware
    {
        /// <summary>
        /// The buttons on the frame's title line.
        /// </summary>
        private IList buttonsField;

        /// <summary>
        /// The composition context.
        /// </summary>
        private CompositionContext compositionContext;

        /// <summary>
        /// The collection of contextual buttons on the left side of the AppBar.
        /// </summary>
        private ObservableCollection<IDisposable> contextButtonsField = new ObservableCollection<IDisposable>();

        /// <summary>
        /// The detail for the detail pane.
        /// </summary>
        private ViewModel detail;

        /// <summary>
        /// View model for the directory view.
        /// </summary>
        private DirectoryViewModel directoryViewModel;

        /// <summary>
        /// The event aggregator.
        /// </summary>
        private IEventAggregator eventAggregatorField;

        /// <summary>
        /// The collection of directory-specific buttons on the right side of the AppBar.
        /// </summary>
        private ObservableCollection<IDisposable> globalButtonsField = new ObservableCollection<IDisposable>();

        /// <summary>
        /// The command to navigate backwards.
        /// </summary>
        private DelegateCommand goBackField;

        /// <summary>
        /// The command to navigate forward.
        /// </summary>
        private DelegateCommand goForwardField;

        /// <summary>
        /// An indication of whether the bottom AppBar is open.
        /// </summary>
        private bool isAppBarOpenField;

        /// <summary>
        /// The navigation service.
        /// </summary>
        private INavigationService navigationServiceField;

        /// <summary>
        /// The current source for this module.
        /// </summary>
        private string pathField;

        /// <summary>
        /// General purpose status metadata field.
        /// </summary>
        private object statusField;

        /// <summary>
        /// The view model for the view button.
        /// </summary>
        private ViewButtonViewModel viewButtonViewModel;

        /// <summary>
        /// The current view.
        /// </summary>
        private string viewField;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameViewModelBase"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="navigationService">The navigation service.</param>
        protected FrameViewModelBase(CompositionContext compositionContext, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            // Validate the arguments.
            if (navigationService == null)
            {
                throw new ArgumentNullException(nameof(navigationService));
            }

            // Initialize the object.
            this.compositionContext = compositionContext;
            this.eventAggregatorField = eventAggregator;
            this.navigationServiceField = navigationService;

            // This will reconcile the forward and backward buttons to the current state of the journal.
            this.navigationServiceField.Navigated += this.OnNavigated;

            // These commands are handled by this object.
            GlobalCommands.OpenAppBar.RegisterCommand(new DelegateCommand<bool>((isOpen) => this.IsAppBarOpen = isOpen));
            GlobalCommands.SetContextAppBar.RegisterCommand(new DelegateCommand<ObservableCollection<IDisposable>>(this.OnSetContextAppBar));
            GlobalCommands.SetGlobalAppBar.RegisterCommand(new DelegateCommand<ObservableCollection<IDisposable>>(this.OnSetGlobalAppBar));
            GlobalCommands.Select.RegisterCommand(new DelegateCommand<ViewModel>(this.OnSetSelection));
            GlobalCommands.ChangeView.RegisterCommand(new DelegateCommand<string>((v) => this.View = v));
            this.goBackField = new DelegateCommand(() => this.navigationServiceField.GoBack(), () => this.navigationServiceField.CanGoBack);
            this.goForwardField = new DelegateCommand(() => this.navigationServiceField.GoForward(), () => this.navigationServiceField.CanGoForward);

            // This button switches between the available views.
            this.viewButtonViewModel = this.compositionContext.GetExport<ViewButtonViewModel>();

            // These property change events are handled by this view model.
            this.PropertyChangedActions["View"] = this.OnViewChanged;
        }

        /// <summary>
        /// Gets the buttons that appear in the upper right-hand portion of the title bar.
        /// </summary>
        public IList Buttons
        {
            get
            {
                if (this.buttonsField == null)
                {
                    this.buttonsField = this.CreateButtons();
                }

                return this.buttonsField;
            }
        }

        /// <summary>
        /// Gets the composition context.
        /// </summary>
        public CompositionContext CompositionContext
        {
            get
            {
                return this.compositionContext;
            }
        }

        /// <summary>
        /// Gets the context buttons in the AppBar.
        /// </summary>
        public IList ContextButtons
        {
            get
            {
                return this.contextButtonsField;
            }
        }

        /// <summary>
        /// Gets or sets the view model for the details pane.
        /// </summary>
        public ViewModel SelectedItem
        {
            get
            {
               return this.detail;
            }

            set
            {
                if (this.detail != value)
                {
                    this.detail = value;
                    this.OnPropertyChanged("SelectedItem");
                }
            }
        }

        /// <summary>
        /// Gets or sets the view model for the directory view.
        /// </summary>
        public DirectoryViewModel Directory
        {
            get
            {
                return this.directoryViewModel;
            }

            set
            {
                if (this.directoryViewModel != value)
                {
                    this.directoryViewModel = value;
                    this.OnPropertyChanged("Directory");
                }
            }
        }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        public IEventAggregator EventAggregator
        {
            get
            {
                return this.eventAggregatorField;
            }
        }

        /// <summary>
        /// Gets the global buttons in the AppBar.
        /// </summary>
        public IList GlobalButtons
        {
            get
            {
                return this.globalButtonsField;
            }
        }

        /// <summary>
        /// Gets the command to navigate backwards.
        /// </summary>
        public DelegateCommand GoBack
        {
            get
            {
                return this.goBackField;
            }
        }

        /// <summary>
        /// Gets the command to navigate backwards.
        /// </summary>
        public DelegateCommand GoForward
        {
            get
            {
                return this.goForwardField;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AppBar is open.
        /// </summary>
        public bool IsAppBarOpen
        {
            get
            {
                return this.isAppBarOpenField;
            }

            set
            {
                if (this.isAppBarOpenField != value)
                {
                    this.isAppBarOpenField = value;
                    this.OnPropertyChanged("IsAppBarOpen");
                }
            }
        }

        /// <summary>
        /// Gets or sets the URI source for the application.
        /// </summary>
        public string Path
        {
            get
            {
                return this.pathField;
            }

            set
            {
                if (this.pathField != value)
                {
                    this.pathField = value;
                    this.OnPropertyChanged("Path");
                }
            }
        }

        /// <summary>
        /// Gets or sets the status metadata.
        /// </summary>
        public object Status
        {
            get
            {
                return this.statusField;
            }

            set
            {
                if (this.statusField != value)
                {
                    this.statusField = value;
                    this.OnPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the current view.
        /// </summary>
        public string View
        {
            get
            {
                return this.viewField;
            }

            set
            {
                if (this.viewField != value)
                {
                    this.viewField = value;
                    this.OnPropertyChanged("View");
                }
            }
        }

        /// <summary>
        /// Gets the button used to switch between views.
        /// </summary>
        public ViewButtonViewModel ViewButton
        {
            get
            {
                return this.viewButtonViewModel;
            }
        }

        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        protected INavigationService NavigationService
        {
            get
            {
                return this.navigationServiceField;
            }
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Validate the navigationContext argument.
            if (navigationContext == null)
            {
                throw new ArgumentNullException(nameof(navigationContext));
            }

            // When we've navigated to this page, make sure the button to navigate backwards reflects the proper state.
            this.GoBack.RaiseCanExecuteChanged();
            this.GoForward.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Called when the implementer has been navigated from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Close the AppBar whenever we navigate away from a page.
            this.IsAppBarOpen = false;
        }

        /// <summary>
        /// Create the frame buttons.
        /// </summary>
        /// <returns>A collection of buttons that appear in the frame.</returns>
        protected virtual IList CreateButtons()
        {
            // The general idea is that subclasses can add their own buttons by overriding this method.
            ObservableCollection<IDisposable> buttons = new ObservableCollection<IDisposable>();
            buttons.Add(this.viewButtonViewModel);
            return buttons;
        }

        /// <summary>
        /// Handles a change to the View property.
        /// </summary>
        private void OnViewChanged()
        {
            // Setting the view of the Directory view model will cause the actual layout of the records to change.
            if (this.Directory != null)
            {
                this.Directory.View = this.View;
            }

            // Setting the state of the view button will change the image and the commands on the button that changes the view.
            if (this.viewButtonViewModel != null)
            {
                this.viewButtonViewModel.State = this.View;
            }
        }

        /// <summary>
        /// Reconciles the <see cref="BreadcrumbViewModelBase"/> with the current URI.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="navigationEventArgs">The navigation event data.</param>
        private void OnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            // Make sure that the navigation buttons reflect the proper state.
            this.GoBack.RaiseCanExecuteChanged();
            this.GoForward.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Sets the details - name of the selected item, the metadata, the image, etc. - of the frame.
        /// </summary>
        /// <param name="viewModelBase">The details of the selected item.</param>
        private void OnSetSelection(ViewModel viewModelBase)
        {
            // Update the detail pane view model with the selected item.
            this.SelectedItem = viewModelBase;
        }

        /// <summary>
        /// Sets the buttons in the Context part of the AppBar.
        /// </summary>
        /// <param name="contextButtons">A collection of controls to appear in the Context part of the AppBar.</param>
        private void OnSetContextAppBar(ObservableCollection<IDisposable> contextButtons)
        {
            this.ContextButtons.Clear();
            foreach (object item in contextButtons)
            {
                this.ContextButtons.Add(item);
            }
        }

        /// <summary>
        /// Sets the buttons in the Global part of the AppBar.
        /// </summary>
        /// <param name="contextButtons">A collection of controls to appear in the Global part of the AppBar.</param>
        private void OnSetGlobalAppBar(ObservableCollection<IDisposable> contextButtons)
        {
            this.GlobalButtons.Clear();
            foreach (object item in contextButtons)
            {
                this.GlobalButtons.Add(item);
            }
        }
    }
}