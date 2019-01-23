// <copyright file="FrameView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views
{
    using System.Windows;
    using DarkBond.Views.Controls;
    using DarkBond.LicenseManager.ViewModels;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FrameView : FrameViewBase
    {
        /// <summary>
        /// Identifies the ViewValue dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewValuePropertyField = DependencyProperty.Register(
            "ViewValue",
            typeof(int),
            typeof(FrameView),
            new PropertyMetadata(0));

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameView"/> class.
        /// </summary>
        /// <param name="frameViewViewModel">The view model.</param>
        public FrameView(FrameViewModel frameViewViewModel)
        {
            // Create a view model for this view.
            this.DataContext = frameViewViewModel;

            // Initialize the IDE maintained components.
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the LicenseViewValue which controls the configuration of controls in the form.
        /// </summary>
        public int ViewValue
        {
            get
            {
                return (int)this.GetValue(FrameView.ViewValuePropertyField);
            }

            set
            {
                this.SetValue(FrameView.ViewValuePropertyField, value);
            }
        }

        /// <summary>
        /// Gets the view model for this view.
        /// </summary>
        public FrameViewModel ViewModel
        {
            get
            {
                return this.DataContext as FrameViewModel;
            }
        }
    }
}