using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Caliburn.Micro;
using Newtonsoft.Json;
using SevenPass.Messages;

namespace SevenPass.Services.Databases
{
    public class RegisteredDbsService : IRegisteredDbsService
    {
        private readonly IEventAggregator _events;
        private readonly StorageFolder _folder;

        public RegisteredDbsService(IEventAggregator events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            _events = events;
            _folder = ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// Lists all registered databases.
        /// </summary>
        /// <returns>The registered databases.</returns>
        public async Task<ICollection<DatabaseRegistration>> ListAsync()
        {
            var files = await _folder.GetFilesAsync();

            var registrations = files
                .Where(x => x.Name.EndsWith(".json",
                    StringComparison.OrdinalIgnoreCase))
                .Select(Read);

            var result = new List<DatabaseRegistration>();
            foreach (var registration in registrations)
            {
                var info = await registration;
                if (info == null)
                    continue;

                result.Add(await registration);
            }

            return result;
        }

        /// <summary>
        /// Registers the specified storage file.
        /// </summary>
        /// <param name="file">The database file.</param>
        /// <returns>The database registration information.</returns>
        public async Task<DatabaseRegistration> RegisterAsync(IStorageFile file)
        {
            var id = Guid.NewGuid();
            var info = new DatabaseRegistration
            {
                Id = id,
                Name = GetName(file.Name),
            };

            // Save file registration
            var registration = await _folder
                .CreateFileAsync(id + ".json");
            await FileIO.WriteTextAsync(registration,
                JsonConvert.SerializeObject(info));
            await file.CopyAsync(_folder, id + ".kdbx");

            // Send notification message
            _events.PublishOnCurrentThread(new DatabaseRegistrationMessage
            {
                Registration = info,
                Action = DatabaseRegistrationActions.Added,
            });

            return info;
        }

        /// <summary>
        /// Removes the specified database from registration.
        /// </summary>
        /// <param name="id">The database ID.</param>
        public async Task RemoveAsync(Guid id)
        {
            var item = await _folder.GetFileAsync(id + ".json");
            if (item != null)
                await item.DeleteAsync();

            item = await _folder.GetFileAsync(id + ".kdbx");
            if (item != null)
                await item.DeleteAsync();
        }

        /// <summary>
        /// Retrieves the database file.
        /// </summary>
        /// <param name="id">The database ID.</param>
        /// <returns>The database file.</returns>
        public async Task<IStorageFile> RetrieveAsync(Guid id)
        {
            return await _folder.GetFileAsync(id + "kdbx");
        }

        /// <summary>
        /// Gets the default database name from file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The default database name.</returns>
        private static string GetName(string fileName)
        {
            if (fileName.EndsWith(".kdbx", StringComparison.OrdinalIgnoreCase))
                fileName = fileName.Substring(0, fileName.Length - 5);

            return fileName;
        }

        /// <summary>
        /// Reads the specified database registration file.
        /// </summary>
        /// <param name="file">The database registration file.</param>
        /// <returns>The parsed registration, or <c>null</c> if not valid.</returns>
        private static async Task<DatabaseRegistration> Read(IStorageFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            try
            {
                var json = await FileIO.ReadTextAsync(file);
                return JsonConvert.DeserializeObject<DatabaseRegistration>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}