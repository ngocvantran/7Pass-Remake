using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using AutoMapper;
using Caliburn.Micro;
using SevenPass.IO.Models;
using SevenPass.Messages;
using SevenPass.Services.Databases;
using SevenPass.Services.Picker;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// Main ViewModel when app is launched, display a list of registered database.
    /// </summary>
    public class MainViewModel : Screen,
        IHandle<DatabaseRegistrationMessage>,
        IHandleWithTask<DatabaseSupportMessage>,
        IHandleWithTask<DuplicateDatabaseMessage>
    {
        private readonly BindableCollection<DatabaseItemViewModel> _databases;
        private readonly IMappingEngine _maps;
        private readonly INavigationService _navigation;
        private readonly IFilePickerService _picker;
        private readonly IRegisteredDbsService _register;

        private DatabaseItemViewModel _selectedDatabase;

        /// <summary>
        /// Gets the bindable list of registered databases.
        /// </summary>
        public BindableCollection<DatabaseItemViewModel> Databases
        {
            get { return _databases; }
        }

        /// <summary>
        /// Gets or sets the selected database.
        /// </summary>
        public DatabaseItemViewModel SelectedDatabase
        {
            get { return _selectedDatabase; }
            set
            {
                _selectedDatabase = value;
                NotifyOfPropertyChange(() => SelectedDatabase);

                Open(value);
            }
        }

        public MainViewModel(INavigationService navigation,
            IRegisteredDbsService register, IMappingEngine maps,
            IFilePickerService picker)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (register == null) throw new ArgumentNullException("register");
            if (maps == null) throw new ArgumentNullException("maps");
            if (picker == null) throw new ArgumentNullException("picker");

            _maps = maps;
            _picker = picker;
            _register = register;
            _navigation = navigation;
            _databases = new BindableCollection<DatabaseItemViewModel>();
        }

        public async Task Handle(DatabaseSupportMessage message)
        {
            var msg = new MessageDialog(
                GetMessage(message.Format))
            {
                Title = "Database File Format",
            };

            await msg.ShowAsync();
        }

        public void Handle(DatabaseRegistrationMessage message)
        {
            var registration = message.Registration;

            switch (message.Action)
            {
                case DatabaseRegistrationActions.Updated:
                case DatabaseRegistrationActions.Removed:
                    var existing = _databases.FirstOrDefault(
                        x => x.Id == registration.Id);

                    if (existing != null)
                        _databases.Remove(existing);

                    break;
            }

            switch (message.Action)
            {
                case DatabaseRegistrationActions.Added:
                case DatabaseRegistrationActions.Updated:
                    _databases.Add(_maps.Map<DatabaseRegistration,
                        DatabaseItemViewModel>(registration));
                    break;
            }
        }

        public async Task Handle(DuplicateDatabaseMessage message)
        {
            var msg = new MessageDialog("The selected database file has already been registered. " +
                "7Pass will automatically use the latest version of the database, " +
                "you don't have to add the database again.")
            {
                Title = "Duplicate Database File",
            };

            await msg.ShowAsync();
        }

        /// <summary>
        /// Registers a new database.
        /// </summary>
        public async Task Register()
        {
            await _picker.PickAsync(
                FilePickTargets.Databases);
        }

        protected override void OnInitialize()
        {
            _databases.Clear();

            var items = _register
                .List()
                .Project(_maps)
                .To<DatabaseItemViewModel>()
                .OrderBy(x => x.Name);

            _databases.AddRange(items);
        }

        private static string GetMessage(FileFormats format)
        {
            switch (format)
            {
                case FileFormats.KeePass1x:
                    return "7Pass Remake does not support KeePass 1.x database files. " +
                        "Please consider converting it to KeePass 2.x format." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.NewVersion:
                    return "The selected database file is created by a newer " +
                        "version of KeePass that is not supported by 7Pass. " +
                        "7Pass should be updated soon to support this version." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.NotSupported:
                    return "The selected file is not supported " +
                        "by 7Pass, or not a KeePass database file." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.OldVersion:
                    return "The selected database file is too old, and is not supported by 7Pass. " +
                        "Please use KeePass 2 on desktop to migrate it to the current format." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.PartialSupported:
                    return "The database is created/modified by a newer version of KeePass. " +
                        "Do not make changes to the database with 7Pass to avoid loss of data.";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Opens the specified database.
        /// </summary>
        /// <param name="item">The database item.</param>
        private void Open(DatabaseItemViewModel item)
        {
            _navigation
                .UriFor<PasswordViewModel>()
                .WithParam(x => x.Id, item.Id)
                .WithParam(x => x.DisplayName, item.Name)
                .Navigate();
        }
    }
}