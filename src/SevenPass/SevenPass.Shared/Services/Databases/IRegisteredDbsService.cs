using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace SevenPass.Services.Databases
{
    /// <summary>
    /// Service to manage a list of registered databases.
    /// </summary>
    public interface IRegisteredDbsService
    {
        /// <summary>
        /// Lists all registered databases.
        /// </summary>
        /// <returns>The registered databases.</returns>
        ICollection<DatabaseRegistration> List();

        /// <summary>
        /// Registers the specified storage file.
        /// </summary>
        /// <param name="file">The database file.</param>
        /// <returns>The database registration information.</returns>
        DatabaseRegistration Register(IStorageFile file);

        /// <summary>
        /// Removes the specified database from registration.
        /// </summary>
        /// <param name="id">The database ID.</param>
        void Remove(string id);

        /// <summary>
        /// Retrieves the database file.
        /// </summary>
        /// <param name="id">The database ID.</param>
        /// <returns>The database file.</returns>
        Task<IStorageFile> RetrieveAsync(string id);
    }
}