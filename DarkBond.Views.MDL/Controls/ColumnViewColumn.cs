// <copyright file="ColumnViewColumn.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Input;
    using DarkBond.ViewModels.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Defines the attributes of a column.
    /// </summary>
    [ContentProperty(Name = "CellTemplate")]
    public class ColumnViewColumn : DependencyObject
    {
        /// <summary>
        /// The ActualWidth DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ActualWidthProperty = DependencyProperty.Register(
            "ActualWidth",
            typeof(double),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The CellBorderBrush DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellBorderBrushProperty = DependencyProperty.Register(
            "CellBorderBrush",
            typeof(Brush),
            typeof(ColumnViewColumn),
            new PropertyMetadata(default(Brush)));

        /// <summary>
        /// The CellBorderThickness DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellBorderThicknessProperty = DependencyProperty.Register(
            "CellBorderThickness",
            typeof(Thickness),
            typeof(ColumnViewColumn),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// The CellPadding DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellPaddingProperty = DependencyProperty.Register(
            "CellPadding",
            typeof(Thickness),
            typeof(ColumnViewColumn),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// The CellTemplate DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register(
            "CellTemplate",
            typeof(DataTemplate),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The Description DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The DesiredLeft DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DesiredLeftProperty = DependencyProperty.Register(
            "DesiredLeft",
            typeof(double),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The HasFilters DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HasFiltersProperty = DependencyProperty.Register(
            "HasFilters",
            typeof(bool),
            typeof(ColumnViewColumn),
            new PropertyMetadata(false));

        /// <summary>
        /// The Header DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The HeaderPadding DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register(
            "HeaderPadding",
            typeof(Thickness),
            typeof(ColumnViewColumn),
            new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// The HeaderTemplate DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The HorizontalCellAlignment DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HorizontalCellAlignmentProperty = DependencyProperty.Register(
            "HorizontalCellAlignment",
            typeof(HorizontalAlignment),
            typeof(ColumnViewColumn),
            new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// The HorizontalHeaderAlignment DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HorizontalHeaderAlignmentProperty = DependencyProperty.Register(
            "HorizontalHeaderAlignment",
            typeof(HorizontalAlignment),
            typeof(ColumnViewColumn),
            new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// The IsVisible DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
            "IsVisible",
            typeof(bool),
            typeof(ColumnViewColumn),
            new PropertyMetadata(true));

        /// <summary>
        /// The Left DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left",
            typeof(double),
            typeof(ColumnViewColumn),
            new PropertyMetadata(0.0));

        /// <summary>
        /// The MaxWidth DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
            "MaxWidth",
            typeof(double),
            typeof(ColumnViewColumn),
            new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// The MinWidth DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
            "MinWidth",
            typeof(double),
            typeof(ColumnViewColumn),
            new PropertyMetadata(20.0));

        /// <summary>
        /// The SortDirection DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(SortDirection),
            typeof(ColumnViewColumn),
            new PropertyMetadata(SortDirection.NotSorted));

        /// <summary>
        /// The SortOrder DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(
            "SortOrder",
            typeof(int),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The SortMemberPath DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SortMemberPathProperty = DependencyProperty.Register(
            "SortMemberPath",
            typeof(string),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The VerticalCellAlignment DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty VerticalCellAlignmentProperty = DependencyProperty.Register(
            "VerticalCellAlignment",
            typeof(VerticalAlignment),
            typeof(ColumnViewColumn),
            new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// The VerticalHeaderAlignment DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty VerticalHeaderAlignmentProperty = DependencyProperty.Register(
            "VerticalHeaderAlignment",
            typeof(VerticalAlignment),
            typeof(ColumnViewColumn),
            new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// The Width DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(GridLength),
            typeof(ColumnViewColumn),
            new PropertyMetadata(GridLength.Auto, ColumnViewColumn.OnWidthPropertyChanged));

        /// <summary>
        /// The ZIndex DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
            "ZIndex",
            typeof(int),
            typeof(ColumnViewColumn),
            null);

        /// <summary>
        /// The delegate for the 'End Drag' command.
        /// </summary>
        private DelegateCommand completeDragField;

        /// <summary>
        /// The delegate for the 'Drag' command.
        /// </summary>
        private DelegateCommand<ColumnDragParameter> dragField;

        /// <summary>
        /// The delegate for the 'Pressed' command.
        /// </summary>
        private DelegateCommand<bool> pressField;

        /// <summary>
        /// The delegate for the 'Start Drag' command.
        /// </summary>
        private DelegateCommand startDragField;

        /// <summary>
        /// The list of filters use to select the rows in the view.
        /// </summary>
        private ObservableCollection<FilterDefinition> filterCollection = new ObservableCollection<FilterDefinition>();

        /// <summary>
        /// The delegate for the 'Set Width' command.
        /// </summary>
        private DelegateCommand<double> setWidthField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewColumn"/> class.
        /// </summary>
        public ColumnViewColumn()
        {
            // In order to keep the 'HasFilters' property reconciled to the list of filters, we need to monitor the collection for changes.
            this.filterCollection.CollectionChanged += (s, e) =>
            {
                this.SetValue(ColumnViewColumn.HasFiltersProperty, this.filterCollection.Count != 0);
            };

            // These commands are handled by this object.
            this.startDragField = new DelegateCommand(this.OnStartedDrag);
            this.dragField = new DelegateCommand<ColumnDragParameter>(this.OnDrag);
            this.completeDragField = new DelegateCommand(this.OnCompletedDrag);
            this.pressField = new DelegateCommand<bool>(this.OnPressed);
            this.setWidthField = new DelegateCommand<double>(this.OnSetWidth);
        }

        /// <summary>
        /// Occurs at the end of a column drag operation.
        /// </summary>
        internal event EventHandler<EventArgs> ColumnDragCompleted;

        /// <summary>
        /// Occurs at the start of a column drag operation.
        /// </summary>
        internal event EventHandler<EventArgs> ColumnDragStarted;

        /// <summary>
        /// Occurs when the column is dragged.
        /// </summary>
        internal event EventHandler<ColumnDragEventArgs> ColumnDragged;

        /// <summary>
        /// Occurs when the column width has changed.
        /// </summary>
        internal event EventHandler<ColumnWidthChangedEventArgs> ColumnWidthChanged;

        /// <summary>
        /// Occurs when the sort column has changed.
        /// </summary>
        internal event EventHandler<SortChangedEventArgs> SortChanged;

        /// <summary>
        /// Gets the current width of the column, in device-independent units (1/96 inches per unit).
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return (double)this.GetValue(ColumnViewColumn.ActualWidthProperty);
            }
        }

        /// <summary>
        /// Gets or sets the cell border brush.
        /// </summary>
        public Brush CellBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(ColumnViewColumn.CellBorderBrushProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.CellBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cell border thickness.
        /// </summary>
        public Thickness CellBorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewColumn.CellBorderThicknessProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.CellBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cell padding.
        /// </summary>
        public Thickness CellPadding
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewColumn.CellPaddingProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.CellPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used by the cell to present data.
        /// </summary>
        public DataTemplate CellTemplate
        {
            get
            {
                return this.GetValue(ColumnViewColumn.CellTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(ColumnViewColumn.CellTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the command that indicates a column drag operation has completed.
        /// </summary>
        public ICommand CompleteDrag
        {
            get
            {
                return this.completeDragField;
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used by the cell to present data.
        /// </summary>
        public double DesiredLeft
        {
            get
            {
                return (double)this.GetValue(ColumnViewColumn.DesiredLeftProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.DesiredLeftProperty, value);
            }
        }

        /// <summary>
        /// Gets the command that indicates a column has been dragged.
        /// </summary>
        public ICommand Drag
        {
            get
            {
                return this.dragField;
            }
        }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetValue(ColumnViewColumn.DescriptionProperty) as string;
            }

            set
            {
                this.SetValue(ColumnViewColumn.DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="FilterDefinition"/> use to select the filters for the view.
        /// </summary>
        public ObservableCollection<FilterDefinition> Filters
        {
            get
            {
                return this.filterCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column has filters defined for it.
        /// </summary>
        public bool HasFilters
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumn.HasFiltersProperty);
            }
        }

        /// <summary>
        /// Gets or sets the header of the column.
        /// </summary>
        public object Header
        {
            get
            {
                return this.GetValue(ColumnViewColumn.HeaderProperty) as object;
            }

            set
            {
                this.SetValue(ColumnViewColumn.HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header padding.
        /// </summary>
        public Thickness HeaderPadding
        {
            get
            {
                return (Thickness)this.GetValue(ColumnViewColumn.HeaderPaddingProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.HeaderPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used by the header to present data.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(ColumnViewColumn.HeaderTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(ColumnViewColumn.HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment for the cells.
        /// </summary>
        public HorizontalAlignment HorizontalCellAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(ColumnViewColumn.HorizontalCellAlignmentProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.HorizontalCellAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment for the headers.
        /// </summary>
        public HorizontalAlignment HorizontalHeaderAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(ColumnViewColumn.HorizontalHeaderAlignmentProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.HorizontalHeaderAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (bool)this.GetValue(ColumnViewColumn.IsVisibleProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.IsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left edge of the column.
        /// </summary>
        public double Left
        {
            get
            {
                return (double)this.GetValue(ColumnViewColumn.LeftProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.LeftProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum width constraint of the column.
        /// </summary>
        public double MaxWidth
        {
            get
            {
                return (double)this.GetValue(ColumnViewColumn.MaxWidthProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.MaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum width constraint of the column.
        /// </summary>
        public double MinWidth
        {
            get
            {
                return (double)this.GetValue(ColumnViewColumn.MinWidthProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.MinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the command that indicates a column header has been pressed.
        /// </summary>
        public ICommand Press
        {
            get
            {
                return this.pressField;
            }
        }

        /// <summary>
        /// Gets the command that indicates a column drag operation has started.
        /// </summary>
        public ICommand StartDrag
        {
            get
            {
                return this.startDragField;
            }
        }

        /// <summary>
        /// Gets the handler for a 'Width Changed' command.
        /// </summary>
        public ICommand SetWidth
        {
            get
            {
                return this.setWidthField;
            }
        }

        /// <summary>
        /// Gets or sets the sort direction (ascending or descending) of the column.
        /// </summary>
        public SortDirection SortDirection
        {
            get
            {
                return (SortDirection)this.GetValue(ColumnViewColumn.SortDirectionProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.SortDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sort order of the column.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return (int)this.GetValue(ColumnViewColumn.SortOrderProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.SortOrderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the member used to sort the view when this column is selected.
        /// </summary>
        public string SortMemberPath
        {
            get
            {
                return this.GetValue(ColumnViewColumn.SortMemberPathProperty) as string;
            }

            set
            {
                this.SetValue(ColumnViewColumn.SortMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment for the cells.
        /// </summary>
        public VerticalAlignment VerticalCellAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(ColumnViewColumn.VerticalCellAlignmentProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.VerticalCellAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment for the headers.
        /// </summary>
        public VerticalAlignment VerticalHeaderAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(ColumnViewColumn.VerticalHeaderAlignmentProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.VerticalHeaderAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the column width or automatic sizing mode.
        /// </summary>
        public GridLength Width
        {
            get
            {
                return (GridLength)this.GetValue(ColumnViewColumn.WidthProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Index of the column header.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(ColumnViewColumn.ZIndexProperty);
            }

            set
            {
                this.SetValue(ColumnViewColumn.ZIndexProperty, value);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", this.Header);
        }

        /// <summary>
        /// Sets the actual width of the column.
        /// </summary>
        /// <param name="width">The actual width.</param>
        internal void SetActualWidth(double width)
        {
            // Set the actual width of the column.  This value will be propagated down to all the cells in the view.
            this.SetValue(ColumnViewColumn.ActualWidthProperty, width);
        }

        /// <summary>
        /// Invoked when the effective property value of the Width property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnWidthPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // The 'ActualWidth' tracks the actual width of the column (such as during a drag operation) but it is seeded whenever the 'Width'
            // property changes.
            ColumnViewColumn columnViewcolumn = dependencyObject as ColumnViewColumn;
            GridLength gridLength = (GridLength)dependencyPropertyChangedEventArgs.NewValue;
            if (gridLength.GridUnitType == GridUnitType.Pixel)
            {
                columnViewcolumn.SetActualWidth(gridLength.Value);
            }
        }

        /// <summary>
        /// Called when the column header is dragged.
        /// </summary>
        /// <param name="columnDragParameter">Describes the drag operation.</param>
        private void OnDrag(ColumnDragParameter columnDragParameter)
        {
            // This will invoke the handler for the click action.
            if (this.ColumnDragged != null)
            {
                this.ColumnDragged(this, new ColumnDragEventArgs(columnDragParameter.Delta, columnDragParameter.MousePosition));
            }
        }

        /// <summary>
        /// Called when the drag operation is completed.
        /// </summary>
        private void OnCompletedDrag()
        {
            // This will invoke the handler for the click action.
            if (this.ColumnDragCompleted != null)
            {
                this.ColumnDragCompleted(this, new EventArgs());
            }
        }

        /// <summary>
        /// Handles a mouse click on the column.
        /// </summary>
        /// <param name="isExtended">true to indicate an extended click operation (shift key pressed).</param>
        private void OnPressed(bool isExtended)
        {
            // This will generate a command to resort the view if the selected column has a property on which to sort.
            if (!string.IsNullOrEmpty(this.SortMemberPath))
            {
                this.SortChanged?.Invoke(this, new SortChangedEventArgs(isExtended));
            }
        }

        /// <summary>
        /// Handles a change to the column width.
        /// </summary>
        /// <param name="width">The new width of the column.</param>
        private void OnSetWidth(double width)
        {
            // This will invoke the handler for a new column width.
            if (this.ColumnWidthChanged != null)
            {
                this.ColumnWidthChanged(this, new ColumnWidthChangedEventArgs(width));
            }
        }

        /// <summary>
        /// Handles the start of a column drag operation.
        /// </summary>
        private void OnStartedDrag()
        {
            // This will invoke the handler for the click action.
            if (this.ColumnDragStarted != null)
            {
                this.ColumnDragStarted(this, new EventArgs());
            }
        }
    }
}