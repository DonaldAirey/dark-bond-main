// <copyright file="UnderwriterView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Forms
{
    using System.Composition;
    using DarkBond.SubscriptionManager.ViewModels.Forms;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Interaction logic for <see cref="UnderwriterView"/>.
    /// </summary>
    public partial class UnderwriterView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterView"/> class.
        /// </summary>
        /// <param name="underwriterViewModel">The underwriter view model.</param>
        [ImportingConstructor]
        public UnderwriterView(UnderwriterViewModel underwriterViewModel)
        {
            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // Create a new data context for this view.
            this.DataContext = underwriterViewModel;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public UnderwriterViewModel ViewModel
        {
            get
            {
                return this.DataContext as UnderwriterViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }
    }
}