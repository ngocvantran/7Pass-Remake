using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
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
    public sealed class MainViewModel : Screen,
        IHandle<DatabaseRegistrationMessage>
    {
        private readonly BindableCollection<DatabaseItemViewModel> _databases;
        private readonly IEventAggregator _events;
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
            IFilePickerService picker, IEventAggregator events)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (register == null) throw new ArgumentNullException("register");
            if (maps == null) throw new ArgumentNullException("maps");
            if (picker == null) throw new ArgumentNullException("picker");
            if (events == null) throw new ArgumentNullException("events");

            _maps = maps;
            _picker = picker;
            _events = events;
            _register = register;
            _navigation = navigation;
            _databases = new BindableCollection<DatabaseItemViewModel>();
        }

        public void Handle(DatabaseRegistrationMessage message)
        {
            switch (message.Action)
            {
                case DatabaseRegistrationActions.Updated:
                case DatabaseRegistrationActions.Removed:
                    var existing = _databases.FirstOrDefault(
                        x => x.Id == message.Id);

                    if (existing != null)
                        _databases.Remove(existing);

                    break;
            }

            switch (message.Action)
            {
                case DatabaseRegistrationActions.Added:
                case DatabaseRegistrationActions.Updated:
                    _databases.Add(Map(message.Registration));
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

        protected override async void OnActivate()
        {
            var tiles = await SecondaryTile.FindAllAsync();
            var list = tiles.ToDictionary(x => x.TileId);

            foreach (var item in _databases)
            {
                SecondaryTile tile;
                list.TryGetValue(item.TileId, out tile);
                item.Tile = tile;
            }
        }

        protected override void OnInitialize()
        {
            _databases.Clear();

            var items = _register
                .List()
                .Select(Map)
                .OrderBy(x => x.Name);

            _databases.AddRange(items);
        }

        private DatabaseItemViewModel Map(DatabaseRegistration registration)
        {
            return _maps.Map(registration,
                new DatabaseItemViewModel(_events));
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