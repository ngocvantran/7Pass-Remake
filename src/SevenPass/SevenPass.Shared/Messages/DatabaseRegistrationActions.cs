using System;

namespace SevenPass.Messages
{
    public enum DatabaseRegistrationActions
    {
        /// <summary>
        /// New database registered.
        /// </summary>
        Added,

        /// <summary>
        /// Database registration updated.
        /// </summary>
        Updated,

        /// <summary>
        /// Database removed from registration.
        /// </summary>
        Removed,
    }
}