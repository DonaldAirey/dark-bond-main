namespace DarkBond.LicenseManager
{
    using System;
    using System.Composition;
    using System.Security.Permissions;
    using System.ServiceModel;
    using DarkBond.ClientModel;
    using DarkBond.ServiceModel;

    /// <summary>
    /// Abstract interface to a thread-safe, multi-tiered DataModel.
    /// </summary>
    [ServiceContract]
    public interface IDataService
    {
        /// <summary>
        /// Creates a Configuration record.
        /// </summary>
        /// <param name="configurationId">The required value for the ConfigurationId column.</param>
        /// <param name="source">The required value for the Source column.</param>
        /// <param name="targetKey">The required value for the TargetKey column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateConfiguration", ReplyAction = "http://tempuri.org/IDataModel/CreateConfigurationResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateConfiguration(string configurationId, string source, string targetKey);

        /// <summary>
        /// Creates a Country record.
        /// </summary>
        /// <param name="abbreviation">The required value for the Abbreviation column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateCountry", ReplyAction = "http://tempuri.org/IDataModel/CreateCountryResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateCountry(string abbreviation, Guid countryId, string externalId0, string name);

        /// <summary>
        /// Creates a Customer record.
        /// </summary>
        /// <param name="address1">The required value for the Address1 column.</param>
        /// <param name="address2">The optional value for the Address2 column.</param>
        /// <param name="city">The required value for the City column.</param>
        /// <param name="company">The optional value for the Company column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="customerId">The required value for the CustomerId column.</param>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="dateOfBirth">The required value for the DateOfBirth column.</param>
        /// <param name="email">The required value for the Email column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="firstName">The optional value for the FirstName column.</param>
        /// <param name="lastName">The required value for the LastName column.</param>
        /// <param name="middleName">The optional value for the MiddleName column.</param>
        /// <param name="phone">The required value for the Phone column.</param>
        /// <param name="postalCode">The required value for the PostalCode column.</param>
        /// <param name="provinceId">The optional value for the ProvinceId column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateCustomer", ReplyAction = "http://tempuri.org/IDataModel/CreateCustomerResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateCustomer(string address1, string address2, string city, string company, Guid countryId, Guid customerId, DateTime dateCreated, DateTime dateModified, DateTime dateOfBirth, string email, string externalId0, string firstName, string lastName, string middleName, string phone, string postalCode, Guid? provinceId);

        /// <summary>
        /// Creates a License record.
        /// </summary>
        /// <param name="customerId">The required value for the CustomerId column.</param>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="developerLicenseTypeCode">The required value for the DeveloperLicenseTypeCode column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="licenseId">The required value for the LicenseId column.</param>
        /// <param name="productId">The required value for the ProductId column.</param>
        /// <param name="runtimeLicenseTypeCode">The required value for the RuntimeLicenseTypeCode column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateLicense", ReplyAction = "http://tempuri.org/IDataModel/CreateLicenseResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateLicense(Guid customerId, DateTime dateCreated, DateTime dateModified, LicenseTypeCode developerLicenseTypeCode, string externalId0, Guid licenseId, Guid productId, LicenseTypeCode runtimeLicenseTypeCode);

        /// <summary>
        /// Creates a LicenseType record.
        /// </summary>
        /// <param name="description">The required value for the Description column.</param>
        /// <param name="licenseTypeCode">The required value for the LicenseTypeCode column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateLicenseType", ReplyAction = "http://tempuri.org/IDataModel/CreateLicenseTypeResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateLicenseType(string description, LicenseTypeCode licenseTypeCode);

        /// <summary>
        /// Creates a Product record.
        /// </summary>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="description">The optional value for the Description column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        /// <param name="productId">The required value for the ProductId column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateProduct", ReplyAction = "http://tempuri.org/IDataModel/CreateProductResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateProduct(DateTime dateCreated, DateTime dateModified, string description, string externalId0, string name, Guid productId);

        /// <summary>
        /// Creates a Province record.
        /// </summary>
        /// <param name="abbreviation">The required value for the Abbreviation column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        /// <param name="provinceId">The required value for the ProvinceId column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/CreateProvince", ReplyAction = "http://tempuri.org/IDataModel/CreateProvinceResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void CreateProvince(string abbreviation, Guid countryId, string externalId0, string name, Guid provinceId);

        /// <summary>
        /// Deletes a Configuration record.
        /// </summary>
        /// <param name="configurationId">The ConfigurationId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        /// <param name="source">The Source key element.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteConfiguration", ReplyAction = "http://tempuri.org/IDataModel/DeleteConfigurationResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteConfiguration(string configurationId, long rowVersion, string source);

        /// <summary>
        /// Deletes a Country record.
        /// </summary>
        /// <param name="countryId">The CountryId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteCountry", ReplyAction = "http://tempuri.org/IDataModel/DeleteCountryResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteCountry(Guid countryId, long rowVersion);

        /// <summary>
        /// Deletes a Customer record.
        /// </summary>
        /// <param name="customerId">The CustomerId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteCustomer", ReplyAction = "http://tempuri.org/IDataModel/DeleteCustomerResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteCustomer(Guid customerId, long rowVersion);

        /// <summary>
        /// Deletes a License record.
        /// </summary>
        /// <param name="licenseId">The LicenseId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteLicense", ReplyAction = "http://tempuri.org/IDataModel/DeleteLicenseResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteLicense(Guid licenseId, long rowVersion);

        /// <summary>
        /// Deletes a LicenseType record.
        /// </summary>
        /// <param name="licenseTypeCode">The LicenseTypeCode key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteLicenseType", ReplyAction = "http://tempuri.org/IDataModel/DeleteLicenseTypeResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteLicenseType(LicenseTypeCode licenseTypeCode, long rowVersion);

        /// <summary>
        /// Deletes a Product record.
        /// </summary>
        /// <param name="productId">The ProductId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteProduct", ReplyAction = "http://tempuri.org/IDataModel/DeleteProductResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteProduct(Guid productId, long rowVersion);

        /// <summary>
        /// Deletes a Province record.
        /// </summary>
        /// <param name="provinceId">The ProvinceId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/DeleteProvince", ReplyAction = "http://tempuri.org/IDataModel/DeleteProvinceResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void DeleteProvince(Guid provinceId, long rowVersion);

        /// <summary>
        /// Collects the set of modified records that will reconcile the client data model to the master data model.
        /// </summary>
        /// <param name="identifier">A unique identifier of an instance of the data.</param>
        /// <param name="sequence">The sequence of the client data model.</param>
        /// <returns>An array of records that will reconcile the client data model to the server.</returns>
        [OperationContract(Action = "http://tempuri.org/IDataModel/Read", ReplyAction = "http://tempuri.org/IDataModel/ReadResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [ServiceKnownType(typeof(LicenseTypeCode))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        DataHeader Read(Guid identifier, long sequence);

        /// <summary>
        /// Updates a Configuration record.
        /// </summary>
        /// <param name="configurationId">The optional value for the configurationId column.</param>
        /// <param name="configurationIdKey">The ConfigurationId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        /// <param name="source">The optional value for the source column.</param>
        /// <param name="sourceKey">The Source key element.</param>
        /// <param name="targetKey">The optional value for the targetKey column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateConfiguration", ReplyAction = "http://tempuri.org/IDataModel/UpdateConfigurationResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateConfiguration(string configurationId, string configurationIdKey, long rowVersion, string source, string sourceKey, string targetKey);

        /// <summary>
        /// Updates a Country record.
        /// </summary>
        /// <param name="abbreviation">The optional value for the abbreviation column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="countryIdKey">The CountryId key element.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateCountry", ReplyAction = "http://tempuri.org/IDataModel/UpdateCountryResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateCountry(string abbreviation, Guid countryId, Guid countryIdKey, string externalId0, string name, long rowVersion);

        /// <summary>
        /// Updates a Customer record.
        /// </summary>
        /// <param name="address1">The optional value for the address1 column.</param>
        /// <param name="address2">The required value for the address2 column.</param>
        /// <param name="city">The optional value for the city column.</param>
        /// <param name="company">The required value for the company column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="customerId">The optional value for the customerId column.</param>
        /// <param name="customerIdKey">The CustomerId key element.</param>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="dateOfBirth">The optional value for the dateOfBirth column.</param>
        /// <param name="email">The optional value for the email column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="firstName">The required value for the firstName column.</param>
        /// <param name="lastName">The optional value for the lastName column.</param>
        /// <param name="middleName">The required value for the middleName column.</param>
        /// <param name="phone">The optional value for the phone column.</param>
        /// <param name="postalCode">The optional value for the postalCode column.</param>
        /// <param name="provinceId">The required value for the provinceId column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateCustomer", ReplyAction = "http://tempuri.org/IDataModel/UpdateCustomerResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateCustomer(string address1, string address2, string city, string company, Guid countryId, Guid customerId, Guid customerIdKey, DateTime dateCreated, DateTime dateModified, DateTime dateOfBirth, string email, string externalId0, string firstName, string lastName, string middleName, string phone, string postalCode, Guid? provinceId, long rowVersion);

        /// <summary>
        /// Updates a License record.
        /// </summary>
        /// <param name="customerId">The optional value for the customerId column.</param>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="developerLicenseTypeCode">The optional value for the developerLicenseTypeCode column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="licenseId">The optional value for the licenseId column.</param>
        /// <param name="licenseIdKey">The LicenseId key element.</param>
        /// <param name="productId">The optional value for the productId column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        /// <param name="runtimeLicenseTypeCode">The optional value for the runtimeLicenseTypeCode column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateLicense", ReplyAction = "http://tempuri.org/IDataModel/UpdateLicenseResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateLicense(Guid customerId, DateTime dateCreated, DateTime dateModified, LicenseTypeCode developerLicenseTypeCode, string externalId0, Guid licenseId, Guid licenseIdKey, Guid productId, long rowVersion, LicenseTypeCode runtimeLicenseTypeCode);

        /// <summary>
        /// Updates a LicenseType record.
        /// </summary>
        /// <param name="description">The optional value for the description column.</param>
        /// <param name="licenseTypeCode">The optional value for the licenseTypeCode column.</param>
        /// <param name="licenseTypeCodeKey">The LicenseTypeCode key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateLicenseType", ReplyAction = "http://tempuri.org/IDataModel/UpdateLicenseTypeResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateLicenseType(string description, LicenseTypeCode licenseTypeCode, LicenseTypeCode licenseTypeCodeKey, long rowVersion);

        /// <summary>
        /// Updates a Product record.
        /// </summary>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="description">The required value for the description column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="productId">The optional value for the productId column.</param>
        /// <param name="productIdKey">The ProductId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateProduct", ReplyAction = "http://tempuri.org/IDataModel/UpdateProductResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateProduct(DateTime dateCreated, DateTime dateModified, string description, string externalId0, string name, Guid productId, Guid productIdKey, long rowVersion);

        /// <summary>
        /// Updates a Province record.
        /// </summary>
        /// <param name="abbreviation">The optional value for the abbreviation column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="provinceId">The optional value for the provinceId column.</param>
        /// <param name="provinceIdKey">The ProvinceId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationContract(Action = "http://tempuri.org/IDataModel/UpdateProvince", ReplyAction = "http://tempuri.org/IDataModel/UpdateProvinceResponse")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        [FaultContract(typeof(ConstraintFault))]
        [FaultContract(typeof(OptimisticConcurrencyFault))]
        [FaultContract(typeof(RecordNotFoundFault))]
        [FaultContract(typeof(SecurityTokenExpiredFault))]
        void UpdateProvince(string abbreviation, Guid countryId, string externalId0, string name, Guid provinceId, Guid provinceIdKey, long rowVersion);
    }

    /// <summary>
    /// A thread-safe, multi-tiered DataModel Service.
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// The data model.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        public DataService(DataModel dataModel)
        {
            this.dataModel = dataModel;
        }

        /// <summary>
        /// Creates a Configuration record.
        /// </summary>
        /// <param name="configurationId">The required value for the ConfigurationId column.</param>
        /// <param name="source">The required value for the Source column.</param>
        /// <param name="targetKey">The required value for the TargetKey column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateConfiguration(string configurationId, string source, string targetKey)
        {
            try
            {
                this.dataModel.CreateConfiguration(configurationId, source, targetKey);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a Country record.
        /// </summary>
        /// <param name="abbreviation">The required value for the Abbreviation column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateCountry(string abbreviation, Guid countryId, string externalId0, string name)
        {
            try
            {
                this.dataModel.CreateCountry(abbreviation, countryId, externalId0, name);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a Customer record.
        /// </summary>
        /// <param name="address1">The required value for the Address1 column.</param>
        /// <param name="address2">The optional value for the Address2 column.</param>
        /// <param name="city">The required value for the City column.</param>
        /// <param name="company">The optional value for the Company column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="customerId">The required value for the CustomerId column.</param>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="dateOfBirth">The required value for the DateOfBirth column.</param>
        /// <param name="email">The required value for the Email column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="firstName">The optional value for the FirstName column.</param>
        /// <param name="lastName">The required value for the LastName column.</param>
        /// <param name="middleName">The optional value for the MiddleName column.</param>
        /// <param name="phone">The required value for the Phone column.</param>
        /// <param name="postalCode">The required value for the PostalCode column.</param>
        /// <param name="provinceId">The optional value for the ProvinceId column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateCustomer(string address1, string address2, string city, string company, Guid countryId, Guid customerId, DateTime dateCreated, DateTime dateModified, DateTime dateOfBirth, string email, string externalId0, string firstName, string lastName, string middleName, string phone, string postalCode, Guid? provinceId)
        {
            try
            {
                this.dataModel.CreateCustomer(address1, address2, city, company, countryId, customerId, dateCreated, dateModified, dateOfBirth, email, externalId0, firstName, lastName, middleName, phone, postalCode, provinceId);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a License record.
        /// </summary>
        /// <param name="customerId">The required value for the CustomerId column.</param>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="developerLicenseTypeCode">The required value for the DeveloperLicenseTypeCode column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="licenseId">The required value for the LicenseId column.</param>
        /// <param name="productId">The required value for the ProductId column.</param>
        /// <param name="runtimeLicenseTypeCode">The required value for the RuntimeLicenseTypeCode column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateLicense(Guid customerId, DateTime dateCreated, DateTime dateModified, LicenseTypeCode developerLicenseTypeCode, string externalId0, Guid licenseId, Guid productId, LicenseTypeCode runtimeLicenseTypeCode)
        {
            try
            {
                this.dataModel.CreateLicense(customerId, dateCreated, dateModified, developerLicenseTypeCode, externalId0, licenseId, productId, runtimeLicenseTypeCode);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a LicenseType record.
        /// </summary>
        /// <param name="description">The required value for the Description column.</param>
        /// <param name="licenseTypeCode">The required value for the LicenseTypeCode column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateLicenseType(string description, LicenseTypeCode licenseTypeCode)
        {
            try
            {
                this.dataModel.CreateLicenseType(description, licenseTypeCode);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a Product record.
        /// </summary>
        /// <param name="dateCreated">The required value for the DateCreated column.</param>
        /// <param name="dateModified">The required value for the DateModified column.</param>
        /// <param name="description">The optional value for the Description column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        /// <param name="productId">The required value for the ProductId column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateProduct(DateTime dateCreated, DateTime dateModified, string description, string externalId0, string name, Guid productId)
        {
            try
            {
                this.dataModel.CreateProduct(dateCreated, dateModified, description, externalId0, name, productId);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Creates a Province record.
        /// </summary>
        /// <param name="abbreviation">The required value for the Abbreviation column.</param>
        /// <param name="countryId">The required value for the CountryId column.</param>
        /// <param name="externalId0">The optional value for the ExternalId0 column.</param>
        /// <param name="name">The required value for the Name column.</param>
        /// <param name="provinceId">The required value for the ProvinceId column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Create, Resource = ClaimResources.Application)]
        public void CreateProvince(string abbreviation, Guid countryId, string externalId0, string name, Guid provinceId)
        {
            try
            {
                this.dataModel.CreateProvince(abbreviation, countryId, externalId0, name, provinceId);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
        }

        /// <summary>
        /// Deletes a Configuration record.
        /// </summary>
        /// <param name="configurationId">The ConfigurationId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        /// <param name="source">The Source key element.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteConfiguration(string configurationId, long rowVersion, string source)
        {
            try
            {
                this.dataModel.DeleteConfiguration(configurationId, rowVersion, source);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a Country record.
        /// </summary>
        /// <param name="countryId">The CountryId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteCountry(Guid countryId, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteCountry(countryId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a Customer record.
        /// </summary>
        /// <param name="customerId">The CustomerId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteCustomer(Guid customerId, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteCustomer(customerId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a License record.
        /// </summary>
        /// <param name="licenseId">The LicenseId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteLicense(Guid licenseId, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteLicense(licenseId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a LicenseType record.
        /// </summary>
        /// <param name="licenseTypeCode">The LicenseTypeCode key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteLicenseType(LicenseTypeCode licenseTypeCode, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteLicenseType(licenseTypeCode, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a Product record.
        /// </summary>
        /// <param name="productId">The ProductId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteProduct(Guid productId, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteProduct(productId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Deletes a Province record.
        /// </summary>
        /// <param name="provinceId">The ProvinceId key element.</param>
        /// <param name="rowVersion">The required value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Delete, Resource = ClaimResources.Application)]
        public void DeleteProvince(Guid provinceId, long rowVersion)
        {
            try
            {
                this.dataModel.DeleteProvince(provinceId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Collects the set of modified records that will reconcile the client data model to the master data model.
        /// </summary>
        /// <param name="identifier">A unique identifier of an instance of the data.</param>
        /// <param name="sequence">The sequence of the client data model.</param>
        /// <returns>An array of records that will reconcile the client data model to the server.</returns>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Read, Resource = ClaimResources.Application)]
        public DataHeader Read(Guid identifier, long sequence)
        {
            return this.dataModel.Read(identifier, sequence);
        }

        /// <summary>
        /// Updates a Configuration record.
        /// </summary>
        /// <param name="configurationId">The optional value for the configurationId column.</param>
        /// <param name="configurationIdKey">The ConfigurationId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        /// <param name="source">The optional value for the source column.</param>
        /// <param name="sourceKey">The Source key element.</param>
        /// <param name="targetKey">The optional value for the targetKey column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateConfiguration(string configurationId, string configurationIdKey, long rowVersion, string source, string sourceKey, string targetKey)
        {
            try
            {
                this.dataModel.UpdateConfiguration(configurationId, configurationIdKey, rowVersion, source, sourceKey, targetKey);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a Country record.
        /// </summary>
        /// <param name="abbreviation">The optional value for the abbreviation column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="countryIdKey">The CountryId key element.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateCountry(string abbreviation, Guid countryId, Guid countryIdKey, string externalId0, string name, long rowVersion)
        {
            try
            {
                this.dataModel.UpdateCountry(abbreviation, countryId, countryIdKey, externalId0, name, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a Customer record.
        /// </summary>
        /// <param name="address1">The optional value for the address1 column.</param>
        /// <param name="address2">The required value for the address2 column.</param>
        /// <param name="city">The optional value for the city column.</param>
        /// <param name="company">The required value for the company column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="customerId">The optional value for the customerId column.</param>
        /// <param name="customerIdKey">The CustomerId key element.</param>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="dateOfBirth">The optional value for the dateOfBirth column.</param>
        /// <param name="email">The optional value for the email column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="firstName">The required value for the firstName column.</param>
        /// <param name="lastName">The optional value for the lastName column.</param>
        /// <param name="middleName">The required value for the middleName column.</param>
        /// <param name="phone">The optional value for the phone column.</param>
        /// <param name="postalCode">The optional value for the postalCode column.</param>
        /// <param name="provinceId">The required value for the provinceId column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateCustomer(string address1, string address2, string city, string company, Guid countryId, Guid customerId, Guid customerIdKey, DateTime dateCreated, DateTime dateModified, DateTime dateOfBirth, string email, string externalId0, string firstName, string lastName, string middleName, string phone, string postalCode, Guid? provinceId, long rowVersion)
        {
            try
            {
                this.dataModel.UpdateCustomer(address1, address2, city, company, countryId, customerId, customerIdKey, dateCreated, dateModified, dateOfBirth, email, externalId0, firstName, lastName, middleName, phone, postalCode, provinceId, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a License record.
        /// </summary>
        /// <param name="customerId">The optional value for the customerId column.</param>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="developerLicenseTypeCode">The optional value for the developerLicenseTypeCode column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="licenseId">The optional value for the licenseId column.</param>
        /// <param name="licenseIdKey">The LicenseId key element.</param>
        /// <param name="productId">The optional value for the productId column.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        /// <param name="runtimeLicenseTypeCode">The optional value for the runtimeLicenseTypeCode column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateLicense(Guid customerId, DateTime dateCreated, DateTime dateModified, LicenseTypeCode developerLicenseTypeCode, string externalId0, Guid licenseId, Guid licenseIdKey, Guid productId, long rowVersion, LicenseTypeCode runtimeLicenseTypeCode)
        {
            try
            {
                this.dataModel.UpdateLicense(customerId, dateCreated, dateModified, developerLicenseTypeCode, externalId0, licenseId, licenseIdKey, productId, rowVersion, runtimeLicenseTypeCode);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a LicenseType record.
        /// </summary>
        /// <param name="description">The optional value for the description column.</param>
        /// <param name="licenseTypeCode">The optional value for the licenseTypeCode column.</param>
        /// <param name="licenseTypeCodeKey">The LicenseTypeCode key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateLicenseType(string description, LicenseTypeCode licenseTypeCode, LicenseTypeCode licenseTypeCodeKey, long rowVersion)
        {
            try
            {
                this.dataModel.UpdateLicenseType(description, licenseTypeCode, licenseTypeCodeKey, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a Product record.
        /// </summary>
        /// <param name="dateCreated">The optional value for the dateCreated column.</param>
        /// <param name="dateModified">The optional value for the dateModified column.</param>
        /// <param name="description">The required value for the description column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="productId">The optional value for the productId column.</param>
        /// <param name="productIdKey">The ProductId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateProduct(DateTime dateCreated, DateTime dateModified, string description, string externalId0, string name, Guid productId, Guid productIdKey, long rowVersion)
        {
            try
            {
                this.dataModel.UpdateProduct(dateCreated, dateModified, description, externalId0, name, productId, productIdKey, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }

        /// <summary>
        /// Updates a Province record.
        /// </summary>
        /// <param name="abbreviation">The optional value for the abbreviation column.</param>
        /// <param name="countryId">The optional value for the countryId column.</param>
        /// <param name="externalId0">The required value for the externalId0 column.</param>
        /// <param name="name">The optional value for the name column.</param>
        /// <param name="provinceId">The optional value for the provinceId column.</param>
        /// <param name="provinceIdKey">The ProvinceId key element.</param>
        /// <param name="rowVersion">The optional value for the rowVersion column.</param>
        [OperationBehavior(TransactionScopeRequired = true)]
        [ClaimsPermission(SecurityAction.Demand, ClaimType = ClaimTypes.Update, Resource = ClaimResources.Application)]
        public void UpdateProvince(string abbreviation, Guid countryId, string externalId0, string name, Guid provinceId, Guid provinceIdKey, long rowVersion)
        {
            try
            {
                this.dataModel.UpdateProvince(abbreviation, countryId, externalId0, name, provinceId, provinceIdKey, rowVersion);
            }
            catch (ConstraintException constraintException)
            {
                throw new FaultException<ConstraintFault>(new ConstraintFault(constraintException.Operation, constraintException.Constraint));
            }
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new FaultException<OptimisticConcurrencyFault>(new OptimisticConcurrencyFault(optimisticConcurrencyException.Table, optimisticConcurrencyException.Key));
            }
            catch (RecordNotFoundException recordNotFoundException)
            {
                throw new FaultException<RecordNotFoundFault>(new RecordNotFoundFault(recordNotFoundException.Table, recordNotFoundException.Key));
            }
        }
    }
}