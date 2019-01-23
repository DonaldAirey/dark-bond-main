// <copyright file="CustomerComboBox.cs" company="Dark Bond, Inc.">
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
    public class CustomerComboBox : ComboBox<Guid?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerComboBox"/> class.
        /// </summary>
        public CustomerComboBox()
        {
            // This ComboBox is designed to be paired to a Customer view model.
            this.DisplayMemberPath = "Name";
            this.SelectedValuePath = "UnderwriterId";
        }
    }
}
