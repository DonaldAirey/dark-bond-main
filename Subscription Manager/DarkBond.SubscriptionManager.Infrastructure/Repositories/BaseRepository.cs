// <copyright file="BaseRepository.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Repositories
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using DarkBond.ServiceModel;

    /// <summary>
    /// Mediates the between the view model and the data model for Underwriters.
    /// </summary>
    public class BaseRepository
    {
        /// <summary>
        /// The channel bindings for the data service.
        /// </summary>
        private Binding binding;

        /// <summary>
        /// The client for the data model service.
        /// </summary>
        private DataServiceClient dataServiceClientField;

        /// <summary>
        /// The endpoint address for the data service.
        /// </summary>
        private EndpointAddress endpointAddress;

        /// <summary>
        /// The security token.
        /// </summary>
        private ClientSecurityToken clientSecurityToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        /// <param name="binding">The binding for the data service channel.</param>
        /// <param name="clientSecurityToken">The client credentials.</param>
        /// <param name="communicationExceptionHandler">The communication exception handler.</param>
        /// <param name="endpointAddress">The endpoint address for the data service channel.</param>
        public BaseRepository(
            Binding binding,
            ClientSecurityToken clientSecurityToken,
            ICommunicationExceptionHandler communicationExceptionHandler,
            EndpointAddress endpointAddress)
        {
            // Validate the parameter.
            if (communicationExceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(communicationExceptionHandler));
            }

            // Validate the parameter.
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            // Validate the parameter.
            if (clientSecurityToken == null)
            {
                throw new ArgumentNullException(nameof(clientSecurityToken));
            }

            // Initialize the object.
            this.CommunicationExceptionHandler = communicationExceptionHandler;
            this.binding = binding;
            this.endpointAddress = endpointAddress;
            this.clientSecurityToken = clientSecurityToken;
        }

        /// <summary>
        /// Gets the communication exception handler.
        /// </summary>
        protected ICommunicationExceptionHandler CommunicationExceptionHandler { get; private set; }

        /// <summary>
        /// Gets a communication channel for executing CRUD operations on the server.
        /// </summary>
        protected DataServiceClient DataServiceClient
        {
            get
            {
                // If the client used to service the foreground tasks has faulted, then generate a new one.  This will time-out periodically in the
                // normal course of operation, so the faulted state is expected.  However, when there are a batch of successive operations, this will
                // speed things up by reusing the previous client connection.
                if (this.dataServiceClientField == null || this.dataServiceClientField.State == CommunicationState.Faulted)
                {
                    this.dataServiceClientField = new DataServiceClient(this.binding, this.endpointAddress, this.clientSecurityToken);
                }

                // This is the client used by this repository.
                return this.dataServiceClientField;
            }
        }
    }
}