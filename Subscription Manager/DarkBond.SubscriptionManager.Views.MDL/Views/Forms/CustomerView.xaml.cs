// <copyright file="CustomerView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views.Forms
{
    using System.Composition;
    using DarkBond.LicenseManager.ViewModels.Forms;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Interaction logic for <see cref="CustomerView"/>.
    /// </summary>
    public partial class CustomerView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerView"/> class.
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        [ImportingConstructor]
        public CustomerView(CustomerViewModel customerViewModel)
        {
            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // Create a new data context for this view.
            this.DataContext = customerViewModel;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public CustomerViewModel ViewModel
        {
            get
            {
                return this.DataContext as CustomerViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }
    }
}