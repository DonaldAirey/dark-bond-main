// <copyright file="FileCache.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using Microsoft.Identity.Client;
    using Windows.Security.Cryptography;
    using Windows.Storage;
    using Windows.Storage.Streams;

    /// <summary>
    /// Implements a simple cache of security tokens.
    /// </summary>
    public class FileCache : TokenCache
    {
        /// <summary>
        /// The path where the saved tokens are kept.
        /// </summary>
        private const string TokenCachePath = "TokenCache.dat";

        /// <summary>
        /// A lock on the file.
        /// </summary>
        private object fileLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCache"/> class.
        /// </summary>
        public FileCache()
        {
            // Initialize the object.
            this.AfterAccess = this.AfterAccessNotification;
            this.BeforeAccess = this.BeforeAccessNotification;
        }

        /// <summary>
        /// Empties the persistent token cache.
        /// </summary>
        /// <param name="clientId">The unique client identifier.</param>
        public override void Clear(string clientId)
        {
            base.Clear(clientId);
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            lock (this.fileLock)
            {
                StorageFile tokenFile = storageFolder.TryGetItemAsync(FileCache.TokenCachePath).GetAwaiter().GetResult() as StorageFile;
                if (tokenFile != null)
                {
                    tokenFile.DeleteAsync().GetAwaiter().GetResult();
                }
            }
        }

        /// <summary>
        /// Triggered right before MSAL needs to access the cache.
        /// </summary>
        /// <param name="tokenCacheNotificationArgs">The notification arguments.</param>
        private void BeforeAccessNotification(TokenCacheNotificationArgs tokenCacheNotificationArgs)
        {
            // Attempt to open the cache of security tokens read them from the encoded data.
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            byte[] array = null;
            lock (this.fileLock)
            {
                StorageFile tokenFile = storageFolder.TryGetItemAsync(FileCache.TokenCachePath).GetAwaiter().GetResult() as StorageFile;
                if (tokenFile != null)
                {
                    IBuffer buffer = FileIO.ReadBufferAsync(tokenFile).GetAwaiter().GetResult();
                    CryptographicBuffer.CopyToByteArray(buffer, out array);
                }
            }

            // This will populate the cache with security tokens.
            this.Deserialize(array);
        }

        /// <summary>
        /// Triggered right after MSAL accessed the cache.
        /// </summary>
        /// <param name="tokenCacheNotificationArgs">The notification arguments.</param>
        private void AfterAccessNotification(TokenCacheNotificationArgs tokenCacheNotificationArgs)
        {
            // Write the security tokens to an encoded file in the local storage.
            if (this.HasStateChanged)
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(this.Serialize());
                lock (this.fileLock)
                {
                    StorageFile tokenFile = storageFolder.CreateFileAsync(FileCache.TokenCachePath, CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
                    FileIO.WriteBufferAsync(tokenFile, buffer).GetAwaiter().GetResult();
                }
            }
        }
    }
}