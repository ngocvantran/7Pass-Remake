using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.ViewModels;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryDetailsViewModel : EntrySubViewModelBase
    {
        private readonly AppBarCommandViewModel[] _cmds;
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

        public EntryDetailsViewModel(INavigationService navigation)
        {
            DisplayName = "Details";

            _cmds = new AppBarCommandViewModel[]
            {
                new OpenUrlExternalCommand(this),
                new OpenUrlIntervalCommand(navigation, this),
            };
        }

        public override IEnumerable<AppBarCommandViewModel> GetCommands()
        {
            return _cmds;
        }

        protected override void OnActivate()
        {
            _cmds.Apply(x => x.Visibility = Visibility.Visible);
        }

        protected override void OnDeactivate(bool close)
        {
            _cmds.Apply(x => x.Visibility = Visibility.Collapsed);
        }

        protected override void Populate(XElement element)
        {
            var strings = element
                .Elements("String")
                .ToLookup(x => (string)x.Element("Key"),
                    x => (string)x.Element("Value"));

            Title = strings["Title"].FirstOrDefault();
            UserName = strings["UserName"].FirstOrDefault();
            Password = strings["Password"].FirstOrDefault();

            var url = new StringBuilder(strings["URL"]
                .FirstOrDefault() ?? string.Empty);

            if (url.Length > 0)
            {
                foreach (var item in strings)
                {
                    var key = "{S:" + item.Key + "}";
                    url.Replace(key, item.First());
                }
            }

            Url = url.ToString();
        }

        public class OpenUrlExternalCommand : AppBarCommandViewModel
        {
            private readonly EntryDetailsViewModel _parent;

            public OpenUrlExternalCommand(EntryDetailsViewModel parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");

                _parent = parent;

                Label = "url";
                Icon = new SymbolIcon(Symbol.Link);
            }

            public override async void Invoke()
            {
                await Launcher.LaunchUriAsync(new Uri(_parent.Url));
            }
        }

        public class OpenUrlIntervalCommand : AppBarCommandViewModel
        {
            private readonly INavigationService _navigation;
            private readonly EntryDetailsViewModel _parent;

            public OpenUrlIntervalCommand(INavigationService navigation,
                EntryDetailsViewModel parent)
            {
                if (navigation == null) throw new ArgumentNullException("navigation");
                if (parent == null) throw new ArgumentNullException("parent");

                _parent = parent;
                _navigation = navigation;

                Label = "browse";
                Icon = new SymbolIcon(Symbol.World);
            }

            public override void Invoke()
            {
                _navigation
                    .UriFor<BrowserViewModel>()
                    .WithParam(x => x.Id, _parent.Id)
                    .WithParam(x => x.Url, _parent.Url)
                    .Navigate();
            }
        }
    }
}