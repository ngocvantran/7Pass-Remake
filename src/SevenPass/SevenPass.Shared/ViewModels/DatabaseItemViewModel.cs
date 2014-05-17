using System;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Messages;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// ViewModel to display database details in a list.
    /// </summary>
    public sealed class DatabaseItemViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;
        private SecondaryTile _tile;

        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the database display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the visibility of the <see cref="Pin"/> action;
        /// </summary>
        public Visibility PinVisibility
        {
            get
            {
                return _tile == null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets the tile associated with this database.
        /// </summary>
        public SecondaryTile Tile
        {
            get { return _tile; }
            set
            {
                _tile = value;
                NotifyOfPropertyChange(() => PinVisibility);
                NotifyOfPropertyChange(() => UnpinVisibility);
            }
        }

        public DatabaseItemViewModel(IEventAggregator events)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            
            _events = events;
        }

        /// <summary>
        /// Gets the tile ID for the database.
        /// </summary>
        public string TileId
        {
            get { return "DB_" + Id; }
        }

        /// <summary>
        /// Gets the visibility of the <see cref="Unpin"/> action;
        /// </summary>
        public Visibility UnpinVisibility
        {
            get
            {
                return _tile != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public async void Delete()
        {
            var msg = new MessageDialog(Name + " will be removed from 7Pass. " +
                "This do not delete the actual database file.")
            {
                Title = "Remove this database?",
                Commands =
                {
                    new UICommand("yes", _ => _events
                        .PublishOnBackgroundThread(new DeleteDatabaseMessage
                        {
                            Id = Id,
                            Tile = _tile,
                        })),
                    new UICommand("no"),
                }
            };

            await msg.ShowAsync();
        }

        public async void Pin()
        {
            var uri = new UriBuilder<PasswordViewModel>()
                .WithParam(x => x.Id, Id)
                .WithParam(x => x.DisplayName, Name)
                .BuildUri();

            var tile = new SecondaryTile("DB_" + Id, Name, uri.AbsoluteUri,
                new Uri("ms-appx:///Assets/Square71x71Logo.scale-240.png"),
                TileSize.Default);

            tile.VisualElements.ShowNameOnSquare150x150Logo = true;

            var created = await tile.RequestCreateAsync();
            if (created)
                Tile = tile;
        }

        public async void Unpin()
        {
            var deleted = await _tile.RequestDeleteAsync();
            if (deleted)
                Tile = null;
        }
    }
}