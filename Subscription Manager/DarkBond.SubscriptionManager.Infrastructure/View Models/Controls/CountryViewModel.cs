// <copyright file="CountryViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DarkBond.ViewModels;

    /// <summary>
    /// The view model for a country.
    /// </summary>
    public class CountryViewModel : ViewModel
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
        /// The country row.
        /// </summary>
        private CountryRow countryRowField;

        /// <summary>
        /// The name.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<CountryRow>> notifyActions = new Dictionary<string, Action<CountryRow>>();

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
        /// Gets or sets the name of the country.
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
        /// Maps the properties of a <see cref="CountryRow"/> to a <see cref="CountryViewModel"/>.
        /// </summary>
        /// <param name="countryRow">A country row.</param>
        public void Map(CountryRow countryRow)
        {
            // Validate the parameter.
            if (countryRow == null)
            {
                throw new ArgumentNullException(nameof(countryRow));
            }

            // Initialize the object.
            this.countryRowField = countryRow;
            countryRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Abbreviation", (p) => this.Abbreviation = p.Abbreviation);
            this.notifyActions.Add("CountryId", (p) => this.CountryId = p.CountryId);
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.countryRowField);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disengage from the notifications from the data model.
            this.countryRowField.PropertyChanged -= this.OnPropertyChanged;

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
            Action<CountryRow> action;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out action))
            {
                action(sender as CountryRow);
            }
        }
    }
}