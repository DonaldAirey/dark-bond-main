// <copyright file="ViewButtonViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System.Windows.Input;

    /// <summary>
    /// View model for the button that allows the user to select a view in a directory of items.
    /// </summary>
    public class ViewButtonViewModel : ButtonViewModel
    {
        /// <summary>
        /// The state of the button.
        /// </summary>
        private string stateField;

        /// <summary>
        /// Gets the command to change the view.
        /// </summary>
        public static ICommand ChangeView
        {
            get
            {
                return GlobalCommands.ChangeView as ICommand;
            }
        }

        /// <summary>
        /// Gets or sets the view value (the view selected and the magnification).
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
    }
}