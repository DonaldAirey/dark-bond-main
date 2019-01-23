// <copyright file="TextViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// View model containing a single text element.
    /// </summary>
    public class TextViewModel : ViewModel
    {
        /// <summary>
        /// The text.
        /// </summary>
        private string textField;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextViewModel"/> class.
        /// </summary>
        public TextViewModel()
        {
            // Initialize the object.
            this.textField = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextViewModel"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public TextViewModel(string text)
        {
            // Initialize the object.
            this.textField = text;
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Text
        {
            get
            {
                return this.textField;
            }

            set
            {
                if (this.textField != value)
                {
                    this.textField = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }
    }
}