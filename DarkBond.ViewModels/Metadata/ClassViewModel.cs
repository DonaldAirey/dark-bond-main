// <copyright file="ClassViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;

    /// <summary>
    /// View model for the header of the item that is displayed in the details bar.
    /// </summary>
    public class ClassViewModel : TextViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassViewModel"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ClassViewModel(string text)
            : base(text)
        {
        }
    }
}