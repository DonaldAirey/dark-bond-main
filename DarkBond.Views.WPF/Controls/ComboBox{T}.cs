// <copyright file="ComboBox{T}.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A strongly typed ComboBox.
    /// </summary>
    /// <typeparam name="T">The value type returned from ComboBox.</typeparam>
    public class ComboBox<T> : ComboBox
    {
        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        private static new readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
            "SelectedValue",
            typeof(T),
            typeof(ComboBox<T>),
            new PropertyMetadata(default(T), ComboBox<T>.OnSelectedValuePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBox{T}"/> class.
        /// </summary>
        public ComboBox()
        {
            // This will reconcile the strongly typed selected value with the generic one in the base class.  The main idea here is to capture most
            // of the functionality of the ComboBox, but when it comes time to setting or extracting a value from the control, we'll get the strongly
            // typed version (which includes nullable types).
            this.SelectionChanged += this.OnSelectionChanged;
        }

        /// <summary>
        /// Gets or sets the value of the selected item.
        /// </summary>
        public new T SelectedValue
        {
            get
            {
                return (T)this.GetValue(ComboBox<T>.SelectedValueProperty);
            }

            set
            {
                this.SetValue(ComboBox<T>.SelectedValueProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the CustomerId property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnSelectedValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // This will reconcile the list of items available with the unique identifier of the selected item.  Note that the stock ComboBox is
            // brain-damaged when it comes to using Enums, so we're going to search for the selected value manually and use the 'SelectedIndex'
            // property as a work-around.
            ComboBox comboBox = dependencyObject as ComboBox;
            if (comboBox.SelectedValue != dependencyPropertyChangedEventArgs.NewValue)
            {
                // This tells us if the selected value is valid for the domain of items.
                bool found = false;

                // Use reflection to find the index of the selected item.  We are doing this here because the base class is brain-damaged when it
                // comes time to finding enums.
                for (int index = 0; index < comboBox.Items.Count; index++)
                {
                    object item = comboBox.Items[index];
                    PropertyInfo propertyInfo = item.GetType().GetRuntimeProperty(comboBox.SelectedValuePath);
                    T value = (T)propertyInfo.GetValue(item, null);
                    if (object.Equals(dependencyPropertyChangedEventArgs.NewValue, value))
                    {
                        comboBox.SelectedIndex = index;
                        found = true;
                        break;
                    }
                }

                // If the selected value was not among the domain of items then the ComboBox will be empty.
                if (!found)
                {
                    comboBox.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="SelectionChangedEventArgs"/> event.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="selectionChangedEventArgs">The event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            // Validate the selectionChangedEventArgs parameter.
            if (selectionChangedEventArgs == null)
            {
                throw new ArgumentNullException(nameof(selectionChangedEventArgs));
            }

            // This will reconcile the newly selected generic item from base class with the strongly type value in the subclass.
            foreach (object selectedItem in selectionChangedEventArgs.AddedItems)
            {
                PropertyInfo propertyInfo = selectedItem.GetType().GetRuntimeProperty(this.SelectedValuePath);
                this.SelectedValue = (T)propertyInfo.GetValue(selectedItem, null);
            }
        }
    }
}