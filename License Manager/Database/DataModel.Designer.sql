CREATE TABLE [dbo].[Configuration]
(
    [ConfigurationId] NVARCHAR(128) NOT NULL,
    [RowVersion] BIGINT NOT NULL,
    [Source] NVARCHAR(64) NOT NULL,
    [TargetKey] NVARCHAR(64) NOT NULL,
	CONSTRAINT [ConfigurationKey] PRIMARY KEY ([ConfigurationId],[Source])
)
go
CREATE TABLE [dbo].[Country]
(
    [Abbreviation] NVARCHAR(MAX) NOT NULL,
    [CountryId] UNIQUEIDENTIFIER NOT NULL,
    [ExternalId0] NVARCHAR(128) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [RowVersion] BIGINT NOT NULL,
	CONSTRAINT [CountryKey] PRIMARY KEY ([CountryId])
)
go
CREATE INDEX [CountryExternalId0Key] ON [Country] ([ExternalId0])
go
CREATE TABLE [dbo].[LicenseType]
(
    [Description] NVARCHAR(128) NOT NULL,
    [LicenseTypeCode] int NOT NULL,
    [RowVersion] BIGINT NOT NULL,
	CONSTRAINT [LicenseTypeKey] PRIMARY KEY ([LicenseTypeCode])
)
go
CREATE TABLE [dbo].[Product]
(
    [DateCreated] DATETIME NOT NULL,
    [DateModified] DATETIME NOT NULL,
    [Description] NVARCHAR(128) NULL,
    [ExternalId0] NVARCHAR(128) NULL,
    [Name] NVARCHAR(128) NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion] BIGINT NOT NULL,
	CONSTRAINT [ProductKey] PRIMARY KEY ([ProductId])
)
go
CREATE INDEX [ProductExternalId0Key] ON [Product] ([ExternalId0])
go
CREATE TABLE [dbo].[Province]
(
    [Abbreviation] NVARCHAR(MAX) NOT NULL,
    [CountryId] UNIQUEIDENTIFIER NOT NULL,
    [ExternalId0] NVARCHAR(128) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [ProvinceId] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion] BIGINT NOT NULL,
	CONSTRAINT [ProvinceKey] PRIMARY KEY ([ProvinceId])
)
go
CREATE INDEX [ProvinceExternalId0Key] ON [Province] ([ExternalId0])
go
CREATE TABLE [dbo].[Customer]
(
    [Address1] NVARCHAR(256) NOT NULL,
    [Address2] NVARCHAR(256) NULL,
    [City] NVARCHAR(128) NOT NULL,
    [Company] NVARCHAR(128) NULL,
    [CountryId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    [DateModified] DATETIME NOT NULL,
    [DateOfBirth] DATETIME NOT NULL,
    [Email] NVARCHAR(128) NOT NULL,
    [ExternalId0] NVARCHAR(128) NULL,
    [FirstName] NVARCHAR(128) NULL,
    [LastName] NVARCHAR(128) NOT NULL,
    [MiddleName] NVARCHAR(128) NULL,
    [Phone] NVARCHAR(64) NOT NULL,
    [PostalCode] NVARCHAR(32) NOT NULL,
    [ProvinceId] UNIQUEIDENTIFIER NULL,
    [RowVersion] BIGINT NOT NULL,
	CONSTRAINT [CustomerKey] PRIMARY KEY ([CustomerId])
)
go
CREATE INDEX [CustomerExternalId0Key] ON [Customer] ([ExternalId0])
go
CREATE TABLE [dbo].[License]
(
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    [DateModified] DATETIME NOT NULL,
    [DeveloperLicenseTypeCode] int NOT NULL,
    [ExternalId0] NVARCHAR(128) NULL,
    [LicenseId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion] BIGINT NOT NULL,
    [RuntimeLicenseTypeCode] int NOT NULL,
	CONSTRAINT [LicenseKey] PRIMARY KEY ([LicenseId])
)
go
CREATE INDEX [LicenseExternalId0Key] ON [License] ([ExternalId0])
go
CREATE PROCEDURE [dbo].[createConfiguration] @configurationId NVARCHAR(128), @rowVersion BIGINT, @source NVARCHAR(64), @targetKey NVARCHAR(64)
AS
BEGIN
    INSERT INTO [dbo].[Configuration] ([ConfigurationId], [RowVersion], [Source], [TargetKey])
    VALUES (@configurationId, @rowVersion, @source, @targetKey)
END
GO
CREATE PROCEDURE [dbo].[deleteConfiguration] @keyConfigurationId NVARCHAR(128), @keySource NVARCHAR(64)
AS
BEGIN
    DELETE [dbo].[Configuration]
    WHERE [ConfigurationId] = @keyConfigurationId AND [Source] = @keySource
END
GO
CREATE PROCEDURE [dbo].[readConfiguration]
AS
BEGIN
    SELECT [ConfigurationId],[RowVersion],[Source],[TargetKey] FROM [dbo].[Configuration]
END
GO
CREATE PROCEDURE [dbo].[updateConfiguration] @configurationId NVARCHAR(128),@rowVersion BIGINT,@source NVARCHAR(64),@targetKey NVARCHAR(64),@keyConfigurationId NVARCHAR(128), @keySource NVARCHAR(64)
AS
BEGIN
    UPDATE [dbo].[Configuration]
    SET
        [ConfigurationId] = @configurationId,
        [RowVersion] = @rowVersion,
        [Source] = @source,
        [TargetKey] = @targetKey
    WHERE [ConfigurationId] = @keyConfigurationId AND [Source] = @keySource
END
GO
CREATE PROCEDURE [dbo].[createCountry] @abbreviation NVARCHAR(MAX), @countryId UNIQUEIDENTIFIER, @externalId0 NVARCHAR(128) NULL, @name NVARCHAR(MAX), @rowVersion BIGINT
AS
BEGIN
    INSERT INTO [dbo].[Country] ([Abbreviation], [CountryId], [ExternalId0], [Name], [RowVersion])
    VALUES (@abbreviation, @countryId, @externalId0, @name, @rowVersion)
END
GO
CREATE PROCEDURE [dbo].[deleteCountry] @keyCountryId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE [dbo].[Country]
    WHERE [CountryId] = @keyCountryId
END
GO
CREATE PROCEDURE [dbo].[readCountry]
AS
BEGIN
    SELECT [Abbreviation],[CountryId],[ExternalId0],[Name],[RowVersion] FROM [dbo].[Country]
END
GO
CREATE PROCEDURE [dbo].[updateCountry] @abbreviation NVARCHAR(MAX),@countryId UNIQUEIDENTIFIER,@externalId0 NVARCHAR(128) NULL,@name NVARCHAR(MAX),@rowVersion BIGINT,@keyCountryId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[Country]
    SET
        [Abbreviation] = @abbreviation,
        [CountryId] = @countryId,
        [ExternalId0] = @externalId0,
        [Name] = @name,
        [RowVersion] = @rowVersion
    WHERE [CountryId] = @keyCountryId
END
GO
CREATE PROCEDURE [dbo].[createCustomer] @address1 NVARCHAR(256), @address2 NVARCHAR(256) NULL, @city NVARCHAR(128), @company NVARCHAR(128) NULL, @countryId UNIQUEIDENTIFIER, @customerId UNIQUEIDENTIFIER, @dateCreated DATETIME, @dateModified DATETIME, @dateOfBirth DATETIME, @email NVARCHAR(128), @externalId0 NVARCHAR(128) NULL, @firstName NVARCHAR(128) NULL, @lastName NVARCHAR(128), @middleName NVARCHAR(128) NULL, @phone NVARCHAR(64), @postalCode NVARCHAR(32), @provinceId UNIQUEIDENTIFIER NULL, @rowVersion BIGINT
AS
BEGIN
    INSERT INTO [dbo].[Customer] ([Address1], [Address2], [City], [Company], [CountryId], [CustomerId], [DateCreated], [DateModified], [DateOfBirth], [Email], [ExternalId0], [FirstName], [LastName], [MiddleName], [Phone], [PostalCode], [ProvinceId], [RowVersion])
    VALUES (@address1, @address2, @city, @company, @countryId, @customerId, @dateCreated, @dateModified, @dateOfBirth, @email, @externalId0, @firstName, @lastName, @middleName, @phone, @postalCode, @provinceId, @rowVersion)
END
GO
CREATE PROCEDURE [dbo].[deleteCustomer] @keyCustomerId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE [dbo].[Customer]
    WHERE [CustomerId] = @keyCustomerId
END
GO
CREATE PROCEDURE [dbo].[readCustomer]
AS
BEGIN
    SELECT [Address1],[Address2],[City],[Company],[CountryId],[CustomerId],[DateCreated],[DateModified],[DateOfBirth],[Email],[ExternalId0],[FirstName],[LastName],[MiddleName],[Phone],[PostalCode],[ProvinceId],[RowVersion] FROM [dbo].[Customer]
END
GO
CREATE PROCEDURE [dbo].[updateCustomer] @address1 NVARCHAR(256),@address2 NVARCHAR(256) NULL,@city NVARCHAR(128),@company NVARCHAR(128) NULL,@countryId UNIQUEIDENTIFIER,@customerId UNIQUEIDENTIFIER,@dateCreated DATETIME,@dateModified DATETIME,@dateOfBirth DATETIME,@email NVARCHAR(128),@externalId0 NVARCHAR(128) NULL,@firstName NVARCHAR(128) NULL,@lastName NVARCHAR(128),@middleName NVARCHAR(128) NULL,@phone NVARCHAR(64),@postalCode NVARCHAR(32),@provinceId UNIQUEIDENTIFIER NULL,@rowVersion BIGINT,@keyCustomerId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[Customer]
    SET
        [Address1] = @address1,
        [Address2] = @address2,
        [City] = @city,
        [Company] = @company,
        [CountryId] = @countryId,
        [CustomerId] = @customerId,
        [DateCreated] = @dateCreated,
        [DateModified] = @dateModified,
        [DateOfBirth] = @dateOfBirth,
        [Email] = @email,
        [ExternalId0] = @externalId0,
        [FirstName] = @firstName,
        [LastName] = @lastName,
        [MiddleName] = @middleName,
        [Phone] = @phone,
        [PostalCode] = @postalCode,
        [ProvinceId] = @provinceId,
        [RowVersion] = @rowVersion
    WHERE [CustomerId] = @keyCustomerId
END
GO
CREATE PROCEDURE [dbo].[createLicense] @customerId UNIQUEIDENTIFIER, @dateCreated DATETIME, @dateModified DATETIME, @developerLicenseTypeCode int, @externalId0 NVARCHAR(128) NULL, @licenseId UNIQUEIDENTIFIER, @productId UNIQUEIDENTIFIER, @rowVersion BIGINT, @runtimeLicenseTypeCode int
AS
BEGIN
    INSERT INTO [dbo].[License] ([CustomerId], [DateCreated], [DateModified], [DeveloperLicenseTypeCode], [ExternalId0], [LicenseId], [ProductId], [RowVersion], [RuntimeLicenseTypeCode])
    VALUES (@customerId, @dateCreated, @dateModified, @developerLicenseTypeCode, @externalId0, @licenseId, @productId, @rowVersion, @runtimeLicenseTypeCode)
END
GO
CREATE PROCEDURE [dbo].[deleteLicense] @keyLicenseId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE [dbo].[License]
    WHERE [LicenseId] = @keyLicenseId
END
GO
CREATE PROCEDURE [dbo].[readLicense]
AS
BEGIN
    SELECT [CustomerId],[DateCreated],[DateModified],[DeveloperLicenseTypeCode],[ExternalId0],[LicenseId],[ProductId],[RowVersion],[RuntimeLicenseTypeCode] FROM [dbo].[License]
END
GO
CREATE PROCEDURE [dbo].[updateLicense] @customerId UNIQUEIDENTIFIER,@dateCreated DATETIME,@dateModified DATETIME,@developerLicenseTypeCode int,@externalId0 NVARCHAR(128) NULL,@licenseId UNIQUEIDENTIFIER,@productId UNIQUEIDENTIFIER,@rowVersion BIGINT,@runtimeLicenseTypeCode int,@keyLicenseId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[License]
    SET
        [CustomerId] = @customerId,
        [DateCreated] = @dateCreated,
        [DateModified] = @dateModified,
        [DeveloperLicenseTypeCode] = @developerLicenseTypeCode,
        [ExternalId0] = @externalId0,
        [LicenseId] = @licenseId,
        [ProductId] = @productId,
        [RowVersion] = @rowVersion,
        [RuntimeLicenseTypeCode] = @runtimeLicenseTypeCode
    WHERE [LicenseId] = @keyLicenseId
END
GO
CREATE PROCEDURE [dbo].[createLicenseType] @description NVARCHAR(128), @licenseTypeCode int, @rowVersion BIGINT
AS
BEGIN
    INSERT INTO [dbo].[LicenseType] ([Description], [LicenseTypeCode], [RowVersion])
    VALUES (@description, @licenseTypeCode, @rowVersion)
END
GO
CREATE PROCEDURE [dbo].[deleteLicenseType] @keyLicenseTypeCode int
AS
BEGIN
    DELETE [dbo].[LicenseType]
    WHERE [LicenseTypeCode] = @keyLicenseTypeCode
END
GO
CREATE PROCEDURE [dbo].[readLicenseType]
AS
BEGIN
    SELECT [Description],[LicenseTypeCode],[RowVersion] FROM [dbo].[LicenseType]
END
GO
CREATE PROCEDURE [dbo].[updateLicenseType] @description NVARCHAR(128),@licenseTypeCode int,@rowVersion BIGINT,@keyLicenseTypeCode int
AS
BEGIN
    UPDATE [dbo].[LicenseType]
    SET
        [Description] = @description,
        [LicenseTypeCode] = @licenseTypeCode,
        [RowVersion] = @rowVersion
    WHERE [LicenseTypeCode] = @keyLicenseTypeCode
END
GO
CREATE PROCEDURE [dbo].[createProduct] @dateCreated DATETIME, @dateModified DATETIME, @description NVARCHAR(128) NULL, @externalId0 NVARCHAR(128) NULL, @name NVARCHAR(128), @productId UNIQUEIDENTIFIER, @rowVersion BIGINT
AS
BEGIN
    INSERT INTO [dbo].[Product] ([DateCreated], [DateModified], [Description], [ExternalId0], [Name], [ProductId], [RowVersion])
    VALUES (@dateCreated, @dateModified, @description, @externalId0, @name, @productId, @rowVersion)
END
GO
CREATE PROCEDURE [dbo].[deleteProduct] @keyProductId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE [dbo].[Product]
    WHERE [ProductId] = @keyProductId
END
GO
CREATE PROCEDURE [dbo].[readProduct]
AS
BEGIN
    SELECT [DateCreated],[DateModified],[Description],[ExternalId0],[Name],[ProductId],[RowVersion] FROM [dbo].[Product]
END
GO
CREATE PROCEDURE [dbo].[updateProduct] @dateCreated DATETIME,@dateModified DATETIME,@description NVARCHAR(128) NULL,@externalId0 NVARCHAR(128) NULL,@name NVARCHAR(128),@productId UNIQUEIDENTIFIER,@rowVersion BIGINT,@keyProductId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[Product]
    SET
        [DateCreated] = @dateCreated,
        [DateModified] = @dateModified,
        [Description] = @description,
        [ExternalId0] = @externalId0,
        [Name] = @name,
        [ProductId] = @productId,
        [RowVersion] = @rowVersion
    WHERE [ProductId] = @keyProductId
END
GO
CREATE PROCEDURE [dbo].[createProvince] @abbreviation NVARCHAR(MAX), @countryId UNIQUEIDENTIFIER, @externalId0 NVARCHAR(128) NULL, @name NVARCHAR(MAX), @provinceId UNIQUEIDENTIFIER, @rowVersion BIGINT
AS
BEGIN
    INSERT INTO [dbo].[Province] ([Abbreviation], [CountryId], [ExternalId0], [Name], [ProvinceId], [RowVersion])
    VALUES (@abbreviation, @countryId, @externalId0, @name, @provinceId, @rowVersion)
END
GO
CREATE PROCEDURE [dbo].[deleteProvince] @keyProvinceId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE [dbo].[Province]
    WHERE [ProvinceId] = @keyProvinceId
END
GO
CREATE PROCEDURE [dbo].[readProvince]
AS
BEGIN
    SELECT [Abbreviation],[CountryId],[ExternalId0],[Name],[ProvinceId],[RowVersion] FROM [dbo].[Province]
END
GO
CREATE PROCEDURE [dbo].[updateProvince] @abbreviation NVARCHAR(MAX),@countryId UNIQUEIDENTIFIER,@externalId0 NVARCHAR(128) NULL,@name NVARCHAR(MAX),@provinceId UNIQUEIDENTIFIER,@rowVersion BIGINT,@keyProvinceId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[Province]
    SET
        [Abbreviation] = @abbreviation,
        [CountryId] = @countryId,
        [ExternalId0] = @externalId0,
        [Name] = @name,
        [ProvinceId] = @provinceId,
        [RowVersion] = @rowVersion
    WHERE [ProvinceId] = @keyProvinceId
END
GO
