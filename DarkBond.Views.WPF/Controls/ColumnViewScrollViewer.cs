// <copyright file="ColumnViewScrollViewer.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A ScrollViewer for the <see cref="ColumnViewDefinition"/>.
    /// </summary>
    public class ColumnViewScrollViewer : ScrollViewer
    {
        /// <summary>
        /// The Columns DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(IList),
            typeof(ColumnViewScrollViewer),
            null);

        /// <summary>
        /// The HeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(ControlTemplate),
            typeof(ColumnViewScrollViewer),
            null);

        /// <summary>
        /// Gets or sets the collection of columns.
        /// </summary>
        /// <value>
        /// The collection of columns.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Columns are set through XAML")]
        public IList Columns
        {
            get
            {
                return this.GetValue(ColumnViewScrollViewer.ColumnsProperty) as IList;
            }

            set
            {
                this.SetValue(ColumnViewScrollViewer.ColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the control template used to present the header.
        /// </summary>
        /// <value>
        /// The control template used to present the header.
        /// </value>
        public ControlTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(ColumnViewScrollViewer.HeaderTemplateProperty) as ControlTemplate;
            }

            set
            {
                this.SetValue(ColumnViewScrollViewer.HeaderTemplateProperty, value);
            }
        }
    }
}