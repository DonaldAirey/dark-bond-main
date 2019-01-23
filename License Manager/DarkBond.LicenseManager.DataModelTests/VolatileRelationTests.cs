// <copyright file="VolatileRelationTests.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.DataModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using ClientModel;
    using Moq;
    using NUnit.Framework;
    using Properties;
    using ServiceModel;

    /// <summary>
    /// Test Fixture for the in-memory cache.
    /// </summary>
    [TestFixture]
    [Category("Volatile")]
    public sealed class VolatileRelationTests : IDisposable
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
        /// Create a parent child relation and commit it.
        /// </summary>
        [Test]
        public void CreateChildCommit()
        {
            Guid countryId = new Guid("{B02781E2-33BD-4D7F-BFCD-EF107B363460}");
            Guid provinceId = new Guid("{E50CED3C-5419-440B-800B-A01B34089DDB}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country.
                this.dataModel.CreateCountry("C", countryId, null, "CreateChildCommit Country");

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the province record.
                this.dataModel.CreateProvince("P", countryId, null, "CreateChildCommit Province", provinceId);

                // Find the parent record.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                Assert.That(countryRow, Is.Not.Null);

                // Validate that the child record was added to the primary index.
                countryRow.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.Abbreviation, Is.EqualTo("P"));
                        Assert.That(provinceRow.CountryId, Is.EqualTo(countryId));
                        Assert.IsNull(provinceRow.ExternalId0);
                        Assert.That(provinceRow.Name, Is.EqualTo("CreateChildCommit Province"));
                        Assert.That(provinceRow.ProvinceId, Is.EqualTo(provinceId));
                        Assert.That(provinceRow.RowState, Is.EqualTo(RowState.Added));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                Assert.That(countryRow, Is.Not.Null);

                // Validate that each of the child rows is unchanged.
                countryRow.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.RowState, Is.EqualTo(RowState.Unchanged));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a child with no parent.
        /// </summary>
        [Test]
        public void CreateChildNoParent()
        {
            Guid countryId = new Guid("{E20C34E9-908C-4885-8772-74E2EC39AD54}");
            Guid provinceId = new Guid("{8D9551A9-5D44-4EED-BA82-C0E938E66C24}");

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the province record, verify that it fails without a parent relation.
                Assert.Throws<ConstraintException>(() => this.dataModel.CreateProvince("P", countryId, null, "CreateChildCommit Province", provinceId));

                // Commit.
                testTransaction.Commit();
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a parent child relation and roll it back.
        /// </summary>
        [Test]
        public void CreateChildRollback()
        {
            Guid countryId = new Guid("{FC7E8790-D2EB-4F45-BA2D-738186F3BAE6}");
            Guid provinceId = new Guid("{C6456CCA-6A1B-41A9-8B49-A23F085E655E}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country.
                this.dataModel.CreateCountry("C", countryId, null, "CreateChildRollback Country");

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Add the province record.
                this.dataModel.CreateProvince("P", countryId, null, "CreateChildRollback Province", provinceId);

                // Find the parent record.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                Assert.That(countryRow, Is.Not.Null);

                // Validate that the child record was added to the primary index.
                countryRow.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.Abbreviation, Is.EqualTo("P"));
                        Assert.That(provinceRow.CountryId, Is.EqualTo(countryId));
                        Assert.IsNull(provinceRow.ExternalId0);
                        Assert.That(provinceRow.Name, Is.EqualTo("CreateChildRollback Province"));
                        Assert.That(provinceRow.ProvinceId, Is.EqualTo(provinceId));
                        Assert.That(provinceRow.RowState, Is.EqualTo(RowState.Added));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                Assert.That(countryRow, Is.Not.Null);

                countryRow.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                int childCounter = this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId).Count;

                // Assert that the child isn't in the index;
                Assert.That(childCounter, Is.EqualTo(0));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a foreign key with an invalid parent and make sure this operation fails.
        /// </summary>
        [Test]
        public void ParentConstraintViolation()
        {
            Guid country1Id = new Guid("{CAC515E1-FB5E-41A8-8DE1-42024FB34993}");
            Guid country2Id = new Guid("{B1F96F66-7CD2-4453-8B3A-5056FF9281C8}");
            Guid customerId = new Guid("{C45D3D42-4E0A-47F2-9DA9-5DDEE48D07C4}");

            // The transaction that sets up the test.
            long rowVersion = default(long);
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the test data.  We're going to update the customer to have a non-existent parent.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateCountryCommit");
                CustomerRow customerRow = this.dataModel.CreateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", country1Id, customerId, DateTime.Now, DateTime.Now, DateTime.Parse("3/5/1991", CultureInfo.InvariantCulture), "junk@email.com", null, "Robbert", "Dodington", null, "617-555-1212", "11280", null);
                rowVersion = customerRow.RowVersion;
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Update the customer to a non-existent country.
                Assert.Throws<ConstraintException>(() => this.dataModel.UpdateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", country2Id, customerId, customerId, DateTime.Now, DateTime.Now, DateTime.Parse("3/5/1991", CultureInfo.InvariantCulture), "junk@email.com", null, "Robbert", "Dodington", null, "617-555-1212", "11280", null, rowVersion));
            }

            // Assert that the transaction was rolled back properly.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the record using the compound non-primary key.  Then check the negative case.
                this.dataModel.CustomerKey.AddReaderLock();
                CustomerRow customerRow = this.dataModel.CustomerKey.Find(customerId);
                customerRow.AddReaderLock();
                Assert.That(customerRow, Is.Not.Null);
                this.dataModel.CountryCustomerCountryIdKey.AddReaderLock();
                List<CustomerRow> customers = this.dataModel.CountryCustomerCountryIdKey.GetCustomerRows(customerRow.CountryId);
                Assert.That(customers.Contains(customerRow), Is.True);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a foreign key relation and test the entire parent/child chain is correct.
        /// </summary>
        [Test]
        public void UpdateCountryCommit()
        {
            Guid country1Id = new Guid("{E1042569-C191-4FDB-9349-16E1AE36832B}");
            Guid country2Id = new Guid("{4A9F9B34-451C-46B0-A1C2-7739DBCA7C86}");
            Guid customerId = new Guid("{1A1A4E3B-1725-4D76-9CE9-AB16F6895241}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the parent records.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateCountryCommit");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateCountryCommit");
                this.dataModel.CreateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", country1Id, customerId, DateTime.Now, DateTime.Now, DateTime.Parse("10/12/1952", CultureInfo.InvariantCulture), "junk@email.com", null, "Henry", "Ford", null, "617-555-1212", "11280", null);
                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                long rowVersion = default(long);
                try
                {
                    this.dataModel.CustomerKey.AcquireReaderLock();
                    CustomerRow customerRow = this.dataModel.CustomerKey.Find(customerId);
                    try
                    {
                        customerRow.AcquireReaderLock();
                        rowVersion = customerRow.RowVersion;
                    }
                    finally
                    {
                        customerRow.ReleaseReaderLock();
                    }
                }
                finally
                {
                    this.dataModel.CustomerKey.ReleaseReaderLock();
                }

                // Update the country.
                this.dataModel.UpdateCustomer("1 Maple Drive", null, "Lincoln", "Boston University", country2Id, customerId, customerId, DateTime.Now, DateTime.Now, DateTime.Parse("10/12/1952", CultureInfo.InvariantCulture), "junk@email.com", null, "Henry", "Ford", null, "617-555-1212", "11280", null, rowVersion);
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the entire chain of relations is correct.
                this.dataModel.CustomerKey.AddReaderLock();
                CustomerRow customerRow = this.dataModel.CustomerKey.Find(customerId);
                customerRow.AddReaderLock();
                Assert.That(customerRow, Is.Not.Null);
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(customerRow.CountryId);
                countryRow.AddReaderLock();
                Assert.That(countryRow, Is.Not.Null);
                this.dataModel.CountryCustomerCountryIdKey.AddReaderLock();
                List<CustomerRow> customers = this.dataModel.CountryCustomerCountryIdKey.GetCustomerRows(customerRow.CountryId);
                Assert.That(customers.Contains(customerRow), Is.True);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Delete a parent and child relation and commit it.
        /// </summary>
        [Test]
        public void DeleteParentCommit()
        {
            Guid countryId = new Guid("{AF4D6856-B0E7-4C92-96D4-754A4C505964}");
            Guid provinceId = new Guid("{E7B0281D-1E8C-4A5C-BE5F-DE5814272B40}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country.
                this.dataModel.CreateCountry("C", countryId, null, "Delete Parent Commit Country");

                // Create the province.
                this.dataModel.CreateProvince("P", countryId, null, "Delete Parent Commit Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Delete the country and all it's child provinces.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                countryRow.AddReaderLock();

                // Delete each of the provinces.
                this.dataModel.CountryProvinceKey.AcquireReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();

                        // Extract the province information from the record.
                        Guid childProvinceId = provinceRow.ProvinceId;
                        long childRowVersion = provinceRow.RowVersion;

                        // Delete each of the provinces.
                        this.dataModel.DeleteProvince(childProvinceId, childRowVersion);
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Delete the country.
                this.dataModel.DeleteCountry(countryId, countryRow.RowVersion);

                // Assert that the parent row has been deleted.
                Assert.That(this.dataModel.CountryKey.Find(countryId), Is.Null);

                // Assert that the child row has been deleted.
                Assert.That(this.dataModel.ProvinceKey.Find(provinceId), Is.Null);

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the parent record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                Assert.That(this.dataModel.CountryKey.Find(countryId), Is.Null);

                // Assert that the child records are still gone after the commit.
                this.dataModel.ProvinceKey.AddReaderLock();
                Assert.That(this.dataModel.ProvinceKey.Find(provinceId), Is.Null);
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Delete a parent and child relation and roll it back.
        /// </summary>
        [Test]
        public void DeleteParentRollback()
        {
            Guid countryId = new Guid("{E79C9422-B14C-4535-BB9C-225C881E3B9C}");
            Guid provinceId = new Guid("{D0795D73-06CF-45A4-B199-E3E073C49FA3}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country.
                this.dataModel.CreateCountry("C", countryId, null, "Delete Parent Rollback Country");

                // Create the province.
                this.dataModel.CreateProvince("P", countryId, null, "Delete Parent Rollback Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Delete the country and all it's child provinces.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                countryRow.AddReaderLock();

                // Delete each of the provinces.
                this.dataModel.CountryProvinceKey.AcquireReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();

                        // Extract the province information from the record.
                        Guid childProvinceId = provinceRow.ProvinceId;
                        long childRowVersion = provinceRow.RowVersion;

                        // Delete each of the provinces.
                        this.dataModel.DeleteProvince(childProvinceId, childRowVersion);
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Delete the country.
                this.dataModel.DeleteCountry(countryId, countryRow.RowVersion);

                // Assert that the parent row has been deleted.
                Assert.That(this.dataModel.CountryKey.Find(countryId), Is.Null);

                // Assert that the child row has been deleted.
                Assert.That(this.dataModel.ProvinceKey.Find(provinceId), Is.Null);

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been restored.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow countryRow = this.dataModel.CountryKey.Find(countryId);
                Assert.That(countryRow, Is.Not.Null);
                Assert.That(countryRow.RowState, Is.EqualTo(RowState.Unchanged));

                // Assert that the child relation was restored.
                countryRow.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();

                        // Assert that the child has been restored correctly.
                        Assert.That(provinceRow.Abbreviation, Is.EqualTo("P"));
                        Assert.That(provinceRow.CountryId, Is.EqualTo(countryId));
                        Assert.That(provinceRow.ExternalId0, Is.Null);
                        Assert.That(provinceRow.Name, Is.EqualTo("Delete Parent Rollback Province"));
                        Assert.That(provinceRow.ProvinceId, Is.EqualTo(provinceId));
                        Assert.That(provinceRow.RowState, Is.EqualTo(RowState.Unchanged));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }
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
        /// Create a parent child relation, change and commit it.
        /// </summary>
        [Test]
        public void UpdateChildCommit()
        {
            Guid country1Id = new Guid("{6367FF81-DE1C-4879-B7B2-DE38287385F5}");
            Guid country2Id = new Guid("{C90DAD96-9CB9-4B18-907E-2824330E1EB7}");
            Guid provinceId = new Guid("{2352DC0B-942F-4944-9BB1-FC3E43869E33}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the countries.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateChildCommit Country1");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateChildCommit Country2");

                // Create the province.
                this.dataModel.CreateProvince("P", country1Id, null, "UpdateChildCommit Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                this.dataModel.ProvinceKey.AddReaderLock();
                ProvinceRow provinceRow = this.dataModel.ProvinceKey.Find(provinceId);
                provinceRow.AddReaderLock();
                long childRowVersion = provinceRow.RowVersion;

                // Change the parent of the child.
                this.dataModel.UpdateProvince("P", country2Id, null, "UpdateChildCommit Province", provinceId, provinceId, childRowVersion);

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow country1Row = this.dataModel.CountryKey.Find(country1Id);
                CountryRow country2Row = this.dataModel.CountryKey.Find(country2Id);
                Assert.That(country1Row, Is.Not.Null);
                Assert.That(country2Row, Is.Not.Null);

                // Assert that the row was removed from the first country.
                country1Row.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                int count = this.dataModel.CountryProvinceKey.GetProvinceRows(country1Row.CountryId).Count;

                // Assert that we found no children associated with the original parent.
                Assert.That(count, Is.EqualTo(0));

                // Assert that the row was moved to the second country.
                country2Row.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(country2Row.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.CountryId, Is.EqualTo(country2Id));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a parent child relation, change and roll it back.
        /// </summary>
        [Test]
        public void UpdateChildRollback()
        {
            Guid country1Id = new Guid("{1DCAE768-A6E9-4FEB-AE11-382CA1A5DDC8}");
            Guid country2Id = new Guid("{BD9AF722-834D-487E-832E-791113C3FF30}");
            Guid provinceId = new Guid("{7E179F79-346D-44F2-85A2-4519FB8998E5}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the countries.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateChildRollback Country1");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateChildRollback Country2");

                // Create the province.
                this.dataModel.CreateProvince("P", country1Id, null, "UpdateChildRollback Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                this.dataModel.ProvinceKey.AddReaderLock();
                ProvinceRow provinceRow = this.dataModel.ProvinceKey.Find(provinceId);
                provinceRow.AddReaderLock();
                long childRowVersion = provinceRow.RowVersion;

                // Change the parent of the child.
                this.dataModel.UpdateProvince("P", country2Id, null, "UpdateChildCommit Province", provinceId, provinceId, childRowVersion);

                // Commit.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow country1Row = this.dataModel.CountryKey.Find(country1Id);
                CountryRow country2Row = this.dataModel.CountryKey.Find(country2Id);
                Assert.That(country1Row, Is.Not.Null);
                Assert.That(country2Row, Is.Not.Null);

                // Assert that the child province was returned to the first country.
                country1Row.AddReaderLock();
                int count = 0;
                this.dataModel.CountryProvinceKey.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(country1Row.CountryId))
                {
                    count++;
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.CountryId, Is.EqualTo(country1Id));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Make sure that we found a row in the original parent.
                Assert.That(count, Is.EqualTo(1));

                // Assert that the child province was removed from the second country.
                country2Row.AddReaderLock();
                count = this.dataModel.CountryProvinceKey.GetProvinceRows(country2Row.CountryId).Count;

                // Make sure that we found didn't find a row in the original parent.
                Assert.That(count, Is.EqualTo(0));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a parent child relation, change it twice (to test the index changes) and commit it.
        /// </summary>
        [Test]
        public void UpdateChildTwiceCommit()
        {
            Guid country1Id = new Guid("{AEC32C18-0AE0-49C5-B113-7C3A0C79990E}");
            Guid country2Id = new Guid("{95F08BF5-AFDD-4439-8658-A3598EEFCFA0}");
            Guid country3Id = new Guid("{2C563944-94DE-49A5-9447-D95846E6062E}");
            Guid provinceId = new Guid("{C9D28173-7EA3-4BC9-8582-5F7E1AE1ED7C}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the countries.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateChildTwiceCommit Country1");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateChildTwiceCommit Country2");
                this.dataModel.CreateCountry("C3", country3Id, null, "UpdateChildTwiceCommit Country3");

                // Create the province.
                this.dataModel.CreateProvince("P", country1Id, null, "UpdateChildTwiceCommit Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                this.dataModel.ProvinceKey.AddReaderLock();
                ProvinceRow provinceRow = this.dataModel.ProvinceKey.Find(provinceId);
                provinceRow.AddReaderLock();

                // Change the parent of the child.
                this.dataModel.UpdateProvince("P", country2Id, null, "UpdateChildTwiceCommit Province", provinceId, provinceId, provinceRow.RowVersion);

                // Change the parent of the child again.
                this.dataModel.UpdateProvince("P", country3Id, null, "UpdateChildTwiceCommit Province", provinceId, provinceId, provinceRow.RowVersion);

                // Commit.
                testTransaction.Commit();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow country1Row = this.dataModel.CountryKey.Find(country1Id);
                CountryRow country2Row = this.dataModel.CountryKey.Find(country2Id);
                CountryRow country3Row = this.dataModel.CountryKey.Find(country3Id);
                Assert.That(country1Row, Is.Not.Null);
                Assert.That(country2Row, Is.Not.Null);
                Assert.That(country3Row, Is.Not.Null);

                // Assert that the row was removed from the first country.
                country1Row.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                int count = this.dataModel.CountryProvinceKey.GetProvinceRows(country1Row.CountryId).Count;

                // Make sure that we found a row in the original parent.
                Assert.That(count, Is.EqualTo(0));

                // Assert that the row was removed from the second country.
                country2Row.AddReaderLock();
                count = this.dataModel.CountryProvinceKey.GetProvinceRows(country2Row.CountryId).Count;

                // Make sure that we found a row in the original parent.
                Assert.That(count, Is.EqualTo(0));

                // Assert that the row was moved to the second country.
                country3Row.AddReaderLock();
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(country3Row.CountryId))
                {
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.CountryId, Is.EqualTo(country3Id));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a parent child relation, change it twice (to test the index changes) and roll it back.
        /// </summary>
        [Test]
        public void UpdateChildTwiceRollback()
        {
            Guid country1Id = new Guid("{C4182CB0-E03E-475D-8563-69175B041B8A}");
            Guid country2Id = new Guid("{B25B4F01-FF70-4D2C-8A10-99C60B52E359}");
            Guid country3Id = new Guid("{AF28A28C-103F-41F5-8173-3A30AC6E30F5}");
            Guid provinceId = new Guid("{BF2B94D2-532A-4A31-843A-B3B0F44B01E7}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the countries.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateChildTwiceRollback Country1");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateChildTwiceRollback Country2");
                this.dataModel.CreateCountry("C3", country3Id, null, "UpdateChildTwiceRollback Country3");

                // Create the province.
                this.dataModel.CreateProvince("P", country1Id, null, "UpdateChildTwiceRollback Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                this.dataModel.ProvinceKey.AddReaderLock();
                ProvinceRow provinceRow = this.dataModel.ProvinceKey.Find(provinceId);
                provinceRow.AddReaderLock();

                // Change the parent of the child.
                this.dataModel.UpdateProvince("P", country2Id, null, "UpdateChildTwiceRollback Province", provinceId, provinceId, provinceRow.RowVersion);

                // Change the parent of the child again.
                this.dataModel.UpdateProvince("P", country3Id, null, "UpdateChildTwiceRollback Province", provinceId, provinceId, provinceRow.RowVersion);

                // Roll it back.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been committed.
                this.dataModel.CountryKey.AddReaderLock();
                CountryRow country1Row = this.dataModel.CountryKey.Find(country1Id);
                CountryRow country2Row = this.dataModel.CountryKey.Find(country2Id);
                CountryRow country3Row = this.dataModel.CountryKey.Find(country3Id);
                Assert.That(country1Row, Is.Not.Null);
                Assert.That(country2Row, Is.Not.Null);
                Assert.That(country3Row, Is.Not.Null);

                // Assert that the row was moved to the second country.
                country1Row.AddReaderLock();
                this.dataModel.CountryProvinceKey.AddReaderLock();
                int count = 0;
                foreach (ProvinceRow provinceRow in this.dataModel.CountryProvinceKey.GetProvinceRows(country1Row.CountryId))
                {
                    count++;
                    try
                    {
                        provinceRow.AcquireReaderLock();
                        Assert.That(provinceRow.CountryId, Is.EqualTo(country1Id));
                    }
                    finally
                    {
                        provinceRow.ReleaseReaderLock();
                    }
                }

                // Make sure that we found a row in the original parent.
                Assert.That(count, Is.EqualTo(1));

                // Assert that the row was removed from the first country.
                country2Row.AddReaderLock();
                count = this.dataModel.CountryProvinceKey.GetProvinceRows(country2Row.CountryId).Count;

                // Make sure there are no children.
                Assert.That(count, Is.EqualTo(0));

                // Assert that the row was removed from the second country.
                country3Row.AddReaderLock();
                count = this.dataModel.CountryProvinceKey.GetProvinceRows(country3Row.CountryId).Count;

                // Make sure there are no children.
                Assert.That(count, Is.EqualTo(0));
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Create a parent child relation, change it twice (to test the index changes) and commit it.
        /// </summary>
        [Test]
        public void UpdateParentPrimaryKey()
        {
            Guid country1Id = new Guid("{C310DE2D-BEEB-4060-BE8C-1DC8130FD98C}");
            Guid country2Id = new Guid("{3CA0D9DB-4934-43C9-AC8A-DB4ACABDCF47}");
            Guid country3Id = new Guid("{893E177C-ADE2-4447-9564-C841E17DE453}");
            Guid provinceId = new Guid("{1060B156-FBB9-47D6-8011-55841108139B}");

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the countries.
                this.dataModel.CreateCountry("C1", country1Id, null, "UpdateParentPrimaryKey Country1");
                this.dataModel.CreateCountry("C2", country2Id, null, "UpdateParentPrimaryKey Country2");

                // Create the province.
                this.dataModel.CreateProvince("P", country1Id, null, "UpdateParentPrimaryKey Province", provinceId);

                testTransaction.Commit();
            }

            // The test transaction.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                long rowVersion = default(long);
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    CountryRow countryRow = this.dataModel.CountryKey.Find(country1Id);
                    try
                    {
                        countryRow.AcquireReaderLock();
                        rowVersion = countryRow.RowVersion;
                    }
                    finally
                    {
                        countryRow.ReleaseReaderLock();
                    }
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }

                // This should generate a constraint exception as we don't allow for the primary key of a parent to be updated as it would create a
                // cascade effect.
                Assert.Throws<ConstraintException>(() => this.dataModel.UpdateCountry("C3", country3Id, country1Id, null, "UpdateParentPrimaryKey Country1", rowVersion));

                // We need to roll this operation back because the constraint was violated.
                testTransaction.Rollback();
            }

            // The transaction that validates the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Validate that the record has been rolled back.
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    this.dataModel.CountryProvinceKey.AcquireReaderLock();
                    CountryRow countryRow = this.dataModel.CountryKey.Find(country1Id);
                    Assert.That(countryRow, Is.Not.Null);
                    try
                    {
                        countryRow.AcquireReaderLock();
                        Assert.That(countryRow.CountryId, Is.EqualTo(country1Id));
                        int count = this.dataModel.CountryProvinceKey.GetProvinceRows(countryRow.CountryId).Count;
                        Assert.That(count, Is.EqualTo(1));
                    }
                    finally
                    {
                        countryRow.ReleaseReaderLock();
                    }
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }
    }
}