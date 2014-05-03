using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace SevenPass.Services.Databases
{
    public class RegisteredDbsService : IRegisteredDbsService
    {
        /// <summary>
        /// Lists all registered databases.
        /// </summary>
        /// <returns>The registered databases.</returns>
        public Task<ICollection<DatabaseRegistration>> ListAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the specified storage file.
        /// </summary>
        /// <param name="file">The database file.</param>
        /// <returns>The database registration information.</returns>
        public Task<DatabaseRegistration> RegisterAsync(IStorageFile file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the database file.
        /// </summary>
        /// <param name="id">The database ID.</param>
        /// <returns>The database file.</returns>
        public Task<IStorageFile> RetrieveAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}