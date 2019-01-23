// <copyright file="TestTransaction.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.DataModelTests
{
    using System;
    using System.Transactions;

    /// <summary>
    /// A mock transaction for the test environment.
    /// </summary>
    public sealed class TestTransaction : IDisposable
    {
        /// <summary>
        /// The committable transaction.
        /// </summary>
        private CommittableTransaction committableTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTransaction"/> class.
        /// </summary>
        public TestTransaction()
        {
            // Initialize the environment with an ambient transaction having a default timeout.
            Transaction.Current = this.committableTransaction = new CommittableTransaction();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTransaction"/> class.
        /// </summary>
        /// <param name="timeSpan">The timeout period for the transaction.</param>
        public TestTransaction(TimeSpan timeSpan)
        {
            // Initialize the environment with an ambient transaction having an explicit timeout.
            Transaction.Current = this.committableTransaction = new CommittableTransaction(timeSpan);
        }

        /// <summary>
        /// Attempts to commit the transaction.
        /// </summary>
        public void Commit()
        {
            // Commit the transaction.
            this.committableTransaction.Commit();
        }

        /// <summary>
        /// Releases the resources that are held by the object.
        /// </summary>
        public void Dispose()
        {
            // If the transaction hasn't been committed by the time it is disposed, then roll it back.
            if (this.committableTransaction.TransactionInformation.Status != TransactionStatus.Committed)
            {
                this.Rollback();
            }

            // Clean up the environment after the transaction is done.
            this.committableTransaction.Dispose();
            GC.SuppressFinalize(this);
            Transaction.Current = null;
        }

        /// <summary>
        /// Rolls back (aborts) the transaction.
        /// </summary>
        public void Rollback()
        {
            this.committableTransaction.Rollback();
        }
    }
}