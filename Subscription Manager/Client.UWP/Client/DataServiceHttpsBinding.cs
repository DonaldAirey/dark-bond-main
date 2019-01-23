// <copyright file="DataServiceHttpsBinding.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// A channel binding that specifies transport mode security.
    /// </summary>
    public class DataServiceHttpsBinding : NetHttpBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHttpsBinding"/> class.
        /// </summary>
        public DataServiceHttpsBinding()
        {
            this.Security.Mode = BasicHttpSecurityMode.Transport;
            this.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            this.MaxReceivedMessageSize = int.MaxValue;
            this.SendTimeout = TimeSpan.FromMilliseconds(10000);
        }
    }
}