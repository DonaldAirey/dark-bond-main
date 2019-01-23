// <copyright file="ProvinceViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DarkBond.ViewModels;

    /// <summary>
    /// The view model for a province (state).
    /// </summary>
    public class ProvinceViewModel : ViewModel
    {
        /// <summary>
        /// The abbreviated version of the name.
        /// </summary>
        private string abbreviationField;

        /// <summary>
        /// The unique identifier of the country.
        /// </summary>
        private Guid countryIdField;

        /// <summary>
        /// The name of the province.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<ProvinceRow>> notifyActions = new Dictionary<string, Action<ProvinceRow>>();

        /// <summary>
        /// The unique identifier of the province.
        /// </summary>
        private Guid provinceIdField;

        /// <summary>
        /// The unique key of a province.
        /// </summary>
        private ProvinceCountryKey provinceCountryKeyField;

        /// <summary>
        /// The province row.
        /// </summary>
        private ProvinceRow provinceRowField;

        /// <summary>
        /// Gets or sets the abbreviated version of the name.
        /// </summary>
        public string Abbreviation
        {
            get
            {
                return this.abbreviationField;
            }

            set
            {
                if (this.abbreviationField != value)
                {
                    this.abbreviationField = value;
                    this.OnPropertyChanged("Abbreviation");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the country.
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
                    this.OnPropertyChanged("CountryId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the province.
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
        /// Gets or sets the province (state).
        /// </summary>
        public Guid ProvinceId
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
        /// Gets or sets the unique identifier of the province (state).
        /// </summary>
        public ProvinceCountryKey ProvinceCountryKey
        {
            get
            {
                return this.provinceCountryKeyField;
            }

            set
            {
                if (this.provinceCountryKeyField != value)
                {
                    this.provinceCountryKeyField = value;
                    this.OnPropertyChanged("ProvinceCountryKey");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvinceViewModel"/> class.
        /// </summary>
        /// <param name="provinceRow">A province row.</param>
        public void Map(ProvinceRow provinceRow)
        {
            // Validate the parameter.
            if (provinceRow == null)
            {
                throw new ArgumentNullException(nameof(provinceRow));
            }

            // Initialize the object.
            this.provinceRowField = provinceRow;
            provinceRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("CountryId", (p) => this.UpdateKey(p.CountryId, p.ProvinceId));
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);
            this.notifyActions.Add("ProvinceId", (p) => this.UpdateKey(p.CountryId, p.ProvinceId));
            this.notifyActions.Add("Abbreviation", (p) => this.Abbreviation = p.Abbreviation);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.provinceRowField);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disengage from the notifications from the data model.
            this.provinceRowField.PropertyChanged -= this.OnPropertyChanged;

            // Empty the notification list in the event this view model is reused.
            this.notifyActions.Clear();

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
            Action<ProvinceRow> action;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out action))
            {
                action(sender as ProvinceRow);
            }
        }

        /// <summary>
        /// Updates the key elements.
        /// </summary>
        /// <param name="countryId">The new country id.</param>
        /// <param name="provinceId">The new province id.</param>
        private void UpdateKey(Guid countryId, Guid provinceId)
        {
            this.ProvinceId = provinceId;
            this.CountryId = countryId;
            this.ProvinceCountryKey = new ProvinceCountryKey()
            {
                CountryId = countryId,
                ProvinceId = provinceId
            };
        }
    }
}