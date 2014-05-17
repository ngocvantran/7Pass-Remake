using System;
using System.Xml.Linq;
using SevenPass.IO.Models;

namespace SevenPass.Services.Cache
{
    public sealed class CachedDatabase
    {
        /// <summary>
        /// Gets or sets the parsed document.
        /// </summary>
        public XDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the database file headers.
        /// </summary>
        public FileHeaders Headers { get; set; }

        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Name { get; set; }
    }
}