// <copyright file="FilterFlyout.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;

    /// <summary>
    /// Displays the filter options for a column in a <see cref="ColumnViewDefinition"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Flyout", Justification = "Spelled Correctly")]
    public class FilterFlyout : Flyout
    {
        /// <summary>
        /// Used to present the filter view models as visual elements.
        /// </summary>
        private FilterItemsControl filterItemsControl = new FilterItemsControl();

        /// <summary>
        /// The command handler for filtering.
        /// </summary>
        private DelegateCommand<FilterDescription> filterCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterFlyout"/> class.
        /// </summary>
        public FilterFlyout()
        {
            // Initialize the object.
            this.Content = this.filterItemsControl;
            this.Placement = FlyoutPlacementMode.Bottom;

            // This control will handle these commands.
            this.filterCommand = new DelegateCommand<FilterDescription>(this.Filter);
            GlobalCommands.Filter.RegisterCommand(this.filterCommand);
        }

        /// <summary>
        /// Gets or sets the source of the filters displayed in this Flyout.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return this.filterItemsControl.ItemsSource;
            }

            set
            {
                this.filterItemsControl.ItemsSource = value;
            }
        }

        /// <summary>
        /// Handles a filter command.
        /// </summary>
        /// <param name="filterItemViewModel">The view model of the filter.</param>
        private void Filter(FilterDescription filterItemViewModel)
        {
            // The flyout will hide itself after each filter item is selected or unselected.
            this.Hide();
        }
    }
}