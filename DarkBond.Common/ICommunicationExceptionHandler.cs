// <copyright file="ICommunicationExceptionHandler.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Provides a handler for communication exceptions.
    /// </summary>
    public interface ICommunicationExceptionHandler
    {
        /// <summary>
        /// Handler for communication exceptions.
        /// </summary>
        /// <param name="exception">The exception that occurred during the operation.</param>
        /// <param name="operation">The operation where the exception occurred.</param>
        /// <returns>True indicates the operation should not be retried, false indicates it should.</returns>
        bool HandleException(Exception exception, string operation);
    }
}