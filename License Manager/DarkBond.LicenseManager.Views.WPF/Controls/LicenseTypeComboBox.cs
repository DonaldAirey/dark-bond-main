// <copyright file="LicenseTypeComboBox.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Controls
{
    using DarkBond.Views.Controls;

    /// <summary>
    /// ComboBox used to select a license type.
    /// </summary>
    public class LicenseTypeComboBox : ComboBox<LicenseTypeCode?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseTypeComboBox"/> class.
        /// </summary>
        public LicenseTypeComboBox()
        {
            // This ComboBox is designed to be paired to a LicenseTypeViewModel.
            this.DisplayMemberPath = "Description";
            this.SelectedValuePath = "LicenseTypeCode";
        }
   }
}