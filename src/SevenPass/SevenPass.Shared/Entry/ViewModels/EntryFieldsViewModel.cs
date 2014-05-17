using System;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Messages;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryFieldsViewModel : EntrySubViewModelBase,
        IHandle<EntryFieldExpandedMessage>
    {
        private readonly IEventAggregator _events;
        private readonly BindableCollection<EntryFieldViewModel> _items;
        private Visibility _listVisibility;

        /// <summary>
        /// Gets the field items.
        /// </summary>
        public IObservableCollection<EntryFieldViewModel> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the visibility of the list.
        /// </summary>
        public Visibility ListVisibility
        {
            get { return _listVisibility; }
            private set
            {
                _listVisibility = value;
                NotifyOfPropertyChange(() => ListVisibility);
                NotifyOfPropertyChange(() => NoFieldVisibility);
            }
        }

        /// <summary>
        /// Gets the visibility of empty data prompt.
        /// </summary>
        public Visibility NoFieldVisibility
        {
            get
            {
                return ListVisibility == Visibility.Visible
                    ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public EntryFieldsViewModel(IEventAggregator events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            _events = events;
            _items = new BindableCollection<EntryFieldViewModel>();
            DisplayName = "Fields";
        }

        public void Handle(EntryFieldExpandedMessage message)
        {
            var expanded = message.Item;

            Items
                .Where(x => !ReferenceEquals(x, expanded))
                .Apply(x => x.IsExpanded = false);
        }

        protected override void Populate(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Items.AddRange(element
                .Elements("String")
                .Select(x => new
                {
                    Key = (string)x.Element("Key"),
                    Value = x.Element("Value"),
                })
                .Where(x => !IsStandardField(x.Key))
                .Select(x => new EntryFieldViewModel(this, _events)
                {
                    Key = x.Key,
                    Value = (string)x.Value,
                    IsProtected = IsProtected(x.Value),
                }));

            Items.Apply(_events.Subscribe);

            ListVisibility = Items.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private static bool IsProtected(XElement element)
        {
            var attr = element.Attribute("Protected");
            return attr != null && (bool)attr;
        }

        private static bool IsStandardField(string key)
        {
            switch (key)
            {
                case "UserName":
                case "Password":
                case "URL":
                case "Notes":
                case "Title":
                    return true;
            }

            return false;
        }
    }
}