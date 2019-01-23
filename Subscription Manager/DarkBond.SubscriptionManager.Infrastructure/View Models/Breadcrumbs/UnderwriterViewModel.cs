// <copyright file="UnderwriterViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.Breadcrumbs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;

    /// <summary>
    /// A breadcrumb for the underwriter.
    /// </summary>
    public class UnderwriterViewModel : CommonBreadcrumbViewModel
    {
        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> notifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        public UnderwriterViewModel(CompositionContext compositionContext, DataModel dataModel)
            : base(compositionContext, dataModel)
        {
            // Initialize the properties of this object.
            this.ImageKey = ImageKeys.Customer;
        }

        /// <summary>
        /// Maps the data model to the view model.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        public void Map(UnderwriterRow underwriterRow)
        {
            // Validate the parameter.
            if (underwriterRow == null)
            {
                throw new ArgumentNullException(nameof(underwriterRow));
            }

            // Instruct the data model to notify this view model of relevant changes.
            this.underwriterRowField = underwriterRow;
            this.underwriterRowField.PropertyChanged += this.OnUnderwriterRowChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("UnderwriterId", this.UpdateIdentifier);

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
            // Disconnect from the data model.
            this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;

            // Allow the base class to finish the disposal.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the data model underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Action<UnderwriterRow> notifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out notifyAction))
            {
                notifyAction(this.underwriterRowField);
            }
        }

        /// <summary>
        /// Update the unique identifier.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        private void UpdateIdentifier(UnderwriterRow underwriterRow)
        {
            // This is used to uniquely identify the object in a URL.
            this.Identifier = underwriterRow.UnderwriterId.ToString("N");

            // This is used to uniquely identify the object in a ordered list.
            this.SortKey = underwriterRow.UnderwriterId;
        }
    }
}