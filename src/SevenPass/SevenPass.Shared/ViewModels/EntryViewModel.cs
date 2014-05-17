using System;
using System.Linq;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Services.Cache;

namespace SevenPass.ViewModels
{
    public class EntryViewModel : Screen
    {
        private readonly ICacheService _cache;
        private string _databaseName;

        private XElement _entry;
        private string _password;
        private string _title;
        private string _url;
        private string _userName;

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
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public EntryViewModel(ICacheService cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            _cache = cache;
        }

        protected override void OnInitialize()
        {
            DatabaseName = _cache.Database.Name;

            _entry = _cache.GetEntry(Id);
            // TODO: handle entry not found

            var strings = _entry
                .Elements("String")
                .ToLookup(x => (string)x.Element("Key"),
                    x => (string)x.Element("Value"));

            Url = strings["URL"].FirstOrDefault();
            Title = strings["Title"].FirstOrDefault();
            UserName = strings["UserName"].FirstOrDefault();
            Password = strings["Password"].FirstOrDefault();
        }
    }
}