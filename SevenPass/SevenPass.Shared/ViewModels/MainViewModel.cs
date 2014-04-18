using System;
using Caliburn.Micro;
using SevenPass.Services.Cache;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// Main ViewModel when app is launched, display a list of registered database.
    /// </summary>
    public class MainViewModel : Screen
    {
        private readonly BindableCollection<DatabaseItemViewModel> _databases;
        private readonly INavigationService _navigation;
        private readonly ICacheService _cache;

        private DatabaseItemViewModel _selectedDatabase;

        /// <summary>
        /// Gets the app name.
        /// </summary>
        public string AppName
        {
            get { return "7Pass Remake"; }
        }

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
            ICacheService cache)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (cache == null) throw new ArgumentNullException("cache");

            _cache = cache;
            _navigation = navigation;
            _databases = new BindableCollection<DatabaseItemViewModel>();

            base.DisplayName = "Databases";
        }

        protected override void OnInitialize()
        {
            _databases.AddRange(new[]
            {
                new DatabaseItemViewModel
                {
                    Name = "Demo Database",
                },
                new DatabaseItemViewModel
                {
                    Name = "My Passwords"
                },
            });
        }

        /// <summary>
        /// Opens the specified database.
        /// </summary>
        /// <param name="item">The database item.</param>
        private void Open(DatabaseItemViewModel item)
        {
            _cache.Cache(new CachedDatabase
            {
                Name = item.Name,
            });

            _navigation
                .UriFor<DatabaseViewModel>()
                .Navigate();
        }
    }
}