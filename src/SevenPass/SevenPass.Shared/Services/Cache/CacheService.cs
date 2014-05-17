using System;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Services.Cache
{
    public class CacheService : ICacheService
    {
        private ILookup<string, XElement> _entries;
        private ILookup<string, XElement> _groups;

        /// <summary>
        /// Gets the cached database.
        /// </summary>
        public CachedDatabase Database { get; private set; }

        /// <summary>
        /// Gets the root group.
        /// </summary>
        public XElement Root { get; private set; }

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

            var groups = database.Document
                .Descendants("Group")
                .ToList();

            Root = groups.FirstOrDefault();
            _groups = groups.ToLookup(x =>
                (string)x.Element("UUID"));
            _entries = groups
                .SelectMany(x => x.Elements("Entry"))
                .ToLookup(x => (string)x.Element("UUID"));
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            Root = null;
            _groups = null;
            _entries = null;
            Database = null;
        }

        /// <summary>
        /// Gets the Entry element with the specified UUID.
        /// </summary>
        /// <param name="uuid">The entry's UUID.</param>
        /// <returns>The specified entry, or <c>null</c> if not found.</returns>
        public XElement GetEntry(string uuid)
        {
            return _entries != null
                ? _entries[uuid].FirstOrDefault()
                : null;
        }

        /// <summary>
        /// Gets the Group element with the specified UUID.
        /// </summary>
        /// <param name="uuid">The group's UUID.</param>
        /// <returns>The specified group, or <c>null</c> if not found.</returns>
        public XElement GetGroup(string uuid)
        {
            return _groups != null
                ? _groups[uuid].FirstOrDefault()
                : null;
        }
    }
}