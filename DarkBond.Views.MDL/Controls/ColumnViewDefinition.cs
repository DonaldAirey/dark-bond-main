// <copyright file="ColumnViewDefinition.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Linq;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// A view that displays items in columns and rows.
    /// </summary>
    [ContentProperty(Name = "Columns")]
    public class ColumnViewDefinition : ViewDefinition
    {
        /// <summary>
        /// Identifies the AllowsColumnReorder DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty AllowsColumnReorderProperty = DependencyProperty.Register(
            "AllowsColumnReorder",
            typeof(bool),
            typeof(ColumnViewDefinition),
            new PropertyMetadata(true));

        /// <summary>
        /// The CellBorderBrush DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellBorderBrushProperty = DependencyProperty.Register(
            "CellBorderBrush",
            typeof(Brush),
            typeof(ColumnViewDefinition),
            new PropertyMetadata(default(Brush)));

        /// <summary>
        /// The CellBorderThickness DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellBorderThicknessProperty = DependencyProperty.Register(
            "CellBorderThickness",
            typeof(Thickness),
            typeof(ColumnViewDefinition),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// The CellPadding DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellPaddingProperty = DependencyProperty.Register(
            "CellPadding",
            typeof(Thickness),
            typeof(ColumnViewDefinition),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Identifies the Columns DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(ColumnViewColumnCollection),
            typeof(ColumnViewDefinition),
            null);

        /// <summary>
        /// The HeaderPadding DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register(
            "HeaderPadding",
            typeof(Thickness),
            typeof(ColumnViewDefinition),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// The HeaderTemplate DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(ControlTemplate),
            typeof(ColumnViewDefinition),
            null);

        /// <summary>
        /// Identifies the ItemBackground DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemBackgroundProperty = DependencyProperty.Register(
            "ItemBackground",
            typeof(Brush),
            typeof(ColumnViewDefinition),
            null);

        /// <summary>
        /// A dictionary for mapping the column collection change action to a handler for that action.
        /// </summary>
        private static Dictionary<NotifyCollectionChangedAction, Action<ColumnViewDefinition, NotifyCollectionChangedEventArgs>> collectionActions =
            new Dictionary<NotifyCollectionChangedAction, Action<ColumnViewDefinition, NotifyCollectionChangedEventArgs>>
            {
                { NotifyCollectionChangedAction.Add, ColumnViewDefinition.AddColumns },
                { NotifyCollectionChangedAction.Move, ColumnViewDefinition.MoveColumns },
                { NotifyCollectionChangedAction.Remove, ColumnViewDefinition.RemoveColumns },
                { NotifyCollectionChangedAction.Reset, ColumnViewDefinition.ResetColumns }
            };

        /// <summary>
        /// The amount of time the animation will take to move a column header.
        /// </summary>
        private static Duration columnMovementDuration = new Duration(TimeSpan.FromMilliseconds(250));

        /// <summary>
        /// Used to toggle the current sort direction.
        /// </summary>
        private static Dictionary<SortDirection, SortDirection> sortDirectionMap = new Dictionary<SortDirection, SortDirection>
        {
            { SortDirection.Ascending, SortDirection.Descending },
            { SortDirection.Descending, SortDirection.Ascending }
        };

        /// <summary>
        /// This is the column that is being dragged during a drag-and-drop operation.
        /// </summary>
        private int draggingColumnIndex;

        /// <summary>
        /// The index of the destination column during a column drag-and-drop operation.
        /// </summary>
        private int destinationColumnIndex;

        /// <summary>
        /// The columns that describe the sort order of the view.
        /// </summary>
        private List<ColumnViewColumn> sortColumns = new List<ColumnViewColumn>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewDefinition"/> class.
        /// </summary>
        public ColumnViewDefinition()
        {
            // Initialize the object.
            this.SetValue(ColumnViewDefinition.ColumnsProperty, new ColumnViewColumnCollection());
            this.Columns.CollectionChanged += this.OnColumnCollectionChanged;

            // These are the predefined properties for this view.
            this.Style = Application.Current.Resources["ColumnViewStyle"] as Style;

            // This will reconcile the current column sort indicators with the global sort order.
            GlobalCommands.Sort.RegisterCommand(new DelegateCommand<Collection<SortDescription>>(this.Sort));
        }

        /// <summary>
        /// Gets or sets a value indicating whether columns in a ColumnView can be reordered by a drag-and-drop operation.
        /// </summary>
        public bool AllowsColumnReorder
        {
            get
            {
                return (bool)this.GetValue(ColumnViewDefinition.AllowsColumnReorderProperty);
            }

            set
            {
                this.SetValue(ColumnViewDefinition.AllowsColumnReorderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush to use when drawing the cell border.
        /// </summary>
        public Brush CellBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(ColumnViewDefinition.CellBorderBrushProperty);
            }

            set
            {
                this.SetValue(ColumnViewDefinition.CellBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the cell border.
        /// </summary>
        public Thickness CellBorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewDefinition.CellBorderThicknessProperty);
            }

            set
            {
                this.SetValue(ColumnViewDefinition.CellBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether columns in a ColumnView can be reordered by a drag-and-drop operation.
        /// </summary>
        public Thickness CellPadding
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewDefinition.CellPaddingProperty);
            }

            set
            {
                this.SetValue(ColumnViewDefinition.CellPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of columns.
        /// </summary>
        public ColumnViewColumnCollection Columns
        {
            get
            {
                return this.GetValue(ColumnViewDefinition.ColumnsProperty) as ColumnViewColumnCollection;
            }
        }

        /// <summary>
        /// Gets or sets the padding used for the column headers.
        /// </summary>
        public Thickness HeaderPadding
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewDefinition.HeaderPaddingProperty);
            }

            set
            {
                this.SetValue(ColumnViewDefinition.HeaderPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets the template used to generate the header.
        /// </summary>
        public ControlTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(ColumnViewDefinition.HeaderTemplateProperty) as ControlTemplate;
            }
        }

        /// <summary>
        /// Gets or sets the background brush used for the row items.
        /// </summary>
        public Brush ItemBackground
        {
            get
            {
                return this.GetValue(ColumnViewDefinition.ItemBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(ColumnViewDefinition.ItemBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Add columns to the view.
        /// </summary>
        /// <param name="columnView">The column view.</param>
        /// <param name="notifyCollectionChangedEventArgs">The collection changed argument.</param>
        private static void AddColumns(ColumnViewDefinition columnView, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // This will attach to the event handlers of each of the columns added to the view.
            foreach (ColumnViewColumn columnViewColumn in notifyCollectionChangedEventArgs.NewItems)
            {
                // The columns report their events to the view which decides how to handle them.
                columnViewColumn.ColumnDragCompleted += columnView.OnColumnDragCompleted;
                columnViewColumn.ColumnDragged += columnView.OnColumnDragged;
                columnViewColumn.ColumnDragStarted += columnView.OnColumnDragStarted;
                columnViewColumn.ColumnWidthChanged += columnView.OnColumnWidthChanged;
                columnViewColumn.SortChanged += columnView.OnSortChanged;

                // If the cell border brush hasn't been explicitly cell for the cell, then use the setting for the entire control.
                if (columnViewColumn.CellBorderBrush == default(Brush))
                {
                    Binding cellBorderBrushBinding = new Binding
                    {
                        Path = new PropertyPath("CellBorderBrush"),
                        Source = columnView
                    };
                    BindingOperations.SetBinding(columnViewColumn, ColumnViewColumn.CellBorderBrushProperty, cellBorderBrushBinding);
                }

                // If the cell border brush hasn't been explicitly cell for the cell, then use the setting for the entire control.
                if (columnViewColumn.CellBorderThickness == default(Thickness))
                {
                    Binding cellBorderThicknessBinding = new Binding
                    {
                        Path = new PropertyPath("CellBorderThickness"),
                        Source = columnView
                    };
                    BindingOperations.SetBinding(columnViewColumn, ColumnViewColumn.CellBorderThicknessProperty, cellBorderThicknessBinding);
                }

                // If the cell padding for a new column hasn't been set explicitly then we're going to bind it to the master cell padding for the
                // entire view.  This allows the view to provide a consistent look-and-feel to the cells, but give the column the freedom to override
                // that value.
                if (columnViewColumn.CellPadding == default(Thickness))
                {
                    Binding cellPaddingBinding = new Binding
                    {
                        Path = new PropertyPath("CellPadding"),
                        Source = columnView
                    };
                    BindingOperations.SetBinding(columnViewColumn, ColumnViewColumn.CellPaddingProperty, cellPaddingBinding);
                }

                // Same general logic with the header padding.  Use a common padding if the column hasn't explicitly overridden it.
                if (columnViewColumn.HeaderPadding == default(Thickness))
                {
                    Binding headerPaddingBinding = new Binding
                    {
                        Path = new PropertyPath("HeaderPadding"),
                        Source = columnView
                    };
                    BindingOperations.SetBinding(columnViewColumn, ColumnViewColumn.HeaderPaddingProperty, headerPaddingBinding);
                }
            }
        }

        /// <summary>
        /// Move a column in the row presenter.
        /// </summary>
        /// <param name="columnView">The row panel.</param>
        /// <param name="notifyCollectionChangedEventArgs">The collection changed argument.</param>
        private static void MoveColumns(ColumnViewDefinition columnView, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
        }

        /// <summary>
        /// Remove a column from the row presenter.
        /// </summary>
        /// <param name="columnView">The row panel.</param>
        /// <param name="notifyCollectionChangedEventArgs">The collection changed argument.</param>
        private static void RemoveColumns(ColumnViewDefinition columnView, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // We no longer want to listen to any of the events generated by the removed columns.
            foreach (ColumnViewColumn columnViewColumn in notifyCollectionChangedEventArgs.OldItems)
            {
                columnViewColumn.ColumnDragCompleted -= columnView.OnColumnDragCompleted;
                columnViewColumn.ColumnDragged -= columnView.OnColumnDragged;
                columnViewColumn.ColumnDragStarted -= columnView.OnColumnDragStarted;
                columnViewColumn.ColumnWidthChanged -= columnView.OnColumnWidthChanged;
                columnViewColumn.SortChanged -= columnView.OnSortChanged;
            }
        }

        /// <summary>
        /// Resets the collection of columns in the row presenter.
        /// </summary>
        /// <param name="columnView">The row panel.</param>
        /// <param name="notifyCollectionChangedEventArgs">The collection changed argument.</param>
        private static void ResetColumns(ColumnViewDefinition columnView, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
        }

        /// <summary>
        /// Find the index of the column that contains the given point.
        /// </summary>
        /// <param name="offset">The horizontal offset of the pointer on the header panel.</param>
        /// <returns>The index of the column that contains the point or -1 if there are no columns.</returns>
        private int FindIndex(double offset)
        {
            // This will determine to which column a given point belongs by creating an area of infinite height bounded by the edges of the column.
            double left = 0;
            for (int columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
            {
                ColumnViewColumn columnViewColumn = this.Columns[columnIndex];
                if (columnViewColumn.IsVisible)
                {
                    double right = left + columnViewColumn.ActualWidth;
                    if (left <= offset && offset < right)
                    {
                        return columnIndex;
                    }

                    left = right;
                }
            }

            // If the point is not within the bounds of the columns, return the terminating columns.  Note that the padding column is not included in
            // the explicit column collection so a point that is off the right edge of the header will return an index between the last explicit
            // column and the padding column.
            return offset < 0.0 ? 0 : this.Columns.Count;
        }

        /// <summary>
        /// Generate the template used to create rows.
        /// </summary>
        /// <returns>The text of a ControlTemplate that can be compiled and used to generate rows.</returns>
        private DataTemplate GenerateRowTemplate()
        {
            // The general idea here is to create a data template that is used to create rows of columnar data.  The need to compile comes from the
            // fact that WPF and Metro both are more efficient when the visual tree is constructed using a "Top Down" approach rather than from the
            // bottom up.  A previous design attempted to construct the cell controls from the column descriptions after the row was created, but
            // caused the 'MeasureOverride' to be called for every cell added to the document and let to very notice-able delays when scrolling
            // through large volumes of data.  The top-down design results in just a couple calls to 'MeasureOverride' and significantly faster
            // drawing.
            XDocument xDocument = new XDocument();
            XNamespace defaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XNamespace controlsNamespace = "using:DarkBond.Views.Controls";
            XElement dataTemplateElement = new XElement(
                defaultNamespace + "DataTemplate",
                new XAttribute(XNamespace.Xmlns + "controls", controlsNamespace));
            xDocument.Add(dataTemplateElement);

            // The row is a stack panel with one or more elements representing the columns.
            XElement panelElement = new XElement(controlsNamespace + "ColumnViewRowPanel");
            dataTemplateElement.Add(panelElement);

            // Create a bare-bones visual tree for each visible column in the view.  The panel will take care of binding it to the features of the
            // column, such as the template to use for the cells, when the panel created from this data template but before it's loaded.
            foreach (ColumnViewColumn columnViewColumn in this.Columns)
            {
                // This is a bare-bones visual tree for this column.  The other attributes of it will be provided by the column definition once it's
                // loaded into the visual tree.
                if (columnViewColumn.IsVisible)
                {
                    panelElement.Add(
                        new XElement(
                            defaultNamespace + "Border",
                            new XElement(defaultNamespace + "ContentPresenter")));
                }
            }

            // This compiled DataTemplate will create rows much faster than a design that attempts to do the same thing programmatically.
            return XamlReader.Load(xDocument.ToString()) as DataTemplate;
        }

        /// <summary>
        /// Handles the start of a drag operation on the column header.
        /// </summary>
        /// <param name="sender">The source of the event. </param>
        /// <param name="eventArgs">The event data.</param>
        private void OnColumnDragStarted(object sender, EventArgs eventArgs)
        {
            // If we're allowing column reordering, then start the column drag operation.
            if (this.AllowsColumnReorder)
            {
                // The general idea when dragging a column header around is to open up a gap where the column that is being dragged can go.  This
                // will keep track starting column and the initial destination.  Note that the destination column changes as the dragging column is
                // moved around on the header.
                ColumnViewColumn columnViewColumn = sender as ColumnViewColumn;
                this.destinationColumnIndex = this.draggingColumnIndex = this.Columns.IndexOf(columnViewColumn);

                // This will place the header above all the other headers as it is dragged around.
                columnViewColumn.ZIndex = 1;
            }
        }

        /// <summary>
        /// Handles the completion of a drag operation on the column header.
        /// </summary>
        /// <param name="sender">The source of the event. </param>
        /// <param name="eventArgs">The event data.</param>
        private void OnColumnDragCompleted(object sender, EventArgs eventArgs)
        {
            // If we're allowing column reordering, then complete the column drag operation.
            if (this.AllowsColumnReorder)
            {
                // This will place the header at the same location as the other headers.
                ColumnViewColumn columnViewColumn = sender as ColumnViewColumn;
                columnViewColumn.ZIndex = 0;

                // When we've completed the manipulation operation, this does the actual work of moving the columns around.
                int newIndex = this.draggingColumnIndex >= this.destinationColumnIndex ? this.destinationColumnIndex : this.destinationColumnIndex - 1;
                this.Columns.Move(this.draggingColumnIndex, newIndex);
            }
        }

        /// <summary>
        /// Handles the dragging of a column header.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="columnDragEventArgs">A description of the drag operation.</param>
        private void OnColumnDragged(object sender, ColumnDragEventArgs columnDragEventArgs)
        {
            // If we're allowing column reordering, then drag the column header.
            if (this.AllowsColumnReorder)
            {
                // Extract the specific column that is being dragged from the generic arguments.
                ColumnViewColumn draggedColumn = sender as ColumnViewColumn;

                // This will do the work of actually moving the dragging column by the given change.
                draggedColumn.Left += columnDragEventArgs.Delta;

                // This is the index of the column that is the current destination for the drag-and-drop operation.  Note that we add the current
                // mouse position (that is, the position relative the left edge of the column being dragged) to the operation so that the column acts
                // like a cursor where the hot-spot is the current mouse position.  When the actual mouse moves over a new column (not just the edge
                // of the column being dragged) we're going to open up a spot where the dragged column can be dropped.
                int currentColumnIndex = this.FindIndex(draggedColumn.Left + columnDragEventArgs.MousePosition);

                // When the proposed destination column has changed (and is not the original index of the column being dragged) then animate the
                // cells to re-order themselves in order to make a room in the header for the dragged column.
                if (currentColumnIndex != this.destinationColumnIndex && currentColumnIndex != this.draggingColumnIndex)
                {
                    // This will open up a gap in the heading where the dragged column can be dropped.  The 'destination' variable is used to set the
                    // destination for the animation.  It is designed to open up a gap in the header where the dragged column can be dropped.
                    double desiredLeft = 0.0;

                    // This will cycle through all the children re-arranging them to produce a gap.  Note that in this form the loop will also
                    // examine the padding header because it's index can also be the destination for an operation.
                    for (int columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                    {
                        // This column will be animated to move into a new location if it's not already at the proper location.
                        ColumnViewColumn columnViewColumn = this.Columns[columnIndex];

                        // The column being dragged occupies it's own space so it doesn't need to counted here.
                        if (!columnViewColumn.IsVisible || columnIndex == this.draggingColumnIndex)
                        {
                            continue;
                        }

                        // This will produce a gap at the target location big enough to hold the dragged column.
                        if (columnIndex == currentColumnIndex)
                        {
                            desiredLeft += draggedColumn.ActualWidth;
                        }

                        // The 'DesiredLeft' property is used to tell us where we want this column to reside.  If it doesn't match up with the
                        // expected left hand side, then we're going to start an animation sequence that will move it there.  The 'Left' property
                        // represents where the column actually is at any given moment, so testing against the desired left hand corner rather than
                        // the actual left hand side prevents us from starting up an animation when one is already in progress.  This is very
                        // important as the animation will jitter if one attempts to re-start it just because the dragging column has moved some
                        // more.
                        if (columnViewColumn.DesiredLeft != desiredLeft)
                        {
                            // This will prevent another storyboard from being executed until the current one is finished.
                            columnViewColumn.DesiredLeft = desiredLeft;

                            // Create an animation sequence that will move the column header from the current position to the desired position.
                            DoubleAnimation doubleAnimation = new DoubleAnimation();
                            doubleAnimation.EnableDependentAnimation = true;
                            doubleAnimation.From = columnViewColumn.Left;
                            doubleAnimation.To = desiredLeft;
                            doubleAnimation.Duration = ColumnViewDefinition.columnMovementDuration;

                            // A storyboard is needed to carry out the animation on the property of a DependencyObject.
                            Storyboard storyboard = new Storyboard();
                            Storyboard.SetTarget(doubleAnimation, columnViewColumn);
                            Storyboard.SetTargetProperty(doubleAnimation, "Left");
                            storyboard.Children.Add(doubleAnimation);
                            storyboard.Begin();
                        }

                        // The destination moves along with each column except for the places where the gap was opened or the dragged column was
                        // ignored.  Thus, it becomes the new value for the left side of the next column in the header.
                        desiredLeft += columnViewColumn.ActualWidth;
                    }

                    // At this point the destination column is recorded.  When the mouse button is lifted, we'll look at this value to see what
                    // position has been selected by the user's mouse.
                    this.destinationColumnIndex = currentColumnIndex;
                }
            }
        }

        /// <summary>
        /// Determines the position of each of the columns in the panel.
        /// </summary>
        private void ArrangeHeader()
        {
            // The panel is a combination of a stack panel and a canvas.  It has the qualities of a canvas to allow columns to move about and be
            // animated, but it acts like a stack panel in that the columns are stacked in the order they're added to the collection.  This will
            // provide the initial layout for the columns when columns are added, removed, moved or resized.  After this, the panel generally acts
            // like a canvas.
            double left = 0.0;
            foreach (ColumnViewColumn columnViewColumn in this.Columns)
            {
                if (columnViewColumn.IsVisible)
                {
                    columnViewColumn.Left = left;
                    left += columnViewColumn.ActualWidth;
                }
            }
        }

        /// <summary>
        /// Executes the command to advise the view model of the sort order.
        /// </summary>
        private void ExecuteSortCommand()
        {
            // Collect the columns that describe the sort order from most significant columns to the least significant.
            Collection<SortDescription> sortDescriptions = new Collection<SortDescription>();
            foreach (ColumnViewColumn sortColumn in this.sortColumns)
            {
                sortDescriptions.Add(
                    new SortDescription
                    {
                        Direction = sortColumn.SortDirection,
                        PropertyName = sortColumn.SortMemberPath
                    });
            }

            // This will tell the view model that the view has a new sort order.
            if (GlobalCommands.Sort.CanExecute(sortDescriptions))
            {
                GlobalCommands.Sort.Execute(sortDescriptions);
            }
        }

        /// <summary>
        /// Handles a change to the column collection.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Provides data for the CollectionChanged event.</param>
        private void OnColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // Use the dictionary to invoke the correct action for handling the change to the column collection.
            ColumnViewDefinition.collectionActions[notifyCollectionChangedEventArgs.Action](this, notifyCollectionChangedEventArgs);

            // The header panel is like a canvas and a stack panel.  Because elements can be moved around and animated, we're not going to stack them
            // in the 'ArrangeOverride', but we're going to do it here each time the column set is changed.
            this.ArrangeHeader();

            // This will recreate the template used for each row in the view.
            this.SetValue(ColumnViewDefinition.ItemTemplateProperty, this.GenerateRowTemplate());
        }

        /// <summary>
        /// Handles a change to the width of a column.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="columnWidthChangedEventArgs">Provides data for the CollectionChanged event.</param>
        private void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs columnWidthChangedEventArgs)
        {
            // This will set the columns width to the value selected by the thumb.  This property change will propagate to the header and row
            // presenters where the other column will reposition themselves to account for the new width.
            ColumnViewColumn columnViewColumn = sender as ColumnViewColumn;
            columnViewColumn.SetActualWidth(columnWidthChangedEventArgs.Width);

            // This will re-arrange the header panel to account for the new column sizes.
            this.ArrangeHeader();
        }

        /// <summary>
        /// Handles a request to sort the view.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="sortChangedEventArgs">The event data.</param>
        private void OnSortChanged(object sender, SortChangedEventArgs sortChangedEventArgs)
        {
            // If the column that generated the request is the most recent item in the list, then we're going to handle this event as a request to
            // toggle the current direction of the sort.
            ColumnViewColumn columnViewColumn = sender as ColumnViewColumn;
            if (this.sortColumns.Count != 0 && this.sortColumns[this.sortColumns.Count - 1] == columnViewColumn)
            {
                columnViewColumn.SortDirection = ColumnViewDefinition.sortDirectionMap[columnViewColumn.SortDirection];
            }
            else
            {
                // Extended sorts are selected by holding down the shift key while clicking on a column heading.  It creates a compound sort using
                // multiple columns.
                if (sortChangedEventArgs.IsExtended)
                {
                    // If the sort description doesn't already contain the sort column, then we're going to add it to the collection of columns used
                    // to describe the sort.  If simple sorting is invoked, then this will be the only column used to sort.  If extended sorting is
                    // requested (by holding down the shift key), then the new column is added to the compound sort description.
                    if (!this.sortColumns.Contains(columnViewColumn))
                    {
                        columnViewColumn.SortDirection = SortDirection.Ascending;
                        this.sortColumns.Add(columnViewColumn);
                    }
                }
                else
                {
                    // Simple sorts will clear out the previous sort.
                    this.sortColumns.Clear();
                    columnViewColumn.SortDirection = SortDirection.Ascending;
                    this.sortColumns.Add(columnViewColumn);
                }
            }

            // This will remove the sorting from any of the columns no longer part of the sort.
            foreach (ColumnViewColumn siblingColumn in this.Columns)
            {
                if (!this.sortColumns.Contains(siblingColumn) && siblingColumn.SortDirection != SortDirection.NotSorted)
                {
                    siblingColumn.SortDirection = SortDirection.NotSorted;
                }
            }

            // Execute the command that advises the view model of the new sort order.
            this.ExecuteSortCommand();
        }

        /// <summary>
        /// Reconciles the sort indicators on the header with the global sort order.
        /// </summary>
        /// <param name="sortDescriptions">The column on which to sort.</param>
        private void Sort(Collection<SortDescription> sortDescriptions)
        {
            // This list keeps track of the currently selected columns used to sort.  We're going to replace the contents with the new sort order
            // specified in the command data.
            this.sortColumns.Clear();

            // The general idea here is that a command to set the sort order can come internally from this control, or externally from the ItemsView
            // or other task that might want to change the displayed order.  While the ordering of the actual items is handled by a CollectionView,
            // the indicators on a set of columns is handled by this view definition.  This code will reconcile the indicators on the header with the
            // sort descriptions.  We leave it as an article of faith that some view model somewhere will organize the items according to the same
            // order so that the headers agree with the order of the items in the display.
            foreach (ColumnViewColumn columnViewColumn in this.Columns)
            {
                // This is designed so that it won't update the properties of the ColumnViewColumns when the already agree with the given sort order.
                bool isFound = false;
                int sortOrder = 0;
                foreach (SortDescription sortDescription in sortDescriptions)
                {
                    if (columnViewColumn.SortMemberPath == sortDescription.PropertyName)
                    {
                        isFound = true;
                        this.sortColumns.Add(columnViewColumn);

                        if (columnViewColumn.SortDirection != sortDescription.Direction)
                        {
                            columnViewColumn.SortDirection = sortDescription.Direction;
                        }

                        if (columnViewColumn.SortOrder != sortOrder)
                        {
                            columnViewColumn.SortOrder = sortOrder++;
                        }
                    }
                }

                // If the ColumnViewColumn isn't specified in the sort order, then make sure it doesn't have indicators left over from some previous
                // sort.
                if (!isFound)
                {
                    if (columnViewColumn.SortDirection != SortDirection.NotSorted)
                    {
                        columnViewColumn.SortDirection = SortDirection.NotSorted;
                    }
                }
            }
        }
    }
}