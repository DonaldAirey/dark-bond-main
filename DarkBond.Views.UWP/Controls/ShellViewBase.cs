// <copyright file="ShellViewBase.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Used to provide a shell view around an application.
    /// </summary>
    public class ShellViewBase : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewBase"/> class.
        /// </summary>
        public ShellViewBase()
        {
            // This keeps the content sized to the container.
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
        }
    }
}