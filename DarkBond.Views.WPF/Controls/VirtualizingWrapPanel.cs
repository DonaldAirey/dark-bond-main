﻿// <copyright file="VirtualizingWrapPanel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

#pragma warning disable CS1591, SA1600, SA1401

    [ExcludeFromCodeCoverage]
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
    {
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty OrientationProperty = StackPanel.OrientationProperty.AddOwner(typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(Orientation.Horizontal));

        private UIElementCollection children;
        private ItemsControl itemsControl;
        private IItemContainerGenerator generator;
        private Point virtualOffset = new Point(0, 0);
        private Size extent = new Size(0, 0);
        private Size viewport = new Size(0, 0);
        private int firstIndex = 0;
        private Size childSize;
        private Size pixelMeasuredViewport = new Size(0, 0);
        private Dictionary<UIElement, Rect> realizedChildLayout = new Dictionary<UIElement, Rect>();
        private WrapPanelAbstraction abstractPanel;
        private bool canHScroll = false;
        private bool canVScroll = false;
        private ScrollViewer owner;

        [TypeConverter(typeof(LengthConverter))]
        public double ItemHeight
        {
            get
            {
                return (double)this.GetValue(ItemHeightProperty);
            }

            set
            {
                this.SetValue(ItemHeightProperty, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double ItemWidth
        {
            get
            {
                return (double)this.GetValue(ItemWidthProperty);
            }

            set
            {
                this.SetValue(ItemWidthProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                return this.canHScroll;
            }

            set
            {
                this.canHScroll = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return this.canVScroll;
            }

            set
            {
                this.canVScroll = value;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return this.extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                return this.extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this.virtualOffset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return this.virtualOffset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return this.viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return this.viewport.Width;
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                return this.owner;
            }

            set
            {
                this.owner = value;
            }
        }

        private Size ChildSlotSize
        {
            get
            {
                return new Size(this.ItemWidth, this.ItemHeight);
            }
        }

        public void SetFirstRowViewItemIndex(int index)
        {
            this.SetVerticalOffset(index / Math.Floor(this.viewport.Width / this.childSize.Width));
            this.SetHorizontalOffset(index / Math.Floor(this.viewport.Height / this.childSize.Height));
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This isn't my code")]
        public int GetFirstVisibleSection()
        {
            int section;
            var maxSection = this.abstractPanel.Max(x => x.Section);
            if (this.Orientation == Orientation.Horizontal)
            {
                section = (int)this.virtualOffset.Y;
            }
            else
            {
                section = (int)this.virtualOffset.X;
            }

            if (section > maxSection)
            {
                section = maxSection;
            }

            return section;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This isn't my code")]
        public int GetFirstVisibleIndex()
        {
            int section = this.GetFirstVisibleSection();
            var item = this.abstractPanel.Where(x => x.Section == section).FirstOrDefault();
            if (item != null)
            {
                return item.Index;
            }

            return 0;
        }

        public void LineDown()
        {
            if (this.Orientation == Orientation.Vertical)
            {
                this.SetVerticalOffset(this.VerticalOffset + 20);
            }
            else
            {
                this.SetVerticalOffset(this.VerticalOffset + 1);
            }
        }

        public void LineLeft()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - 20);
            }
            else
            {
                this.SetHorizontalOffset(this.HorizontalOffset - 1);
            }
        }

        public void LineRight()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + 20);
            }
            else
            {
                this.SetHorizontalOffset(this.HorizontalOffset + 1);
            }
        }

        public void LineUp()
        {
            if (this.Orientation == Orientation.Vertical)
            {
                this.SetVerticalOffset(this.VerticalOffset - 20);
            }
            else
            {
                this.SetVerticalOffset(this.VerticalOffset - 1);
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            var gen = (ItemContainerGenerator)this.generator.GetItemContainerGeneratorForPanel(this);
            var element = (UIElement)visual;
            int itemIndex = gen.IndexFromContainer(element);
            while (itemIndex == -1)
            {
                element = (UIElement)VisualTreeHelper.GetParent(element);
                itemIndex = gen.IndexFromContainer(element);
            }

            Rect elementRect = this.realizedChildLayout[element];
            if (this.Orientation == Orientation.Horizontal)
            {
                double viewportHeight = this.pixelMeasuredViewport.Height;
                if (elementRect.Bottom > viewportHeight)
                {
                    this.virtualOffset.Y += 1;
                }
                else if (elementRect.Top < 0)
                {
                    this.virtualOffset.Y -= 1;
                }
            }
            else
            {
                double viewportWidth = this.pixelMeasuredViewport.Width;
                if (elementRect.Right > viewportWidth)
                {
                    this.virtualOffset.X += 1;
                }
                else if (elementRect.Left < 0)
                {
                    this.virtualOffset.X -= 1;
                }
            }

            this.InvalidateMeasure();
            return elementRect;
        }

        public void MouseWheelDown()
        {
            this.PageDown();
        }

        public void MouseWheelLeft()
        {
            this.PageLeft();
        }

        public void MouseWheelRight()
        {
            this.PageRight();
        }

        public void MouseWheelUp()
        {
            this.PageUp();
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + (this.viewport.Height * 0.8));
        }

        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - (this.viewport.Width * 0.8));
        }

        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + (this.viewport.Width * 0.8));
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - (this.viewport.Height * 0.8));
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "offset", Justification = "This isn't my code")]
        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || this.viewport.Width >= this.extent.Width)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.viewport.Width >= this.extent.Width)
                {
                    offset = this.extent.Width - this.viewport.Width;
                }
            }

            this.virtualOffset.X = offset;

            if (this.owner != null)
            {
                this.owner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
            this.firstIndex = this.GetFirstVisibleIndex();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || this.viewport.Height >= this.extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.viewport.Height >= this.extent.Height)
                {
                    offset = this.extent.Height - this.viewport.Height;
                }
            }

            this.virtualOffset.Y = offset;

            if (this.owner != null)
            {
                this.owner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
            this.firstIndex = this.GetFirstVisibleIndex();
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Validate the argument.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            switch (e.Key)
            {
                case Key.Down:
                    this.NavigateDown();
                    e.Handled = true;
                    break;
                case Key.Left:
                    this.NavigateLeft();
                    e.Handled = true;
                    break;
                case Key.Right:
                    this.NavigateRight();
                    e.Handled = true;
                    break;
                case Key.Up:
                    this.NavigateUp();
                    e.Handled = true;
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            this.abstractPanel = null;
            this.ResetScrollInfo();
        }

        protected override void OnInitialized(EventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(this.Resizing);
            base.OnInitialized(e);
            this.itemsControl = ItemsControl.GetItemsOwner(this);
            this.children = this.InternalChildren;
            this.generator = this.ItemContainerGenerator;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.itemsControl == null || this.itemsControl.Items.Count == 0)
            {
                return availableSize;
            }

            if (this.abstractPanel == null)
            {
                this.abstractPanel = new WrapPanelAbstraction(this.itemsControl.Items.Count);
            }

            this.pixelMeasuredViewport = availableSize;

            this.realizedChildLayout.Clear();

            Size realizedFrameSize = availableSize;

            int itemCount = this.itemsControl.Items.Count;
            int firstVisibleIndex = this.GetFirstVisibleIndex();

            GeneratorPosition startPos = this.generator.GeneratorPositionFromIndex(firstVisibleIndex);

            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
            int current = firstVisibleIndex;
            int visibleSections = 1;
            using (this.generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                bool stop = false;
                bool isHorizontal = this.Orientation == Orientation.Horizontal;
                double currentX = 0;
                double currentY = 0;
                double maxItemSize = 0;
                int currentSection = this.GetFirstVisibleSection();
                while (current < itemCount)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = this.generator.GenerateNext(out newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        // Figure out if we need to insert the child at the end or somewhere in the middle
                        if (childIndex >= this.children.Count)
                        {
                            this.AddInternalChild(child);
                        }
                        else
                        {
                            this.InsertInternalChild(childIndex, child);
                        }

                        this.generator.PrepareItemContainer(child);
                        child.Measure(this.ChildSlotSize);
                    }
                    else
                    {
                        // The child has already been created, let's be sure it's in the right spot
                        Debug.Assert(child == this.children[childIndex], "Wrong child was generated");
                    }

                    this.childSize = child.DesiredSize;
                    Rect childRect = new Rect(new Point(currentX, currentY), this.childSize);
                    if (isHorizontal)
                    {
                        maxItemSize = Math.Max(maxItemSize, childRect.Height);
                        if (childRect.Right > realizedFrameSize.Width)
                        {
                            currentY = currentY + maxItemSize;
                            currentX = 0;
                            maxItemSize = childRect.Height;
                            childRect.X = currentX;
                            childRect.Y = currentY;
                            currentSection++;
                            visibleSections++;
                        }

                        if (currentY > realizedFrameSize.Height)
                        {
                            stop = true;
                        }

                        currentX = childRect.Right;
                    }
                    else
                    {
                        maxItemSize = Math.Max(maxItemSize, childRect.Width);
                        if (childRect.Bottom > realizedFrameSize.Height)
                        {
                            currentX = currentX + maxItemSize;
                            currentY = 0;
                            maxItemSize = childRect.Width;
                            childRect.X = currentX;
                            childRect.Y = currentY;
                            currentSection++;
                            visibleSections++;
                        }

                        if (currentX > realizedFrameSize.Width)
                        {
                            stop = true;
                        }

                        currentY = childRect.Bottom;
                    }

                    this.realizedChildLayout.Add(child, childRect);
                    this.abstractPanel.SetItemSection(current, currentSection);

                    if (stop)
                    {
                        break;
                    }

                    current++;
                    childIndex++;
                }
            }

            this.CleanUpItems(firstVisibleIndex, current - 1);

            this.ComputeExtentAndViewport(availableSize, visibleSections);

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.children != null)
            {
                foreach (UIElement child in this.children)
                {
                    var layoutInfo = this.realizedChildLayout[child];
                    child.Arrange(layoutInfo);
                }
            }

            return finalSize;
        }

        private void Resizing(object sender, EventArgs e)
        {
            if (this.viewport.Width != 0)
            {
                int firstIndexCache = this.firstIndex;
                this.abstractPanel = null;
                this.MeasureOverride(this.viewport);
                this.SetFirstRowViewItemIndex(this.firstIndex);
                this.firstIndex = firstIndexCache;
            }
        }

        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            for (int i = this.children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = this.generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    this.generator.Remove(childGeneratorPos, 1);
                    this.RemoveInternalChildRange(i, 1);
                }
            }
        }

        private void ComputeExtentAndViewport(Size pixelMeasuredViewportSize, int visibleSections)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                this.viewport.Height = visibleSections;
                this.viewport.Width = pixelMeasuredViewportSize.Width;
            }
            else
            {
                this.viewport.Width = visibleSections;
                this.viewport.Height = pixelMeasuredViewportSize.Height;
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                this.extent.Height = this.abstractPanel.SectionCount + this.ViewportHeight - 1;
            }
            else
            {
                this.extent.Width = this.abstractPanel.SectionCount + this.ViewportWidth - 1;
            }

            this.owner.InvalidateScrollInfo();
        }

        private void ResetScrollInfo()
        {
            this.virtualOffset.X = 0;
            this.virtualOffset.Y = 0;
        }

        private int GetNextSectionClosestIndex(int itemIndex)
        {
            var abstractItem = this.abstractPanel[itemIndex];
            if (abstractItem.Section < this.abstractPanel.SectionCount - 1)
            {
                var ret = this.abstractPanel.
                    Where(x => x.Section == abstractItem.Section + 1).
                    OrderBy(x => Math.Abs(x.SectionIndex - abstractItem.SectionIndex)).
                    First();
                return ret.Index;
            }
            else
            {
                return itemIndex;
            }
        }

        private int GetLastSectionClosestIndex(int itemIndex)
        {
            var abstractItem = this.abstractPanel[itemIndex];
            if (abstractItem.Section > 0)
            {
                var ret = this.abstractPanel.
                    Where(x => x.Section == abstractItem.Section - 1).
                    OrderBy(x => Math.Abs(x.SectionIndex - abstractItem.SectionIndex)).
                    First();
                return ret.Index;
            }
            else
            {
                return itemIndex;
            }
        }

        private void NavigateDown()
        {
            var gen = this.generator.GetItemContainerGeneratorForPanel(this);
            UIElement selected = (UIElement)Keyboard.FocusedElement;
            int itemIndex = gen.IndexFromContainer(selected);
            int depth = 0;
            while (itemIndex == -1)
            {
                selected = (UIElement)VisualTreeHelper.GetParent(selected);
                itemIndex = gen.IndexFromContainer(selected);
                depth++;
            }

            DependencyObject next = null;
            if (this.Orientation == Orientation.Horizontal)
            {
                int nextIndex = this.GetNextSectionClosestIndex(itemIndex);
                next = gen.ContainerFromIndex(nextIndex);
                while (next == null)
                {
                    this.SetVerticalOffset(this.VerticalOffset + 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(nextIndex);
                }
            }
            else
            {
                if (itemIndex == this.abstractPanel.ItemCount - 1)
                {
                    return;
                }

                next = gen.ContainerFromIndex(itemIndex + 1);
                while (next == null)
                {
                    this.SetHorizontalOffset(this.HorizontalOffset + 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(itemIndex + 1);
                }
            }

            while (depth != 0)
            {
                next = VisualTreeHelper.GetChild(next, 0);
                depth--;
            }

            (next as UIElement).Focus();
        }

        private void NavigateLeft()
        {
            var gen = this.generator.GetItemContainerGeneratorForPanel(this);

            UIElement selected = (UIElement)Keyboard.FocusedElement;
            int itemIndex = gen.IndexFromContainer(selected);
            int depth = 0;
            while (itemIndex == -1)
            {
                selected = (UIElement)VisualTreeHelper.GetParent(selected);
                itemIndex = gen.IndexFromContainer(selected);
                depth++;
            }

            DependencyObject next = null;
            if (this.Orientation == Orientation.Vertical)
            {
                int nextIndex = this.GetLastSectionClosestIndex(itemIndex);
                next = gen.ContainerFromIndex(nextIndex);
                while (next == null)
                {
                    this.SetHorizontalOffset(this.HorizontalOffset - 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(nextIndex);
                }
            }
            else
            {
                if (itemIndex == 0)
                {
                    return;
                }

                next = gen.ContainerFromIndex(itemIndex - 1);
                while (next == null)
                {
                    this.SetVerticalOffset(this.VerticalOffset - 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(itemIndex - 1);
                }
            }

            while (depth != 0)
            {
                next = VisualTreeHelper.GetChild(next, 0);
                depth--;
            }

            (next as UIElement).Focus();
        }

        private void NavigateRight()
        {
            var gen = this.generator.GetItemContainerGeneratorForPanel(this);
            UIElement selected = (UIElement)Keyboard.FocusedElement;
            int itemIndex = gen.IndexFromContainer(selected);
            int depth = 0;
            while (itemIndex == -1)
            {
                selected = (UIElement)VisualTreeHelper.GetParent(selected);
                itemIndex = gen.IndexFromContainer(selected);
                depth++;
            }

            DependencyObject next = null;
            if (this.Orientation == Orientation.Vertical)
            {
                int nextIndex = this.GetNextSectionClosestIndex(itemIndex);
                next = gen.ContainerFromIndex(nextIndex);
                while (next == null)
                {
                    this.SetHorizontalOffset(this.HorizontalOffset + 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(nextIndex);
                }
            }
            else
            {
                if (itemIndex == this.abstractPanel.ItemCount - 1)
                {
                    return;
                }

                next = gen.ContainerFromIndex(itemIndex + 1);
                while (next == null)
                {
                    this.SetVerticalOffset(this.VerticalOffset + 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(itemIndex + 1);
                }
            }

            while (depth != 0)
            {
                next = VisualTreeHelper.GetChild(next, 0);
                depth--;
            }

            (next as UIElement).Focus();
        }

        private void NavigateUp()
        {
            var gen = this.generator.GetItemContainerGeneratorForPanel(this);
            UIElement selected = (UIElement)Keyboard.FocusedElement;
            int itemIndex = gen.IndexFromContainer(selected);
            int depth = 0;
            while (itemIndex == -1)
            {
                selected = (UIElement)VisualTreeHelper.GetParent(selected);
                itemIndex = gen.IndexFromContainer(selected);
                depth++;
            }

            DependencyObject next = null;
            if (this.Orientation == Orientation.Horizontal)
            {
                int nextIndex = this.GetLastSectionClosestIndex(itemIndex);
                next = gen.ContainerFromIndex(nextIndex);
                while (next == null)
                {
                    this.SetVerticalOffset(this.VerticalOffset - 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(nextIndex);
                }
            }
            else
            {
                if (itemIndex == 0)
                {
                    return;
                }

                next = gen.ContainerFromIndex(itemIndex - 1);
                while (next == null)
                {
                    this.SetHorizontalOffset(this.HorizontalOffset - 1);
                    this.UpdateLayout();
                    next = gen.ContainerFromIndex(itemIndex - 1);
                }
            }

            while (depth != 0)
            {
                next = VisualTreeHelper.GetChild(next, 0);
                depth--;
            }

            (next as UIElement).Focus();
        }

        private class ItemAbstraction
        {
            private WrapPanelAbstraction panel;

            private int section = -1;

            private int sectionIndex = -1;

            public ItemAbstraction(WrapPanelAbstraction panel, int index)
            {
                this.panel = panel;
                this.Index = index;
            }

            public int Index
            {
                get;
                private set;
            }

            public int SectionIndex
            {
                get
                {
                    if (this.sectionIndex == -1)
                    {
                        return (this.Index % this.panel.AverageItemsPerSection) - 1;
                    }

                    return this.sectionIndex;
                }

                set
                {
                    if (this.sectionIndex == -1)
                    {
                        this.sectionIndex = value;
                    }
                }
            }

            public int Section
            {
                get
                {
                    if (this.section == -1)
                    {
                        return this.Index / this.panel.AverageItemsPerSection;
                    }

                    return this.section;
                }

                set
                {
                    if (this.section == -1)
                    {
                        this.section = value;
                    }
                }
            }
        }

        private class WrapPanelAbstraction : IEnumerable<ItemAbstraction>
        {
            private int currentSetSection = -1;
            private int currentSetItemIndex = -1;
            private int itemsInCurrentSecction = 0;
            private object syncRoot = new object();

            public WrapPanelAbstraction(int itemCount)
            {
                List<ItemAbstraction> items = new List<ItemAbstraction>(itemCount);
                for (int i = 0; i < itemCount; i++)
                {
                    ItemAbstraction item = new ItemAbstraction(this, i);
                    items.Add(item);
                }

                this.Items = new ReadOnlyCollection<ItemAbstraction>(items);
                this.AverageItemsPerSection = itemCount;
                this.ItemCount = itemCount;
            }

            public int AverageItemsPerSection
            {
                get;
                set;
            }

            public int ItemCount
            {
                get;
                private set;
            }

            public int SectionCount
            {
                get
                {
                    int ret = this.currentSetSection + 1;
                    if (this.currentSetItemIndex + 1 < this.Items.Count)
                    {
                        int itemsLeft = this.Items.Count - this.currentSetItemIndex;
                        ret += (itemsLeft / this.AverageItemsPerSection) + 1;
                    }

                    return ret;
                }
            }

            private ReadOnlyCollection<ItemAbstraction> Items
            {
                get;
                set;
            }

            public ItemAbstraction this[int index]
            {
                get
                {
                    return this.Items[index];
                }
            }

            public void SetItemSection(int index, int section)
            {
                lock (this.syncRoot)
                {
                    if (section <= this.currentSetSection + 1 && index == this.currentSetItemIndex + 1)
                    {
                        this.currentSetItemIndex++;
                        this.Items[index].Section = section;
                        if (section == this.currentSetSection + 1)
                        {
                            this.currentSetSection = section;
                            if (section > 0)
                            {
                                this.AverageItemsPerSection = index / section;
                            }

                            this.itemsInCurrentSecction = 1;
                        }
                        else
                        {
                            this.itemsInCurrentSecction++;
                        }

                        this.Items[index].SectionIndex = this.itemsInCurrentSecction - 1;
                    }
                }
            }

            public IEnumerator<ItemAbstraction> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }

#pragma warning restore CS1591, SA1600, SA1401

}