// <copyright file="LabeledValueViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// View model for a metadata item with a label.
    /// </summary>
    public class LabeledValueViewModel : ViewModel
    {
        /// <summary>
        /// The label for the value.
        /// </summary>
        private string labelField;

        /// <summary>
        /// The value.
        /// </summary>
        private object valueField;

        /// <summary>
        /// Gets or sets the label for the value.
        /// </summary>
        public string Label
        {
            get
            {
                return this.labelField;
            }

            set
            {
                if (this.labelField != value)
                {
                    this.labelField = value;
                    this.OnPropertyChanged("Label");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get
            {
                return this.valueField;
            }

            set
            {
                if (this.valueField != value)
                {
                    this.valueField = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }
    }
}