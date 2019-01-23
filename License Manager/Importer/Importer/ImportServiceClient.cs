// <copyright file="ImportServiceClient.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ImportService
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using DarkBond.ClientModel;

    /// <summary>
    /// The client for the import service.
    /// </summary>
    public partial class ImportServiceClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportServiceClient"/> class.
        /// </summary>
        /// <param name="binding">The endpoint binding.</param>
        /// <param name="endpointAddress">The endpoint address.</param>
        /// <param name="clientSecurityToken">The security token.</param>
        public ImportServiceClient(Binding binding, EndpointAddress endpointAddress, ClientSecurityToken clientSecurityToken)
            : this(binding, endpointAddress)
        {
            this.ChannelFactory.Endpoint.EndpointBehaviors.Remove(typeof(ClientCredentials));
            this.ChannelFactory.Endpoint.EndpointBehaviors.Add(new SecurityTokenEndpointBehavior(clientSecurityToken));
        }
    }
}