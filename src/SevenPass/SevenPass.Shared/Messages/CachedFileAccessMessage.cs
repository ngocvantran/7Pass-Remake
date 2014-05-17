using System;

namespace SevenPass.Messages
{
    public sealed class CachedFileAccessMessage
    {
        /// <summary>
        /// Gets or sets the database filename.
        /// </summary>
        public string Name { get; set; }
    }
}