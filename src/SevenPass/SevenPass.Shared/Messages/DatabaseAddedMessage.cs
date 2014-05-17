using System;
using SevenPass.Services.Databases;

namespace SevenPass.Messages
{
    public sealed class DatabaseRegistrationMessage
    {
        /// <summary>
        /// Gets or sets the registration action.
        /// </summary>
        public DatabaseRegistrationActions Action { get; set; }

        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the registration of the added database.
        /// </summary>
        public DatabaseRegistration Registration { get; set; }
    }
}