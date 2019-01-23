// <copyright file="ClientCredentialType.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The different types of client credentials supported by this client.
    /// </summary>
    public enum ClientCredentialType
    {
        /// <summary>
        /// Anonymous authentication.
        /// </summary>
        Anonymous,

        /// <summary>
        /// Certificate authentication.
        /// </summary>
        Certificate,

        /// <summary>
        /// Windows Integrated authentication.
        /// </summary>
        Windows
    }
}