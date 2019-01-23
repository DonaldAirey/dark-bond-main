// <copyright file="UnderwriterMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Mappers
{
    using System;
    using System.Composition;
    using System.Globalization;
    using DarkBond.SubscriptionManager.Entities;

    /// <summary>
    /// Used to map underwriter records.
    /// </summary>
    [Export(typeof(IUnderwriterMapper))]
    public class UnderwriterMapper : IUnderwriterMapper
    {
        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="underwriterViewModel">The property view model.</param>
        public void Clear(ViewModels.Forms.UnderwriterViewModel underwriterViewModel)
        {
            // Validate the parameter
            if (underwriterViewModel == null)
            {
                throw new ArgumentNullException(nameof(underwriterViewModel));
            }

            underwriterViewModel.Address1 = null;
            underwriterViewModel.Address2 = null;
            underwriterViewModel.City = null;
            underwriterViewModel.UnderwriterId = default(Guid);
            underwriterViewModel.CountryId = Guid.Empty;
            underwriterViewModel.Email = null;
            underwriterViewModel.PrimaryContact = null;
            underwriterViewModel.Name = null;
            underwriterViewModel.PhoneNumber = null;
            underwriterViewModel.PostalCode = null;
            underwriterViewModel.ProvinceId = Guid.Empty;
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="underwriterRow">The data model row.</param>
        /// <param name="underwriterViewModel">The property view model record.</param>
        public void Map(UnderwriterRow underwriterRow, ViewModels.Forms.UnderwriterViewModel underwriterViewModel)
        {
            // Validate the parameter
            if (underwriterRow == null)
            {
                throw new ArgumentNullException(nameof(underwriterRow));
            }

            // Validate the parameter
            if (underwriterViewModel == null)
            {
                throw new ArgumentNullException(nameof(underwriterViewModel));
            }

            // Map the data from the data model to the view model.
            underwriterViewModel.Address1 = underwriterRow.Address1;
            underwriterViewModel.Address2 = underwriterRow.Address2;
            underwriterViewModel.City = underwriterRow.City;
            underwriterViewModel.UnderwriterId = underwriterRow.UnderwriterId;
            underwriterViewModel.CountryId = underwriterRow.CountryId;
            underwriterViewModel.DateOfBirth = underwriterRow.DateOfBirth;
            underwriterViewModel.Email = underwriterRow.Email;
            underwriterViewModel.Name = underwriterRow.Name;
            underwriterViewModel.PrimaryContact = underwriterRow.PrimaryContact;
            underwriterViewModel.PhoneNumber = underwriterRow.Phone;
            underwriterViewModel.PostalCode = underwriterRow.PostalCode;
            underwriterViewModel.ProvinceId = underwriterRow.ProvinceId;
        }

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="underwriterRow">The data model row.</param>
        /// <param name="underwriterViewModel">The list view model.</param>
        public void Map(UnderwriterRow underwriterRow, ViewModels.ListViews.UnderwriterViewModel underwriterViewModel)
        {
            // Validate the parameter
            if (underwriterRow == null)
            {
                throw new ArgumentNullException(nameof(underwriterRow));
            }

            // Validate the parameter
            if (underwriterViewModel == null)
            {
                throw new ArgumentNullException(nameof(underwriterViewModel));
            }

            // Map the data from the data model to the view model.
            underwriterViewModel.Address1 = underwriterRow.Address1;
            underwriterViewModel.Address2 = underwriterRow.Address2;
            underwriterViewModel.City = underwriterRow.City;
            underwriterViewModel.UnderwriterId = underwriterRow.UnderwriterId;
            underwriterViewModel.CountryId = underwriterRow.CountryId;
            underwriterViewModel.DateCreated = underwriterRow.DateCreated;
            underwriterViewModel.DateModified = underwriterRow.DateModified;
            underwriterViewModel.Email = underwriterRow.Email;
            underwriterViewModel.PrimaryContact = underwriterRow.PrimaryContact;
            underwriterViewModel.Name = underwriterRow.Name;
            underwriterViewModel.PhoneNumber = underwriterRow.Phone;
            underwriterViewModel.PostalCode = underwriterRow.PostalCode;
            underwriterViewModel.ProvinceId = underwriterRow.ProvinceId;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="underwriterViewModel">The property view model.</param>
        /// <param name="underwriter">The data model row.</param>
        /// <returns>A data model row.</returns>
        public Underwriter Map(ViewModels.Forms.UnderwriterViewModel underwriterViewModel, Underwriter underwriter)
        {
            // Validate the parameter.
            if (underwriterViewModel == null)
            {
                throw new ArgumentNullException(nameof(underwriterViewModel));
            }

            // Validate the parameter.
            if (underwriter == null)
            {
                throw new ArgumentNullException(nameof(underwriter));
            }

            // Map the data from the view model into the data model.
            underwriter.Address1 = underwriterViewModel.Address1;
            underwriter.Address2 = string.IsNullOrEmpty(underwriterViewModel.Address2) ? null : underwriterViewModel.Address2;
            underwriter.City = underwriterViewModel.City;
            underwriter.CountryId = underwriterViewModel.CountryId;
            underwriter.UnderwriterId = underwriterViewModel.UnderwriterId.Value;
            underwriter.DateOfBirth = underwriterViewModel.DateOfBirth;
            underwriter.Email = underwriterViewModel.Email;
            underwriter.PrimaryContact = string.IsNullOrEmpty(underwriterViewModel.PrimaryContact) ? null : underwriterViewModel.PrimaryContact.Trim();
            underwriter.Name = string.IsNullOrEmpty(underwriterViewModel.Name) ? null : underwriterViewModel.Name.Trim();
            underwriter.PhoneNumber = underwriterViewModel.PhoneNumber;
            underwriter.PostalCode = underwriterViewModel.PostalCode;
            underwriter.ProvinceId = underwriterViewModel.ProvinceId;

            // A fully populate underwriter.
            return underwriter;
        }
    }
}