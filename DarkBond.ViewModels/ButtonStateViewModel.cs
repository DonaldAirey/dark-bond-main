// <copyright file="ButtonStateViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Windows.Input;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for the button that allows the user to select a view in a directory of items.
    /// </summary>
    public class ButtonStateViewModel : ViewModel
    {
        /// <summary>
        /// The command.
        /// </summary>
        private object commandField;

        /// <summary>
        /// The command parameter.
        /// </summary>
        private object commandParameterField;

        /// <summary>
        /// The state of the button.
        /// </summary>
        private string stateField;

        /// <summary>
        /// The URI of the image that displays the current state.
        /// </summary>
        private Uri uriField;

        /// <summary>
        /// Gets or sets command for this mode of the button.
        /// </summary>
        public object Command
        {
            get
            {
                return this.commandField;
            }

            set
            {
                if (this.commandField != value)
                {
                    this.commandField = value;
                    this.OnPropertyChanged("Command");
                }
            }
        }

        /// <summary>
        /// Gets or sets command parameter for this mode of the button.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.commandParameterField;
            }

            set
            {
                if (this.commandParameterField != value)
                {
                    this.commandParameterField = value;
                    this.OnPropertyChanged("CommandParameter");
                }
            }
        }

        /// <summary>
        /// Gets or sets state for this mode of the button.
        /// </summary>
        public string State
        {
            get
            {
                return this.stateField;
            }

            set
            {
                if (this.stateField != value)
                {
                    this.stateField = value;
                    this.OnPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Gets or sets command parameter for this mode of the button.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uriField;
            }

            set
            {
                if (this.uriField != value)
                {
                    this.uriField = value;
                    this.OnPropertyChanged("Uri");
                }
            }
        }
    }
}