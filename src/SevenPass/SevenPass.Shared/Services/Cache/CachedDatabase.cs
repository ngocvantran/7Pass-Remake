using System;

namespace SevenPass.Services.Cache
{
    public class CachedDatabase
    {
        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Name { get; set; }
    }
}