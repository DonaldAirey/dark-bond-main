// <copyright file="ColumnViewRowPanelXamlType.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using DarkBond.Views.Controls;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Hand-crafted metadata for the <see cref="ColumnViewRowPanel"/> class.
    /// </summary>
    public class ColumnViewRowPanelXamlType : XamlType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewRowPanelXamlType"/> class.
        /// </summary>
        public ColumnViewRowPanelXamlType()
            : base(typeof(StackPanel), typeof(ColumnViewRowPanel))
        {
            // Set the properties of this type.
            this.IsConstructible = true;
        }

        /// <summary>
        /// Sets its values for initialization and returns a usable instance.
        /// </summary>
        /// <returns>The usable instance.</returns>
        public override object ActivateInstance()
        {
            return new ColumnViewRowPanel();
        }
    }
}