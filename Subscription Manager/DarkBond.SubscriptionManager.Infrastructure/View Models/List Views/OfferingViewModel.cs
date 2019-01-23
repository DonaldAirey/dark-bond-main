// <copyright file="OfferingViewModel.cs" company="Dark Bond, Inc.">
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
    using DarkBond.ServiceModel;
    using DarkBond.SubscriptionManager.Common;
    using DarkBond.SubscriptionManager.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for a offering for which subscription can be issued.
    /// </summary>
    public class OfferingViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The description of the offering.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The number of subscriptions owned by this offering.
        /// </summary>
        private int subscriptionsOwned;

        /// <summary>
        /// The command to create a subscription.
        /// </summary>
        private DelegateCommand subscriptionNew;

        /// <summary>
        /// The display name of the item.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<OfferingRow>> notifyActions = new Dictionary<string, Action<OfferingRow>>();

        /// <summary>
        /// The command to delete a offering.
        /// </summary>
        private DelegateCommand offeringDelete;

        /// <summary>
        /// The unique identifier of this offering.
        /// </summary>
        private Guid offeringIdField;

        /// <summary>
        /// The command to open a offering.
        /// </summary>
        private DelegateCommand offeringOpen;

        /// <summary>
        /// The command to update the properties.
        /// </summary>
        private DelegateCommand offeringProperties;

        /// <summary>
        /// The offering row.
        /// </summary>
        private OfferingRow offeringRowField;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferingViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription services.</param>
        public OfferingViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object
            this.ImageKey = ImageKeys.Product;

            // These commands are handled by this view model.
            this.subscriptionNew = new DelegateCommand(
                () => this.SubscriptionService.NavigateToProductLicense(this.OfferingId),
                () => GlobalCommands.New.RegisteredCommands.Count == 1);
            this.offeringDelete = new DelegateCommand(
                () => this.SubscriptionService.DeleteProductAsync(this.OfferingId),
                () => this.SubscriptionService.CanDeleteProduct(this.OfferingId));
            this.offeringOpen = new DelegateCommand(
                () => GlobalCommands.Locate.Execute(this.Uri),
                () => GlobalCommands.Open.RegisteredCommands.Count == 1);
            this.offeringProperties = new DelegateCommand(
                () => this.SubscriptionService.NavigateToProduct(this.OfferingId),
                () => this.SubscriptionService.CanNavigateToProduct(this.OfferingId));
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
        /// Gets or sets the unique identifier of this offering.
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
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets the number of subscriptions owned by this offering.
        /// </summary>
        public int SubscriptionsOwned
        {
            get
            {
                return this.subscriptionsOwned;
            }

            set
            {
                if (this.subscriptionsOwned != value)
                {
                    this.subscriptionsOwned = value;
                    this.OnPropertyChanged("SubscriptionsOwned");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this object.
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
        /// Gets or sets the unique identifier of this offering.
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
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                return new Uri(this.Parent.Uri.OriginalString + "\\" + this.OfferingId.ToString("N"));
            }
        }

        /// <summary>
        /// Maps the properties of a OfferingRow in the data model to the properties of this view model.
        /// </summary>
        /// <param name="offeringRow">The offering row.</param>
        public void Map(OfferingRow offeringRow)
        {
            // Validate the parameter.
            if (offeringRow == null)
            {
                throw new ArgumentNullException(nameof(offeringRow));
            }

            // Initialize the object.
            this.offeringRowField = offeringRow;
            this.offeringRowField.PropertyChanged += this.OnOfferingRowChanged;
            this.DataModel.OfferingSubscriptionOfferingIdKey.RelationChanged += this.OnLicenseRelationChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("OfferingId", this.UpdateOfferingId);
            this.notifyActions.Add("Description", (c) => this.Description = c.Description);
            this.notifyActions.Add("Name", (c) => this.Name = c.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.offeringRowField);
            }
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            ObservableCollection<IDisposable> appBarItems = base.CreateAppBarItems();

            // The Delete command.
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Label = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            appBarItems.Add(deleteButton);

            // The Edit command.
            ButtonViewModel editButton = this.CompositionContext.GetExport<ButtonViewModel>();
            editButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.OfferingId));
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
            openButtonItem.Command = GlobalCommands.Open;
            openButtonItem.Header = Resources.Open;
            openButtonItem.ImageKey = ImageKeys.Open;
            contextButtonViewItems.Add(openButtonItem);

            // New Button
            DropDownButtonViewModel newButtonItem = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // New License Menu Item
            DropDownButtonViewModel newProductButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newProductButton.Command = GlobalCommands.New;
            newProductButton.Header = Resources.NewLicense;
            newProductButton.ImageKey = ImageKeys.License;
            newButtonItem.Items.Add(newProductButton);

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

            // Open Menu Item
            OpenMenuItemViewModel openMenuItem = this.CompositionContext.GetExport<OpenMenuItemViewModel>();
            openMenuItem.Command = GlobalCommands.Locate;
            openMenuItem.CommandParameter = this.Uri;
            openMenuItem.Header = Resources.Open;
            openMenuItem.ImageKey = ImageKeys.Open;
            contextMenuViewItems.Add(openMenuItem);

            // New License Menu Item
            MenuItemViewModel newProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newProductMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProductLicense(this.OfferingId));
            newProductMenuItem.Header = Resources.NewLicense;
            newProductMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newProductMenuItem);

            // Delete Menu Item
            MenuItemViewModel deleteProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteProductMenuItem.Command = new DelegateCommand(
                () => this.SubscriptionService.DeleteProductAsync(this.OfferingId),
                () => this.SubscriptionService.CanDeleteProduct(this.OfferingId));
            deleteProductMenuItem.Header = Resources.Delete;
            deleteProductMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteProductMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToProduct(this.OfferingId));
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
                this.offeringRowField.PropertyChanged -= this.OnOfferingRowChanged;
                this.DataModel.OfferingSubscriptionOfferingIdKey.RelationChanged -= this.OnLicenseRelationChanged;

                // Remove all the actions that update this view model from the data model.
                this.notifyActions.Clear();

                // Make sure we unregister any composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Delete.UnregisterCommand(this.offeringDelete);
                    GlobalCommands.New.UnregisterCommand(this.subscriptionNew);
                    GlobalCommands.Open.UnregisterCommand(this.offeringOpen);
                    GlobalCommands.Properties.UnregisterCommand(this.offeringProperties);
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
                GlobalCommands.Delete.RegisterCommand(this.offeringDelete);
                GlobalCommands.New.RegisterCommand(this.subscriptionNew);
                GlobalCommands.Open.RegisterCommand(this.offeringOpen);
                GlobalCommands.Properties.RegisterCommand(this.offeringProperties);
            }
            else
            {
                GlobalCommands.Delete.UnregisterCommand(this.offeringDelete);
                GlobalCommands.New.UnregisterCommand(this.subscriptionNew);
                GlobalCommands.Open.UnregisterCommand(this.offeringOpen);
                GlobalCommands.Properties.UnregisterCommand(this.offeringProperties);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles a change to the License child relations.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyRelationChangedEventArgs">The event data.</param>
        private void OnLicenseRelationChanged(object sender, NotifyRelationChangedEventArgs<Guid> notifyRelationChangedEventArgs)
        {
            // Handle the different update actions.
            switch (notifyRelationChangedEventArgs.Action)
            {
                case NotifyRelationChangedAction.Add:

                    // Adding a child increments the subscriptions owned count.
                    if (this.OfferingId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.SubscriptionsOwned++;
                    }

                    break;

                case NotifyRelationChangedAction.Remove:

                    // Removing a child decrements the subscriptions owned count.
                    if (this.OfferingId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.SubscriptionsOwned--;
                    }

                    break;

                case NotifyRelationChangedAction.Change:

                    // Adding a child increments the subscriptions owned count.
                    if (this.OfferingId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.SubscriptionsOwned++;
                    }

                    // Removing a child decrements the subscriptions owned count.
                    if (this.OfferingId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.SubscriptionsOwned--;
                    }

                    break;
            }
        }

        /// <summary>
        /// Handles a change to the data model offering row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnOfferingRowChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<OfferingRow> notifyAction;
            if (this.notifyActions.TryGetValue(e.PropertyName, out notifyAction))
            {
                notifyAction(this.offeringRowField);
            }
        }

        /// <summary>
        /// Handles a change to the OfferingId property.
        /// </summary>
        /// <param name="offeringRow">The data model version of the offering data.</param>
        private void UpdateOfferingId(OfferingRow offeringRow)
        {
            // This will update the metadata that displays the number of subscriptions owned.
            this.OfferingId = offeringRow.OfferingId;
            this.SortKey = offeringRow.OfferingId;
            this.SubscriptionsOwned = this.DataModel.OfferingSubscriptionOfferingIdKey.GetSubscriptionRows(this.OfferingId).Count;
        }
    }
}