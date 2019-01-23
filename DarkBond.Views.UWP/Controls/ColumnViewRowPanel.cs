// <copyright file="ColumnViewRowPanel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// The panel used for each row of the column view.
    /// </summary>
    /// <remarks>
    /// The DataTemplate for this panel is compiled from source code that is constructed dynamically based on how many visible columns there are.
    /// This is a much faster way to build rows as the visual tree is constructed from the top down rather than from the bottom up.  However, we need
    /// to bind this skeleton of a template to the actual columns so they will take on the shape described with the column definitions.  This panel
    /// will dynamically find the owning ColumnView and bind the elements to the column definitions found there.
    /// </remarks>
    public class ColumnViewRowPanel : StackPanel
    {
        /// <summary>
        /// The source of the column descriptions.
        /// </summary>
        private ColumnViewDefinition columnView;

        /// <summary>
        /// The owner of the panel.
        /// </summary>
        private ItemsView itemsView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewRowPanel"/> class.
        /// </summary>
        public ColumnViewRowPanel()
        {
            // Initialize the object.
            this.Orientation = Orientation.Horizontal;

            // We need to know when the visual parent has changed in order to bind and unbind the columns.
            this.LayoutUpdated += this.OnLayoutUpdated;
        }

        /// <summary>
        /// Handles a change to the layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data. </param>
        private void OnLayoutUpdated(object sender, object e)
        {
            // The row panel is going to bind to properties on the column description or the owning control.  In order to do this properly, it needs to reference and dereference the ancestors as it is loaded and unloaded as a view.
            this.itemsView = VisualTreeExtensions.FindParent<ItemsView>(this);

            // If this panel has no parent, then it has been unloaded and must clear the bindings to the view and the column description.  If the
            // panel has been given an ItemsView ancestor, then we want to find the column description and bind the cell properties to it.
            if (this.itemsView == null)
            {
                // At this point, we have been disconnected from the visual tree and we want to remove the bindings.
                if (this.columnView != null)
                {
                    this.UnbindElements();
                    this.columnView = null;
                }
            }
            else
            {
                // At this point we've been added to the visual tree and we want to bind to the column set.
                ColumnViewDefinition currentColumnView = this.itemsView.Current as ColumnViewDefinition;
                if (this.columnView != currentColumnView)
                {
                    // Bind the properties of the row to the ColumnView when this panel is attached to the visual tree.
                    if (currentColumnView != null)
                    {
                        this.columnView = currentColumnView;
                        this.BindElements();
                    }
                }
            }
        }

        /// <summary>
        /// Bind the elements of this panel to the columns of the <see cref="ColumnViewDefinition"/>.
        /// </summary>
        private void BindElements()
        {
            // A binding for the background color of the panel.
            Binding backgroundBinding = new Binding();
            backgroundBinding.Path = new PropertyPath("ItemBackground");
            backgroundBinding.Source = this.columnView;
            this.SetBinding(ColumnViewRowPanel.BackgroundProperty, backgroundBinding);

            // This will cycle through all the visible columns of the owning control and bind to the columns found there.  This gives the rows their
            // shape and content.
            int index = 0;
            foreach (ColumnViewColumn columnViewColumn in this.columnView.Columns)
            {
                // Skip the invisible columns.
                if (!columnViewColumn.IsVisible)
                {
                    continue;
                }

                // This section binds the properties to the border of the cell.
                Border border = this.Children[index++] as Border;

                // Bind the BorderBrush property.
                Binding cellBorderBrushBinding = new Binding();
                cellBorderBrushBinding.Path = new PropertyPath("CellBorderBrush");
                cellBorderBrushBinding.Source = columnViewColumn;
                border.SetBinding(Border.BorderBrushProperty, cellBorderBrushBinding);

                // Bind the BorderThickness property.
                Binding cellBorderThicknessBinding = new Binding();
                cellBorderThicknessBinding.Path = new PropertyPath("CellBorderThickness");
                cellBorderThicknessBinding.Source = columnViewColumn;
                border.SetBinding(Border.BorderThicknessProperty, cellBorderThicknessBinding);

                // Bind the Padding property.
                Binding paddingBinding = new Binding();
                paddingBinding.Path = new PropertyPath("CellPadding");
                paddingBinding.Source = columnViewColumn;
                border.SetBinding(Border.PaddingProperty, paddingBinding);

                // Bind the MaxWidth property.
                Binding maxWidthBinding = new Binding();
                maxWidthBinding.Path = new PropertyPath("MaxWidth");
                maxWidthBinding.Source = columnViewColumn;
                border.SetBinding(Border.MaxWidthProperty, maxWidthBinding);

                // Bind the MinWidth property.
                Binding minWidthBinding = new Binding();
                minWidthBinding.Path = new PropertyPath("MinWidth");
                minWidthBinding.Source = columnViewColumn;
                border.SetBinding(Border.MinWidthProperty, minWidthBinding);

                // Bind the Width property.
                Binding widthBinding = new Binding();
                widthBinding.Path = new PropertyPath("ActualWidth");
                widthBinding.Source = columnViewColumn;
                border.SetBinding(Border.WidthProperty, widthBinding);

                // This section binds the properties to the content presenter.
                ContentPresenter contentPresenter = border.Child as ContentPresenter;

                // Bind the ContentTemplate property.
                Binding cellTemplateBinding = new Binding();
                cellTemplateBinding.Path = new PropertyPath("CellTemplate");
                cellTemplateBinding.Source = columnViewColumn;
                contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, cellTemplateBinding);

                // Bind the FontSize property.
                Binding fontSizeBinding = new Binding();
                fontSizeBinding.Path = new PropertyPath("FontSize");
                fontSizeBinding.Source = this.itemsView;
                contentPresenter.SetBinding(ContentPresenter.FontSizeProperty, fontSizeBinding);

                // Bind the HorizontalAlignment property.
                Binding horizontalAlignmentBinding = new Binding();
                horizontalAlignmentBinding.Path = new PropertyPath("HorizontalCellAlignment");
                horizontalAlignmentBinding.Source = columnViewColumn;
                contentPresenter.SetBinding(ContentPresenter.HorizontalAlignmentProperty, horizontalAlignmentBinding);

                // Bind the VerticalAlignment property.
                Binding verticalAlignmentBinding = new Binding();
                verticalAlignmentBinding.Path = new PropertyPath("VerticalCellAlignment");
                verticalAlignmentBinding.Source = columnViewColumn;
                contentPresenter.SetBinding(ContentPresenter.VerticalAlignmentProperty, verticalAlignmentBinding);
            }
        }

        /// <summary>
        /// Removes the bindings to the owning <see cref="ColumnViewDefinition"/>.
        /// </summary>
        private void UnbindElements()
        {
            // Clear the background binding.
            this.ClearValue(ColumnViewRowPanel.BackgroundProperty);

            // Cycle through all the child elements of the control and remove the bindings to the owner's columns.
            foreach (Border border in this.Children)
            {
                ContentPresenter contentPresenter = border.Child as ContentPresenter;

                // Clear the bindings on the border.
                border.ClearValue(Border.MaxWidthProperty);
                border.ClearValue(Border.MinWidthProperty);
                border.ClearValue(Border.PaddingProperty);
                border.ClearValue(Border.WidthProperty);

                // Clear the bindings on the presenter.
                contentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
                contentPresenter.ClearValue(ContentPresenter.FontSizeProperty);
                contentPresenter.ClearValue(ContentPresenter.HorizontalAlignmentProperty);
                contentPresenter.ClearValue(ContentPresenter.VerticalAlignmentProperty);
            }
        }
    }
}