using System;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public class EntryDetailsViewModel : EntrySubViewModelBase
    {
        private string _password;
        private string _title;
        private string _url;
        private string _userName;

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

        public EntryDetailsViewModel()
        {
            base.DisplayName = "Details";
        }

        protected override void Populate(XElement element)
        {
            var strings = element
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