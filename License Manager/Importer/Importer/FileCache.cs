// <copyright file="FileCache.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Implements a simple cache of security tokens.
    /// </summary>
    public class FileCache : TokenCache
    {
        /// <summary>
        /// The name of the security token cache file.
        /// </summary>
        private const string SecurityTokenCacheFileName = "TokenCache.dat";

        /// <summary>
        /// A lock on the file.
        /// </summary>
        private static readonly object FileLock = new object();

        /// <summary>
        /// The path to the file where the tokens are kept.
        /// </summary>
        private string securityTokenCachePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCache"/> class.
        /// </summary>
        public FileCache()
        {
            // Initialize the object.
            this.AfterAccess = this.AfterAccessNotification;
            this.BeforeAccess = this.BeforeAccessNotification;

            // The security token cache file lives in the application directory.
            this.securityTokenCachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                FileCache.SecurityTokenCacheFileName);
        }

        /// <summary>
        /// Empties the persistent store.
        /// </summary>
        /// <param name="clientId">The unique client identifier.</param>
        public override void Clear(string clientId)
        {
            base.Clear(clientId);
            File.Delete(this.securityTokenCachePath);
        }

        /// <summary>
        /// Triggered right before MSAL needs to access the cache.
        /// </summary>
        /// <param name="args">The notification arguments.</param>
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            // Reload the cache from the persistent store in case it changed since the last access.
            lock (FileCache.FileLock)
            {
                if (File.Exists(this.securityTokenCachePath))
                {
                    this.Deserialize(ProtectedData.Unprotect(File.ReadAllBytes(this.securityTokenCachePath), null, DataProtectionScope.CurrentUser));
                }
            }
        }

        /// <summary>
        /// Triggered right after MSAL accessed the cache.
        /// </summary>
        /// <param name="args">The notification arguments.</param>
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.HasStateChanged)
            {
                lock (FileCache.FileLock)
                {
                    // reflect changes in the persistent store
                    File.WriteAllBytes(this.securityTokenCachePath, ProtectedData.Protect(this.Serialize(), null, DataProtectionScope.CurrentUser));

                    // Once the write operation took place, restore the HasStateChanged bit to false.
                    this.HasStateChanged = false;
                }
            }
        }
    }
}