// <copyright file="IUnderwriterMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Interface for mapping underwriter records.
    /// </summary>
    public interface IUnderwriterMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="underwriterViewModel">The property view model.</param>
        void Clear(ViewModels.Forms.UnderwriterViewModel underwriterViewModel);

        /// <summary>
        /// Maps the data model row into the property view model.
        /// </summary>
        /// <param name="underwriterRow">The data model row.</param>
        /// <param name="underwriterViewModel">The property view model.</param>
        void Map(UnderwriterRow underwriterRow, ViewModels.Forms.UnderwriterViewModel underwriterViewModel);

        /// <summary>
        /// Maps the data model row into the list view model.
        /// </summary>
        /// <param name="underwriterRow">The data model row.</param>
        /// <param name="underwriterViewModel">The list view model.</param>
        void Map(UnderwriterRow underwriterRow, ViewModels.ListViews.UnderwriterViewModel underwriterViewModel);

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="underwriterViewModel">The property view model.</param>
        /// <param name="underwriter">A underwriter business entity.</param>
        /// <returns>A data model row.</returns>
        Underwriter Map(ViewModels.Forms.UnderwriterViewModel underwriterViewModel, Underwriter underwriter);
    }
}