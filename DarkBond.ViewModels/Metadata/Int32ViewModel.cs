// <copyright file="Int32ViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Globalization;

    /// <summary>
    /// View model for presenting the number of items.
    /// </summary>
    public class Int32ViewModel : TextViewModel
    {
        /// <summary>
        /// The date.
        /// </summary>
        private int int32Field;

        /// <summary>
        /// The format of the value.
        /// </summary>
        private string formatField = "g";

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32ViewModel"/> class.
        /// </summary>
        public Int32ViewModel()
        {
            // These property change events are handled by this class.
            this.PropertyChangedActions["Format"] = this.OnPropertyChanged;
            this.PropertyChangedActions["Value"] = this.OnPropertyChanged;

            // This will initialize the view model with the current count.
            this.OnPropertyChanged();
        }

        /// <summary>
        /// Gets or sets the format used to display the value.
        /// </summary>
        public string Format
        {
            get
            {
                return this.formatField;
            }

            set
            {
                if (this.formatField != value)
                {
                    this.formatField = value;
                    this.OnPropertyChanged("Format");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of items.
        /// </summary>
        public int Value
        {
            get
            {
                return this.int32Field;
            }

            set
            {
                if (this.int32Field != value)
                {
                    this.int32Field = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// Handles a change to the <see cref="int"/> property.
        /// </summary>
        private void OnPropertyChanged()
        {
            // Format the text that is displayed and then signal the view that new text is available.
            this.Text = this.int32Field.ToString(this.formatField, CultureInfo.InvariantCulture);
        }
    }
}