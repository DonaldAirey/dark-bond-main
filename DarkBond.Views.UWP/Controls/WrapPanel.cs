// <copyright file="WrapPanel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Positions child elements sequentially from left to right or top to bottom.  When elements extend beyond the panel edge, elements are
    /// positioned in the next row or column.
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Identifies the <see cref="P:WinRTXamlToolkit.Controls.WrapPanel.ItemHeight" /> DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight",
            typeof(double),
            typeof(WrapPanel),
            new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:WinRTXamlToolkit.Controls.WrapPanel.ItemWidth" /> DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth",
            typeof(double),
            typeof(WrapPanel),
            new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:WinRTXamlToolkit.Controls.WrapPanel.Orientation" /> DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(WrapPanel),
            new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        /// <summary>
        /// A value indicating whether a DependencyProperty change handler should ignore the next change notification.  This is used to reset the
        /// value of properties without performing any of the actions in their change handlers.
        /// </summary>
        private bool ignorePropertyChange;

        /// <summary>
        /// Gets or sets the height of the layout area for each item that is contained in a <see cref="WrapPanel" /> .
        /// </summary>
        /// <value>
        /// The height applied to the layout area of each item that is contained within a <see cref="WrapPanel" /> .  The
        /// default value is <see cref="double.NaN" /> .
        /// </value>
        public double ItemHeight
        {
            get
            {
                return (double)this.GetValue(WrapPanel.ItemHeightProperty);
            }

            set
            {
                this.SetValue(WrapPanel.ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the layout area for each item that is contained in a <see cref="WrapPanel" /> .
        /// </summary>
        /// <value>
        /// The width that applies to the layout area of each item that is contained in a <see cref="WrapPanel" /> .  The
        /// default value is <see cref="F:System.Double.NaN" /> .
        /// </value>
        public double ItemWidth
        {
            get
            {
                return (double)this.GetValue(WrapPanel.ItemWidthProperty);
            }

            set
            {
                this.SetValue(WrapPanel.ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the direction in which child elements are arranged.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(WrapPanel.OrientationProperty);
            }

            set
            {
                this.SetValue(WrapPanel.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Arranges and sizes the <see cref="WrapPanel" /> control and its child elements.
        /// </summary>
        /// <param name="finalSize">
        /// The area within the parent that the <see cref="WrapPanel" /> should use arrange itself and its children.
        /// </param>
        /// <returns>The actual size used by the <see cref="WrapPanel" /> .</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Variables tracking the size of the current line, and the maximum size available to fill.  Note that the line might represent a row or
            // a column depending on the orientation.
            Orientation orientation = this.Orientation;
            OrientedSize lineSize = new OrientedSize(orientation);
            OrientedSize maximumSize = new OrientedSize(orientation, finalSize.Width, finalSize.Height);

            // Determine the constraints for individual items
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            double indirectOffset = 0;
            double? directDelta = (orientation == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);

            // Measure each of the Children.  We will process the elements one line at a time, just like during measure, but we will wait until we've
            // completed an entire line of elements before arranging them.  The lineStart and lineEnd variables track the size of the currently
            // arranged line.
            UIElementCollection children = this.Children;
            int count = children.Count;
            int lineStart = 0;
            for (int lineEnd = 0; lineEnd < count; lineEnd++)
            {
                UIElement element = children[lineEnd];

                // Get the size of the element
                OrientedSize elementSize = new OrientedSize(
                    orientation,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (lineSize.Direct + elementSize.Direct > maximumSize.Direct)
                {
                    // Then we just completed a line and we should arrange it
                    this.ArrangeLine(lineStart, lineEnd, directDelta, indirectOffset, lineSize.Indirect);

                    // Move the current element to a new line
                    indirectOffset += lineSize.Indirect;
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size
                    if (elementSize.Direct > maximumSize.Direct)
                    {
                        // Arrange the element as a single line
                        this.ArrangeLine(lineEnd, ++lineEnd, directDelta, indirectOffset, elementSize.Indirect);

                        // Move to a new line
                        indirectOffset += lineSize.Indirect;
                        lineSize = new OrientedSize(orientation);
                    }

                    // Advance the start index to a new line after arranging
                    lineStart = lineEnd;
                }
                else
                {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Arrange any elements on the last line
            if (lineStart < count)
            {
                this.ArrangeLine(lineStart, count, directDelta, indirectOffset, lineSize.Indirect);
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="WrapPanel" /> in anticipation of arranging them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="availableSize">The size available to child elements of the wrap panel.</param>
        /// <returns>The size required by the <see cref="WrapPanel" /> and its elements.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Variables tracking the size of the current line, the total size measured so far, and the maximum size available to fill.  Note that
            // the line might represent a row or a column depending on the orientation.
            Orientation orientation = this.Orientation;
            OrientedSize lineSize = new OrientedSize(orientation);
            OrientedSize totalSize = new OrientedSize(orientation);
            OrientedSize maximumSize = new OrientedSize(orientation, availableSize.Width, availableSize.Height);

            // Determine the constraints for individual items
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            Size itemSize = new Size(hasFixedWidth ? itemWidth : availableSize.Width, hasFixedHeight ? itemHeight : availableSize.Height);

            // Measure each of the Children
            foreach (UIElement element in this.Children)
            {
                // Determine the size of the element
                element.Measure(itemSize);
                OrientedSize elementSize = new OrientedSize(
                    orientation,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (lineSize.Direct + elementSize.Direct > maximumSize.Direct)
                {
                    // Update the total size with the direct and indirect growth
                    // for the current line
                    totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
                    totalSize.Indirect += lineSize.Indirect;

                    // Move the element to a new line
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size, place it on a line by itself
                    if (elementSize.Direct > maximumSize.Direct)
                    {
                        // Update the total size for the line occupied by this single element
                        totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
                        totalSize.Indirect += elementSize.Indirect;

                        // Move to a new line
                        lineSize = new OrientedSize(orientation);
                    }
                }
                else
                {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Update the total size with the elements on the last line
            totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            totalSize.Indirect += lineSize.Indirect;

            // Return the total size required as an un-oriented quantity
            return new Size(totalSize.Width, totalSize.Height);
        }

        /// <summary>
        /// Property changed handler for ItemHeight and ItemWidth.
        /// </summary>
        /// <param name="dependencyObject">WrapPanel that changed its ItemHeight or ItemWidth.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event arguments.</param>
        private static void OnItemHeightOrWidthPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            WrapPanel source = (WrapPanel)dependencyObject;
            double value = (double)dependencyPropertyChangedEventArgs.NewValue;

            // Ignore the change if requested
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }

            // Validate the length (which must either be NaN or a positive, or a finite number).
            if (!double.IsNaN(value) && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                // Reset the property to its original state before throwing
                source.ignorePropertyChange = true;
                source.SetValue(dependencyPropertyChangedEventArgs.Property, (double)dependencyPropertyChangedEventArgs.OldValue);

                string message = string.Format(CultureInfo.InvariantCulture, "Invalid width or height value {0}", value);
                throw new InvalidOperationException(message);
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        /// <summary>
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">WrapPanel that changed its Orientation.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event arguments.</param>
        private static void OnOrientationPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            WrapPanel source = (WrapPanel)dependencyObject;
            Orientation value = (Orientation)dependencyPropertyChangedEventArgs.NewValue;

            // Ignore the change if requested
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }

            // Validate the Orientation
            if (value != Orientation.Horizontal && value != Orientation.Vertical)
            {
                // Reset the property to its original state before throwing
                source.ignorePropertyChange = true;
                source.SetValue(WrapPanel.OrientationProperty, (Orientation)dependencyPropertyChangedEventArgs.OldValue);
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid Orientation={0}", value));
            }

            // Orientation affects measuring.
            source.InvalidateMeasure();
        }

        /// <summary>
        /// Arrange a sequence of elements in a single line.
        /// </summary>
        /// <param name="lineStart">Index of the first element in the sequence to arrange.</param>
        /// <param name="lineEnd">Index of the last element in the sequence to arrange.</param>
        /// <param name="directDelta">Optional fixed growth in the primary direction.</param>
        /// <param name="indirectOffset">Offset of the line in the indirect direction.</param>
        /// <param name="indirectGrowth">Shared indirect growth of the elements on this line.</param>
        private void ArrangeLine(int lineStart, int lineEnd, double? directDelta, double indirectOffset, double indirectGrowth)
        {
            double directOffset = 0.0;

            Orientation o = this.Orientation;
            bool isHorizontal = o == Orientation.Horizontal;

            UIElementCollection children = this.Children;
            for (int index = lineStart; index < lineEnd; index++)
            {
                // Get the size of the element
                UIElement element = children[index];
                OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                // Determine if we should use the element's desired size or the
                // fixed item width or height
                double directGrowth = directDelta != null ?
                    directDelta.Value :
                    elementSize.Direct;

                // Arrange the element
                Rect bounds = isHorizontal ?
                    new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                element.Arrange(bounds);

                directOffset += directGrowth;
            }
        }
    }
}