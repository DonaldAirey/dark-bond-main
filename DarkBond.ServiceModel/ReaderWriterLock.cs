// <copyright file="ReaderWriterLock.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Transactions;

    /// <summary>
    /// Represents a lock that is used to manage access to a resource, allowing multiple transactions for reading or exclusive access for writing.
    /// </summary>
    public class ReaderWriterLock
    {
        /// <summary>
        /// A set of all the readers (transactions) waiting for a read lock.
        /// </summary>
        private HashSet<Transaction> readers = new HashSet<Transaction>();

        /// <summary>
        /// The number of readers waiting for a read lock.
        /// </summary>
        private int readerWaiters;

        /// <summary>
        /// Used to synchronize access to the reader lock.
        /// </summary>
        private object readRoot = new object();

        /// <summary>
        /// Used to synchronize access to the housekeeping fields.
        /// </summary>
        private object syncRoot = new object();

        /// <summary>
        /// The owner of the write lock.
        /// </summary>
        private Transaction writer = null;

        /// <summary>
        /// Used to synchronize access to the writer lock.
        /// </summary>
        private object writeRoot = new object();

        /// <summary>
        /// The number of writers waiting for a write lock.
        /// </summary>
        private int writerWaiters;

        /// <summary>
        /// Gets a value indicating whether the owner of a token holds any locks.
        /// </summary>
        public bool IsLockHeld
        {
            get
            {
                // All locks are acquired and released in the context of an ambient transaction.
                Transaction transaction = Transaction.Current;

                try
                {
                    Monitor.Enter(this.syncRoot);

                    // A lock is held if either a writer or reader transaction is associated with this object.
                    return transaction != null && (this.writer == transaction || this.readers.Contains(transaction));
                }
                finally
                {
                    Monitor.Exit(this.syncRoot);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the owner of a token holds a reader lock.
        /// </summary>
        public bool IsReaderLockHeld
        {
            get
            {
                // All locks are acquired and released in the context of an ambient transaction.
                Transaction transaction = Transaction.Current;

                try
                {
                    Monitor.Enter(this.syncRoot);

                    // A lock is held if a reader transaction is associated with this object.
                    return transaction != null && this.readers.Contains(transaction);
                }
                finally
                {
                    Monitor.Exit(this.syncRoot);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the owner of a token holds the writer lock.
        /// </summary>
        public bool IsWriterLockHeld
        {
            get
            {
                // All locks are acquired and released in the context of an ambient transaction.
                Transaction transaction = Transaction.Current;

                try
                {
                    Monitor.Enter(this.syncRoot);

                    // A writer lock is held if a writer transaction is associated with this object.
                    return transaction != null && this.writer == transaction;
                }
                finally
                {
                    Monitor.Exit(this.syncRoot);
                }
            }
        }

        /// <summary>
        /// Gets the list of readers.
        /// </summary>
        public IReadOnlyCollection<Transaction> Readers
        {
            get
            {
                try
                {
                    Monitor.Enter(this.syncRoot);

                    // The list of reader transactions can be used to diagnose locking problems.
                    return new ReadOnlyCollection<Transaction>(this.readers.ToList());
                }
                finally
                {
                    Monitor.Exit(this.syncRoot);
                }
            }
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        public Transaction Writer
        {
            get
            {
                try
                {
                    Monitor.Enter(this.syncRoot);

                    // The writer transaction can be used to diagnose locking problems.
                    return this.writer;
                }
                finally
                {
                    Monitor.Exit(this.syncRoot);
                }
            }
        }

        /// <summary>
        /// Acquires a reader lock for this resource.
        /// </summary>
        public void AcquireReaderLock()
        {
            // All locks are acquired and released in the context of an ambient transaction.
            Transaction transaction = Transaction.Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Locks can only be acquired in a transaction.");
            }

            try
            {
                // Lock the housekeeping fields while we acquire the lock.
                Monitor.Enter(this.syncRoot);

                // At this point we can join the number of readers waiting to read.
                this.readerWaiters++;

                // If another transaction has exclusive rights to this object, then we need to join a collection of transactions waiting for the read
                // lock to become available.
                if (this.writer != transaction)
                {
                    try
                    {
                        try
                        {
                            // If an exclusive writer lock is owned, then this will wait until another transaction pulses the readers and allows them
                            // to wake up.  Note that while this transaction is waiting to be woken up, we are not going to stop writers from trying
                            // to acquire a lock.
                            Monitor.Enter(this.readRoot);
                            Monitor.Exit(this.syncRoot);
                            while (this.writer != null)
                            {
                                // When this returns, all the readers waiting for this lock will have been awakend and be allowed to own the reader
                                // lock.
                                Monitor.Wait(this.readRoot);
                            }
                        }
                        finally
                        {
                            // At this point we acquired a lock or were terminated when the transaction timed-out.
                            Monitor.Exit(this.readRoot);
                        }
                    }
                    finally
                    {
                        // We need to re-acquire access to the housekeeping fields again so we can conclude the lock operation.
                        Monitor.Enter(this.syncRoot);
                    }

                    // If the transaction didn't time-out while waiting for the reader lock, then we're free to acquire it now (if your transaction
                    // is in this list, you ownn a reader lock on the object).
                    this.readers.Add(transaction);
                }
            }
            finally
            {
                // When we've obtained the reader lock, we can drop the number of transactions waiting for the lock to balance the books.
                --this.readerWaiters;

                // We no longer need to keep the housekeeping fields locked.
                Monitor.Exit(this.syncRoot);
            }
        }

        /// <summary>
        /// Acquires a writer lock for this resource.
        /// </summary>
        public void AcquireWriterLock()
        {
            // All locks are acquired and released in the context of an ambient transaction.
            Transaction transaction = Transaction.Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Locks can only be acquired in a transaction.");
            }

            try
            {
                // Lock the housekeeping fields while we acquire the lock.
                Monitor.Enter(this.syncRoot);

                // This keeps track of the number of transactions waiting for a write lock on this object.
                this.writerWaiters++;

                // If this transaction doesn't already own the write lock, then we will either aquire it or wait until we can acquire it.
                if (this.writer != transaction)
                {
                    // If we are already own a reader lock, then it will be promoted to a writer lock.
                    this.readers.Remove(transaction);

                    // While either another transaction has the write lock or there are readers, we are going to wait here until a transaction issues
                    // a pulses to wake up the writers.
                    while (this.writer != null || this.readers.Count != 0)
                    {
                        try
                        {
                            try
                            {
                                // We will wait here until we are pulsed by another transaction releasing either all the reader locks or the writer
                                // lock.  Note that we don't prevent readers from queuing up while we wait to be woken up.  Alternatively, if the
                                // transaction times-out, then the thread will be aborted and this block will exit through the 'finally' statements.
                                Monitor.Enter(this.writeRoot);
                                Monitor.Exit(this.syncRoot);
                                Monitor.Wait(this.writeRoot);
                            }
                            finally
                            {
                                // At this point we either acquired the lock, or timed out.  In either event, we don't need to lock the writers
                                // fields any longer.
                                Monitor.Exit(this.writeRoot);
                            }
                        }
                        finally
                        {
                            // Re-acquire access to the housekeeping fields again so we can conclude the lock operation.
                            Monitor.Enter(this.syncRoot);
                        }
                    }

                    // This transaction now has exclusive access to this resource.
                    this.writer = transaction;
                }
            }
            finally
            {
                // Decrement the number of transactions waiting on the writer.
                --this.writerWaiters;

                // We no longer need to keep the housekeeping fields locked.
                Monitor.Exit(this.syncRoot);
            }
        }

        /// <summary>
        /// Releases every lock held by this resource.
        /// </summary>
        public void ReleaseLock()
        {
            // All locks are acquired and released in the context of an ambient transaction.
            Transaction transaction = Transaction.Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Locks can only be released in a transaction.");
            }

            try
            {
                // Lock the housekeeping fields while we release the lock.
                Monitor.Enter(this.syncRoot);

                // Remove the reader lock for this transaction.
                this.readers.Remove(transaction);

                // If there are no more readers and there are one or more writers waiting for it, then wake up all the writers and let them fight it
                // out for exclusive ownership of the resource.
                if (this.readers.Count == 0 && this.writerWaiters != 0)
                {
                    try
                    {
                        Monitor.Enter(this.writeRoot);
                        Monitor.Pulse(this.writeRoot);
                    }
                    finally
                    {
                        Monitor.Exit(this.writeRoot);
                    }
                }

                // If a writer lock is held by this transaction we will clear it and first wake up all the readers.  If there are no readers, then
                // we'll try to wake a writer.
                if (this.writer == transaction)
                {
                    // This will clear the writer lock.
                    this.writer = null;

                    // If there are readers queued up at this point, then wake them all.
                    if (this.readerWaiters > 0)
                    {
                        try
                        {
                            Monitor.Enter(this.readRoot);
                            Monitor.PulseAll(this.readRoot);
                        }
                        finally
                        {
                            Monitor.Exit(this.readRoot);
                        }
                    }
                    else
                    {
                        // If there are writers queued up, then wake them all and let them fight it out for the winner of the lock.
                        if (this.writerWaiters > 0)
                        {
                            try
                            {
                                Monitor.Enter(this.writeRoot);
                                Monitor.Pulse(this.writeRoot);
                            }
                            finally
                            {
                                Monitor.Exit(this.writeRoot);
                            }
                        }
                    }
                }
            }
            finally
            {
                // This releases the housekeeping fields for the next thread.
                Monitor.Exit(this.syncRoot);
            }
        }

        /// <summary>
        /// Releases the reader lock on this resource.
        /// </summary>
        public void ReleaseReaderLock()
        {
            // All locks are acquired and released in the context of an ambient transaction.
            Transaction transaction = Transaction.Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Locks can only be released in a transaction.");
            }

            try
            {
                // Lock the housekeeping fields while we release the lock.
                Monitor.Enter(this.syncRoot);

                // If a writer transaction already has a lock on this object, then we don't need to release anything.
                if (this.writer != transaction)
                {
                    // Remove the reader lock for this transaction.
                    this.readers.Remove(transaction);

                    // If there are no more readers and there are one or more writers waiting for it, then wake up all the writers and let them fight
                    // it out for exclusive ownership of the resource.
                    if (this.readers.Count == 0 && this.writerWaiters != 0)
                    {
                        try
                        {
                            Monitor.Enter(this.writeRoot);
                            Monitor.Pulse(this.writeRoot);
                        }
                        finally
                        {
                            Monitor.Exit(this.writeRoot);
                        }
                    }
                }
            }
            finally
            {
                // This releases the housekeeping fields for the next thread.
                Monitor.Exit(this.syncRoot);
            }
        }

        /// <summary>
        /// Releases the writer lock on this resource.
        /// </summary>
        public void ReleaseWriterLock()
        {
            // All locks are acquired and released in the context of an ambient transaction.
            Transaction transaction = Transaction.Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Locks can only be released in a transaction.");
            }

            try
            {
                // Lock the housekeeping fields while we release the lock.
                Monitor.Enter(this.syncRoot);

                // Remove the writer lock for this transaction.
                this.writer = null;

                // Readers get first crack at acquiring a lock that was held by a writer.  This will wake all the readers when we have readers queued
                // up.
                if (this.readerWaiters != 0)
                {
                    try
                    {
                        Monitor.Enter(this.readRoot);
                        Monitor.PulseAll(this.readRoot);
                    }
                    finally
                    {
                        Monitor.Exit(this.readRoot);
                    }
                }
                else
                {
                    // If there are no readers and there are one or more writers waiting for the lock, then wake up all the writers and let them
                    // fight it out for exclusive ownership of the resource.
                    if (this.writerWaiters != 0)
                    {
                        try
                        {
                            Monitor.Enter(this.writeRoot);
                            Monitor.Pulse(this.writeRoot);
                        }
                        finally
                        {
                            Monitor.Exit(this.writeRoot);
                        }
                    }
                }
            }
            finally
            {
                // This releases the housekeeping fields for the next thread.
                Monitor.Exit(this.syncRoot);
            }
        }
    }
}