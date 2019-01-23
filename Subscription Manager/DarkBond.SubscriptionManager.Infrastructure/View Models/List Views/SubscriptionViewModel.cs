// <copyright file="SubscriptionViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.ViewModels.ListViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// A subscription who can own a subscription.
    /// </summary>
    public class SubscriptionViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The unique identifier of the underwriter.
        /// </summary>
        private Guid underwriterIdField;

        /// <summary>
        /// The underwriter name.
        /// </summary>
        private string underwriterNameField;

        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// A table that drives the notifications when underwriter row properties change.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> underwriterNotifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// The description of the developer subscription.
        /// </summary>
        private string developerLicenseDescriptionField;

        /// <summary>
        /// The command to delete a subscription.
        /// </summary>
        private DelegateCommand subscriptionDelete;

        /// <summary>
        /// The unique identifier of this subscription.
        /// </summary>
        private Guid subscriptionIdField;

        /// <summary>
        /// The command to update the properties.
        /// </summary>
        private DelegateCommand subscriptionProperties;

        /// <summary>
        /// The subscription row.
        /// </summary>
        private SubscriptionRow subscriptionRowField;

        /// <summary>
        /// A table that drives the notifications when subscription row properties change.
        /// </summary>
        private Dictionary<string, Action<SubscriptionRow>> notifyActions = new Dictionary<string, Action<SubscriptionRow>>();

        /// <summary>
        /// The unique identifier of the offering.
        /// </summary>
        private Guid offeringIdField;

        /// <summary>
        /// The offering name.
        /// </summary>
        private string offeringNameField;

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// A table that drives the notifications when offering row properties change.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> offeringNotifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// The description of the runtime subscription.
        /// </summary>
        private string runtimeLicenseDescriptionField;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription service.</param>
        public SubscriptionViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object
            this.ImageKey = ImageKeys.License;

            // These composite commands are handled by this view model.
            this.subscriptionDelete = new DelegateCommand(
                () => this.SubscriptionService.DeleteLicenseAsync(this.SubscriptionId),
                () => this.SubscriptionService.CanDeleteLicense(this.SubscriptionId));
            this.subscriptionProperties = new DelegateCommand(
                () => this.SubscriptionService.NavigateToLicense(this.SubscriptionId),
                () => this.SubscriptionService.CanNavigateToLicense(this.SubscriptionId));
        }

        /// <summary>
        /// Gets or sets the unique identifier of the underwriter.
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
        /// Gets or sets the name of the underwriter.
        /// </summary>
        public string CustomerName
        {
            get
            {
                return this.underwriterNameField;
            }

            set
            {
                if (this.underwriterNameField != value)
                {
                    this.underwriterNameField = value;
                    this.OnPropertyChanged("CustomerName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the date the item was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the item was last modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the description of the developer subscription.
        /// </summary>
        public string DeveloperLicenseDescription
        {
            get
            {
                return this.developerLicenseDescriptionField;
            }

            set
            {
                if (this.developerLicenseDescriptionField != value)
                {
                    this.developerLicenseDescriptionField = value;
                    this.OnPropertyChanged("DeveloperLicenseDescription");
                }
            }
        }

        /// <summary>
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriptions are grouped by underwriter.
        /// </summary>
        public bool IsUnderwriterView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriptions are grouped by offering.
        /// </summary>
        public bool IsOfferingView { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this subscription.
        /// </summary>
        public Guid SubscriptionId
        {
            get
            {
                return this.subscriptionIdField;
            }

            set
            {
                if (this.subscriptionIdField != value)
                {
                    this.subscriptionIdField = value;
                    this.OnPropertyChanged("SubscriptionId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the country that selects the offerings.
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
        /// Gets or sets the name of the offering.
        /// </summary>
        public string ProductName
        {
            get
            {
                return this.offeringNameField;
            }

            set
            {
                if (this.offeringNameField != value)
                {
                    this.offeringNameField = value;
                    this.OnPropertyChanged("ProductName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the runtime subscription.
        /// </summary>
        public string RuntimeLicenseDescription
        {
            get
            {
                return this.runtimeLicenseDescriptionField;
            }

            set
            {
                if (this.runtimeLicenseDescriptionField != value)
                {
                    this.runtimeLicenseDescriptionField = value;
                    this.OnPropertyChanged("RuntimeLicenseDescription");
                }
            }
        }

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.SubscriptionId.ToString().Substring(0, 18) + "...";
            }
        }

        /// <summary>
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder("urn:///DarkBond.SubscriptionManager.Views;/DarkBond.SubscriptionManager.Views.Forms.SubscriptionView");
                uriBuilder.Query = "subscriptionId={" + this.SubscriptionId.ToString() + "}";
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Maps the properties of a SubscriptionRow in the data model to the properties of this view model.
        /// </summary>
        /// <param name="subscriptionRow">The underwriter row.</param>
        public void Map(SubscriptionRow subscriptionRow)
        {
            // Validate the parameter.
            if (subscriptionRow == null)
            {
                throw new ArgumentNullException(nameof(subscriptionRow));
            }

            // Instruct the License row to notify this view model of relevant changes.
            this.subscriptionRowField = subscriptionRow;
            this.subscriptionRowField.PropertyChanged += this.OnSubscriptionRowChanged;

            // This table drives the updating of the view model when the subscription properties changes.
            this.notifyActions.Add("UnderwriterId", (c) => this.UnderwriterId = c.UnderwriterId);
            this.notifyActions.Add("SubscriptionId", this.UpdateSubscriptionId);
            this.notifyActions.Add("OfferingId", (c) => this.OfferingId = c.OfferingId);

            // Initialize the view model with the subscription properties.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.subscriptionRowField);
            }

            // Instruct the Customer row to notify this view model of relevant changes.
            this.underwriterRowField = this.DataModel.UnderwriterKey.Find(this.subscriptionRowField.UnderwriterId);
            this.underwriterRowField.PropertyChanged += this.OnUnderwriterRowChanged;

            // Initialize the view model with the underwriter properties.
            foreach (string property in this.underwriterNotifyActions.Keys)
            {
                this.underwriterNotifyActions[property](this.underwriterRowField);
            }

            // Instruct the Product row to notify this view model of relevant changes.
            this.offeringRowField = this.DataModel.OfferingKey.Find(this.subscriptionRowField.OfferingId);
            this.offeringRowField.PropertyChanged += this.OnOfferingRowChanged;

            // This table drives the updating of the view model when the offering properties changes.
            this.offeringNotifyActions.Add("Name", (p) => this.ProductName = p.Name);

            // Initialize the view model with the offering properties.
            foreach (string property in this.offeringNotifyActions.Keys)
            {
                this.offeringNotifyActions[property](this.offeringRowField);
            }
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            ObservableCollection<IDisposable> appBarItems = new ObservableCollection<IDisposable>();

            // The Delete command.
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Label = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            appBarItems.Add(deleteButton);

            // The Edit command.
            ButtonViewModel editButton = this.CompositionContext.GetExport<ButtonViewModel>();
            editButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToLicense(this.SubscriptionId));
            editButton.Label = Resources.Edit;
            editButton.ImageKey = ImageKeys.Edit;
            appBarItems.Add(editButton);

            // This is the set of appBar items for this ListView item.
            return appBarItems;
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextButtonViewItems = base.CreateContextButtonItems();

            // Open Button
            ButtonViewModel openButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            openButtonItem.Command = new DelegateCommand(() => { }, () => false);
            openButtonItem.Header = Resources.Open;
            openButtonItem.ImageKey = ImageKeys.Open;
            contextButtonViewItems.Add(openButtonItem);

            // New Button
            ButtonViewModel newButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            newButtonItem.Command = new DelegateCommand(() => { }, () => false);
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButtonItem.Command = GlobalCommands.Properties;
            propertiesButtonItem.Header = Resources.Properties;
            propertiesButtonItem.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButtonItem);

            // This is the context menu.
            return contextButtonViewItems;
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // Delete Menu Item
            MenuItemViewModel deleteLicenseMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteLicenseMenuItem.Command = GlobalCommands.Delete;
            deleteLicenseMenuItem.Header = Resources.Delete;
            deleteLicenseMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteLicenseMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = GlobalCommands.Properties;
            propertiesMenuItem.Header = Resources.Properties;
            propertiesMenuItem.ImageKey = ImageKeys.Properties;
            contextMenuViewItems.Add(propertiesMenuItem);

            // This is the context menu.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // This will unhook us from the data model when the item is no longer displayed.
            if (disposing)
            {
                // Disengage from notifications from the data model.
                this.subscriptionRowField.PropertyChanged -= this.OnSubscriptionRowChanged;
                this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;
                this.offeringRowField.PropertyChanged -= this.OnOfferingRowChanged;

                // Remove all the actions that update this view model from the data model.
                this.notifyActions.Clear();

                // Make sure we unregister the composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Delete.UnregisterCommand(this.subscriptionDelete);
                    GlobalCommands.Properties.UnregisterCommand(this.subscriptionProperties);
                }
            }

            // Allow the base class to complete the method.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the IsSelected property.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            // In order for the 'gang' operations to work, the individual commands must be added to the composite command (and removed when no longer
            // needed).
            if (this.IsSelected)
            {
                GlobalCommands.Delete.RegisterCommand(this.subscriptionDelete);
                GlobalCommands.Properties.RegisterCommand(this.subscriptionProperties);
            }
            else
            {
                GlobalCommands.Delete.UnregisterCommand(this.subscriptionDelete);
                GlobalCommands.Properties.UnregisterCommand(this.subscriptionProperties);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles a change to the underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UnderwriterRow underwriterRow = sender as UnderwriterRow;
            Action<UnderwriterRow> underwriterNotifyAction;
            if (this.underwriterNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out underwriterNotifyAction))
            {
                underwriterNotifyAction(underwriterRow);
            }
        }

        /// <summary>
        /// Handles a change to the subscription row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnSubscriptionRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SubscriptionRow subscriptionRow = sender as SubscriptionRow;
            Action<SubscriptionRow> subscriptionNotifyAction;
            if (this.notifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out subscriptionNotifyAction))
            {
                subscriptionNotifyAction(subscriptionRow);
            }
        }

        /// <summary>
        /// Handles a change to the offering row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="propertyChangedEventArgs">The event data.</param>
        private void OnOfferingRowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OfferingRow offeringRow = sender as OfferingRow;
            Action<OfferingRow> offeringNotifyAction;
            if (this.offeringNotifyActions.TryGetValue(propertyChangedEventArgs.PropertyName, out offeringNotifyAction))
            {
                offeringNotifyAction(offeringRow);
            }
        }

        /// <summary>
        /// Updates the view model properties when the SubscriptionId column changes.
        /// </summary>
        /// <param name="subscriptionRow">The License row.</param>
        private void UpdateSubscriptionId(SubscriptionRow subscriptionRow)
        {
            this.SortKey = subscriptionRow.SubscriptionId;
            this.SubscriptionId = subscriptionRow.SubscriptionId;
        }
    }
}