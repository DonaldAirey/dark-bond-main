// <copyright file="NormalTextViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;

    /// <summary>
    /// A view model for button drop down controls.
    /// </summary>
    public class NormalTextViewModel : ViewModel
    {
        /// <summary>
        /// The normal text.
        /// </summary>
        private string textField;

        /// <summary>
        /// Gets or sets the normal text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.textField;
            }

            set
            {
                if (value != this.textField)
                {
                    this.textField = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }
    }
}
