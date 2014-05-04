using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.IO;
using SevenPass.Services.Cache;
using SevenPass.Services.Databases;
using SevenPass.Services.Picker;

namespace SevenPass.ViewModels
{
    public class PasswordViewModel : Screen
    {
        private readonly ICacheService _cache;
        private readonly INavigationService _navigation;
        private readonly PasswordData _password;
        private readonly IFilePickerService _picker;
        private readonly IRegisteredDbsService _registrations;
        private string _keyfileName;

        public bool CanClearKeyfile
        {
            get { return _password.HasKeyFile; }
        }

        /// <summary>
        /// Determines whether user has provided a valid master key.
        /// </summary>
        public bool CanOpenDatabase
        {
            get { return _password.IsValid; }
        }

        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the visibility of the Keyfile group
        /// </summary>
        public Visibility KeyfileGroupVisibility
        {
            get
            {
                return CanClearKeyfile
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets name of the file used as keyfile.
        /// </summary>
        public string KeyfileName
        {
            get { return _keyfileName; }
            set
            {
                _keyfileName = value;
                NotifyOfPropertyChange(() => KeyfileName);
            }
        }

        /// <summary>
        /// Gets or sets the master password.
        /// </summary>
        public string Password
        {
            get { return _password.Password; }
            set
            {
                _password.Password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanOpenDatabase);
            }
        }

        public PasswordViewModel(IFilePickerService picker,
            IRegisteredDbsService registrations, ICacheService cache,
            INavigationService navigation)
        {
            if (picker == null) throw new ArgumentNullException("picker");
            if (registrations == null) throw new ArgumentNullException("registrations");
            if (cache == null) throw new ArgumentNullException("cache");
            if (navigation == null) throw new ArgumentNullException("navigation");

            _cache = cache;
            _picker = picker;
            _navigation = navigation;
            _registrations = registrations;
            _password = new PasswordData();
        }

        public async Task AddKeyFile(IStorageFile file)
        {
            KeyfileName = file.Name;
            using (var input = await file.OpenReadAsync())
                await _password.AddKeyFile(input);

            NotifyOfPropertyChange(() => CanOpenDatabase);
            NotifyOfPropertyChange(() => CanClearKeyfile);
            NotifyOfPropertyChange(() => KeyfileGroupVisibility);
        }

        /// <summary>
        /// Removes the keyfile.
        /// </summary>
        public void ClearKeyfile()
        {
            KeyfileName = null;
            _password.ClearKeyfile();

            NotifyOfPropertyChange(() => CanOpenDatabase);
            NotifyOfPropertyChange(() => CanClearKeyfile);
            NotifyOfPropertyChange(() => KeyfileGroupVisibility);
        }

        public async Task OpenDatabase()
        {
            var database = await _registrations
                .RetrieveAsync(Id);

            using (var fs = await database.OpenReadAsync())
            {
                // TODO: handle errors & display transformation progress
                var headers = await FileFormat.Headers(fs);
                var masterKey = await _password
                    .GetMasterKey(headers.Headers);

                using (var decrypted = await FileFormat
                    .Decrypt(fs, masterKey, headers.Headers))
                {
                    // TODO: verify start bytes

                    // Parse content
                    var doc = FileFormat.ParseContent(
                        decrypted, headers.Headers.UseGZip);

                    // TODO: verify headers integrity

                    _cache.Cache(new CachedDatabase
                    {
                        Id = Id,
                        Document = doc,
                        Name = DisplayName,
                        Headers = headers.Headers,
                    });

                    _navigation
                        .UriFor<DatabaseViewModel>()
                        .Navigate();
                }
            }
        }

        public async Task PickKeyfile()
        {
            await _picker.PickAsync(
                FilePickTargets.KeyFile);
        }
    }
}