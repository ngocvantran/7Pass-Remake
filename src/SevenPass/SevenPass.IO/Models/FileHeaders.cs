using System;

namespace SevenPass.IO.Models
{
    /// <summary>
    /// Headers of a KeePass 2.x database file.
    /// </summary>
    public class FileHeaders
    {
        /// <summary>
        /// Gets or sets the database schema.
        /// Only major and minor version is used.
        /// </summary>
        public Version Schema { get; set; }
    }
}