// <copyright file="ColumnViewHeaderPanel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a panel for the column header of a <see cref="ColumnViewDefinition"/>.
    /// </summary>
    public class ColumnViewHeaderPanel : Canvas
    {
        /// <summary>
        /// The owner of this panel.
        /// </summary>
        private ColumnViewDefinition columnView;

        /// <summary>
        /// This is the primary source of scrolling instructions.
        /// </summary>
        private ScrollViewer mainScrollViewer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewHeaderPanel"/> class.
        /// </summary>
        public ColumnViewHeaderPanel()
        {
            // We need to know when the visual parent has changed in order to bind and unbind the columns.
            this.LayoutUpdated += this.OnLayoutUpdated;
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of layout.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children. </param>
        /// <returns>The actual size that is used after the element is arranged in layout.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // The items are positioned in the panel according to the left edge and the measured width of the header.  The left edge is allowed to
            // move as the user drags columns to the desired location.  The elements are also animated during the column drag-and-drop operations.
            // Finally, each column header is stretched to be as large as the largest header element.
            foreach (ColumnViewColumnHeader columnHeader in this.Children)
            {
                columnHeader.Arrange(new Rect(Canvas.GetLeft(columnHeader), 0.0, columnHeader.DesiredSize.Width, finalSize.Height));
            }

            // The size is not altered through the layout of the child elements.
            return finalSize;
        }

        /// <summary>
        /// Measures the child elements of a Canvas in anticipation of arranging them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="availableSize">An upper limit Size that should not be exceeded.</param>
        /// <returns>A Size that represents the size that is required to arrange child content.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // This keeps track of the space needed by this row.
            Size totalSize = default(Size);

            // This panel acts like a canvas and a stack panel.  Like a canvas, elements can move around, but like a stack panel, the height and
            // width are determined by the linear arrangement of the child elements.
            foreach (ColumnViewColumnHeader columnHeader in this.Children)
            {
                // Measure each column header.  The width of this panel is the aggregate of all the columns and the height is the maximum of all the
                // columns.
                columnHeader.Measure(new Size(availableSize.Width, double.PositiveInfinity));
                totalSize.Width += columnHeader.DesiredSize.Width;
                totalSize.Height = Math.Max(totalSize.Height, columnHeader.DesiredSize.Height);
            }

            // This is the total space required for this panel.
            return totalSize;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call ApplyTemplate.
        /// </summary>
        /// <param name="oldParent">The visual previous parent.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            // This will hook us into the scroll viewer so we can keep track of changes to the viewport.
            this.mainScrollViewer = this.TemplatedParent as ScrollViewer;
            if (this.mainScrollViewer != null)
            {
                this.mainScrollViewer.ScrollChanged += this.OnMasterScrollChanged;
            }

            // Allow the base class to handle the reset of parent change event.
            base.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// Handles a change to the layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data. </param>
        private void OnLayoutUpdated(object sender, object e)
        {
            // The header panel is going to subscribe to any changes made to the set of columns.  In order to do this properly, it needs to reference and dereference the
            // ColumnViewDefinition as it is loaded and unloaded as a view.
            ItemsView itemsView = VisualTreeExtensions.FindParent<ItemsView>(this);

            // If this panel has no parent, then it has been unloaded and must release the reference to the view containing the column descriptions.
            // If the panel has been given an ItemsView ancestor, then we want to find the column descriptions and listen for any changes.
            if (itemsView == null)
            {
                // At this point, we have been disconnected from the visual tree and we want to remove the reference to the column set.
                if (this.columnView != null)
                {
                    this.columnView.Columns.CollectionChanged -= this.OnColumnsCollectionChanged;
                    this.columnView = null;
                }
            }
            else
            {
                // At this point we've been added to the visual tree and we want to listen for any changes to the column set.
                ColumnViewDefinition currentColumnViewDefinition = itemsView.Current as ColumnViewDefinition;
                if (this.columnView != currentColumnViewDefinition)
                {
                    // Check to see if we've been connected or disconnected.  Because of the capricious nature of the 'OnLayoutUpdated', this can
                    // happen in more than one place.
                    if (currentColumnViewDefinition == null)
                    {
                        // At this point, we have been disconnected from the visual tree and we want to remove the reference to the column set.
                        this.columnView.Columns.CollectionChanged -= this.OnColumnsCollectionChanged;
                        this.columnView = null;
                    }
                    else
                    {
                        // The 'OnLayoutUpdated' is called many times for different reasons.  This makes sure that we only reference the column
                        // collection once.
                        this.columnView = currentColumnViewDefinition;
                        this.columnView.Columns.CollectionChanged += this.OnColumnsCollectionChanged;

                        // This will force the initial configuration of the column headers in the panel.
                        this.OnColumnsCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                }
            }
        }

        /// <summary>
        /// Handles a change to the collection of <see cref="ColumnViewColumn"/>.
        /// </summary>
        /// <param name="sender">The object where the event is attached.</param>
        /// <param name="notifyCollectionChangedEventArgs">The event data.</param>
        private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // Whenever the set of columns changes, clear out the previous column headers and create a new set.
            this.Children.Clear();

            // This will run through each of the visible columns in the view and create a column header.
            foreach (ColumnViewColumn columnViewColumn in this.columnView.Columns)
            {
                if (columnViewColumn.IsVisible)
                {
                    // This will create the visual tree of the column header and provide it with a column definition as a data context.  The
                    // properties that define the shape, font, position, commands, and so on will be bound to the column header definitions when the
                    // template is applied.
                    ColumnViewColumnHeader columnViewColumnHeader = new ColumnViewColumnHeader();
                    columnViewColumnHeader.DataContext = columnViewColumn;

                    // Add this column header to the panel.
                    this.Children.Add(columnViewColumnHeader);
                }
            }
        }

        /// <summary>
        /// Occurs when changes are detected to the scroll position, extent, or viewport size.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="scrollChangedEventArgs">The event data.</param>
        private void OnMasterScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            // This header presenter is ususally shrowded in a ScrollViewer to handle the scrolling.  When the master scroll viewer (which provides
            // the instructions for scrolling, as opposed to the parent ScrollViewer which is simply a panel that moves behind the master viewport)
            // tells us to scroll, we'll change the offset of the parent viewer (if it exists).
            ScrollViewer scrollViewer = this.Parent as ScrollViewer;
            if (scrollViewer != null && this.mainScrollViewer == scrollChangedEventArgs.OriginalSource)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollChangedEventArgs.HorizontalOffset);
            }
        }
    }
}