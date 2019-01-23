// <copyright file="CommunicationExceptionHandler.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DarkBond;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.Strings;
    using Windows.UI.Popups;

    /// <summary>
    /// Handles the communication exceptions.
    /// </summary>
    public class CommunicationExceptionHandler : ICommunicationExceptionHandler
    {
        /// <summary>
        /// This dictionary is used to associate the kind of communication exception with a specific handler.
        /// </summary>
        private static Dictionary<Type, Func<Exception, string, bool>> faultDictionary = new Dictionary<Type, Func<Exception, string, bool>>
        {
            { typeof(FaultException), CommunicationExceptionHandler.HandleGenericFault },
            { typeof(EndpointNotFoundException), CommunicationExceptionHandler.HandleEndpointNotFoundException },
            { typeof(CommunicationException), CommunicationExceptionHandler.HandleCommunicationException },
            { typeof(CommunicationObjectFaultedException), CommunicationExceptionHandler.HandleFaultedObjectException },
            { typeof(ServerTooBusyException), CommunicationExceptionHandler.HandleEndpointNotFoundException },
            { typeof(FaultException<ArgumentFault>), CommunicationExceptionHandler.HandleArgumentFault },
            { typeof(FaultException<OptimisticConcurrencyFault>), CommunicationExceptionHandler.HandleOptimisticConcurrencyFault },
            { typeof(FaultException<ConstraintFault>), CommunicationExceptionHandler.HandleConstraintFault },
            { typeof(FaultException<RecordNotFoundFault>), CommunicationExceptionHandler.HandleRecordNotFoundFault }
        };

        /// <summary>
        /// Indicates that an error messages is currently being displayed.
        /// </summary>
        private static bool isShowingMessage = false;

        /// <summary>
        /// A queue of error message dialogs to be displayed in the foreground.
        /// </summary>
        private static Queue<Message> messages = new Queue<Message>();

        /// <summary>
        /// The synchronization context for running tasks in the foreground.
        /// </summary>
        private static SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        /// <summary>
        /// Handler for communication exceptions.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        public bool HandleException(Exception exception, string operation)
        {
            // Validate the exception argument
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            // If a specific handler is provided for the exception, the allow that handler to deal with the exception.
            Func<Exception, string, bool> handler;
            if (CommunicationExceptionHandler.faultDictionary.TryGetValue(exception.GetType(), out handler))
            {
                return handler(exception, operation);
            }

            // Otherwise we will fail without notifying the user.  This is a bad thing in general and all possible exceptions should be handled
            // above.
            return false;
        }

        /// <summary>
        /// Handles the <see cref="ArgumentFault"/> fault.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleArgumentFault(Exception exception, string operation)
        {
            // Construct the error message dialog.  Note that all messages prominently display the operation that failed.
            CommunicationExceptionHandler.DisplayMessage(Errors.ArgumentFault, Resources.ApplicationName);

            // This indicates that the operation should not be retried.
            return false;
        }

        /// <summary>
        /// Handles the <see cref="ConstraintFault"/> fault.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleConstraintFault(Exception exception, string operation)
        {
            // Recast to the original fault.
            FaultException<ConstraintFault> faultException = exception as FaultException<ConstraintFault>;

            // Construct the error message dialog.  Note that all messages prominently display the operation that failed.
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Errors.ConstraintFault,
                faultException.Detail.Operation,
                faultException.Detail.Constraint);
            CommunicationExceptionHandler.DisplayMessage(message, Resources.ApplicationName);

            // This indicates that the operation should not be retried.
            return false;
        }

        /// <summary>
        /// Handles the <see cref="System.ServiceModel.EndpointNotFoundException"/> exception.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleEndpointNotFoundException(Exception exception, string operation)
        {
            // This indicates that the operation should be retried.  The endpoint may come back on line.
            return true;
        }

        /// <summary>
        /// Handles the <see cref="System.ServiceModel.CommunicationException"/> exception.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleCommunicationException(Exception exception, string operation)
        {
            // This indicates that the operation should be retried.  The endpoint may come back on line.
            return true;
        }

        /// <summary>
        /// Handles the <see cref="CommunicationObjectFaultedException"/> exception.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleFaultedObjectException(Exception exception, string operation)
        {
            // This indicates that the operation should be retried.  When a service isn't available, faulted objects will stack up.  The issue can be
            // resolved when the first operation that failed is allowed to go through, then the communication object will be cleared and this
            // operation can be tried again.
            return true;
        }

        /// <summary>
        /// Handles the generic fault.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleGenericFault(Exception exception, string operation)
        {
            // Recast to the original fault.
            FaultException faultException = exception as FaultException;

            // Construct the error message dialog.  Note that all messages prominently display the operation that failed.
            string message = string.Format(CultureInfo.InvariantCulture, faultException.Message);
            CommunicationExceptionHandler.DisplayMessage(message, Resources.ApplicationName);

            // This indicates that the operation should not be retried.
            return false;
        }

        /// <summary>
        /// Handles the <see cref="OptimisticConcurrencyFault"/> fault.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleOptimisticConcurrencyFault(Exception exception, string operation)
        {
            // Construct the error message dialog.  Note that all messages prominently display the operation that failed.
            CommunicationExceptionHandler.DisplayMessage(Errors.OptimisticConcurrencyFault, Resources.ApplicationName);

            // This indicates that the operation should not be retried.
            return false;
        }

        /// <summary>
        /// Handles the <see cref="RecordNotFoundFault"/> fault.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        private static bool HandleRecordNotFoundFault(Exception exception, string operation)
        {
            // Recast to the original fault.
            FaultException<RecordNotFoundFault> faultException = exception as FaultException<RecordNotFoundFault>;

            // This will build a string version of the elements of the key that failed.
            StringBuilder elementText = new StringBuilder(faultException.Detail.KeyElements[0].ToString());
            for (int index = 1; index < faultException.Detail.KeyElements.Count; index++)
            {
                elementText.Append(string.Format(CultureInfo.InvariantCulture, ", {0}", faultException.Detail.KeyElements[index]));
            }

            // Construct the error message dialog.  Note that all messages prominently display the operation that failed.
            string message = string.Format(
                CultureInfo.InvariantCulture,
                Errors.RecordNotFoundFault,
                faultException.Detail.TableName,
                elementText);
            CommunicationExceptionHandler.DisplayMessage(message, Resources.ApplicationName);

            // This indicates that the operation should not be retried.
            return false;
        }

        /// <summary>
        /// Displays an error message and waits for the user to dismiss it.
        /// </summary>
        /// <param name="content">The message to be displayed.</param>
        /// <param name="title">The title of the message box.</param>
        private static void DisplayMessage(string content, string title)
        {
            // This will place the message dialog in a queue and prime the pump that will display the dialogs sequentially in the foreground.
            CommunicationExceptionHandler.messages.Enqueue(new Message { Content = content, Title = title });
            CommunicationExceptionHandler.synchronizationContext.Post(CommunicationExceptionHandler.ServiceMessageQueue, null);
        }

        /// <summary>
        /// Handles the message dialogs in the queue.
        /// </summary>
        /// <param name="state">The (unused) thread state.</param>
        private static async void ServiceMessageQueue(object state)
        {
            // One or many error messages may accrue while handling the operations asynchronously.  Those message dialogs are placed in a queue.  As
            // the user reads them and dismisses them, they are removed from the queue and the next item will be displayed until the queue is empty.
            if (!isShowingMessage)
            {
                // This will display the dialog in the foreground thread and wait for the user to dismiss it.
                isShowingMessage = true;
                Message message = CommunicationExceptionHandler.messages.Dequeue();
                MessageDialog messageDialog = new MessageDialog(message.Content, message.Title);
                await messageDialog.ShowAsync();
                isShowingMessage = false;

                // If the queue isn't empty, then ask the foreground to process another dialog.  This continues until the queue is empty.
                if (CommunicationExceptionHandler.messages.Count != 0)
                {
                    CommunicationExceptionHandler.synchronizationContext.Post(CommunicationExceptionHandler.ServiceMessageQueue, null);
                }
            }
        }

        /// <summary>
        /// Used to queue up messages.
        /// </summary>
        private struct Message
        {
            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// Gets or sets the caption.
            /// </summary>
            public string Title { get; set; }
        }
    }
}