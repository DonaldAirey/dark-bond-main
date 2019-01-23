// <copyright file="VolatileSingleRecordTests.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.DataModelTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using ClientModel;
    using Moq;
    using NUnit.Framework;
    using Properties;

    /// <summary>
    /// Test Fixture for the in-memory cache.
    /// </summary>
    [TestFixture]
    [Category("Volatile")]
    public sealed class VolatileSingleRecordTests : IDisposable
    {
        /// <summary>
        /// The data model to be tested.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Initialize the environment for tests.
        /// </summary>
        [OneTimeSetUp]
        public void Initialize()
        {
            // Create a file-based listener in the home directory.
            string logPath = Path.Combine(
                Environment.ExpandEnvironmentVariables("%HomeDrive%"),
                Environment.ExpandEnvironmentVariables("%HomePath%"),
                "Documents",
                "Output.Log");
            Trace.Listeners.Add(new TextWriterTraceListener(logPath));
            Trace.AutoFlush = true;

            // Create a mock for the persistent store.
            var mockPersistentStore = new Mock<IPersistentStore>();
            mockPersistentStore.Setup(p => p.Read()).Returns(new List<object[]>());

            // This is a data model with a mocked persistent store.
            this.dataModel = new DataModel(mockPersistentStore.Object);
        }

        /// <summary>
        /// Create a configuration record and commit.
        /// </summary>
        [Test]
        public void CreateConfigurationCommit()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // This is the count of rows before the test.
                this.dataModel.Configuration.AddReaderLock();
                beforeCount = this.dataModel.Configuration.Count;
            }

            // The test transaction.
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "CreateConfigurationCommit", "TargetKey");

                // Validate that the record was added to the primary index.
                Assert.That(configurationRow, Is.Not.Null);
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Added));

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount + 1));

                // Validate the state of the row.
                this.dataModel.ConfigurationKey.AddReaderLock();
                configurationRow = this.dataModel.ConfigurationKey.Find("Test", "CreateConfigurationCommit");
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Unchanged));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("Test"));
                Assert.That(configurationRow.Source, Is.EqualTo("CreateConfigurationCommit"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("TargetKey"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a configuration record and roll it back.
        /// </summary>
        [Test]
        public void CreateConfigurationRollback()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // This is the count of rows before the test.
                this.dataModel.Configuration.AddReaderLock();
                beforeCount = this.dataModel.Configuration.Count;
            }

            // The test transaction.
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "CreateConfigurationRollback", "TargetKey");

                // Validate that the record was added to the primary index.
                Assert.That(configurationRow, Is.Not.Null);
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Added));

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // The record shouldn't be in the primary index, the count of records should be the same as before the test, and the row should be
                // detached.
                this.dataModel.ConfigurationKey.AddReaderLock();
                Assert.That(this.dataModel.ConfigurationKey.Find("Test", "CreateConfigurationRollback"), Is.Null);
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount));
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Detached));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a country record and commit.
        /// </summary>
        [Test]
        public void CreateCountryCommit()
        {
            Guid countryId = new Guid("{78239E22-EA5D-479B-AE87-8F124A304662}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the country record.
                CountryRow countryRow = this.dataModel.CreateCountry("BS1", countryId, "BALTICSTATE1", "Baltic State");

                // Validate that the record was added to the unique index.
                Assert.That(countryRow, Is.Not.Null);
                Assert.That(countryRow.RowState, Is.EqualTo(RowState.Added));
                Assert.That(this.dataModel.CountryExternalId0Key.Find("BALTICSTATE1"), Is.Not.Null);

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.CountryExternalId0Key.AddReaderLock();

                // Validate that the row is in the unique index.
                Assert.That(this.dataModel.CountryExternalId0Key.Find("BALTICSTATE1"), Is.Not.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a country record and commit.
        /// </summary>
        [Test]
        public void CreateCountryRollback()
        {
            Guid countryId = new Guid("{3F56A5BF-6548-47AB-8C34-1E637002A8F6}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the country record.
                CountryRow countryRow = this.dataModel.CreateCountry("BS2", countryId, "BALTICSTATE2", "Baltic State");

                // Validate that the record was added to the unique index.
                Assert.That(countryRow, Is.Not.Null);
                Assert.That(countryRow.RowState, Is.EqualTo(RowState.Added));
                Assert.That(this.dataModel.CountryExternalId0Key.Find("BALTICSTATE2"), Is.Not.Null);

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.CountryExternalId0Key.AddReaderLock();

                // Validate that the row has been removed from the unique index.
                Assert.That(this.dataModel.CountryExternalId0Key.Find("BALTICSTATE2"), Is.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a record using a compound key and commit it.
        /// </summary>
        [Test]
        public void CreateCustomerCommit()
        {
            Guid countryId = new Guid("{6EEB2BFC-1272-499B-9F0D-E8139E1EDF17}");
            Guid provinceId = new Guid("{85C75FC7-AC48-425A-902F-7C3B7104D9DE}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the parent records.
                this.dataModel.CreateCountry("C", countryId, null, "CreateCustomerCommit");
                this.dataModel.CreateProvince("P", countryId, null, "CreateCustomerCommit", provinceId);
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the customer records.
                this.dataModel.CreateCustomer("10 Main Street", null, "Concord", "Evil Corp.", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("6/22/1995", CultureInfo.InvariantCulture), "adam@home.com", "CUSTOMER1", "Adam", "Saunders", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("3/5/1991", CultureInfo.InvariantCulture), "junk@email.com", "CUSTOMER2", "Bob", "Berg", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("2031 East Street", null, "Boston", "Google", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("5/6/2001", CultureInfo.InvariantCulture), "colms.puffdaddy@music.org", "CUSTOMER3", "Charlie", "Hill", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("7 Bay Place", null, "Bristol", "Taco Bell", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("1/5/1961", CultureInfo.InvariantCulture), "ggrouper@gmail.com", "CUSTOMER4", "Deacon", "James", null, "617-555-1212", "11280", provinceId);
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record using the non-primary key.  Then check the negative case.
                this.dataModel.CustomerExternalId0Key.AddReaderLock();
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER1"), Is.Not.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER2"), Is.Not.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER3"), Is.Not.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER4"), Is.Not.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER5"), Is.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a record using a compound key and roll it back.
        /// </summary>
        [Test]
        public void CreateCustomerRollback()
        {
            Guid countryId = new Guid("{6A4793CC-A6BD-4488-AF71-CAB9BA80E7AD}");
            Guid provinceId = new Guid("{BB446120-CA28-423E-A34A-AFB11F2E0D04}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the parent records.
                this.dataModel.CreateCountry("C", countryId, null, "CreateCustomerCommit");
                this.dataModel.CreateProvince("P", countryId, null, "CreateCustomerCommit", provinceId);
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the customer records.
                this.dataModel.CreateCustomer("10 Main Street", null, "Concord", "Evil Corp.", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("6/22/1995", CultureInfo.InvariantCulture), "adam@home.com", "CUSTOMER11", "Adam", "Saunders", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("3/5/1991", CultureInfo.InvariantCulture), "junk@email.com", "CUSTOMER12", "Bob", "Berg", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("2031 East Street", null, "Boston", "Google", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("5/6/2001", CultureInfo.InvariantCulture), "colms.puffdaddy@music.org", "CUSTOMER13", "Charlie", "Hill", null, "617-555-1212", "11280", provinceId);
                this.dataModel.CreateCustomer("7 Bay Place", null, "Bristol", "Taco Bell", countryId, Guid.NewGuid(), DateTime.Now, DateTime.Now, DateTime.Parse("1/5/1961", CultureInfo.InvariantCulture), "ggrouper@gmail.com", "CUSTOMER14", "Deacon", "James", null, "617-555-1212", "11280", provinceId);
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record using the non-primary key.  Then check the negative case.
                this.dataModel.CustomerExternalId0Key.AddReaderLock();
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER11"), Is.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER12"), Is.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER13"), Is.Null);
                Assert.That(this.dataModel.CustomerExternalId0Key.Find("CUSTOMER14"), Is.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Exercise all the parameters in the Create method.
        /// </summary>
        [Test]
        public void CreateParameters()
        {
            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.CreateConfiguration(null, null, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.CreateConfiguration("Test", null, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.CreateConfiguration("Test", "CreateParameters", null));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Delete a configuration record and commit it.
        /// </summary>
        [Test]
        public void DeleteConfigurationCommit()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            long rowVersion = long.MinValue;
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "DeleteConfigurationCommit", "TargetKey");

                // Get the number of rows before the test.
                beforeCount = this.dataModel.Configuration.Count;

                // The row version is needed for optimistic concurrency.
                rowVersion = configurationRow.RowVersion;

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Delete the configuration record.
                this.dataModel.DeleteConfiguration("Test", rowVersion, "DeleteConfigurationCommit");

                // Validate that the primary index was updated.
                Assert.That(this.dataModel.ConfigurationKey.Find("Test", "DeleteConfigurationCommit"), Is.Null);
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Deleted));

                // Commit it.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount - 1));

                // Validate that the primary index was updated and that the row is detached.
                this.dataModel.ConfigurationKey.AddReaderLock();
                Assert.That(this.dataModel.ConfigurationKey.Find("Test", "DeleteConfigurationCommit"), Is.Null);
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Detached));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Update a configuration record and roll it back.
        /// </summary>
        [Test]
        public void DeleteConfigurationRollback()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            long rowVersion = long.MinValue;
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "DeleteConfigurationRollback", "TargetKey");

                // Get the number of rows before the test.
                beforeCount = this.dataModel.Configuration.Count;

                // The row version is needed for optimistic concurrency.
                rowVersion = configurationRow.RowVersion;

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Delete the configuration record.
                this.dataModel.DeleteConfiguration("Test", rowVersion, "DeleteConfigurationRollback");

                // Validate that the primary index was updated.
                Assert.That(this.dataModel.ConfigurationKey.Find("Test", "DeleteConfigurationRollback"), Is.Null);
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Deleted));

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount));

                // Validate that the primary index has been restored.
                this.dataModel.ConfigurationKey.AddReaderLock();
                configurationRow = this.dataModel.ConfigurationKey.Find("Test", "DeleteConfigurationRollback");
                Assert.That(configurationRow, Is.Not.Null);

                // Validate the row's values.
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Unchanged));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("Test"));
                Assert.That(configurationRow.RowVersion, Is.EqualTo(rowVersion));
                Assert.That(configurationRow.Source, Is.EqualTo("DeleteConfigurationRollback"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("TargetKey"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Exercise all the parameters in the Delete method.
        /// </summary>
        [Test]
        public void DeleteParameters()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Test", "DeleteParameters", "TargetKey");

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.DeleteConfiguration(null, 0, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.DeleteConfiguration("Test", 0, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate an optimistic concurrency exception.
                Assert.Throws<OptimisticConcurrencyException>(() => this.dataModel.DeleteConfiguration("Test", 0, "DeleteParameters"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Dispose of the managed resources.
        /// </summary>
        public void Dispose()
        {
            this.dataModel.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Attempt to update a non-existent record.
        /// </summary>
        [Test]
        public void RecordNotFound()
        {
            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                Assert.Throws<RecordNotFoundException>(() => this.dataModel.UpdateConfiguration("RecordNotFound", "Test", 0, "RecordNotFound", "Test", "TargetKey"));

                // Commit.
                testTransaction.Commit();
            }
        }

        /// <summary>
        /// Exercise all the row threading violations.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "source", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "targetKey", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "rowVersion", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "configurationId", Justification = "Reviewed")]
        [Test]
        public void RowThreadingViolations()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Test", "RowThreadingViolations", "TargetKey");

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Get the record.
                this.dataModel.ConfigurationKey.AddReaderLock();
                ConfigurationRow configurationRow = this.dataModel.ConfigurationKey.Find("Test", "RowThreadingViolations");

                // The row isn't locked, so this should generate an exception.
                Assert.IsFalse(configurationRow.IsLockHeld);
                Assert.IsFalse(configurationRow.IsReaderLockHeld);
                Assert.IsFalse(configurationRow.IsWriterLockHeld);
                Assert.Throws<LockException>(() => { string configurationId = configurationRow.ConfigurationId; });
                Assert.Throws<LockException>(() => { long rowVersion = configurationRow.RowVersion; });
                Assert.Throws<LockException>(() => { string source = configurationRow.Source; });
                Assert.Throws<LockException>(() => { string targetKey = configurationRow.TargetKey; });

                try
                {
                    configurationRow.AcquireReaderLock();

                    // Validate that the reader lock is held.
                    Assert.IsTrue(configurationRow.IsLockHeld);
                    Assert.IsTrue(configurationRow.IsReaderLockHeld);
                    Assert.IsFalse(configurationRow.IsWriterLockHeld);
                }
                finally
                {
                    configurationRow.ReleaseReaderLock();
                }

                // Validate that the reader lock is no longer held.
                Assert.IsFalse(configurationRow.IsLockHeld);
                Assert.IsFalse(configurationRow.IsReaderLockHeld);
                Assert.IsFalse(configurationRow.IsWriterLockHeld);

                try
                {
                    configurationRow.AcquireWriterLock();

                    // Validate that the reader lock is held.
                    Assert.IsTrue(configurationRow.IsLockHeld);
                    Assert.IsFalse(configurationRow.IsReaderLockHeld);
                    Assert.IsTrue(configurationRow.IsWriterLockHeld);
                }
                finally
                {
                    configurationRow.ReleaseWriterLock();
                }

                // Validate that the reader lock is no longer held.
                Assert.IsFalse(configurationRow.IsLockHeld);
                Assert.IsFalse(configurationRow.IsReaderLockHeld);
                Assert.IsFalse(configurationRow.IsWriterLockHeld);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Exercise all the table threading violations.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "rows", Justification = "Reviewed")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "count", Justification = "Reviewed")]
        [Test]
        public void TableThreadingViolations()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Test", "TableThreadingViolations", "TargetKey");

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Verify that the table doesn't hold a lock.
                Assert.IsFalse(this.dataModel.Configuration.IsLockHeld);

                // Verify that the table doesn't hold a lock.
                Assert.IsFalse(this.dataModel.Configuration.IsWriterLockHeld);

                // The table isn't locked, so this should generate an exception.
                Assert.Throws<LockException>(() => this.dataModel.ConfigurationKey.Find("Test", "DeleteConfigurationCommit"));

                // The table isn't locked, so this should generate an exception.
                Assert.Throws<LockException>(() => { int count = this.dataModel.Configuration.Count; });

                // The table isn't locked, so this should generate an exception.
                Assert.Throws<LockException>(() => this.dataModel.Configuration.GetEnumerator());

                // The table isn't locked, so this should generate an exception.
                Assert.Throws<LockException>(() => { IEnumerator rows = (IEnumerator)this.dataModel.Configuration.GetEnumerator(); });

                try
                {
                    this.dataModel.Configuration.AcquireReaderLock();

                    // Validate that the reader lock is held.
                    Assert.IsTrue(this.dataModel.Configuration.IsLockHeld);
                    Assert.IsTrue(this.dataModel.Configuration.IsReaderLockHeld);
                    Assert.IsFalse(this.dataModel.Configuration.IsWriterLockHeld);
                }
                finally
                {
                    this.dataModel.Configuration.ReleaseReaderLock();
                }

                // Validate that the reader lock is no longer held.
                Assert.IsFalse(this.dataModel.Configuration.IsLockHeld);
                Assert.IsFalse(this.dataModel.Configuration.IsReaderLockHeld);
                Assert.IsFalse(this.dataModel.Configuration.IsWriterLockHeld);

                try
                {
                    this.dataModel.Configuration.AcquireWriterLock();

                    // Validate that the writer lock works.
                    Assert.IsTrue(this.dataModel.Configuration.IsLockHeld);
                    Assert.IsFalse(this.dataModel.Configuration.IsReaderLockHeld);
                    Assert.IsTrue(this.dataModel.Configuration.IsWriterLockHeld);
                }
                finally
                {
                    this.dataModel.Configuration.ReleaseWriterLock();
                }

                // Validate that the writer lock is no longer held.
                Assert.IsFalse(this.dataModel.Configuration.IsLockHeld);
                Assert.IsFalse(this.dataModel.Configuration.IsReaderLockHeld);
                Assert.IsFalse(this.dataModel.Configuration.IsWriterLockHeld);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Update a configuration record and commit it.
        /// </summary>
        [Test]
        public void UpdateConfigurationCommit()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            long rowVersion = long.MinValue;
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "OldUpdateConfigurationCommit", "TargetKey");

                // This is the count of rows before the test.
                beforeCount = this.dataModel.Configuration.Count;

                // The row version is needed for optimistic concurrency.
                rowVersion = configurationRow.RowVersion;

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Update the configuration record.
                this.dataModel.UpdateConfiguration("NewTest", "Test", rowVersion, "NewUpdateConfigurationCommit", "OldUpdateConfigurationCommit", "NewTargetKey");

                // Validate that the primary index was updated.
                configurationRow = this.dataModel.ConfigurationKey.Find("NewTest", "NewUpdateConfigurationCommit");
                Assert.That(configurationRow, Is.Not.Null);

                // Validate the row's state before the commit.
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Modified));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("NewTest"));
                Assert.That(configurationRow.Source, Is.EqualTo("NewUpdateConfigurationCommit"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("NewTargetKey"));

                // Commit it.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was added.
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount));

                // Validate that the primary index was updated.
                this.dataModel.ConfigurationKey.AddReaderLock();
                configurationRow = this.dataModel.ConfigurationKey.Find("NewTest", "NewUpdateConfigurationCommit");
                Assert.That(configurationRow, Is.Not.Null);

                // Validate the row's values after the commit.
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Unchanged));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("NewTest"));
                Assert.That(configurationRow.Source, Is.EqualTo("NewUpdateConfigurationCommit"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("NewTargetKey"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Update a configuration record and roll it back.
        /// </summary>
        [Test]
        public void UpdateConfigurationRollback()
        {
            // The transaction that sets up the test.
            int beforeCount = int.MinValue;
            long rowVersion = long.MinValue;
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "OldUpdateConfigurationRollback", "TargetKey");

                // This is the count of rows before the test.
                beforeCount = this.dataModel.Configuration.Count;

                // The row version is needed for optimistic concurrency.
                rowVersion = configurationRow.RowVersion;

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Update the configuration record.
                this.dataModel.UpdateConfiguration("NewTest", "Test", rowVersion, "NewUpdateConfigurationRollback", "OldUpdateConfigurationRollback", "NewTargetKey");

                // Validate that the primary index was updated.
                configurationRow = this.dataModel.ConfigurationKey.Find("NewTest", "NewUpdateConfigurationRollback");
                Assert.That(configurationRow, Is.Not.Null);

                // Validate the row's state before the commmit.
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Modified));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("NewTest"));
                Assert.That(configurationRow.Source, Is.EqualTo("NewUpdateConfigurationRollback"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("NewTargetKey"));

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the row was not added.
                this.dataModel.Configuration.AddReaderLock();
                Assert.That(this.dataModel.Configuration.Count, Is.EqualTo(beforeCount));

                // Validate that the primary index was not updated.
                this.dataModel.ConfigurationKey.AddReaderLock();
                Assert.That(this.dataModel.ConfigurationKey.Find("NewTest", "NewUpdateConfigurationRollback"), Is.Null);
                configurationRow = this.dataModel.ConfigurationKey.Find("Test", "OldUpdateConfigurationRollback");
                Assert.That(configurationRow, Is.Not.Null);

                // Validate the row's values.
                configurationRow.AddReaderLock();
                Assert.That(configurationRow.RowState, Is.EqualTo(RowState.Unchanged));
                Assert.That(configurationRow.ConfigurationId, Is.EqualTo("Test"));
                Assert.That(configurationRow.Source, Is.EqualTo("OldUpdateConfigurationRollback"));
                Assert.That(configurationRow.TargetKey, Is.EqualTo("TargetKey"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Update a configuration record and roll it back.
        /// </summary>
        [Test]
        public void UpdateConfigurationAndSearchCommit()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Delta", "UpdateConfigurationAndSearchCommit", "TargetKey");
                this.dataModel.CreateConfiguration("Alpha", "UpdateConfigurationAndSearchCommit", "TargetKey");
                this.dataModel.CreateConfiguration("Epsilon", "UpdateConfigurationAndSearchCommit", "TargetKey");
                this.dataModel.CreateConfiguration("Gamma", "UpdateConfigurationAndSearchCommit", "TargetKey");
                this.dataModel.CreateConfiguration("Beta", "UpdateConfigurationAndSearchCommit", "TargetKey");

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record.
                this.dataModel.ConfigurationKey.AddReaderLock();
                ConfigurationRow configurationRow = this.dataModel.ConfigurationKey.Find("Delta", "UpdateConfigurationAndSearchCommit");
                configurationRow.AddReaderLock();

                // This will physically move the record.
                this.dataModel.UpdateConfiguration("Zeta", "Delta", configurationRow.RowVersion, "UpdateConfigurationAndSearchCommit", "UpdateConfigurationAndSearchCommit", "TargetKey");

                // Commit it.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record in it's new location.
                this.dataModel.ConfigurationKey.AddReaderLock();
                Assert.That(this.dataModel.ConfigurationKey.Find("Zeta", "UpdateConfigurationAndSearchCommit"), Is.Not.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Update a configuration record and roll it back.
        /// </summary>
        [Test]
        public void UpdateConfigurationAndSearchRollback()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Delta", "UpdateConfigurationAndSearchRollback", "TargetKey");
                this.dataModel.CreateConfiguration("Alpha", "UpdateConfigurationAndSearchRollback", "TargetKey");
                this.dataModel.CreateConfiguration("Epsilon", "UpdateConfigurationAndSearchRollback", "TargetKey");
                this.dataModel.CreateConfiguration("Gamma", "UpdateConfigurationAndSearchRollback", "TargetKey");
                this.dataModel.CreateConfiguration("Beta", "UpdateConfigurationAndSearchRollback", "TargetKey");

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record.
                this.dataModel.ConfigurationKey.AddReaderLock();
                ConfigurationRow configurationRow = this.dataModel.ConfigurationKey.Find("Delta", "UpdateConfigurationAndSearchRollback");
                configurationRow.AddReaderLock();

                // This will physically move the record.
                this.dataModel.UpdateConfiguration("Zeta", "Delta", configurationRow.RowVersion, "UpdateConfigurationAndSearchRollback", "UpdateConfigurationAndSearchRollback", "TargetKey");

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record in it's old location.
                this.dataModel.ConfigurationKey.AddReaderLock();
                Assert.That(this.dataModel.ConfigurationKey.Find("Delta", "UpdateConfigurationAndSearchRollback"), Is.Not.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Exercise all the parameters in the Update method.
        /// </summary>
        [Test]
        public void UpdateParameters()
        {
            // The transaction that sets up the test.
            long rowVersion;
            ConfigurationRow configurationRow = null;
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                configurationRow = this.dataModel.CreateConfiguration("Test", "UpdateParameters", "TargetKey");

                // Get the row version.
                rowVersion = configurationRow.RowVersion;

                // Commit the test data.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.UpdateConfiguration(null, null, rowVersion, null, null, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.UpdateConfiguration(null, "Test", rowVersion, null, null, null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.UpdateConfiguration(null, "Test", rowVersion, null, "UpdateParameters", null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.UpdateConfiguration("Test", "Test", rowVersion, null, "UpdateParameters", null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate a null argument exception.
                Assert.Throws<ArgumentNullException>(() => this.dataModel.UpdateConfiguration("Test", "Test", rowVersion, "NewUpdateParameters", "UpdateParameters", null));
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Generate an optimistic concurrency exception.
                Assert.Throws<OptimisticConcurrencyException>(() => this.dataModel.UpdateConfiguration("Test", "Test", 0, "NewUpdateParameters", "UpdateParameters", "NewTargetKey"));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Violate a primary index.
        /// </summary>
        [Test]
        public void ViolatePrimaryIndex()
        {
            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                this.dataModel.CreateConfiguration("Test", "ViolatePrimaryIndex", "TargetKey");

                // Commit.
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the configuration record.
                Assert.Throws<DuplicateKeyException>(() => this.dataModel.CreateConfiguration("Test", "ViolatePrimaryIndex", "TargetKey"));

                // Commit.
                testTransaction.Commit();
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }
    }
}