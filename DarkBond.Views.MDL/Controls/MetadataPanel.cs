// <copyright file="MetadataPanel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Panel for arranging metadata.
    /// </summary>
    public class MetadataPanel : WrapPanel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataPanel"/> class.
        /// </summary>
        public MetadataPanel()
        {
            // Initialize the object.
            this.Orientation = Orientation.Vertical;
        }

        /// <summary>
        /// Arranges and sizes the <see cref="MetadataPanel" /> control and its child elements.
        /// </summary>
        /// <param name="finalSize">
        /// The area within the parent that the <see cref="MetadataPanel" /> should use arrange itself and its children.
        /// </param>
        /// <returns>The actual size used by the <see cref="MetadataPanel" /> .</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Variables tracking the size of the current line, and the maximum size available to fill.  Note that the line might represent a row or
            // a column depending on the orientation.  Also note that the metadata tags have their own accounting so they can be aligned.
            Orientation orientation = this.Orientation;
            OrientedSize lineSize = new OrientedSize(orientation);
            OrientedSize maximumSize = new OrientedSize(orientation, finalSize.Width, finalSize.Height);
            OrientedSize maxLabelSize = new OrientedSize(orientation);

            // Determine the constraints for individual items
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            double indirectOffset = 0;
            double? directDelta = (orientation == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);

            // Arrange each of the Children.  We will process the elements one line at a time, just like during measure, but we will wait until we've
            // completed an entire line of elements before arranging them so that column/row metrics can be evaluated.  The lineStart and lineEnd
            // variables track the size of the currently arranged line.
            UIElementCollection children = this.Children;
            int count = children.Count;
            int lineStart = 0;
            for (int lineEnd = 0; lineEnd < count; lineEnd++)
            {
                // This iteration will arrange this child in the panel.
                UIElement uiElement = children[lineEnd];

                // Get the size of the element
                OrientedSize elementSize = new OrientedSize(
                    orientation,
                    hasFixedWidth ? itemWidth : uiElement.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : uiElement.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (lineSize.Direct + elementSize.Direct > maximumSize.Direct)
                {
                    // The maximum indirect size of the metadata labels can't be evaluated until we've looked at every item in the line.  At this
                    // point we've determined that a line is complete and we can measure it using the boundaries of only those elements on the line.
                    this.ArrangeLine(lineStart, lineEnd, directDelta, indirectOffset, lineSize.Indirect + maxLabelSize.Indirect);

                    // Move the current element to a new line taking into account the maximum width of the metadata labels.
                    indirectOffset += lineSize.Indirect + maxLabelSize.Indirect;

                    // This will seed the maximum metadata label variable for the remainder of the line.  Also note that the indirect size of the
                    // actual element is reduced by the indirect label size.
                    MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                    if (metadataHeader != null)
                    {
                        OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                        elementSize.Indirect -= labelSize.Indirect;
                        maxLabelSize.Indirect = labelSize.Indirect;
                    }

                    // This will hold the aggregate direct and the maximum indirect values as we arrange the line.
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size
                    if (elementSize.Direct > maximumSize.Direct)
                    {
                        // Arrange the element as a single line
                        this.ArrangeLine(lineEnd, ++lineEnd, directDelta, indirectOffset, elementSize.Indirect + maxLabelSize.Indirect);

                        // Move to a new line
                        indirectOffset += lineSize.Indirect + maxLabelSize.Indirect;
                        lineSize = new OrientedSize(orientation);
                    }

                    // Advance the start index to a new line after arranging
                    lineStart = lineEnd;
                }
                else
                {
                    // If the line contains a metadata label then we're going to align all the labels.  The general idea is to calculate the maximum
                    // length of all the labels independently of the content and then calculate a justification so they all line up.
                    MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                    if (metadataHeader != null)
                    {
                        OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                        elementSize.Indirect -= labelSize.Indirect;
                        maxLabelSize.Indirect = Math.Max(maxLabelSize.Indirect, labelSize.Indirect);
                    }

                    // Aggregate the line in the direction of the orientation and calculate the maximum width or height in the indirect axis.  Note
                    // that the indirect length is just the content - the length of the label has been removed and will be managed separately.
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // If any lines are left over, then arrange then on the final line.
            if (lineStart < count)
            {
                this.ArrangeLine(lineStart, count, directDelta, indirectOffset, lineSize.Indirect + maxLabelSize.Indirect);
            }

            // This is the final size of this panel.
            return finalSize;
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of the layout cycle.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this object can give to child objects.  Infinity can be specified as a value to indicate that the object will
        /// size to whatever content is available.
        /// </param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects or based
        /// on other considerations such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // Variables tracking the size of the current line, the total size measured so far, and the maximum size available to fill.  Note that
            // the line might represent a row or a column depending on the orientation.
            Orientation orientation = this.Orientation;
            OrientedSize lineSize = new OrientedSize(orientation);
            OrientedSize totalSize = new OrientedSize(orientation);
            OrientedSize maximumSize = new OrientedSize(orientation, availableSize.Width, availableSize.Height);
            OrientedSize maxLabelSize = new OrientedSize(orientation);

            // Determine if constraints exist for individual items and, if so, what they are.
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            Size itemSize = new Size(hasFixedWidth ? itemWidth : availableSize.Width, hasFixedHeight ? itemHeight : availableSize.Height);

            // Measure each of the children in the panel.  If a line exceeds the constraints a new row/column is continued in the next available
            // space.  Each row/column will be only as wide/high as the largest element on that line.
            foreach (UIElement uiElement in this.Children)
            {
                // Determine the size of the element
                uiElement.Measure(itemSize);
                OrientedSize elementSize = new OrientedSize(
                    orientation,
                    hasFixedWidth ? itemWidth : uiElement.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : uiElement.DesiredSize.Height);

                // If this element falls over the edge of the line
                if (lineSize.Direct + elementSize.Direct > maximumSize.Direct)
                {
                    // Update the total size with the direct and indirect growth for the current line
                    totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
                    totalSize.Indirect += lineSize.Indirect + maxLabelSize.Indirect;

                    // If the current visual element contains a metadata label, then we'll use it to calculate the width of the largest label in this
                    // line.  This maximum will be used below to calculate a justification margin that will align all the labels on their right edge.
                    MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                    if (metadataHeader != null)
                    {
                        OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                        elementSize.Indirect -= labelSize.Indirect;
                        maxLabelSize.Indirect = labelSize.Indirect;
                    }

                    // This provides the seed measurements for the new row/column.
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size, place it on a line all by itself.
                    if (elementSize.Direct > maximumSize.Direct)
                    {
                        // Update the total size for the line occupied by this single element
                        totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
                        totalSize.Indirect += elementSize.Indirect + maxLabelSize.Indirect;

                        // Move to a new line
                        lineSize = new OrientedSize(orientation);
                    }
                }
                else
                {
                    // If the current visual element contains a metadata label, then we'll use it to calculate the width of the largest label in this
                    // line.  This maximum will be used below to calculate a justification margin that will align all the labels on their right edge.
                    MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                    if (metadataHeader != null)
                    {
                        OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                        elementSize.Indirect -= labelSize.Indirect;
                        maxLabelSize.Indirect = Math.Max(maxLabelSize.Indirect, labelSize.Indirect);
                    }

                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Update the total size with the elements on the last line
            totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            totalSize.Indirect += lineSize.Indirect + maxLabelSize.Indirect;

            // This is the total size required by this panel (unpacked from the OrientationSize structure).
            return new Size(totalSize.Width, totalSize.Height);
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
            Orientation orientation = this.Orientation;
            bool isHorizontal = orientation == Orientation.Horizontal;
            UIElementCollection children = this.Children;

            // Measure the size of the metadata label column/row.  This is used to justify the metadata elements so the labels all are aligned to the
            // left.
            OrientedSize maxMetadataHeaderSize = new OrientedSize(orientation);
            for (int index = lineStart; index < lineEnd; index++)
            {
                // Get the size of the element
                UIElement uiElement = children[index];

                // If the current visual element contains a metadata label, then we'll use it to calculate the width of the largest label in this
                // line.  This maximum will be used below to calculate a justification margin that will align all the labels on their right edge.
                MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                if (metadataHeader != null)
                {
                    OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                    maxMetadataHeaderSize.Indirect = Math.Max(maxMetadataHeaderSize.Indirect, labelSize.Indirect);
                }
            }

            // Arrange each element on the line.
            for (int index = lineStart; index < lineEnd; index++)
            {
                // Get the size of the element
                UIElement uiElement = children[index];
                OrientedSize uiElementSize = new OrientedSize(orientation, uiElement.DesiredSize.Width, uiElement.DesiredSize.Height);

                // Determine if we should use the element's desired size or the fixed item width or height
                double directGrowth = directDelta != null ? directDelta.Value : uiElementSize.Direct;

                // This will calculate the justification margin needed to align the metadata labels.
                double justification = 0.0;
                MetadataHeader metadataHeader = VisualTreeExtensions.FindChild<MetadataHeader>(uiElement);
                if (metadataHeader != null)
                {
                    OrientedSize labelSize = new OrientedSize(orientation, metadataHeader.DesiredSize.Width, metadataHeader.DesiredSize.Height);
                    justification = maxMetadataHeaderSize.Indirect - labelSize.Indirect;
                }

                // Arrange the elements on the line adding in enough justification so that metadata labels are aligned on the right edge.
                Rect bounds = isHorizontal ?
                    new Rect(directOffset, indirectOffset + justification, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset + justification, directOffset, indirectGrowth, directGrowth);
                uiElement.Arrange(bounds);

                // This moves the cursor up to the next line.
                directOffset += directGrowth;
            }
        }
    }
}