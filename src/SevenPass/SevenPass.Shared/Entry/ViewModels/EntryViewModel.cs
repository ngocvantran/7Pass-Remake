using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryViewModel : Screen
    {
        private readonly ICacheService _cache;
        private readonly BindableCollection<AppBarCommandViewModel> _commands;
        private readonly IEntrySubViewModel[] _views;

        private string _databaseName;

        /// <summary>
        /// Gets the app bar commands.
        /// </summary>
        public IObservableCollection<AppBarCommandViewModel> Commands
        {
            get { return _commands; }
        }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                NotifyOfPropertyChange(() => DatabaseName);
            }
        }

        /// <summary>
        /// Gets or sets the entry UUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEntrySubViewModel[] Items
        {
            get { return _views; }
        }

        public EntryViewModel(ICacheService cache,
            IEnumerable<IEntrySubViewModel> views)
        {
            if (cache == null) throw new ArgumentNullException("cache");
            if (views == null) throw new ArgumentNullException("views");

            _cache = cache;
            _views = views.ToArray();

            _commands = new BindableCollection<AppBarCommandViewModel>(
                _views.SelectMany(x => x.GetCommands()));
        }

        protected override void OnInitialize()
        {
            DatabaseName = _cache.Database.Name;

            var entry = _cache.GetEntry(Id);
            if (entry == null)
            {
                // TODO: handle entry not found
                return;
            }

            foreach (var view in _views)
            {
                view.Id = Id;
                view.Loads(entry);
            }
        }
    }
}