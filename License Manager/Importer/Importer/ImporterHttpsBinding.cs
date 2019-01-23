﻿// <copyright file="ImporterHttpsBinding.cs" company="Dark Bond, Inc.">
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
    public class ImporterHttpsBinding : NetHttpBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImporterHttpsBinding"/> class.
        /// </summary>
        public ImporterHttpsBinding()
        {
            this.Security.Mode = BasicHttpSecurityMode.Transport;
            this.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            this.MaxReceivedMessageSize = int.MaxValue;
            this.SendTimeout = TimeSpan.FromMilliseconds(10000);
        }
    }
}