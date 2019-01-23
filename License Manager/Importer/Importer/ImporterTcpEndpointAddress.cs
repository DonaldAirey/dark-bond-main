﻿// <copyright file="ImporterTcpEndpointAddress.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ImportService
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// Creates the data service endpoint address.
    /// </summary>
    public class ImporterTcpEndpointAddress : EndpointAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImporterTcpEndpointAddress"/> class.
        /// </summary>
        public ImporterTcpEndpointAddress()
            : base(ImporterTcpEndpointAddress.EndpointUri, ImporterTcpEndpointAddress.DnsIdentity)
        {
        }

        /// <summary>
        /// Gets the DNS identity for the service endpoint.
        /// </summary>
        private static EndpointIdentity DnsIdentity
        {
            get
            {
                // The WCF package for tcp.net has a bug in it that makes it incompatible with certificates with multiple Subject Alternate Names
                // (SAN).  The HTTP handlers are smart enought to scan the list and match any of the SANs, but the TCP simply matches the last item
                // in the list.  Note that if the certificate should change, and it comes from GoDaddy, you'll need to replace this value with
                // whatever is the last name in the SAN list.
                return new DnsEndpointIdentity("staging.darkbond.com");
            }
        }

        /// <summary>
        /// Gets the endpoint URI.
        /// </summary>
        private static Uri EndpointUri
        {
            get
            {
#if PRODUCTION
                return new Uri("net.tcp://production.darkbond.com/license_manager/import_service");
#elif STAGING
                return new Uri("net.tcp://staging.darkbond.com/license_manager/import_service");
#else
                return new Uri("net.tcp://development.darkbond.com/license_manager/import_service");
#endif
            }
        }
    }
}