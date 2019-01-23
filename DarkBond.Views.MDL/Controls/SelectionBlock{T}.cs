// <copyright file="SelectionBlock{T}.cs" company="Dark Bond, Inc.">
//    Copyright © 2016 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.View.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A strongly typed SelectionBlock.
    /// </summary>
    /// <typeparam name="T">The value type returned from SelectionBlock.</typeparam>
    public class SelectionBlock<T> : SelectionBlock
    {
        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        private static DependencyProperty selectedValuePropertyField = DependencyProperty.Register(
            "SelectedValue",
            typeof(object),
            typeof(SelectionBlock<T>),
            new PropertyMetadata(default(T), SelectionBlock<T>.OnSelectedValuePropertyChanged));

        /// <summary>
        /// Used to prevent recursion when setting the index of the selected item.
        /// </summary>
        private bool isInternalUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionBlock{T}"/> class.
        /// </summary>
        public SelectionBlock()
        {
            // This will reconcile the strongly typed selected value with the generic one in the base class.  The main idea here is to capture most
            // of the functionality of the SelectionBlock, but when it comes time to setting or extracting a value from the control, we'll get the
            // strongly typed version (which includes nullable types).
            this.SelectionChanged += this.OnSelectionChanged;

            // This will re-select the item after the data context has been changed, which happens often when controls are reused.
            this.DataContextChanged += this.OnDataContextChanged;
        }

        /// <summary>
        /// Gets the SelectedValueProperty DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Reviewed")]
        public static new DependencyProperty SelectedValueProperty
        {
            get
            {
                return SelectionBlock<T>.selectedValuePropertyField;
            }
        }

        /// <summary>
        /// Gets or sets the value of the selected item.
        /// </summary>
        public new T SelectedValue
        {
            get
            {
                return (T)this.GetValue(SelectionBlock<T>.selectedValuePropertyField);
            }

            set
            {
                this.SetValue(SelectionBlock<T>.selectedValuePropertyField, value);
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the CustomerId property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnSelectedValuePropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // This will reconcile the list of items available with the unique identifier of the selected item.  Note that the stock SelectionBlock
            // is brain-damaged when it comes to using Enums, so we're going to search for the selected value manually and use the 'SelectedIndex'
            // property as a work-around.
            SelectionBlock<T> selectionBlock = dependencyObject as SelectionBlock<T>;
            selectionBlock.SetSelectedIndex((T)dependencyPropertyChangedEventArgs.NewValue);
        }

        /// <summary>
        /// Handles a change to the data context.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="dataContextChangedEventArgs">The event arguments.</param>
        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs dataContextChangedEventArgs)
        {
            // This will basically refresh the index to the selected value which is lost when the data context changes.
            this.SetSelectedIndex(this.SelectedValue);
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
                throw new ArgumentNullException("selectionChangedEventArgs");
            }

            // This prevents a recursive update of the selected value when the change comes from property change handler.
            if (!this.isInternalUpdate)
            {
                // This will reconcile the newly selected generic item from base class with the strongly type value in the subclass.
                foreach (object selectedItem in selectionChangedEventArgs.AddedItems)
                {
                    PropertyInfo propertyInfo = selectedItem.GetType().GetRuntimeProperty(this.SelectedValuePath);
                    this.SelectedValue = (T)propertyInfo.GetValue(selectedItem, null);
                }
            }
        }

        /// <summary>
        /// Sets the selected index to reflect the selected value.
        /// </summary>
        /// <param name="newValue">The selected value.</param>
        private void SetSelectedIndex(object newValue)
        {
            // This tells us if the selected value is valid for the domain of items.
            bool found = false;

            // Use reflection to find the index of the selected item.  We're using System.Reflection here because the base class is brain-damaged
            // when it comes time to finding enums.
            for (int index = 0; index < this.Items.Count; index++)
            {
                object item = this.Items[index];
                PropertyInfo propertyInfo = item.GetType().GetRuntimeProperty(this.SelectedValuePath);
                T value = (T)propertyInfo.GetValue(item, null);
                if (value.Equals(newValue))
                {
                    try
                    {
                        this.isInternalUpdate = true;
                        this.SelectedIndex = index;
                        found = true;
                    }
                    finally
                    {
                        this.isInternalUpdate = false;
                    }

                    break;
                }
            }

            // If the selected value was not among the domain of items then the SelectionBlock will be empty.
            if (!found)
            {
                this.SelectedIndex = -1;
            }
        }
    }
}