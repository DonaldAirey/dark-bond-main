// <copyright file="CommonBreadcrumbViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Composition;
    using DarkBond.ViewModels;

    /// <summary>
    /// View Model for the directory that shows the customers.
    /// </summary>
    public abstract class CommonBreadcrumbViewModel : BreadcrumbItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonBreadcrumbViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        protected CommonBreadcrumbViewModel(CompositionContext compositionContext, DataModel dataModel)
        {
            // Validate the compositionContext parameter.
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Initialize the object.
            this.CompositionContext = compositionContext;
            this.DataModel = dataModel;
        }

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        protected CompositionContext CompositionContext { get; private set; }

        /// <summary>
        /// Gets the data model.
        /// </summary>
        protected DataModel DataModel { get; private set; }
    }
}