using System;
using Windows.UI.StartScreen;
using Caliburn.Micro;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// ViewModel to display database details in a list.
    /// </summary>
    public sealed class DatabaseItemViewModel
    {
        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the database display name.
        /// </summary>
        public string Name { get; set; }

        public void Delete() {}

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
            await tile.RequestCreateAsync();
        }
    }
}