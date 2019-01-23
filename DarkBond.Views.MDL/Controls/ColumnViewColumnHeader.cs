// <copyright file="ColumnViewColumnHeader.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// The view for a column header in the <see cref="ColumnViewDefinition"/>.
    /// </summary>
    public class ColumnViewColumnHeader : ContentControl
    {
        /// <summary>
        /// The CompleteDrag DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CompleteDragProperty = DependencyProperty.Register(
            "CompleteDrag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The Drag DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DragProperty = DependencyProperty.Register(
            "Drag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The Filters DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(
            "Filters",
            typeof(IList),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The HasFilters DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HasFiltersProperty = DependencyProperty.Register(
            "HasFilters",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsAscending DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsAscendingProperty = DependencyProperty.Register(
            "IsAscending",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsDescending DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsDescendingProperty = DependencyProperty.Register(
            "IsDescending",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsFilterOpen DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsFilterOpenProperty = DependencyProperty.Register(
            "IsFilterOpen",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsPressed DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register(
            "IsPressed",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(false, ColumnViewColumnHeader.OnIsPressedPropertyChanged));

        /// <summary>
        /// The IsSelected DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(false, new PropertyChangedCallback(ColumnViewColumnHeader.OnIsSelectedPropertyChanged)));

        /// <summary>
        /// The Press DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty PressProperty = DependencyProperty.Register(
            "Press",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The StartDrag DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty StartDragProperty = DependencyProperty.Register(
            "StartDrag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The SetWidth DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SetWidthProperty = DependencyProperty.Register(
            "SetWidth",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The SortDirection DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(SortDirection),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(SortDirection.NotSorted, ColumnViewColumnHeader.OnSortDirectionPropertyChanged));

        /// <summary>
        /// The SortOrder DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(
            "SortOrder",
            typeof(int),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// If the mouse moves more than this, we'll consider the action a gesture.  Otherwise, it's a tapped event.
        /// </summary>
        private const double MouseMoveThreshold = 4.0;

        /// <summary>
        /// The command handler for filtering.
        /// </summary>
        private DelegateCommand<FilterDescription> filterCommand;

        /// <summary>
        /// Used to indication that a manipulation is active.
        /// </summary>
        private bool isManipulating;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewColumnHeader"/> class.
        /// </summary>
        public ColumnViewColumnHeader()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(ColumnViewColumnHeader);

            // This control will handle these commands.
            this.filterCommand = new DelegateCommand<FilterDescription>(this.Filter);
            GlobalCommands.Filter.RegisterCommand(this.filterCommand);

            // This will update the panel when any of the header elements change.
            this.SizeChanged += this.OnSizeChanged;
        }

        /// <summary>
        /// Gets or sets the command invoked when a column completes a drag operation.
        /// </summary>
        public ICommand CompleteDrag
        {
            get
            {
                return (ICommand)this.GetValue(ColumnViewColumnHeader.CompleteDragProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.CompleteDragProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command invoked when a column is dragged.
        /// </summary>
        public ICommand Drag
        {
            get
            {
                return (ICommand)this.GetValue(ColumnViewColumnHeader.DragProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.DragProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the filter for this column is visible or not.
        /// </summary>
        public IList Filters
        {
            get
            {
                return this.GetValue(ColumnViewColumnHeader.FiltersProperty) as IList;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column is sorting its values in ascending order.
        /// </summary>
        public bool HasFilters
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.HasFiltersProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column is sorting its values in ascending order.
        /// </summary>
        public bool IsAscending
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.IsAscendingProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column is sorting its values in descending order.
        /// </summary>
        public bool IsDescending
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.IsDescendingProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the filter for this column is visible or not.
        /// </summary>
        public bool IsFilterOpen
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.IsFilterOpenProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.IsFilterOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a ColumnViewColumnHeader is currently activated.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.IsSelectedProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a ColumnViewColumnHeader is currently activated.
        /// </summary>
        public bool IsPressed
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumnHeader.IsPressedProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.IsPressedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command invoked when a column header is pressed.
        /// </summary>
        public ICommand Press
        {
            get
            {
                return (ICommand)this.GetValue(ColumnViewColumnHeader.PressProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.PressProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command invoked when a column width has changed.
        /// </summary>
        public ICommand SetWidth
        {
            get
            {
                return this.GetValue(ColumnViewColumnHeader.SetWidthProperty) as ICommand;
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.SetWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        public SortDirection SortDirection
        {
            get
            {
                return (SortDirection)this.GetValue(ColumnViewColumnHeader.SortDirectionProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.SortDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return (int)this.GetValue(ColumnViewColumnHeader.SortOrderProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.SortOrderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command invoked when a column starts a drag operation.
        /// </summary>
        public ICommand StartDrag
        {
            get
            {
                return (ICommand)this.GetValue(ColumnViewColumnHeader.StartDragProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumnHeader.StartDragProperty, value);
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Binding in the Setter of Styles doesn't work in Windows Store applications, so we need to perform the bindings manually where when the
            // template is applied.  These are the equivalent of 'FindAncestor' binding operations and they appear to be unnecessary in WPF.  From
            // these ancestors, we're going to bind the properties of the column header in a way that's agnostic to WPF and Metro.
            ItemsView itemsView = VisualTreeExtensions.FindParent<ItemsView>(this);
            ColumnViewColumn columnViewColumn = this.DataContext as ColumnViewColumn;
            ContentPresenter contentPresenter = this.GetTemplateChild("ContentPresenter") as ContentPresenter;

            // Bind the ActualWidth property.
            Binding widthBinding = new Binding();
            widthBinding.Path = new PropertyPath("ActualWidth");
            widthBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.WidthProperty, widthBinding);

            // Bind the CompleteDrag command.
            this.CompleteDrag = columnViewColumn.CompleteDrag;

            // Bind the Drag command.
            this.Drag = columnViewColumn.Drag;

            // Bind the Filters property.
            Binding filtersBinding = new Binding();
            filtersBinding.Path = new PropertyPath("Filters");
            filtersBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.FiltersProperty, filtersBinding);

            // Bind the HasFilters property.
            Binding hasFiltersBinding = new Binding();
            hasFiltersBinding.Path = new PropertyPath("HasFilters");
            hasFiltersBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.HasFiltersProperty, hasFiltersBinding);

            // Bind the Left property.
            Binding leftBinding = new Binding();
            leftBinding.Path = new PropertyPath("Left");
            leftBinding.Source = columnViewColumn;
            this.SetBinding(Canvas.LeftProperty, leftBinding);

            // Bind the MaxWidth property.
            Binding maxWidthBinding = new Binding();
            maxWidthBinding.Path = new PropertyPath("MaxWidth");
            maxWidthBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.MaxWidthProperty, maxWidthBinding);

            // Bind the MinWidth property.
            Binding minWidthBinding = new Binding();
            minWidthBinding.Path = new PropertyPath("MinWidth");
            minWidthBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.MinWidthProperty, minWidthBinding);

            // Bind the HeaderPadding property.
            Binding headerPaddingBinding = new Binding();
            headerPaddingBinding.Path = new PropertyPath("HeaderPadding");
            headerPaddingBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.PaddingProperty, headerPaddingBinding);

            // Bind the Press command.
            this.Press = columnViewColumn.Press;

            // Bind the SetWidth command.
            this.SetWidth = columnViewColumn.SetWidth;

            // Bind the SortDirection property.
            Binding sortDirectionBinding = new Binding();
            sortDirectionBinding.Path = new PropertyPath("SortDirection");
            sortDirectionBinding.Source = columnViewColumn;
            this.SetBinding(ColumnViewColumnHeader.SortDirectionProperty, sortDirectionBinding);

            // Bind the StartDrag command.
            this.StartDrag = columnViewColumn.StartDrag;

            // Bind the ZIndex property.
            Binding zIndexBinding = new Binding();
            zIndexBinding.Path = new PropertyPath("ZIndex");
            zIndexBinding.Source = columnViewColumn;
            this.SetBinding(Canvas.ZIndexProperty, zIndexBinding);

            // Bind the FontSize property.
            Binding fontSizeBinding = new Binding();
            fontSizeBinding.Path = new PropertyPath("FontSize");
            fontSizeBinding.Source = itemsView;
            contentPresenter.SetBinding(ContentPresenter.FontSizeProperty, fontSizeBinding);

            // Bind the Header property.
            Binding headerBinding = new Binding();
            headerBinding.Path = new PropertyPath("Header");
            headerBinding.Source = columnViewColumn;
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, headerBinding);

            // Bind the HorizontalHeaderAlignment property.
            Binding horizontalHeaderAlignmentBinding = new Binding();
            horizontalHeaderAlignmentBinding.Path = new PropertyPath("HorizontalHeaderAlignment");
            horizontalHeaderAlignmentBinding.Source = columnViewColumn;
            contentPresenter.SetBinding(ContentPresenter.HorizontalAlignmentProperty, horizontalHeaderAlignmentBinding);

            // Bind the VerticalHeaderAlignment property.
            Binding verticalHeaderAlignmentBinding = new Binding();
            verticalHeaderAlignmentBinding.Path = new PropertyPath("VerticalHeaderAlignment");
            verticalHeaderAlignmentBinding.Source = columnViewColumn;
            contentPresenter.SetBinding(ContentPresenter.VerticalAlignmentProperty, verticalHeaderAlignmentBinding);

            // The visual states can only be set when there is a template to recognize the states.
            VisualStateManager.GoToState(this, this.SortDirection.ToString(), true);

            // Allow the base class to handle the rest of the event.
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Called before the ManipulationCompleted event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            // This will allow the pointer events to update the visual state again.
            this.isManipulating = false;

            // If someone can handle the start column drag command then invoke it.
            if (this.CompleteDrag != null)
            {
                this.CompleteDrag.Execute(null);
            }

            // Set the visual state to indicate that nothing should be highlighted.
            VisualStateManager.GoToState(this, "Normal", true);

            // Allow the base class to handle the rest of the event.
            base.OnManipulationCompleted(e);
        }

        /// <summary>
        /// Called before the ManipulationDelta event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            // Validate the 'e' parameter
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // This makes the width grow and shrink with the pinching or zooming out gesture.
            if (this.SetWidth != null)
            {
                if (e.Delta.Scale != 1.0)
                {
                    // If the new width doesn't exceed any of the boundaries that we've set for this column, then change the width.
                    double width = this.Width * e.Delta.Scale;
                    if (this.MinWidth < width && width <= this.MaxWidth)
                    {
                        if (this.SetWidth.CanExecute(width))
                        {
                            this.SetWidth.Execute(width);
                        }
                    }
                }
            }

            // If someone can handle the start column drag command then invoke it.
            if (this.Drag != null)
            {
                this.Drag.Execute(new ColumnDragParameter(e.Delta.Translation.X, e.Position.X));
            }

            // Allow the base class to handle the rest of the event.
            base.OnManipulationDelta(e);
        }

        /// <summary>
        /// Called before the ManipulationStarted event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            // This will inhibit the pointer events from trying to override the visual state.
            this.isManipulating = true;

            // If someone can handle the start column drag command then invoke it.
            if (this.StartDrag != null)
            {
                this.StartDrag.Execute(null);
            }

            // Set the visual state to indicate the pointer is over the control.
            VisualStateManager.GoToState(this, "PointerOver", true);

            // Allow the base class to handle the rest of the event.
            base.OnManipulationStarted(e);
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            // Set the visual state to indicate the pointer is over the control (only when the column is not being manipulated).
            if (!this.isManipulating)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }

            // Allow the base class to handle the rest of the event.
            base.OnPointerEntered(e);
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            // Set the visual state to indicate that nothing should be highlighted (only when the column is not being manipulated).
            if (!this.isManipulating)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }

            // Allow the base class to handle the rest of the event.
            base.OnPointerExited(e);
        }

        /// <summary>
        /// Called before the RightTapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            // Validate the 'e' parameter
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Present the flyout menu when the column possesses a filter.
            if (this.HasFilters)
            {
                // The positioning of the flyout will be relative to the header.
                FilterFlyout filterFlyout = new FilterFlyout();
                filterFlyout.ItemsSource = this.Filters;
                filterFlyout.ShowAt(this);
                e.Handled = true;
            }

            // Allow the base class to handle the rest of the event.
            base.OnRightTapped(e);
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            // This will set the property which will, eventually, set the visual state to indicate the header is pressed.
            this.IsPressed = true;

            // Allow the base class to handle the rest of the event.
            base.OnPointerPressed(e);
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            // This will clear the property which will, eventually, set the visual state to indicate the header is no longer pressed.
            this.IsPressed = false;

            // Allow the base class to handle the rest of the event.
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            // Validate the 'e' parameter
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // See if someone can handle the sorting command and, if so, sort the column and mark the event as being handled.
            if (this.Press != null)
            {
                this.Press.Execute(false);
                e.Handled = true;
            }

            // Allow the base class to handle the rest of the event.
            base.OnTapped(e);
        }

        /// <summary>
        /// Invoked when the effective property value of the IsPressedProperty changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsPressedPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ColumnViewColumnHeader columnHeader = dependencyObject as ColumnViewColumnHeader;
            bool isPressed = (bool)dependencyPropertyChangedEventArgs.NewValue;
            VisualStateManager.GoToState(columnHeader, isPressed ? "Pressed" : "Normal", true);
        }

        /// <summary>
        /// Handles a change to the dependency properties.
        /// </summary>
        /// <param name="dependencyObject">The object that originated the event.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The event arguments.</param>
        private static void OnIsSelectedPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // The filter is closed when the selection is turned off.  The selection is a property that combines the keystrokes and the mouse
            // gestures into a single state the shows the menu header has been selected by the user for some operation.  The visual cues for the
            // header are driven by this property and so is the state of the filter drop down.
            ColumnViewColumnHeader columnHeader = dependencyObject as ColumnViewColumnHeader;
            if ((bool)dependencyPropertyChangedEventArgs.OldValue)
            {
                if (columnHeader.IsFilterOpen)
                {
                    columnHeader.SetValue(ColumnViewColumnHeader.IsFilterOpenProperty, false);
                }
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the SortDirectionProperty changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnSortDirectionPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Change the visual state when the sort direction changes.
            ColumnViewColumnHeader columnViewColumnHeader = dependencyObject as ColumnViewColumnHeader;
            SortDirection sortDirection = (SortDirection)dependencyPropertyChangedEventArgs.NewValue;
            VisualStateManager.GoToState(columnViewColumnHeader, sortDirection.ToString(), true);
        }

        /// <summary>
        /// Handles a filter command.
        /// </summary>
        /// <param name="filterDescription">The view model of the filter.</param>
        private void Filter(FilterDescription filterDescription)
        {
            // The flyout will hide itself after each filter item is selected or unselected.
            this.IsFilterOpen = false;
        }

        /// <summary>
        /// Occurs when either the ActualHeight or the ActualWidth property changes value on a column header.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // The child elements of a canvas do not automatically inform the parent when their size has changed.  However, we need to know about a
            // change in size because the height of the panel is based on the largest header element.  This shows up dramatically with image data
            // which is loaded asynchronously.  At some point after the header has loaded and measured itself, an event will come along that updates
            // the image data.  This step is required to have the header automatically adjust to these kinds of outside events.
            Canvas parent = VisualTreeExtensions.FindParent<Canvas>(this);
            parent.InvalidateMeasure();
        }
    }
}