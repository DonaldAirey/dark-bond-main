// <copyright file="GlobalCommands.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// Global Commands.
    /// </summary>
    public sealed class GlobalCommands
    {
        /// <summary>
        /// Close Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Close = new CompositeCommand();

        /// <summary>
        /// Copy Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Copy = new CompositeCommand();

        /// <summary>
        /// Change the current view.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand ChangeView = new CompositeCommand();

        /// <summary>
        /// Clear the selections of an item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand ClearSelection = new CompositeCommand();

        /// <summary>
        /// Clear the selections of all items.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand ClearSelectionAll = new CompositeCommand();

        /// <summary>
        /// Collapse the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Collapse = new CompositeCommand();

        /// <summary>
        /// Cut Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Cut = new CompositeCommand();

        /// <summary>
        /// Delete the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Delete = new CompositeCommand();

        /// <summary>
        /// Enables a filter.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Filter = new CompositeCommand();

        /// <summary>
        /// Expand the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Expand = new CompositeCommand();

        /// <summary>
        /// Navigate backward.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand GoBackward = new CompositeCommand();

        /// <summary>
        /// Navigate forward.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand GoForward = new CompositeCommand();

        /// <summary>
        /// Help Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Help = new CompositeCommand();

        /// <summary>
        /// Set the Source URI for an application.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Locate = new CompositeCommand();

        /// <summary>
        /// New Command
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand New = new CompositeCommand();

        /// <summary>
        /// Open the AppBar.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand OpenAppBar = new CompositeCommand();

        /// <summary>
        /// Open the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Open = new CompositeCommand();

        /// <summary>
        /// Paste Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Paste = new CompositeCommand();

        /// <summary>
        /// Open the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Properties = new CompositeCommand();

        /// <summary>
        /// Redo Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Redo = new CompositeCommand();

        /// <summary>
        /// Rename the selected item.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Rename = new CompositeCommand();

        /// <summary>
        /// Select items.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Select = new CompositeCommand();

        /// <summary>
        /// Select all.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SelectAll = new CompositeCommand();

        /// <summary>
        /// Select none.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SelectNone = new CompositeCommand();

        /// <summary>
        /// Set the Context AppBar.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SetContextAppBar = new CompositeCommand();

        /// <summary>
        /// Set the Global AppBar.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SetGlobalAppBar = new CompositeCommand();

        /// <summary>
        /// Set the Source URI for an application.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SignIn = new CompositeCommand();

        /// <summary>
        /// Indicates that the user has successfully signed in.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand SignedIn = new CompositeCommand();

        /// <summary>
        /// Change the sort order.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Sort = new CompositeCommand();

        /// <summary>
        /// Toggle the visibility of the details pane.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand ToggleDetailsPane = new CompositeCommand();

        /// <summary>
        /// Toggle the visibility of the navigation pane.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand ToggleNavigationPane = new CompositeCommand();

        /// <summary>
        /// Undo Command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly CompositeCommand Undo = new CompositeCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommands"/> class.
        /// </summary>
        private GlobalCommands()
        {
        }
    }
}