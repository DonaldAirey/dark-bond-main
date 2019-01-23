// <copyright file="TokeCacheHelper.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost
{
    using System.IO;
    using System.Reflection;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Supports reading and writing the user's security tokens to a persistent store.
    /// </summary>
    static class TokenCacheHelper
    {
        /// <summary>
        /// A lock to serialize access to the file.
        /// </summary>
        private static readonly object fileLock = new object();

        /// <summary>
        /// The full path of the persistent store.
        /// </summary>
        private static string filePath = Assembly.GetExecutingAssembly().Location + "msalcache.txt";

        /// <summary>
        /// The token cache.
        /// </summary>
        static TokenCache tokenCache;

        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public static TokenCache GetTokenCache()
        {
            if (TokenCacheHelper.tokenCache == null)
            {
                TokenCacheHelper.tokenCache = new TokenCache();
                TokenCacheHelper.tokenCache.SetBeforeAccess(BeforeAccessNotification);
                TokenCacheHelper.tokenCache.SetAfterAccess(AfterAccessNotification);
            }

            return TokenCacheHelper.tokenCache;
        }

        /// <summary>
        /// Clears the persistent store for the token cache.
        /// </summary>
        public static void Clear()
        {
            lock (TokenCacheHelper.fileLock)
            {
                File.Delete(TokenCacheHelper.filePath);
            }
        }

        public static void BeforeAccessNotification(TokenCacheNotificationArgs tokenCacheNotificationArgs)
        {
            lock (TokenCacheHelper.fileLock)
            {
                if (File.Exists(TokenCacheHelper.filePath))
                {
                    tokenCacheNotificationArgs.TokenCache.Deserialize(File.ReadAllBytes(TokenCacheHelper.filePath));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenCacheNotificationArgs"></param>
        public static void AfterAccessNotification(TokenCacheNotificationArgs tokenCacheNotificationArgs)
        {
            // if the access operation resulted in a cache update
            if (tokenCacheNotificationArgs.TokenCache.HasStateChanged)
            {
                lock (TokenCacheHelper.fileLock)
                {
                    // Write the changes to the persistent store.
                    File.WriteAllBytes(TokenCacheHelper.filePath, tokenCacheNotificationArgs.TokenCache.Serialize());
                    tokenCacheNotificationArgs.TokenCache.HasStateChanged = false;
                }
            }
        }
    }
}