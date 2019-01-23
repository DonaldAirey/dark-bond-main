// <copyright file="ServiceOptions.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Airey</author>
namespace DarkBond.ServiceModel
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Options for connecting to the Web API.
    /// </summary>
    public class ServiceOptions : IOptions<ServiceOptions>
    {
        /// <summary>
        /// Gets or sets the base URL of the service.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets the value
        /// </summary>
        public ServiceOptions Value => this;
    }
}