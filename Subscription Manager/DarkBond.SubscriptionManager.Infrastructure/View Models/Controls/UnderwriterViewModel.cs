// <copyright file="UnderwriterViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using DarkBond.ViewModels;

    /// <summary>
    /// The view model of a underwriter.
    /// </summary>
    public class UnderwriterViewModel : ViewModel
    {
        /// <summary>
        /// The first address line.
        /// </summary>
        private string address1Field;

        /// <summary>
        /// The second address line.
        /// </summary>
        private string address2Field;

        /// <summary>
        /// The city.
        /// </summary>
        private string cityField;

        /// <summary>
        /// The country.
        /// </summary>
        private Guid countryIdField;

        /// <summary>
        /// The unique identifier of this underwriter.
        /// </summary>
        private Guid underwriterIdField;

        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailField;

        /// <summary>
        /// The first name.
        /// </summary>
        private string primaryContactField;

        /// <summary>
        /// The last name.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> notifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// The phone number.
        /// </summary>
        private string phoneField;

        /// <summary>
        /// The postal code.
        /// </summary>
        private string postalCodeField;

        /// <summary>
        /// The province (state).
        /// </summary>
        private Guid? provinceIdField;

        /// <summary>
        /// Gets or sets the first address line.
        /// </summary>
        public string Address1
        {
            get
            {
                return this.address1Field;
            }

            set
            {
                if (this.address1Field != value)
                {
                    this.address1Field = value;
                    this.OnPropertyChanged("Address1");
                }
            }
        }

        /// <summary>
        /// Gets or sets the second address line.
        /// </summary>
        public string Address2
        {
            get
            {
                return this.address2Field;
            }

            set
            {
                if (this.address2Field != value)
                {
                    this.address2Field = value;
                    this.OnPropertyChanged("Address2");
                }
            }
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City
        {
            get
            {
                return this.cityField;
            }

            set
            {
                if (this.cityField != value)
                {
                    this.cityField = value;
                    this.OnPropertyChanged("City");
                }
            }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public Guid CountryId
        {
            get
            {
                return this.countryIdField;
            }

            set
            {
                if (this.countryIdField != value)
                {
                    this.countryIdField = value;
                    this.OnPropertyChanged("Country");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for this underwriter.
        /// </summary>
        public Guid UnderwriterId
        {
            get
            {
                return this.underwriterIdField;
            }

            set
            {
                if (this.underwriterIdField != value)
                {
                    this.underwriterIdField = value;
                    this.OnPropertyChanged("UnderwriterId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email
        {
            get
            {
                return this.emailField;
            }

            set
            {
                if (this.emailField != value)
                {
                    this.emailField = value;
                    this.OnPropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string PrimaryContact
        {
            get
            {
                return this.primaryContactField;
            }

            set
            {
                if (this.primaryContactField != value)
                {
                    this.primaryContactField = value;
                    this.OnPropertyChanged("PrimaryContact");
                }
            }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameField;
            }

            set
            {
                if (this.nameField != value)
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string Phone
        {
            get
            {
                return this.phoneField;
            }

            set
            {
                if (this.phoneField != value)
                {
                    this.phoneField = value;
                    this.OnPropertyChanged("Phone");
                }
            }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }

            set
            {
                if (this.postalCodeField != value)
                {
                    this.postalCodeField = value;
                    this.OnPropertyChanged("PostalCode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the province (state).
        /// </summary>
        public Guid? ProvinceId
        {
            get
            {
                return this.provinceIdField;
            }

            set
            {
                if (this.provinceIdField != value)
                {
                    this.provinceIdField = value;
                    this.OnPropertyChanged("ProvinceId");
                }
            }
        }

        /// <summary>
        /// Maps the properties of a <see cref="CountryRow"/> to a <see cref="CountryViewModel"/>.
        /// </summary>
        /// <param name="underwriterRow">A country row.</param>
        public void Map(UnderwriterRow underwriterRow)
        {
            // Validate the parameter.
            if (underwriterRow == null)
            {
                throw new ArgumentNullException(nameof(underwriterRow));
            }

            // Initialize the object.
            this.underwriterRowField = underwriterRow;
            underwriterRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Address1", (p) => this.Address1 = p.Address1);
            this.notifyActions.Add("Address2", (p) => this.Address2 = p.Address2);
            this.notifyActions.Add("City", (p) => this.City = p.City);
            this.notifyActions.Add("CountryId", (p) => this.CountryId = p.CountryId);
            this.notifyActions.Add("UnderwriterId", (p) => this.UnderwriterId = p.UnderwriterId);
            this.notifyActions.Add("Email", (p) => this.Email = p.Email);
            this.notifyActions.Add("PrimaryContact", (p) => this.PrimaryContact = p.PrimaryContact);
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);
            this.notifyActions.Add("Phone", (p) => this.Phone = p.Phone);
            this.notifyActions.Add("PostalCode", (p) => this.PostalCode = p.PostalCode);
            this.notifyActions.Add("ProvinceId", (p) => this.ProvinceId = p.ProvinceId);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.underwriterRowField);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disengage from the notifications from the data model.
            this.underwriterRowField.PropertyChanged -= this.OnPropertyChanged;

            // Allow the base class to dispose.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the property of the underlying data model.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<UnderwriterRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(sender as UnderwriterRow);
            }
        }
    }
}