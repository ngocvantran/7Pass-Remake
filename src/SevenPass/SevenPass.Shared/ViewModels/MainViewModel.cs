using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using SevenPass.Messages;
using SevenPass.Services.Databases;
using SevenPass.Services.Picker;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// Main ViewModel when app is launched, display a list of registered database.
    /// </summary>
    public class MainViewModel : Screen,
        IHandle<DatabaseRegistrationMessage>
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

        /// <summary>
        /// Registers a new database.
        /// </summary>
        public async Task Register()
        {
            await _picker.PickAsync(
                FilePickTargets.Databases);
        }

        protected override async void OnInitialize()
        {
            _databases.Clear();

            var registrations = await _register.ListAsync();

            var items = registrations
                .Project(_maps)
                .To<DatabaseItemViewModel>()
                .OrderBy(x => x.Name);

            _databases.AddRange(items);
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