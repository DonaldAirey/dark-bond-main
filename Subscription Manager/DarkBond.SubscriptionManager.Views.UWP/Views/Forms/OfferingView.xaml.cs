// <copyright file="OfferingView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Forms
{
    using System.Composition;
    using DarkBond.SubscriptionManager.ViewModels.Forms;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Interaction logic for <see cref="OfferingView"/>.
    /// </summary>
    public partial class OfferingView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingView"/> class.
        /// </summary>
        /// <param name="offeringViewModel">The offering view model.</param>
        [ImportingConstructor]
        public OfferingView(OfferingViewModel offeringViewModel)
        {
            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // Create a new data context for this view.
            this.DataContext = offeringViewModel;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public OfferingViewModel ViewModel
        {
            get
            {
                return this.DataContext as OfferingViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }
    }
}