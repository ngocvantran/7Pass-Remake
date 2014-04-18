using System;

namespace SevenPass.Services.Cache
{
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Gets the cached database.
        /// </summary>
        public CachedDatabase Database { get; private set; }

        /// <summary>
        /// Stores the specified database in cache.
        /// </summary>
        /// <param name="database">The dataabase to be cached.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="database"/> cannot be <c>null</c>.
        /// </exception>
        public void Cache(CachedDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            Database = database;
        }
    }
}