// <copyright file="ColumnViewColumnHeader.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// The view for a column header in the <see cref="ColumnViewDefinition"/>.
    /// </summary>
    public class ColumnViewColumnHeader : ContentControl
    {
        /// <summary>
        /// The CompleteDrag DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty CompleteDragProperty = DependencyProperty.Register(
            "CompleteDrag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The Drag DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty DragProperty = DependencyProperty.Register(
            "Drag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The Filters DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(
            "Filters",
            typeof(IList),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The HasFilters DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HasFiltersProperty = DependencyProperty.Register(
            "HasFilters",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsAscending DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsAscendingProperty = DependencyProperty.Register(
            "IsAscending",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsDescending DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsDescendingProperty = DependencyProperty.Register(
            "IsDescending",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsFilterOpen DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsFilterOpenProperty = DependencyProperty.Register(
            "IsFilterOpen",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The IsPressed DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register(
            "IsPressed",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(false, ColumnViewColumnHeader.OnIsPressedPropertyChanged));

        /// <summary>
        /// The IsSelected DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(false, new PropertyChangedCallback(ColumnViewColumnHeader.OnIsSelectedPropertyChanged)));

        /// <summary>
        /// The Press DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty PressProperty = DependencyProperty.Register(
            "Press",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The StartDrag DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty StartDragProperty = DependencyProperty.Register(
            "StartDrag",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The SetWidth DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SetWidthProperty = DependencyProperty.Register(
            "SetWidth",
            typeof(ICommand),
            typeof(ColumnViewColumnHeader),
            null);

        /// <summary>
        /// The SortDirection DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(SortDirection),
            typeof(ColumnViewColumnHeader),
            new PropertyMetadata(SortDirection.NotSorted, ColumnViewColumnHeader.OnSortDirectionPropertyChanged));

        /// <summary>
        /// The SortOrder DependencyProperty.
        /// </summary>
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
        /// The name of the part where the content is displayed.  Used for clipping when the filter button is visible.
        /// </summary>
        private const string ContentElementPartName = "PART_Content";

        /// <summary>
        /// Name of the open filter part of the header.
        /// </summary>
        private const string FilterButtonPartName = "PART_FilterButton";

        /// <summary>
        /// Name of the gripper part.
        /// </summary>
        private const string ReszieGripperPartName = "PART_HeaderGripper";

        /// <summary>
        /// The cursor used to resize columns.
        /// </summary>
        private static Cursor verticalSplitCursor = new Cursor(
            Application.GetResourceStream(new Uri("/DarkBond.Views;component/Assets/VerticalSplit.cur", UriKind.Relative)).Stream);

        /// <summary>
        /// The command handler for filtering.
        /// </summary>
        private DelegateCommand<FilterDescription> filterCommand;

        /// <summary>
        /// The element where the content of the header appears.  This element is clipped when the filter button is visible.
        /// </summary>
        private FrameworkElement contentElement;

        /// <summary>
        /// This is the element used to open up the filter from the header.
        /// </summary>
        private Button filterButton;

        /// <summary>
        /// Used to indication that a manipulation is active.
        /// </summary>
        private bool isManipulating;

        /// <summary>
        /// The last position of the mouse of the last drag operation.
        /// </summary>
        private Point lastPoint;

        /// <summary>
        /// The resize gripper.
        /// </summary>
        private Thumb resizeGripper;

        /// <summary>
        /// The starting position, in the coordinates of this control, for a drag-and-drop operation.
        /// </summary>
        private Point startPosition;

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
        /// <value>
        /// The command invoked when a column completes a drag operation.
        /// </value>
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
        /// <value>
        /// The command invoked when a column is dragged.
        /// </value>
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
        /// <value>
        /// A value indicating whether the filter for this column is visible or not.
        /// </value>
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
        /// <value>
        /// A value indicating whether the column is sorting its values in ascending order.
        /// </value>
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
        /// <value>
        /// A value indicating whether the column is sorting its values in ascending order.
        /// </value>
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
        /// <value>
        /// A value indicating whether the column is sorting its values in descending order.
        /// </value>
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
        /// <value>
        /// A value indicating whether the filter for this column is visible or not.
        /// </value>
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
        /// <value>
        /// A value indicating whether a ColumnViewColumnHeader is currently activated.
        /// </value>
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
        /// <value>
        /// A value indicating whether a ColumnViewColumnHeader is currently activated.
        /// </value>
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
        /// <value>
        /// The command invoked when a column header is pressed.
        /// </value>
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
        /// <value>
        /// The command invoked when a column width has changed.
        /// </value>
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
        /// <value>
        /// The sort direction.
        /// </value>
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
        /// <value>
        /// The sort order.
        /// </value>
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
        /// <value>
        /// The command invoked when a column starts a drag operation.
        /// </value>
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
        public override void OnApplyTemplate()
        {
            // Binding in the Setter of Styles doesn't work in Windows Store applications, so we need to perform the bindings manually where when the
            // template is applied.  These are the equivalent of 'FindAncestor' binding operations.  From these, we're going to bind the properties
            // of the column header to the column view.
            ColumnViewDefinition columnView = VisualTreeExtensions.FindParent<ColumnViewDefinition>(this);
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
            fontSizeBinding.Source = columnView;
            contentPresenter.SetBinding(TextBlock.FontSizeProperty, fontSizeBinding);

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

            // Bind the Header property.
            Binding headerTemplateBinding = new Binding();
            headerTemplateBinding.Path = new PropertyPath("HeaderTemplate");
            headerTemplateBinding.Source = columnViewColumn;
            contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, headerTemplateBinding);

            // Bind the VerticalHeaderAlignment property.
            Binding verticalHeaderAlignmentBinding = new Binding();
            verticalHeaderAlignmentBinding.Path = new PropertyPath("VerticalHeaderAlignment");
            verticalHeaderAlignmentBinding.Source = columnViewColumn;
            contentPresenter.SetBinding(ContentPresenter.VerticalAlignmentProperty, verticalHeaderAlignmentBinding);

            // The visual states can only be set when there is a template to recognize the states.
            VisualStateManager.GoToState(this, this.SortDirection.ToString(), true);

            // Allow the base class to handle the rest of the event.
            base.OnApplyTemplate();

            // This will attach the gripper to events that allow the column header to be resized and will give it a cursor that lets the user know
            // when resizing will work.
            this.resizeGripper = this.GetTemplateChild(ColumnViewColumnHeader.ReszieGripperPartName) as Thumb;
            if (this.resizeGripper != null)
            {
                this.resizeGripper.DragDelta += new DragDeltaEventHandler(this.OnColumnHeaderResize);
                this.resizeGripper.MouseDown += new MouseButtonEventHandler(this.OnGripperClicked);
                this.resizeGripper.MouseUp += new MouseButtonEventHandler(this.OnGripperClicked);
                this.resizeGripper.MouseDoubleClick += new MouseButtonEventHandler(this.OnGripperDoubleClicked);
                this.resizeGripper.Cursor = ColumnViewColumnHeader.verticalSplitCursor;
            }

            // When the filter button is visible we're going to clip the header so that it doesn't bleed into the filter button.  Since the button,
            // the filter button and all the controls in the header are transparent (for a visually striking effect), it is necessary to clip rather
            // than just paint over.
            this.contentElement = this.GetTemplateChild(ColumnViewColumnHeader.ContentElementPartName) as FrameworkElement;
            if (this.contentElement != null)
            {
                this.contentElement.Clip = new RectangleGeometry(new Rect(new Size(double.PositiveInfinity, double.PositiveInfinity)));
                this.contentElement.SizeChanged += (s, e) =>
                {
                    this.ClipContent();
                };
            }

            // This will attach logic that will open or close the filter button when that part of the header is pressed.
            this.filterButton = this.GetTemplateChild(ColumnViewColumnHeader.FilterButtonPartName) as Button;
            if (this.filterButton != null)
            {
                this.filterButton.Click += this.OnFilterButtonClick;
                this.filterButton.IsVisibleChanged += (s, e) =>
                {
                    this.ClipContent();
                };
                this.filterButton.SizeChanged += (s, e) =>
                {
                    this.ClipContent();
                };
            }
        }

        /// <summary>
        /// Invoked just before the IsKeyboardFocusWithinChanged event is raised by this element.
        /// </summary>
        /// <param name="e">A DependencyPropertyChangedEventArgs that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            // Allow the base class to handle the event.
            base.OnIsKeyboardFocusWithinChanged(e);

            // This will set the IsSelected property do true when the keyboard focus is within the filter drop down.  False when the keyboard focus
            // leaves the filter drop down.
            if (this.IsKeyboardFocusWithin && !this.IsSelected)
            {
                this.SetValue(ColumnViewColumnHeader.IsSelectedProperty, true);
            }

            if (!this.IsKeyboardFocusWithin && this.IsSelected)
            {
                this.SetValue(ColumnViewColumnHeader.IsSelectedProperty, false);
            }
        }

        /// <summary>
        /// Provides class handling for the LostMouseCapture routed event that occurs when this control is no longer receiving mouse event messages.
        /// </summary>
        /// <param name="e">The event data for the LostMouseCapture event.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Release the 'pressed' property.  This is used to give a visual cue as to the header's state.
            if (e.OriginalSource == this)
            {
                this.SetValue(ColumnViewColumnHeader.IsPressedProperty, false);
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseEnter attached event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            // Allow the base class to handle the event.
            base.OnMouseEnter(e);

            // This will force the 'IsSelected' property to true when the mouse is over the 'Open Filter' button.
            if (this.IsMouseOver != this.IsSelected)
            {
                this.SetValue(ColumnViewColumnHeader.IsSelectedProperty, this.IsMouseOver);
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseLeave attached event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // Allow the base class to handle the event.
            base.OnMouseLeave(e);

            // This will force the 'IsSelected' property to true when the mouse is over the 'Open Filter' button.
            if (this.IsMouseOver != this.IsSelected)
            {
                this.SetValue(ColumnViewColumnHeader.IsSelectedProperty, this.IsMouseOver);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is raised on this element.
        /// </summary>
        /// <param name="e">
        /// The MouseButtonEventArgs that contains the event data.  The event data reports that the left mouse button was pressed.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // If the mouse can be captures then we'll initiate a gesture recognition.  The
            if (this.CaptureMouse())
            {
                this.startPosition = e.GetPosition(VisualTreeExtensions.FindParent<ColumnViewHeaderPanel>(this));
                this.SetValue(ColumnViewColumnHeader.IsPressedProperty, true);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">
        /// The MouseButtonEventArgs that contains the event data.  The event data reports that the left mouse button was released.
        /// </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Validate the argument.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Release the mouse capture when the button is release.
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            // This is a brute-force method of creating a gesture.  If the mouse hasn't moved very far from the point it was clicked, then we're
            // going to consider this gesture a 'click' and execute a sort on the column.  Otherwise, the parent HeaderPresenter will take up the job
            // and manage the gesture as a command to reorder the columns.
            Point point = e.GetPosition(VisualTreeExtensions.FindParent<ColumnViewHeaderPanel>(this));
            double distance = Math.Sqrt(Math.Pow(this.startPosition.X - point.X, 2.0) + Math.Pow(this.startPosition.Y - point.Y, 2.0));
            if (distance < ColumnViewColumnHeader.MouseMoveThreshold)
            {
                bool isExtended = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                if (this.Press != null && this.Press.CanExecute(isExtended))
                {
                    this.Press.Execute(isExtended);
                }
            }

            // This is the other part of creating a gesture from mouse movement.  If there's an active drag operation going on then releasing the
            // mouse button indicates the completion of that operation.
            if (this.isManipulating)
            {
                this.isManipulating = false;
                if (this.CompleteDrag != null)
                {
                    this.CompleteDrag.Execute(null);
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseMove attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Validate the argument.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Allow the base class to handle the event.
            base.OnMouseMove(e);

            // This will force the 'IsSelected' property to true when the mouse is over the 'Open Filter' button.
            if (this.IsMouseOver != this.IsSelected)
            {
                this.SetValue(ColumnViewColumnHeader.IsSelectedProperty, this.IsMouseOver);
            }

            // If the left button is pressed while the mouse is being moved, then we probably have a drag manipulation.
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // The reference point for motion is the origin of the panel that holds the column headers.  We'll use this to tell how much the
                // mouse has moved from the last time we generated a manipulation event.
                Point point = e.GetPosition(VisualTreeExtensions.FindParent<ColumnViewHeaderPanel>(this));

                // This will start a drag operation if one hasn't been started yet.  If we have already started a drag operation, then the change in
                // the position from the last time we recognized the mouse movement is transmitted to anyone listening.  This operation was designed
                // to look identical to a Manipulation event handler in Windows 8.1, with most of the logic for handling the action pushed into the
                // ColumnViewDefinition control.
                if (this.isManipulating)
                {
                    if (this.Drag != null)
                    {
                        this.Drag.Execute(new ColumnDragParameter(point.X - this.lastPoint.X, e.GetPosition(this).X));
                    }
                }
                else
                {
                    this.isManipulating = true;
                    if (this.StartDrag != null)
                    {
                        this.StartDrag.Execute(null);
                    }
                }

                // Remembering the last point allows us to generate deltas instead of absolutes.
                this.lastPoint = point;
            }
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
            // Extract the generic arguments.
            ColumnViewColumnHeader columnViewColumnHeader = dependencyObject as ColumnViewColumnHeader;
            bool isPressed = (bool)dependencyPropertyChangedEventArgs.NewValue;

            // Set the visual state to reflect whether the header is pressed or not.
            VisualStateManager.GoToState(columnViewColumnHeader, isPressed ? "Pressed" : "Normal", true);
        }

        /// <summary>
        /// Handles a change to the dependency properties.
        /// </summary>
        /// <param name="dependencyObject">The object that originated the event.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The event arguments.</param>
        private static void OnIsSelectedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ColumnViewColumnHeader columnViewColumnHeader = dependencyObject as ColumnViewColumnHeader;
            bool isSelected = (bool)dependencyPropertyChangedEventArgs.NewValue;

            // The filter drop down is closed when header is no longer selected.  The selection is a property that combines the keystrokes and the
            // mouse gestures into a single state the shows the menu header has been selected by the user for some operation.
            if (!isSelected && columnViewColumnHeader.IsFilterOpen)
            {
                columnViewColumnHeader.SetValue(ColumnViewColumnHeader.IsFilterOpenProperty, false);
            }

            // Calculate the selected state of the header.
            string selectedState = "Unselected";
            if (isSelected)
            {
                selectedState = columnViewColumnHeader.HasFilters ? "SelectedWithFilters" : "SelectedWithoutFilters";
            }

            // Set the visual state to reflect the selected state (with or without filters).
            VisualStateManager.GoToState(columnViewColumnHeader, selectedState, true);
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
        /// This will clip the content of the header when the filter button is visible.
        /// </summary>
        private void ClipContent()
        {
            // If we have both a content part and filter button part, then we can clip the content so that it won't bleed into the filter button when
            // the filter button is visible.  Note the addition of the padding which gives a nice separation between the header and the filter
            // button, even when the header is right justified.
            if (this.contentElement != null && this.filterButton != null)
            {
                RectangleGeometry rectangleGeometry = this.contentElement.Clip as RectangleGeometry;
                double width = this.filterButton.Visibility == Visibility.Visible ?
                    Math.Max(0.0, this.contentElement.ActualWidth - this.filterButton.ActualWidth - this.Padding.Right) :
                    this.contentElement.ActualWidth;
                rectangleGeometry.Rect = new Rect(new Size(width, this.contentElement.ActualHeight));
            }
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
        /// Represents a method that will handle the DragDelta routed event of a Thumb control.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnColumnHeaderResize(object sender, DragDeltaEventArgs e)
        {
            if (this.SetWidth != null)
            {
                double width = Math.Max(this.ActualWidth + e.HorizontalChange, 0.0);
                if (this.SetWidth.CanExecute(width))
                {
                    this.SetWidth.Execute(width);
                }
            }

            // The drag operation was handled.
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the filter button is pressed.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        {
            // Toggle the filter visibility.
            this.IsFilterOpen = !this.IsFilterOpen;
        }

        /// <summary>
        /// Handles a mouse click on the gripper.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnGripperClicked(object sender, MouseButtonEventArgs e)
        {
            // This will prevent a single click on the gripper from being passed on to the header which is only interested in the single clicks that
            // indicate that a sorting operation is requested.
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when a mouse button is clicked two or more times.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnGripperDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            // This event has been handled.
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when either the ActualHeight or the ActualWidth property changes value on a <see cref="ColumnViewColumnHeader"/>.
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