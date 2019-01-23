// <copyright file="ProductView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views.Forms
{
    using System.Composition;
    using DarkBond.LicenseManager.ViewModels.Forms;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Interaction logic for <see cref="ProductView"/>.
    /// </summary>
    public partial class ProductView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductView"/> class.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        [ImportingConstructor]
        public ProductView(ProductViewModel productViewModel)
        {
            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // Create a new data context for this view.
            this.DataContext = productViewModel;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public ProductViewModel ViewModel
        {
            get
            {
                return this.DataContext as ProductViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }
    }
}