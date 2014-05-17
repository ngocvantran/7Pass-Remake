using System;
using System.Xml.Linq;

namespace SevenPass.Services.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// Gets the cached database.
        /// </summary>
        CachedDatabase Database { get; }

        /// <summary>
        /// Gets the root group.
        /// </summary>
        XElement Root { get; }

        /// <summary>
        /// Stores the specified database in cache.
        /// </summary>
        /// <param name="database">The dataabase to be cached.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="database"/> cannot be <c>null</c>.
        /// </exception>
        void Cache(CachedDatabase database);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the Entry element with the specified UUID.
        /// </summary>
        /// <param name="uuid">The entry's UUID.</param>
        /// <returns>The specified entry, or <c>null</c> if not found.</returns>
        XElement GetEntry(string uuid);

        /// <summary>
        /// Gets the Group element with the specified UUID.
        /// </summary>
        /// <param name="uuid">The group's UUID.</param>
        /// <returns>The specified group, or <c>null</c> if not found.</returns>
        XElement GetGroup(string uuid);
    }
}