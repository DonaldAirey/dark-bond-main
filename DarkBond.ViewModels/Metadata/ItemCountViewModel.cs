// <copyright file="ItemCountViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System.Globalization;
    using DarkBond.ViewModels.Strings;

    /// <summary>
    /// View model for presenting the number of items.
    /// </summary>
    public class ItemCountViewModel : TextViewModel
    {
        /// <summary>
        /// The number of items.
        /// </summary>
        private int countField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCountViewModel"/> class.
        /// </summary>
        public ItemCountViewModel()
        {
            // These property change events are handled by this class.
            this.PropertyChangedActions["Count"] = this.OnCountChanged;

            // This will initialize the view model with the current count.
            this.OnCountChanged();
        }

        /// <summary>
        /// Gets or sets the number of items.
        /// </summary>
        public int Count
        {
            get
            {
                return this.countField;
            }

            set
            {
                if (this.countField != value)
                {
                    this.countField = value;
                    this.OnPropertyChanged("Count");
                }
            }
        }

        /// <summary>
        /// Handles a change to the Count property.
        /// </summary>
        private void OnCountChanged()
        {
            // Format a message showing how many items are displayed in the view.
            this.Text = string.Format(
                CultureInfo.InvariantCulture,
                Labels.ItemFormat,
                this.countField,
                this.countField == 1 ? Labels.Item : Labels.Items);
        }
    }
}
