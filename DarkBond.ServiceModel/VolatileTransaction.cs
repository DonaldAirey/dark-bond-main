// <copyright file="VolatileTransaction.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Transactions;

    /// <summary>
    /// A transaction to add or reject a group of changes to a volatile data store.
    /// </summary>
    public class VolatileTransaction : IEnlistmentNotification
    {
        /// <summary>
        /// A collection of actions that are executed to commit a transaction.
        /// </summary>
        private List<Action> commitActions = new List<Action>();

        /// <summary>
        /// A collection of actions that clean up resources when a transaction is completed or rolled back.
        /// </summary>
        private List<Action> finalActions = new List<Action>();

        /// <summary>
        /// A collection of actions that are executed to roll back a transaction.
        /// </summary>
        private List<Action> rollbackActions = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VolatileTransaction"/> class.
        /// </summary>
        /// <param name="transaction">The .NET transaction.</param>
        public VolatileTransaction(Transaction transaction)
        {
            // Validate the 'transaction' parameter.
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            // This enables this object to participate in the larger transaction.
            transaction.EnlistVolatile(this, EnlistmentOptions.None);

            // We need to know the thread that has to be killed in the even that this transaction goes south.
            this.Thread = Thread.CurrentThread;
            this.Transaction = Transaction.Current;
        }

        /// <summary>
        /// Gets the thread on which the transaction executes.
        /// </summary>
        public Thread Thread { get; private set; }

        /// <summary>
        /// Gets the original transaction.
        /// </summary>
        public Transaction Transaction { get; private set; }

        /// <summary>
        /// Adds an action to be invoked when the transaction is finished.
        /// </summary>
        /// <param name="action">The action taken after the transaction is finalized.</param>
        public void AddFinally(Action action)
        {
            // Add the action to the list that will be committed as a unit.
            this.finalActions.Add(action);
        }

        /// <summary>
        /// Adds a pair of actions to the transaction that will commit or rollback a unit of work.
        /// </summary>
        /// <param name="commitAction">The action taken to commit the transaction.</param>
        /// <param name="rollbackAction">The action taken to roll back the transaction.</param>
        public void AddActions(Action commitAction, Action rollbackAction)
        {
            // Add the actions to the lists that will be committed or rolled back as a unit.
            this.commitActions.Add(commitAction);
            this.rollbackActions.Add(rollbackAction);
        }

        /// <summary>
        /// Adds a row lock to the list of locks that must be released at the end of a transaction.
        /// </summary>
        /// <param name="enlistment">
        /// Facilitates communication between an enlisted transaction participant and the transaction manager during the final phase of the
        /// transaction.
        /// </param>
        public void Commit(Enlistment enlistment)
        {
            if (enlistment == null)
            {
                throw new ArgumentNullException("enlistment");
            }

            try
            {
                // Provide a transaction context for the commit operations.
                Transaction.Current = this.Transaction;

                // Execute all of the commit actions.
                for (int actionIndex = 0; actionIndex < this.commitActions.Count; actionIndex++)
                {
                    this.commitActions[actionIndex]();
                }

                // Execute all of the final actions.
                for (int actionIndex = 0; actionIndex < this.finalActions.Count; actionIndex++)
                {
                    this.finalActions[actionIndex]();
                }
            }
            finally
            {
                Transaction.Current = null;
            }

            // Notify the transaction manager that we're finished.
            enlistment.Done();
        }

        /// <summary>
        /// Adds a row lock to the list of locks that must be released at the end of a transaction.
        /// </summary>
        /// <param name="enlistment">
        /// Facilitates communication between an enlisted transaction participant and the transaction manager during the final phase of the
        /// transaction.
        /// </param>
        public void InDoubt(Enlistment enlistment)
        {
            if (enlistment == null)
            {
                throw new ArgumentNullException("enlistment");
            }

            // Notify the transaction manager that we're finished.
            enlistment.Done();
        }

        /// <summary>
        /// Indicates that the transaction can be committed.
        /// </summary>
        /// <param name="preparingEnlistment">
        /// Facilitates communication between an enlisted transaction participant and the transaction manager during the final phase of the
        /// transaction.
        /// </param>
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            if (preparingEnlistment == null)
            {
                throw new ArgumentNullException("preparingEnlistment");
            }

            // Notify the transaction manager that we're finished.
            preparingEnlistment.Prepared();
        }

        /// <summary>
        /// Rollback (undo) any of the changes made during the transaction.
        /// </summary>
        /// <param name="enlistment">
        /// Facilitates communication between an enlisted transaction participant and the transaction manager during the final phase of the
        /// transaction.
        /// </param>
        public void Rollback(Enlistment enlistment)
        {
            if (enlistment == null)
            {
                throw new ArgumentNullException("enlistment");
            }

            try
            {
                // Provide a transaction context for the rollback operations.
                Transaction.Current = this.Transaction;

                // Execute all of the rollback actions.
                for (int actionIndex = this.commitActions.Count - 1; actionIndex >= 0; actionIndex--)
                {
                    this.rollbackActions[actionIndex]();
                }

                // Execute all of the final actions.
                for (int actionIndex = 0; actionIndex < this.finalActions.Count; actionIndex++)
                {
                    this.finalActions[actionIndex]();
                }
            }
            finally
            {
                Transaction.Current = null;
            }

            // Notify the transaction manager that we're finished.
            enlistment.Done();
        }
    }
}