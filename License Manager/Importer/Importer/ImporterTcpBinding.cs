// <copyright file="ImporterTcpBinding.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ImportService
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// A channel binding that specifies transport mode security.
    /// </summary>
    public class ImporterTcpBinding : NetTcpBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImporterTcpBinding"/> class.
        /// </summary>
        public ImporterTcpBinding()
        {
            this.Security.Mode = SecurityMode.Transport;
            this.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            this.MaxReceivedMessageSize = int.MaxValue;
            this.SendTimeout = TimeSpan.FromMilliseconds(300000);
        }
    }
}