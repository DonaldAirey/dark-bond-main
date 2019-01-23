// <copyright file="CommonTreeViewViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.TreeViews
{
    using System;
    using System.Composition;
    using DarkBond.ViewModels;

    /// <summary>
    /// A navigation tree view item for the product.
    /// </summary>
    public class CommonTreeViewViewModel : NavigationTreeItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonTreeViewViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        protected CommonTreeViewViewModel(CompositionContext compositionContext, DataModel dataModel, ILicenseService licenseService)
        {
            // Validate the parameter
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // Validate the parameter
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Validate the parameter
            if (licenseService == null)
            {
                throw new ArgumentNullException(nameof(licenseService));
            }

            // Initialize the object.
            this.CompositionContext = compositionContext;
            this.DataModel = dataModel;
            this.LicenseService = licenseService;
        }

        /// <summary>
        /// Gets the composition context.
        /// </summary>
        protected CompositionContext CompositionContext { get; private set; }

        /// <summary>
        /// Gets the data model.
        /// </summary>
        protected DataModel DataModel { get; private set; }

        /// <summary>
        /// Gets the license service.
        /// </summary>
        protected ILicenseService LicenseService { get; private set; }
    }
}