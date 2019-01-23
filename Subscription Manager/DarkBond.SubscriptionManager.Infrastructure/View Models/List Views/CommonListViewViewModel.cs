// <copyright file="CommonListViewViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.ListViews
{
    using System;
    using System.Composition;
    using DarkBond.ViewModels;

    /// <summary>
    /// A subscription who can own a subscription.
    /// </summary>
    public abstract class CommonListViewViewModel : ListItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonListViewViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        protected CommonListViewViewModel(CompositionContext compositionContext, DataModel dataModel, ISubscriptionService subscriptionService)
        {
            // Validate the parameter.
            if (compositionContext == null)
            {
                throw new ArgumentNullException(nameof(compositionContext));
            }

            // Validate the parameter.
            if (dataModel == null)
            {
                throw new ArgumentNullException(nameof(dataModel));
            }

            // Validate the parameter.
            if (subscriptionService == null)
            {
                throw new ArgumentNullException(nameof(subscriptionService));
            }

            // Initialize the object.
            this.CompositionContext = compositionContext;
            this.DataModel = dataModel;
            this.SubscriptionService = subscriptionService;
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
        /// Gets the subscription service.
        /// </summary>
        protected ISubscriptionService SubscriptionService { get; private set; }
    }
}