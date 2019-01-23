// <copyright file="MultithreadingTests.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.DataModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using Moq;
    using NUnit.Framework;
    using Properties;
    using ServiceModel;

    /// <summary>
    /// Test Fixture for the in-memory cache.
    /// </summary>
    [TestFixture]
    [Category("Volatile")]
    public sealed class MultithreadingTests : IDisposable
    {
        /// <summary>
        /// Maximum amount of time for a transaction to complete.
        /// </summary>
        private static TimeSpan timeLimit = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The country id.
        /// </summary>
        private static Guid country1Id = new Guid("{6367FF81-DE1C-4879-B7B2-DE38287385F5}");

        /// <summary>
        /// The country id.
        /// </summary>
        private static Guid country2Id = new Guid("{0BB3961A-15F7-4C2F-B793-67088E2CE467}");

        /// <summary>
        /// The country id.
        /// </summary>
        private static Guid country3Id = new Guid("{E6C1E552-EF46-44A2-B47A-CA811D767207}");

        /// <summary>
        /// The data model to be tested.
        /// </summary>
        private DataModel dataModel;

        /// <summary>
        /// Used to signal that the first writer has acquired a lock.
        /// </summary>
        private ManualResetEvent readerTestSetupComplete = new ManualResetEvent(false);

        /// <summary>
        /// Used to indicate that the second thread is finished.
        /// </summary>
        private ManualResetEvent testCompleted = new ManualResetEvent(false);

        /// <summary>
        /// Used to signal that several startup events have completed.
        /// </summary>
        private CountdownEvent task1SetupComplete = new CountdownEvent(0);

        /// <summary>
        /// Used to signal that several startup events have completed.
        /// </summary>
        private ManualResetEvent task2SetupComplete = new ManualResetEvent(false);

        /// <summary>
        /// Cleans up the environment after a test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Spelled Correctly")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "Spelled Correctly")]
        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            if (Transaction.Current != null)
            {
                throw new InvalidOperationException("Transaction has not been cleared.");
            }
        }

        /// <summary>
        /// Initialize the environment for tests.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Spelled Correctly")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "Spelled Correctly")]
        [OneTimeSetUp]
        public void OneTimeSetUp()
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
            IPersistentStore persistentStore = mockPersistentStore.Object;
            mockPersistentStore.Setup(p => p.Read()).Returns(new List<object[]>());

            // This is a data model with a mocked persistent store.
            this.dataModel = new DataModel(persistentStore);
        }

        /// <summary>
        /// Create a parent child relation, change and commit it.
        /// </summary>
        [Test]
        public void Deadlock()
        {
            // Reset the signals
            this.task1SetupComplete.Reset(1);
            this.testCompleted.Reset();

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country
                this.dataModel.CreateCountry("C1", MultithreadingTests.country1Id, null, "Deadlock Country1");

                testTransaction.Commit();
            }

            // Create and start the test threads.
            List<Task> tasks = new List<Task>();
            try
            {
                tasks.Add(Task.Factory.StartNew(this.DeadlockTask1, TaskCreationOptions.LongRunning));
                tasks.Add(Task.Factory.StartNew(this.DeadlockTask2, TaskCreationOptions.LongRunning));
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException aggregateException)
            {
                Assert.That(aggregateException.InnerExceptions.Count, Is.EqualTo(1));
                Assert.That(aggregateException.InnerExceptions[0], Is.TypeOf(typeof(ThreadAbortException)));
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
            this.readerTestSetupComplete.Dispose();
            this.testCompleted.Dispose();
            this.task1SetupComplete.Dispose();
            this.task2SetupComplete.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Tests the ability to pick up a reader lock after multiple writer locks have been released.
        /// </summary>
        [Test]
        public void ReaderReleaseLock()
        {
            // This is the number of simultaneous readers that will hold a resource.
            int writers = 5;

            // Reset the signals.  Writes will block each other, so we're going to wait for just one of them to signal that it has a write lock
            // before trying to acquire a reader lock.
            this.task1SetupComplete.Reset(writers);
            this.task2SetupComplete.Reset();

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country
                this.dataModel.CreateCountry("C3", MultithreadingTests.country3Id, null, "ReaderRelease Country");
                testTransaction.Commit();
            }

            // Create and start the test threads.  We're going to spawn several writers and a single reader that will wait for all the locks to be
            // released.  The test is complete when the reader has acquired the lock from a writer.
            List<Task> tasks = new List<Task>();
            try
            {
                for (int count = 0; count < writers; count++)
                {
                    tasks.Add(Task.Factory.StartNew(this.ReaderReleaseTask1, TaskCreationOptions.LongRunning));
                }

                tasks.Add(Task.Factory.StartNew(this.ReaderReleaseTask2, TaskCreationOptions.LongRunning));
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count != 0)
                {
                    throw aggregateException.InnerExceptions[0];
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Tests the ability to pick up a writer lock when multiple reader locks have been released.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "s", Justification = "Reviewed")]
        [Test]
        public void NotInTransaction()
        {
            // If we do have an ambient transaction laying around, make sure it's been disposed.  Then clear it so it doesn't interfere with the
            // test.
            if (Transaction.Current != null)
            {
                Assert.Throws<ObjectDisposedException>(() => { TransactionStatus s = Transaction.Current.TransactionInformation.Status; });
                Transaction.Current = null;
            }

            // Generate the exception when not executed in a transaction.
            Assert.Throws<InvalidOperationException>(() => this.dataModel.AcquireReaderLock());
            Assert.Throws<InvalidOperationException>(() => this.dataModel.AcquireWriterLock());
            Assert.Throws<InvalidOperationException>(() => this.dataModel.ReleaseReaderLock());
            Assert.Throws<InvalidOperationException>(() => this.dataModel.ReleaseWriterLock());
            Assert.Throws<InvalidOperationException>(() => this.dataModel.ReleaseLock());

            // Without a transaction, none of the status functions should ever return a 'true'.
            Assert.That(this.dataModel.IsLockHeld, Is.False);
            Assert.That(this.dataModel.IsReaderLockHeld, Is.False);
            Assert.That(this.dataModel.IsWriterLockHeld, Is.False);
        }

        /// <summary>
        /// Tests the ability to pick up a writer lock when multiple reader locks have been released.
        /// </summary>
        [Test]
        public void WriterReleaseLock()
        {
            // This is the number of simultaneous readers that will hold a resource.
            int readers = 5;

            // Reset the signals.  Readers can all share the lock, so we're going to wait for all of them to acquire a lock before signalling the
            // writer to acquire a lock.
            this.task1SetupComplete.Reset(readers);
            this.testCompleted.Reset();

            // The transaction that sets up the test.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Create the country
                this.dataModel.CreateCountry("C2", MultithreadingTests.country2Id, null, "WriterRelease Country");
                testTransaction.Commit();
            }

            // Create and start the test threads.  We're going to spawn several readers and a single writer that will wait for all the locks to be
            // released.  The test is complete when the writer has acquired the lock.
            List<Task> tasks = new List<Task>();
            try
            {
                for (int count = 0; count < readers; count++)
                {
                    tasks.Add(Task.Factory.StartNew(this.WriterReleaseTask1, TaskCreationOptions.LongRunning));
                }

                tasks.Add(Task.Factory.StartNew(this.WriterReleaseTask2, TaskCreationOptions.LongRunning));
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count != 0)
                {
                    throw aggregateException.InnerExceptions[0];
                }
            }

            // Make sure all the locks have been release.
            Assert.That(this.dataModel.CreateLockReport(), Is.False, Resources.ObjectsStillLocked);
        }

        /// <summary>
        /// Creates a writer lock that isn't released.
        /// </summary>
        private void DeadlockTask1()
        {
            // The second transaction will attempt to read the record.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Set the ambient transaction.
                VolatileTransaction volatileTransaction = this.dataModel.Transaction;

                // Create a lock on the country row that was created in the setup.
                this.dataModel.CountryKey.AcquireReaderLock();
                volatileTransaction.AddFinally(this.dataModel.CountryKey.ReleaseReaderLock);
                CountryRow countryRow = this.dataModel.CountryKey.Find(country1Id);
                countryRow.AcquireWriterLock();
                volatileTransaction.AddFinally(countryRow.ReleaseWriterLock);

                // Signal that the multi-threading test is ready and then wait for the second task to complete.
                this.task1SetupComplete.Signal();
                this.testCompleted.WaitOne();

                // The other transaction should be aborted.  This is a good transaction and can be committed.
                testTransaction.Commit();
            }
        }

        /// <summary>
        /// Waits for a reader lock that is never granted.
        /// </summary>
        private void DeadlockTask2()
        {
            // Wait for the first task to set-up the scenario.
            this.task1SetupComplete.Wait();

            try
            {
                // The second transaction will attempt to read the record but it will timeout after ten seconds.
                using (TestTransaction testTransaction = new TestTransaction(MultithreadingTests.timeLimit))
                {
                    // Set the ambient transaction.
                    VolatileTransaction volatileTransaction = this.dataModel.Transaction;

                    // Attempt to read the country row that has a write lock on it.  The transaction will be rolled back and the thread will be
                    // aborted because the first thread is going to take forever to release the lock.
                    this.dataModel.CountryKey.AcquireReaderLock();
                    volatileTransaction.AddFinally(this.dataModel.CountryKey.ReleaseReaderLock);
                    CountryRow countryRow = this.dataModel.CountryKey.Find(country1Id);
                    countryRow.AcquireReaderLock();
                }
            }
            finally
            {
                this.testCompleted.Set();
            }
        }

        /// <summary>
        /// Creates a writer lock that isn't released.
        /// </summary>
        private void ReaderReleaseTask1()
        {
            // The second transaction will attempt to read the record.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the row, but make sure the table isn't held
                CountryRow countryRow = null;
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    countryRow = this.dataModel.CountryKey.Find(country3Id);
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }

                // This will wait until all writers and one reader have found the row that they're going to lock.
                this.task1SetupComplete.Signal();
                this.task2SetupComplete.WaitOne();

                // Create a lock on the country row that was created in the setup.
                countryRow.AddWriterLock();

                // Allow the reader thread to attempt to acquire the lock which should result in the thread waiting until this transaction releases
                // the writer lock.
                Thread.Sleep(1);

                // This is a good transaction and can be committed.
                testTransaction.Commit();
            }
        }

        /// <summary>
        /// Waits for a reader lock that is never granted.
        /// </summary>
        private void ReaderReleaseTask2()
        {
            // The second transaction will attempt to read the record but it will timeout after ten seconds.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Attempt to read the country row that has a write lock on it.  The transaction will be rolled back and the thread will be
                // aborted because the first thread is going to take forever to release the lock.
                CountryRow countryRow = null;
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    countryRow = this.dataModel.CountryKey.Find(country3Id);
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }

                // This will wait until all writers and one reader have found the row that they're going to lock.
                this.task1SetupComplete.Wait();
                this.task2SetupComplete.Set();

                // Give the writers threads a chance to acquire the writer lock.
                Thread.Sleep(0);

                // This is the main purpose of the thread: acquire a reader lock when several writers already have it.
                countryRow.AddReaderLock();

                // This is a good transaction and can be committed.
                testTransaction.Commit();
            }
        }

        /// <summary>
        /// Creates a writer lock that isn't released.
        /// </summary>
        private void WriterReleaseTask1()
        {
            // The second transaction will attempt to read the record.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the shared country row.
                CountryRow countryRow = null;
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    countryRow = this.dataModel.CountryKey.Find(country2Id);
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }

                // Signal the writing transaction that the setup is complete.
                this.task1SetupComplete.Signal();
                this.task2SetupComplete.WaitOne();

                // Lock the row for reading.
                countryRow.AddReaderLock();

                // Hold the reader lock while the writer attempts to get it.
                Thread.Sleep(1);

                // This is a good transaction and can be committed.
                testTransaction.Commit();
            }
        }

        /// <summary>
        /// Waits for a reader lock that is never granted.
        /// </summary>
        private void WriterReleaseTask2()
        {
            // The second transaction will attempt to read the record but it will timeout after ten seconds.
            using (TestTransaction testTransaction = new TestTransaction())
            {
                // Find the shared country row.
                CountryRow countryRow = null;
                try
                {
                    this.dataModel.CountryKey.AcquireReaderLock();
                    countryRow = this.dataModel.CountryKey.Find(country2Id);
                }
                finally
                {
                    this.dataModel.CountryKey.ReleaseReaderLock();
                }

                // Wait for the reading tasks to all acquire a lock.  Then signal that we're about the acquire a writer lock.
                this.task1SetupComplete.Wait();
                this.task2SetupComplete.Set();

                // This is the main purpose of the thread: acquire a writer lock when several readers already have it.
                countryRow.AddWriterLock();
            }
        }
    }
}