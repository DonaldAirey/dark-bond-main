// <copyright file="FrameView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views
{
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.SubscriptionManager.ViewModels;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FrameView : Page
    {
        /// <summary>
        /// Identifies the ViewValue dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ViewValueProperty = DependencyProperty.Register(
            "ViewValue",
            typeof(int),
            typeof(FrameView),
            new PropertyMetadata(0));

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameView"/> class.
        /// </summary>
        /// <param name="frameViewViewModel">The view model.</param>
        [ImportingConstructor]
        public FrameView(FrameViewModel frameViewViewModel)
        {
            // Create a view model for this view.
            this.DataContext = frameViewViewModel;

            // Initialize the IDE maintained components.
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the SubscriptionViewValue which controls the configuration of controls in the form.
        /// </summary>
        public int ViewValue
        {
            get
            {
                return (int)this.GetValue(FrameView.ViewValueProperty);
            }

            set
            {
                this.SetValue(FrameView.ViewValueProperty, value);
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