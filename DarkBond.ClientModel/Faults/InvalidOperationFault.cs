// <copyright file="InvalidOperationFault.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ClientModel
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A general fault indication.
    /// </summary>
    [DataContract]
    public class InvalidOperationFault
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOperationFault"/> class.
        /// </summary>
        /// <param name="message">The constraint violation message.</param>
        public InvalidOperationFault(string message)
        {
            // Initialize the object
            this.Message = message;
        }

        /// <summary>
        /// Gets the message of the fault.
        /// </summary>
        [DataMember]
        public string Message
        {
            get;
            private set;
        }
    }
}