// <copyright file="UnderwriterViewModel.cs" company="Dark Bond, Inc.">
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
    /// View model for a underwriter who can own subscriptions.
    /// </summary>
    public class UnderwriterViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The underwriter's address.
        /// </summary>
        private string address1Field;

        /// <summary>
        /// The underwriter's address.
        /// </summary>
        private string address2Field;

        /// <summary>
        /// The underwriter's city.
        /// </summary>
        private string cityField;

        /// <summary>
        /// The abbreviation of the country.
        /// </summary>
        private string countryAbbreviationField;

        /// <summary>
        /// The underwriter's country.
        /// </summary>
        private Guid countryIdField;

        /// <summary>
        /// A table that drives the notifications when country row attributes change.
        /// </summary>
        private Dictionary<string, Action<CountryRow>> countryNotifyActions = new Dictionary<string, Action<CountryRow>>();

        /// <summary>
        /// The country row.
        /// </summary>
        private CountryRow countryRowField;

        /// <summary>
        /// The command to delete a underwriter.
        /// </summary>
        private DelegateCommand underwriterDelete;

        /// <summary>
        /// The unique identifier of this underwriter.
        /// </summary>
        private Guid underwriterIdField;

        /// <summary>
        /// The command to update the properties.
        /// </summary>
        private DelegateCommand underwriterProperties;

        /// <summary>
        /// A table that drives the notifications when underwriter row attributes change.
        /// </summary>
        private Dictionary<string, Action<UnderwriterRow>> notifyActions = new Dictionary<string, Action<UnderwriterRow>>();

        /// <summary>
        /// The underwriter row.
        /// </summary>
        private UnderwriterRow underwriterRowField;

        /// <summary>
        /// The date of birth.
        /// </summary>
        private DateTime dateOfBirthField;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailField;

        /// <summary>
        /// The first name.
        /// </summary>
        private string primaryContactField;

        /// <summary>
        /// An indication whether the underwriter is a member.
        /// </summary>
        private bool isMemberField;

        /// <summary>
        /// The last name.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The command to create a subscription.
        /// </summary>
        private DelegateCommand subscriptionNew;

        /// <summary>
        /// The number of subscriptions owned by this underwriter.
        /// </summary>
        private int subscriptionsOwned;

        /// <summary>
        /// The command to open a underwriter.
        /// </summary>
        private DelegateCommand underwriterOpen;

        /// <summary>
        /// The phone number.
        /// </summary>
        private string phoneNumberField;

        /// <summary>
        /// The abbreviation of the province.
        /// </summary>
        private string provinceAbbreviationField;

        /// <summary>
        /// The underwriter's state.
        /// </summary>
        private Guid? provinceIdField;

        /// <summary>
        /// A table that drives the notifications when province row attributes change.
        /// </summary>
        private Dictionary<string, Action<ProvinceRow>> provinceNotifyActions = new Dictionary<string, Action<ProvinceRow>>();

        /// <summary>
        /// The province row.
        /// </summary>
        private ProvinceRow provinceRow;

        /// <summary>
        /// The underwriter's zip code.
        /// </summary>
        private string postalCodeField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderwriterViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="subscriptionService">The subscription service.</param>
        public UnderwriterViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ISubscriptionService subscriptionService)
            : base(compositionContext, dataModel, subscriptionService)
        {
            // Initialize the object
            this.ImageKey = ImageKeys.Customer;

            // These commands are handled by this view model.
            this.underwriterDelete = new DelegateCommand(
                () => this.SubscriptionService.DeleteCustomerAsync(this.UnderwriterId),
                () => this.SubscriptionService.CanDeleteCustomer(this.UnderwriterId));
            this.underwriterOpen = new DelegateCommand(
                () => GlobalCommands.Locate.Execute(this.Uri),
                () => GlobalCommands.Open.RegisteredCommands.Count == 1);
            this.underwriterProperties = new DelegateCommand(
                () => this.SubscriptionService.NavigateToCustomer(this.UnderwriterId),
                () => this.SubscriptionService.CanNavigateToCustomer(this.UnderwriterId));
            this.subscriptionNew = new DelegateCommand(
                () => this.SubscriptionService.NavigateToCustomerLicense(this.UnderwriterId),
                () => GlobalCommands.New.RegisteredCommands.Count == 1);
        }

        /// <summary>
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the unique identifier of this underwriter.
        /// </summary>
        public string CountryAbbreviation
        {
            get
            {
                return this.countryAbbreviationField;
            }

            set
            {
                if (this.countryAbbreviationField != value)
                {
                    this.countryAbbreviationField = value;
                    this.OnPropertyChanged("CountryAbbreviation");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the date the item was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the item was last modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the date and time the object was created.
        /// </summary>
        public DateTime DateOfBirth
        {
            get
            {
                return this.dateOfBirthField;
            }

            set
            {
                if (this.dateOfBirthField != value)
                {
                    this.dateOfBirthField = value;
                    this.OnPropertyChanged("DateOfBirth");
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
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether underwriter is a member.
        /// </summary>
        public bool IsMember
        {
            get
            {
                return this.isMemberField;
            }

            set
            {
                if (this.isMemberField != value)
                {
                    this.isMemberField = value;
                    this.OnPropertyChanged("IsMember");
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
        /// Gets or sets the number of subscriptions owned by this underwriter.
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
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return this.phoneNumberField;
            }

            set
            {
                if (this.phoneNumberField != value)
                {
                    this.phoneNumberField = value;
                    this.OnPropertyChanged("PhoneNumber");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets or sets the abbreviation of the province.
        /// </summary>
        public string ProvinceAbbreviation
        {
            get
            {
                return this.provinceAbbreviationField;
            }

            set
            {
                if (this.provinceAbbreviationField != value)
                {
                    this.provinceAbbreviationField = value;
                    this.OnPropertyChanged("ProvinceAbbreviation");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this underwriter.
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
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                return new Uri(this.Parent.Uri.OriginalString + "\\" + this.UnderwriterId.ToString("N"));
            }
        }

        /// <summary>
        /// Maps the properties of a UnderwriterRow in the data model to the properties of this view model.
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
            this.DataModel.UnderwriterSubscriptionUnderwriterIdKey.RelationChanged += this.OnLicenseRelationChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("Address1", (c) => this.Address1 = c.Address1);
            this.notifyActions.Add("Address2", (c) => this.Address2 = c.Address2);
            this.notifyActions.Add("City", (c) => this.City = c.City);
            this.notifyActions.Add("CountryId", this.UpdateCountryId);
            this.notifyActions.Add("UnderwriterId", this.UpdateUnderwriterId);
            this.notifyActions.Add("DateOfBirth", (c) => this.DateOfBirth = c.DateOfBirth);
            this.notifyActions.Add("Email", (c) => this.Email = c.Email);
            this.notifyActions.Add("PrimaryContact", (c) => this.PrimaryContact = c.PrimaryContact);
            this.notifyActions.Add("Name", (c) => this.Name = c.Name);
            this.notifyActions.Add("PhoneNumber", (c) => this.PhoneNumber = c.Phone);
            this.notifyActions.Add("PostalCode", (c) => this.PostalCode = c.PostalCode);
            this.notifyActions.Add("ProvinceId", this.UpdateProvince);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.underwriterRowField);
            }
        }

        /// <inheritdoc/>
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
            editButton.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomer(this.UnderwriterId));
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
            DropDownButtonViewModel newCustomerButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newCustomerButton.Command = GlobalCommands.New;
            newCustomerButton.Header = Resources.License;
            newCustomerButton.ImageKey = ImageKeys.License;
            newButtonItem.Items.Add(newCustomerButton);

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
            openMenuItem.Command = GlobalCommands.Open;
            openMenuItem.Header = Resources.Open;
            openMenuItem.ImageKey = ImageKeys.Open;
            contextMenuViewItems.Add(openMenuItem);

            // New License Menu Item
            MenuItemViewModel newCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newCustomerMenuItem.Command = new DelegateCommand(() => this.SubscriptionService.NavigateToCustomerLicense(this.UnderwriterId));
            newCustomerMenuItem.Header = Resources.NewLicense;
            newCustomerMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newCustomerMenuItem);

            // Delete Menu Item
            MenuItemViewModel deleteCustomerMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteCustomerMenuItem.Command = GlobalCommands.Delete;
            deleteCustomerMenuItem.Header = Resources.Delete;
            deleteCustomerMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteCustomerMenuItem);

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
                this.underwriterRowField.PropertyChanged -= this.OnUnderwriterRowChanged;
                this.DataModel.UnderwriterSubscriptionUnderwriterIdKey.RelationChanged -= this.OnLicenseRelationChanged;
                this.countryRowField.PropertyChanged -= this.OnCountryRowChanged;
                if (this.provinceRow != null)
                {
                    this.provinceRow.PropertyChanged -= this.OnProvinceRowChanged;
                }

                // Remove all the actions that update this view model from the data model.
                this.notifyActions.Clear();

                // Make sure we unregister any composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Delete.UnregisterCommand(this.underwriterDelete);
                    GlobalCommands.New.UnregisterCommand(this.subscriptionNew);
                    GlobalCommands.Open.UnregisterCommand(this.underwriterOpen);
                    GlobalCommands.Properties.UnregisterCommand(this.underwriterProperties);
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
                GlobalCommands.Delete.RegisterCommand(this.underwriterDelete);
                GlobalCommands.New.RegisterCommand(this.subscriptionNew);
                GlobalCommands.Open.RegisterCommand(this.underwriterOpen);
                GlobalCommands.Properties.RegisterCommand(this.underwriterProperties);
            }
            else
            {
                GlobalCommands.Delete.UnregisterCommand(this.underwriterDelete);
                GlobalCommands.New.UnregisterCommand(this.subscriptionNew);
                GlobalCommands.Open.UnregisterCommand(this.underwriterOpen);
                GlobalCommands.Properties.UnregisterCommand(this.underwriterProperties);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles a change to the data model country row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnCountryRowChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<CountryRow> notifyAction;
            if (this.countryNotifyActions.TryGetValue(e.PropertyName, out notifyAction))
            {
                notifyAction(this.countryRowField);
            }
        }

        /// <summary>
        /// Handles a change to the data model underwriter row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnUnderwriterRowChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<UnderwriterRow> notifyAction;
            if (this.notifyActions.TryGetValue(e.PropertyName, out notifyAction))
            {
                notifyAction(this.underwriterRowField);
            }
        }

        /// <summary>
        /// Handles a change to the License child relations.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyRelationChangedEventArgs">The event data.</param>
        private void OnLicenseRelationChanged(object sender, NotifyRelationChangedEventArgs<Guid> notifyRelationChangedEventArgs)
        {
            // This will update the context menu items to reflect the relation change.
            this.underwriterDelete.RaiseCanExecuteChanged();

            // Handle the different update actions.
            switch (notifyRelationChangedEventArgs.Action)
            {
                case NotifyRelationChangedAction.Add:

                    // Adding a child increments the subscriptions owned count.
                    if (this.UnderwriterId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.SubscriptionsOwned++;
                    }

                    break;

                case NotifyRelationChangedAction.Remove:

                    // Removing a child decrements the subscriptions owned count.
                    if (this.UnderwriterId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.SubscriptionsOwned--;
                    }

                    break;

                case NotifyRelationChangedAction.Change:

                    // Adding a child increments the subscriptions owned count.
                    if (this.UnderwriterId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.SubscriptionsOwned++;
                    }

                    // Removing a child decrements the subscriptions owned count.
                    if (this.UnderwriterId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.SubscriptionsOwned--;
                    }

                    break;
            }
        }

        /// <summary>
        /// Handles a change to the data model province row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnProvinceRowChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<ProvinceRow> notifyAction;
            if (this.provinceNotifyActions.TryGetValue(e.PropertyName, out notifyAction))
            {
                notifyAction(this.provinceRow);
            }
        }

        /// <summary>
        /// Update the CountryId property.
        /// </summary>
        /// <param name="underwriterRow">The data model underwriter.</param>
        private void UpdateCountryId(UnderwriterRow underwriterRow)
        {
            // Remove the event handler from the previous country.
            if (this.countryRowField != null)
            {
                this.countryRowField.PropertyChanged -= this.OnCountryRowChanged;
                this.countryNotifyActions.Clear();
            }

            // Update the country id.
            this.CountryId = underwriterRow.CountryId;

            // Instruct the Country row to notify this view model of relevant changes.
            this.countryRowField = this.DataModel.CountryKey.Find(underwriterRow.CountryId);
            this.countryRowField.PropertyChanged += this.OnCountryRowChanged;

            // This table drives the updating of the view model when the underwriter properties changes.
            this.countryNotifyActions.Add("Abbreviation", (c) => this.CountryAbbreviation = c.Abbreviation);

            // Initialize the view model with the data model.
            foreach (string property in this.countryNotifyActions.Keys)
            {
                this.countryNotifyActions[property](this.countryRowField);
            }
        }

        /// <summary>
        /// Update the UnderwriterId property.
        /// </summary>
        /// <param name="underwriterRow">The data model underwriter.</param>
        private void UpdateUnderwriterId(UnderwriterRow underwriterRow)
        {
            // This will update the metadata that displays the number of subscriptions owned.
            this.UnderwriterId = underwriterRow.UnderwriterId;
            this.SortKey = underwriterRow.UnderwriterId;
            this.SubscriptionsOwned = this.DataModel.UnderwriterSubscriptionUnderwriterIdKey.GetSubscriptionRows(this.UnderwriterId).Count;
        }

        /// <summary>
        /// Update the province identifier.
        /// </summary>
        /// <param name="underwriterRow">The underwriter row.</param>
        private void UpdateProvince(UnderwriterRow underwriterRow)
        {
            // If the previous province was hooked into the data model, then unhook it.
            if (this.provinceRow != null)
            {
                if (this.ProvinceId.HasValue)
                {
                    this.provinceRow.PropertyChanged -= this.OnProvinceRowChanged;
                    this.ProvinceAbbreviation = null;
                    this.provinceNotifyActions.Clear();
                }
            }

            // Update the province id.
            this.ProvinceId = underwriterRow.ProvinceId;

            // Hook into the changes to the province table only if there's a link to it.
            if (underwriterRow.ProvinceId.HasValue)
            {
                // Instruct the Province row to notify this view model of relevant changes.
                this.provinceRow = this.DataModel.ProvinceKey.Find(this.underwriterRowField.ProvinceId.Value);
                this.provinceRow.PropertyChanged += this.OnProvinceRowChanged;

                // This table drives the updating of the view model when the underwriter properties changes.
                this.provinceNotifyActions.Add("Abbreviation", (p) => this.ProvinceAbbreviation = p.Abbreviation);

                // Initialize the view model with the data model.
                foreach (string property in this.provinceNotifyActions.Keys)
                {
                    this.provinceNotifyActions[property](this.provinceRow);
                }
            }
        }
    }
}