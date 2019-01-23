// <copyright file="DateBox.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a lightweight control for displaying small amounts of flow content.
    /// </summary>
    public class DateBox : TextBox
    {
        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(DateBox),
            new PropertyMetadata("d", DateBox.OnFormatPropertyChanged));

        /// <summary>
        /// Identifies the Date dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Date",
            typeof(DateTime),
            typeof(DateBox),
            new PropertyMetadata(default(DateTime), DateBox.OnDatePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateBox"/> class.
        /// </summary>
        public DateBox()
        {
            // This force the object to display the default value in the default format.
            this.Text = this.Date == default(DateTime) ? string.Empty : this.Date.ToString(this.Format, CultureInfo.CurrentCulture);

            // Format the value when we lose the focus.
            this.LostFocus += this.OnLostFocus;
        }

        /// <summary>
        /// Gets or sets the format field used to display the <see cref="DateTime"/>.
        /// </summary>
        public string Format
        {
            get
            {
                return this.GetValue(DateBox.FormatProperty) as string;
            }

            set
            {
                this.SetValue(DateBox.FormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date of the <see cref="DateBox"/>.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return (DateTime)this.GetValue(DateBox.ValueProperty);
            }

            set
            {
                this.SetValue(DateBox.ValueProperty, value);
            }
        }

        /// <summary>
        /// Handles a change to the Format property.
        /// </summary>
        /// <param name="dependencyObject">The object that originated the event.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // This will convert the value into a text string that can be displayed in the base TextBlock.
            DateBox dateBox = dependencyObject as DateBox;
            dateBox.Text = dateBox.Date == default(DateTime) ? string.Empty : dateBox.Date.ToString(dateBox.Format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Handles a change to the Date property.
        /// </summary>
        /// <param name="dependencyObject">The object that originated the event.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The event arguments.</param>
        private static void OnDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // This will convert the value into a text string that can be displayed in the base TextBlock.
            DateBox dateBox = dependencyObject as DateBox;
            DateTime value = (DateTime)dependencyPropertyChangedEventArgs.NewValue;
            dateBox.Text = value == default(DateTime) ? string.Empty : value.ToString(dateBox.Format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Occurs when this element looses the focus.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="routedEventArgs">The event data.</param>
        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            // Convert the text to a DateTime.
            try
            {
                this.Date = string.IsNullOrEmpty(this.Text) ? default(DateTime) : DateTime.Parse(this.Text, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                this.Date = default(DateTime);
            }
        }
    }
}