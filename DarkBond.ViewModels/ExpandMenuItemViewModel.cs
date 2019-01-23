// <copyright file="ExpandMenuItemViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System.ComponentModel;
    using DarkBond.ViewModels.Strings;

    /// <summary>
    /// A View Model for a menu item that controls expanding or collapsing an outline level.
    /// </summary>
    public class ExpandMenuItemViewModel : MenuItemViewModel
    {
        /// <summary>
        /// Indicates whether the outline level associated with this menu item is expanded.
        /// </summary>
        private bool isExpandedField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandMenuItemViewModel"/> class.
        /// </summary>
        public ExpandMenuItemViewModel()
        {
            // Outline level are initially collapsed, so by default we will offer the option to expand the node.
            this.Header = Labels.Expand;
            this.Command = GlobalCommands.Expand;

            // This event handler will watch for a change to the 'IsExpanded' property and change the menu item's view model when the status of the
            // state of expansion changes.
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the outline level associated with this menu item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpandedField;
            }

            set
            {
                if (this.isExpandedField != value)
                {
                    this.isExpandedField = value;
                    this.OnPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event raised when a property is changed on a component.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="propertyChangedEventArgs">A PropertyChangedEventArgs that contains the event data. </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // If the expanded state of this object has changed then evaluate the proper view model to transition to the next state.
            if (propertyChangedEventArgs.PropertyName == "IsExpanded")
            {
                // If the item is expanded, then allow the user to collapse it, and vica-versa.
                if (this.IsExpanded)
                {
                    this.Header = Labels.Collapse;
                    this.Command = GlobalCommands.Collapse;
                }
                else
                {
                    this.Header = Labels.Expand;
                    this.Command = GlobalCommands.Expand;
                }
            }
        }
    }
}