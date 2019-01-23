// <copyright file="ProvinceComboBox.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Controls
{
    using System;
    using DarkBond.Views.Controls;

    /// <summary>
    /// ComboBox used to select a underwriter.
    /// </summary>
    public class ProvinceComboBox : ComboBox<ProvinceCountryKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvinceComboBox"/> class.
        /// </summary>
        public ProvinceComboBox()
        {
            // This ComboBox is designed to be paired to a Product view model.
            this.DisplayMemberPath = "Name";
            this.SelectedValuePath = "ProvinceCountryKey";
        }
    }
}
