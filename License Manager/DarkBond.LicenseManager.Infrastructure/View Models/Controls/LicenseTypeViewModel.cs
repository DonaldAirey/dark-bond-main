// <copyright file="LicenseTypeViewModel.cs" company="Dark Bond, Inc.">
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
    /// View model for license types.
    /// </summary>
    public class LicenseTypeViewModel : ViewModel
    {
        /// <summary>
        /// The description of the license type.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The unique identifier of the license type.
        /// </summary>
        private LicenseTypeCode licenseTypeCodeField;

        /// <summary>
        /// The licenseType row.
        /// </summary>
        private LicenseTypeRow licenseTypeRow;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<LicenseTypeRow>> notifyActions = new Dictionary<string, Action<LicenseTypeRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseTypeViewModel"/> class.
        /// </summary>
        /// <param name="licenseTypeRow">A licenseType row.</param>
        public LicenseTypeViewModel(LicenseTypeRow licenseTypeRow)
        {
            // Validate the parameter.
            if (licenseTypeRow == null)
            {
                throw new ArgumentNullException(nameof(licenseTypeRow));
            }

            // Initialize the object.
            this.licenseTypeRow = licenseTypeRow;
            licenseTypeRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Description", (p) => this.Description = p.Description);
            this.notifyActions.Add("LicenseTypeCode", (p) => this.LicenseTypeCode = p.LicenseTypeCode);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.licenseTypeRow);
            }
        }

        /// <summary>
        /// Gets or sets the description of the license type.
        /// </summary>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }

            set
            {
                if (this.descriptionField != value)
                {
                    this.descriptionField = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the license type.
        /// </summary>
        public LicenseTypeCode LicenseTypeCode
        {
            get
            {
                return this.licenseTypeCodeField;
            }

            set
            {
                if (this.licenseTypeCodeField != value)
                {
                    this.licenseTypeCodeField = value;
                    this.OnPropertyChanged("LicenseTypeCode");
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disengage from the notifications from the data model.
            this.licenseTypeRow.PropertyChanged -= this.OnPropertyChanged;

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
            Action<LicenseTypeRow> action;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out action))
            {
                action(sender as LicenseTypeRow);
            }
        }
    }
}