﻿// <copyright file="ArgumentNullFault.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ClientModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A fault that occurs when the arguments don't meet the expected values.
    /// </summary>
    [DataContract]
    public class ArgumentNullFault
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullFault"/> class.
        /// </summary>
        /// <param name="message">The message of the fault.</param>
        public ArgumentNullFault(string message)
        {
            // Initialize the object.
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