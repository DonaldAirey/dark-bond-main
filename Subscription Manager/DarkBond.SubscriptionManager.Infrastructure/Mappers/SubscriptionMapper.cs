// <copyright file="SubscriptionMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using System;
    using System.Composition;
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Used to map subscription records.
    /// </summary>
    [Export(typeof(ISubscriptionMapper))]
    public class SubscriptionMapper : ISubscriptionMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="subscriptionViewModel">The property view model.</param>
        public void Clear(ViewModels.Forms.SubscriptionViewModel subscriptionViewModel)
        {
            // Validate the parameter.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Clear the view model.
            subscriptionViewModel.UnderwriterId = null;
            subscriptionViewModel.SubscriptionId = default(Guid);
            subscriptionViewModel.OfferingId = null;
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="subscriptionRow">The data model row.</param>
        /// <param name="subscriptionViewModel">The property view model record.</param>
        public void Map(SubscriptionRow subscriptionRow, ViewModels.Forms.SubscriptionViewModel subscriptionViewModel)
        {
            // Validate the parameter.
            if (subscriptionRow == null)
            {
                throw new ArgumentNullException(nameof(subscriptionRow));
            }

            // Validate the parameter.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Copy the data model into the view model.
            subscriptionViewModel.UnderwriterId = subscriptionRow.UnderwriterId;
            subscriptionViewModel.SubscriptionId = subscriptionRow.SubscriptionId;
            subscriptionViewModel.OfferingId = subscriptionRow.OfferingId;
        }

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="subscriptionRow">The data model row.</param>
        /// <param name="subscriptionViewModel">The list view model.</param>
        public void Map(SubscriptionRow subscriptionRow, ViewModels.ListViews.SubscriptionViewModel subscriptionViewModel)
        {
            // Validate the parameter.
            if (subscriptionRow == null)
            {
                throw new ArgumentNullException(nameof(subscriptionRow));
            }

            // Validate the parameter.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Copy the data model into the view model.
            subscriptionViewModel.UnderwriterId = subscriptionRow.UnderwriterId;
            subscriptionViewModel.DateCreated = subscriptionRow.DateCreated;
            subscriptionViewModel.DateModified = subscriptionRow.DateModified;
            subscriptionViewModel.SubscriptionId = subscriptionRow.SubscriptionId;
            subscriptionViewModel.OfferingId = subscriptionRow.OfferingId;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="subscriptionViewModel">The property view model.</param>
        /// <param name="subscription">A License business entity.</param>
        /// <returns>A data model row.</returns>
        public Subscription Map(ViewModels.Forms.SubscriptionViewModel subscriptionViewModel, Subscription subscription)
        {
            // Validate the parameter.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Validate the parameter.
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            subscription.UnderwriterId = subscriptionViewModel.UnderwriterId.Value;
            subscription.SubscriptionId = subscriptionViewModel.SubscriptionId.Value;
            subscription.OfferingId = subscriptionViewModel.OfferingId.Value;
            return subscription;
        }
    }
}
