using System;
using SevenPass.IO.Models;

namespace SevenPass.Messages
{
    public sealed class DatabaseSupportMessage
    {
        /// <summary>
        /// Gets or sets the database file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the database file format.
        /// </summary>
        public FileFormats Format { get; set; }
    }
}