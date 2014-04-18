using System;

namespace SevenPass.Services.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// Gets the cached database.
        /// </summary>
        CachedDatabase Database { get; }

        /// <summary>
        /// Stores the specified database in cache.
        /// </summary>
        /// <param name="database">The dataabase to be cached.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="database"/> cannot be <c>null</c>.
        /// </exception>
        void Cache(CachedDatabase database);
    }
}