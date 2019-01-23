// <copyright file="OfferingMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using System;
    using System.Composition;
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Used to map offering records.
    /// </summary>
    [Export(typeof(IOfferingMapper))]
    public class OfferingMapper : IOfferingMapper
    {
        /// <summary>
        /// Clears the view model.
        /// </summary>
        /// <param name="offeringViewModel">The view model.</param>
        public void Clear(ViewModels.Forms.OfferingViewModel offeringViewModel)
        {
            // Validate the parameter.
            if (offeringViewModel == null)
            {
                throw new ArgumentNullException(nameof(offeringViewModel));
            }

            offeringViewModel.Description = null;
            offeringViewModel.Name = null;
            offeringViewModel.OfferingId = default(Guid);
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="offeringRow">The data model row.</param>
        /// <param name="offeringViewModel">The property view model record.</param>
        public void Map(OfferingRow offeringRow, ViewModels.Forms.OfferingViewModel offeringViewModel)
        {
            // Validate the parameter.
            if (offeringRow == null)
            {
                throw new ArgumentNullException(nameof(offeringRow));
            }

            // Validate the parameter.
            if (offeringViewModel == null)
            {
                throw new ArgumentNullException(nameof(offeringViewModel));
            }

            // Map the data model into the view model.
            offeringViewModel.Description = offeringRow.Description;
            offeringViewModel.Name = offeringRow.Name;
            offeringViewModel.OfferingId = offeringRow.OfferingId;
        }

        /// <summary>
        /// Maps the data model row into the tree view model record.
        /// </summary>
        /// <param name="offeringRow">The data model row.</param>
        /// <param name="offeringViewModel">The tree view model record.</param>
        public void Map(OfferingRow offeringRow, ViewModels.ListViews.OfferingViewModel offeringViewModel)
        {
            // Validate the parameter.
            if (offeringRow == null)
            {
                throw new ArgumentNullException(nameof(offeringRow));
            }

            // Validate the parameter.
            if (offeringViewModel == null)
            {
                throw new ArgumentNullException(nameof(offeringViewModel));
            }

            // Map the data model into the view model.
            offeringViewModel.DateCreated = offeringRow.DateCreated;
            offeringViewModel.DateModified = offeringRow.DateModified;
            offeringViewModel.Description = offeringRow.Description;
            offeringViewModel.Name = offeringRow.Name;
            offeringViewModel.OfferingId = offeringRow.OfferingId;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="offeringViewModel">The property view model.</param>
        /// <param name="offering">A offering business entity.</param>
        /// <returns>A data model row.</returns>
        public Offering Map(ViewModels.Forms.OfferingViewModel offeringViewModel, Offering offering)
        {
            // Validate the parameter.
            if (offeringViewModel == null)
            {
                throw new ArgumentNullException(nameof(offeringViewModel));
            }

            // Validate the parameter.
            if (offering == null)
            {
                throw new ArgumentNullException(nameof(offering));
            }

            offering.Description = string.IsNullOrEmpty(offeringViewModel.Description) ? null : offeringViewModel.Description;
            offering.Name = offeringViewModel.Name.Trim();
            offering.OfferingId = offeringViewModel.OfferingId.Value;
            return offering;
        }
    }
}