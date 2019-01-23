// <copyright file="CustomerMapper.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Mappers
{
    using System;
    using System.Composition;
    using System.Globalization;
    using DarkBond.LicenseManager.Entities;

    /// <summary>
    /// Used to map customer records.
    /// </summary>
    [Export(typeof(ICustomerMapper))]
    public class CustomerMapper : ICustomerMapper
    {
        /// <summary>
        /// Creates a display name from the data model record of a customer.
        /// </summary>
        /// <param name="customerRow">The data model record of a customer.</param>
        /// <returns>A name to be displayed in the user interface.</returns>
        public static string CreateDisplayName(CustomerRow customerRow)
        {
            // Validate the parameter
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            return CreateDisplayName(customerRow.LastName, customerRow.FirstName);
        }

        /// <summary>
        /// Creates a display name from the data model record of a customer.
        /// </summary>
        /// <param name="lastName">The first name.</param>
        /// <param name="firstName">The last name.</param>
        /// <returns>A name to be displayed in the user interface.</returns>
        public static string CreateDisplayName(string lastName, string firstName)
        {
            // It's possible to have a person with just a single name, either first or last.  This will format the displayed name accordingly.
            string name = string.Empty;
            if (lastName == null)
            {
                name = firstName;
            }
            else
            {
                if (firstName == null)
                {
                    name = lastName;
                }
                else
                {
                    name = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", lastName, firstName);
                }
            }

            return name.Trim();
        }

        /// <summary>
        /// Clears a view model of all data.
        /// </summary>
        /// <param name="customerViewModel">The property view model.</param>
        public void Clear(ViewModels.Forms.CustomerViewModel customerViewModel)
        {
            // Validate the parameter
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            customerViewModel.Address1 = null;
            customerViewModel.Address2 = null;
            customerViewModel.City = null;
            customerViewModel.CustomerId = default(Guid);
            customerViewModel.Company = null;
            customerViewModel.CountryId = Guid.Empty;
            customerViewModel.Email = null;
            customerViewModel.FirstName = null;
            customerViewModel.MiddleName = null;
            customerViewModel.LastName = null;
            customerViewModel.PhoneNumber = null;
            customerViewModel.PostalCode = null;
            customerViewModel.ProvinceId = Guid.Empty;
        }

        /// <summary>
        /// Maps the data model row into the property view model record.
        /// </summary>
        /// <param name="customerRow">The data model row.</param>
        /// <param name="customerViewModel">The property view model record.</param>
        public void Map(CustomerRow customerRow, ViewModels.Forms.CustomerViewModel customerViewModel)
        {
            // Validate the parameter
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            // Validate the parameter
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            // Map the data from the data model to the view model.
            customerViewModel.Address1 = customerRow.Address1;
            customerViewModel.Address2 = customerRow.Address2;
            customerViewModel.City = customerRow.City;
            customerViewModel.CustomerId = customerRow.CustomerId;
            customerViewModel.Company = customerRow.Company;
            customerViewModel.CountryId = customerRow.CountryId;
            customerViewModel.DateOfBirth = customerRow.DateOfBirth;
            customerViewModel.Email = customerRow.Email;
            customerViewModel.FirstName = customerRow.FirstName;
            customerViewModel.MiddleName = customerRow.MiddleName;
            customerViewModel.LastName = customerRow.LastName;
            customerViewModel.PhoneNumber = customerRow.Phone;
            customerViewModel.PostalCode = customerRow.PostalCode;
            customerViewModel.ProvinceId = customerRow.ProvinceId;
        }

        /// <summary>
        /// Maps the data model row into the list view model record.
        /// </summary>
        /// <param name="customerRow">The data model row.</param>
        /// <param name="customerViewModel">The list view model.</param>
        public void Map(CustomerRow customerRow, ViewModels.ListViews.CustomerViewModel customerViewModel)
        {
            // Validate the parameter
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            // Validate the parameter
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            // Map the data from the data model to the view model.
            customerViewModel.Name = CustomerMapper.CreateDisplayName(customerRow);
            customerViewModel.Address1 = customerRow.Address1;
            customerViewModel.Address2 = customerRow.Address2;
            customerViewModel.City = customerRow.City;
            customerViewModel.CustomerId = customerRow.CustomerId;
            customerViewModel.Company = customerRow.Company;
            customerViewModel.CountryId = customerRow.CountryId;
            customerViewModel.DateCreated = customerRow.DateCreated;
            customerViewModel.DateModified = customerRow.DateModified;
            customerViewModel.Email = customerRow.Email;
            customerViewModel.FirstName = customerRow.FirstName;
            customerViewModel.MiddleName = customerRow.MiddleName;
            customerViewModel.LastName = customerRow.LastName;
            customerViewModel.PhoneNumber = customerRow.Phone;
            customerViewModel.PostalCode = customerRow.PostalCode;
            customerViewModel.ProvinceId = customerRow.ProvinceId;
        }

        /// <summary>
        /// Maps the property view model into the data model row.
        /// </summary>
        /// <param name="customerViewModel">The property view model.</param>
        /// <param name="customer">The data model row.</param>
        /// <returns>A data model row.</returns>
        public Customer Map(ViewModels.Forms.CustomerViewModel customerViewModel, Customer customer)
        {
            // Validate the parameter.
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            // Validate the parameter.
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            // Map the data from the view model into the data model.
            customer.Address1 = customerViewModel.Address1;
            customer.Address2 = string.IsNullOrEmpty(customerViewModel.Address2) ? null : customerViewModel.Address2;
            customer.City = customerViewModel.City;
            customer.Company = customerViewModel.Company;
            customer.CountryId = customerViewModel.CountryId;
            customer.CustomerId = customerViewModel.CustomerId.Value;
            customer.DateOfBirth = customerViewModel.DateOfBirth;
            customer.Email = customerViewModel.Email;
            customer.FirstName = string.IsNullOrEmpty(customerViewModel.FirstName) ? null : customerViewModel.FirstName.Trim();
            customer.MiddleName = string.IsNullOrEmpty(customerViewModel.MiddleName) ? null : customerViewModel.MiddleName.Trim();
            customer.LastName = string.IsNullOrEmpty(customerViewModel.LastName) ? null : customerViewModel.LastName.Trim();
            customer.PhoneNumber = customerViewModel.PhoneNumber;
            customer.PostalCode = customerViewModel.PostalCode;
            customer.ProvinceId = customerViewModel.ProvinceId;

            // A fully populate customer.
            return customer;
        }
    }
}