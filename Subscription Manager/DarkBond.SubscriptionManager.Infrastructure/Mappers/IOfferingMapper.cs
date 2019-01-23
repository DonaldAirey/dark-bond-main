// <copyright file="IOfferingMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Interface for mapping offering records.
    /// </summary>
    public interface IOfferingMapper
    {
        /// <summary>
        /// Clears the view model.
        /// </summary>
        /// <param name="offeringViewModel">The view model.</param>
        void Clear(ViewModels.Forms.OfferingViewModel offeringViewModel);

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="offeringRow">The data model row.</param>
        /// <param name="offeringViewModel">The property view model record.</param>
        void Map(OfferingRow offeringRow, ViewModels.Forms.OfferingViewModel offeringViewModel);

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="offeringRow">The data model row.</param>
        /// <param name="offeringViewModel">The list view model.</param>
        void Map(OfferingRow offeringRow, ViewModels.ListViews.OfferingViewModel offeringViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="offeringViewModel">The property view model.</param>
        /// <param name="offering">A offering business entity.</param>
        /// <returns>A data model row.</returns>
        Offering Map(ViewModels.Forms.OfferingViewModel offeringViewModel, Offering offering);
    }
}