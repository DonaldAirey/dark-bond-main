// <copyright file="GuidBlock.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Provides a lightweight control for GUIDs.
    /// </summary>
    public class GuidBlock : TextBlock
    {
        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(GuidBlock),
            new PropertyMetadata("D", GuidBlock.OnFormatPropertyChanged));

        /// <summary>
        /// Identifies the Guid dependency property.
        /// </summary>
        public static readonly DependencyProperty GuidProperty = DependencyProperty.Register(
            "Guid",
            typeof(Guid),
            typeof(GuidBlock),
            new PropertyMetadata(Guid.Empty, GuidBlock.OnGuidPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidBlock"/> class.
        /// </summary>
        public GuidBlock()
        {
            // This will set the object to display it's initial value.
            this.Text = this.Guid.ToString(this.Format);

            // Handles a change to the text.
            this.SourceUpdated += this.OnSourceUpdated;
        }

        /// <summary>
        /// Gets or sets the format field used to display the <see cref="System.Guid"/>.
        /// </summary>
        public string Format
        {
            get
            {
                return this.GetValue(GuidBlock.FormatProperty) as string;
            }

            set
            {
                this.SetValue(GuidBlock.FormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="GuidBlock"/>.
        /// </summary>
        public Guid Guid
        {
            get
            {
                return (Guid)this.GetValue(GuidBlock.GuidProperty);
            }

            set
            {
                this.SetValue(GuidBlock.GuidProperty, value);
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
            GuidBlock guidBlock = dependencyObject as GuidBlock;
            guidBlock.Text = guidBlock.Guid.ToString(guidBlock.Format);
        }

        /// <summary>
        /// Handles a change to the Value property.
        /// </summary>
        /// <param name="dependencyObject">The object that originated the event.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The event arguments.</param>
        private static void OnGuidPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // This will convert the value into a text string that can be displayed in the base TextBlock.
            GuidBlock guidBlock = dependencyObject as GuidBlock;
            Guid value = (Guid)dependencyPropertyChangedEventArgs.NewValue;
            guidBlock.Text = value.ToString(guidBlock.Format);
        }

        /// <summary>
        /// Invoked when the text changes.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="textChangedEventArgs">The event data.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            // Convert the text to a Guid.
            this.Guid = string.IsNullOrEmpty(this.Text) ? Guid.Empty : System.Guid.Parse(this.Text);
        }

        /// <summary>
        /// Handles a change to the source value.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="dataTransferEventArgs">The event data.</param>
        private void OnSourceUpdated(object sender, DataTransferEventArgs dataTransferEventArgs)
        {
            // Convert the text to a Guid.
            this.Guid = string.IsNullOrEmpty(this.Text) ? Guid.Empty : System.Guid.Parse(this.Text);
        }
    }
}