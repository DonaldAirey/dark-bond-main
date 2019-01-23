// <copyright file="ClientInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Description;

    /// <summary>
    /// Information about each endpoint that subscribes to data from the simulator.
    /// </summary>
    public class ClientInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the endpoint configuration.
        /// </summary>
        private string endpointField;

        /// <summary>
        /// The user's password.
        /// </summary>
        private string passwordField;

        /// <summary>
        /// The subject name.
        /// </summary>
        private X500DistinguishedName subjectNameField;

        /// <summary>
        /// The thumbprint.
        /// </summary>
        private string thumbprintField;

        /// <summary>
        /// The user name.
        /// </summary>
        private string userNameField;

        /// <summary>
        /// Invoked when the property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the name of the endpoint configuration.
        /// </summary>
        public string Endpoint
        {
            get
            {
                return this.endpointField;
            }

            set
            {
                if (this.endpointField != value)
                {
                    this.endpointField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Endpoint"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.passwordField;
            }

            set
            {
                if (this.passwordField != value)
                {
                    this.passwordField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Password"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the SubjectName of the Certificate.
        /// </summary>
        public X500DistinguishedName SubjectName
        {
            get
            {
                return this.subjectNameField;
            }

            set
            {
                if (this.subjectNameField != value)
                {
                    this.subjectNameField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SubjectName"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Thumbprint of the Certificate.
        /// </summary>
        public string Thumbprint
        {
            get
            {
                return this.thumbprintField;
            }

            set
            {
                if (this.thumbprintField != value)
                {
                    this.thumbprintField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Thumbprint"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userNameField;
            }

            set
            {
                if (this.userNameField != value)
                {
                    this.userNameField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UserName"));
                }
            }
        }

        /// <summary>
        /// Initializes a web service client with the tenant's credentials.
        /// </summary>
        /// <param name="clientCredentials">The web service to be initialized.</param>
        public void InitializeCredentials(ClientCredentials clientCredentials)
        {
            // Validate the clientCredentials argument
            if (clientCredentials == null)
            {
                throw new ArgumentNullException("clientCredentials");
            }

            // When the command line provides a user name, we poke user name credentials into the channel.
            if (!string.IsNullOrEmpty(this.UserName))
            {
                clientCredentials.UserName.UserName = this.UserName;
                clientCredentials.UserName.Password = this.Password;
            }

            // When the command line provides a thumbprint, we poke a certificate into the channel.
            if (!string.IsNullOrEmpty(this.Thumbprint))
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2 x509Certificate2 = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    this.Thumbprint,
                    true).OfType<X509Certificate2>().FirstOrDefault();
                clientCredentials.ClientCertificate.Certificate = x509Certificate2;
            }

            // When the command line provides a distinguished subject name, we poke the corresponding certificate into the channel.
            if (this.SubjectName != null)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2 x509Certificate2 = store.Certificates.Find(
                    X509FindType.FindBySubjectDistinguishedName,
                    this.SubjectName.Decode(X500DistinguishedNameFlags.Reversed | X500DistinguishedNameFlags.UseCommas),
                    true).OfType<X509Certificate2>().FirstOrDefault();
                clientCredentials.ClientCertificate.Certificate = x509Certificate2;
            }
        }

        /// <summary>
        /// Sends out a notification that a property has changed.
        /// </summary>
        /// <param name="propertyChangedEventArgs">The PropertyChanged event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // Notify anyone listening that the property has changed.
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, propertyChangedEventArgs);
            }
        }
    }
}