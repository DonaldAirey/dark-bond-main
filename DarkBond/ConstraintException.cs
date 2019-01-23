// <copyright file="ConstraintException.cs" company="Dark Bond, Inc.">
//     Copyright © 2015 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    using System;

    /// <summary>
    /// Represents errors that occur when locking records for a transaction.
    /// </summary>
    public class ConstraintException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        public ConstraintException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConstraintException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        /// <param name="message">The message that gives more information about the Win32 error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
        /// exception is specified.</param>
        public ConstraintException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}