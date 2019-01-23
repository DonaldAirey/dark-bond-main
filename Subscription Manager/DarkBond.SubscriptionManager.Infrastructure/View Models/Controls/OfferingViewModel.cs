// <copyright file="OfferingViewModel.cs" company="Dark Bond, Inc.">
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
    /// View model for a offering.
    /// </summary>
    public class OfferingViewModel : ViewModel
    {
        /// <summary>
        /// The description of the offering.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The name of the offering.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> notifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// The unique identifier of the offering.
        /// </summary>
        private Guid offeringIdField;

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// Gets or sets the description of the offering.
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
        /// Gets or sets the name of the offering.
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
        /// Gets or sets the unique identifier of the offering.
        /// </summary>
        public Guid OfferingId
        {
            get
            {
                return this.offeringIdField;
            }

            set
            {
                if (this.offeringIdField != value)
                {
                    this.offeringIdField = value;
                    this.OnPropertyChanged("OfferingId");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingViewModel"/> class.
        /// </summary>
        /// <param name="offeringRow">A offering row.</param>
        public void Map(OfferingRow offeringRow)
        {
            // Validate the parameter.
            if (offeringRow == null)
            {
                throw new ArgumentNullException(nameof(offeringRow));
            }

            // Initialize the object.
            this.offeringRowField = offeringRow;
            offeringRow.PropertyChanged += this.OnPropertyChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Description", (p) => this.Description = p.Description);
            this.notifyActions.Add("Name", (p) => this.Name = p.Name);
            this.notifyActions.Add("OfferingId", (p) => this.OfferingId = p.OfferingId);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.offeringRowField);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // Disengage from the notifications from the data model.
            this.offeringRowField.PropertyChanged -= this.OnPropertyChanged;

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
            Action<OfferingRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(sender as OfferingRow);
            }
        }
    }
}