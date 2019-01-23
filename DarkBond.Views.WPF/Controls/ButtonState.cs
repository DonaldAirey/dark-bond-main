// <copyright file="ButtonState.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// View model for the button that allows the user to select a view in a directory of items.
    /// </summary>
    public class ButtonState : FrameworkElement
    {
        /// <summary>
        /// Identifies the Command DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ButtonState),
            null);

        /// <summary>
        /// Identifies the CommandParameter DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(ButtonState),
            null);

        /// <summary>
        /// Identifies the Header DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(ButtonState),
            null);

        /// <summary>
        /// Identifies the State DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(string),
            typeof(ButtonState),
            null);

        /// <summary>
        /// Identifies the State DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty UriProperty = DependencyProperty.Register(
            "Uri",
            typeof(Uri),
            typeof(ButtonState),
            null);

        /// <summary>
        /// Gets or sets the command associated with this state.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return this.GetValue(ButtonState.CommandProperty) as ICommand;
            }

            set
            {
                this.SetValue(ButtonState.CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command associated with this state.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(ButtonState.CommandParameterProperty);
            }

            set
            {
                this.SetValue(ButtonState.CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header associated with this state.
        /// </summary>
        public string Header
        {
            get
            {
                return this.GetValue(ButtonState.HeaderProperty) as string;
            }

            set
            {
                this.SetValue(ButtonState.HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets this state.
        /// </summary>
        public string State
        {
            get
            {
                return this.GetValue(ButtonState.StateProperty) as string;
            }

            set
            {
                this.SetValue(ButtonState.StateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the uriSource.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.GetValue(ButtonState.UriProperty) as Uri;
            }

            set
            {
                this.SetValue(ButtonState.UriProperty, value);
            }
        }
    }
}