// <copyright file="SubscriptionTypeComboBox.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Controls
{
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.Views.Controls;

    /// <summary>
    /// ComboBox used to select a subscription type.
    /// </summary>
    public class SubscriptionTypeComboBox : ComboBox<SubscriptionTypeCode?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTypeComboBox"/> class.
        /// </summary>
        public SubscriptionTypeComboBox()
        {
            // This ComboBox is designed to be paired to a SubscriptionTypeViewModel.
            this.DisplayMemberPath = "Description";
            this.SelectedValuePath = "SubscriptionTypeCode";
        }
   }
}