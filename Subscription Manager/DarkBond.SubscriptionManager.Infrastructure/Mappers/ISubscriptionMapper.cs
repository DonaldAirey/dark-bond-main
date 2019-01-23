// <copyright file="ISubscriptionMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using DarkBond.SubscriptionManager;
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Interface for mapping subscription records.
    /// </summary>
    public interface ISubscriptionMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="subscriptionViewModel">The property view model.</param>
        void Clear(ViewModels.Forms.SubscriptionViewModel subscriptionViewModel);

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="subscriptionRow">The data model row.</param>
        /// <param name="subscriptionViewModel">The property view model record.</param>
        void Map(SubscriptionRow subscriptionRow, ViewModels.Forms.SubscriptionViewModel subscriptionViewModel);

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="subscriptionRow">The data model row.</param>
        /// <param name="subscriptionViewModel">The list view model.</param>
        void Map(SubscriptionRow subscriptionRow, ViewModels.ListViews.SubscriptionViewModel subscriptionViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="subscriptionViewModel">The property view model.</param>
        /// <param name="subscription">A License business entity.</param>
        /// <returns>A data model row.</returns>
        Subscription Map(ViewModels.Forms.SubscriptionViewModel subscriptionViewModel, Subscription subscription);
    }
}