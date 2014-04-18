using System;
using Caliburn.Micro;
using SevenPass.Services.Cache;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// ViewModel to display database data.
    /// </summary>
    public class DatabaseViewModel : Screen
    {
        private readonly CachedDatabase _db;

        public string DatabaseName
        {
            get { return _db.Name; }
        }

        public DatabaseViewModel(ICacheService cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            _db = cache.Database;
            base.DisplayName = "Passwords";
        }
    }
}