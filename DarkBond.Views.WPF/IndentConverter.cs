// <copyright file="IndentConverter.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using DarkBond.Views.Controls;

    /// <summary>
    /// Converts a nested counter into a series of spaces for an indentation.
    /// </summary>
    public class IndentConverter : IValueConverter
    {
        /// <summary>
        /// Provides a way to apply custom logic to a binding of an indentation level.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double indentLevel = 0.0;
            double indentSize = 0.0;
            TreeViewItem treeViewItem = value as TreeViewItem;
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(treeViewItem);
            while (itemsControl != null)
            {
                treeViewItem = itemsControl as TreeViewItem;
                if (treeViewItem != null)
                {
                    indentLevel += 1.0;
                }
                else
                {
                    NavigationTree navigator = itemsControl as NavigationTree;
                    if (navigator != null)
                    {
                        indentSize = navigator.Indent;
                    }
                }

                itemsControl = ItemsControl.ItemsControlFromItemContainer(treeViewItem);
            }

            return indentLevel * indentSize;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}