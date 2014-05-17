using System;
using Windows.UI.StartScreen;

namespace SevenPass.Messages
{
    public class DeleteDatabaseMessage
    {
        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tile to be deleted.
        /// </summary>
        public SecondaryTile Tile { get; set; }
    }
}