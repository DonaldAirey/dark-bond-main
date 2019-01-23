// <copyright file="DataServiceTcpBinding.cs" company="Dark Bond, Inc.">
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
    public class DataServiceTcpBinding : NetTcpBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceTcpBinding"/> class.
        /// </summary>
        public DataServiceTcpBinding()
        {
            this.Security.Mode = SecurityMode.Transport;
            this.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            this.MaxReceivedMessageSize = int.MaxValue;
            this.SendTimeout = TimeSpan.FromMilliseconds(300000);
        }
    }
}