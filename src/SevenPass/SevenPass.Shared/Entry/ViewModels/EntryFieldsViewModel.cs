using System;
using System.Linq;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Messages;

namespace SevenPass.Entry.ViewModels
{
    // TODO: collapse selected item on Back button press

    public class EntryFieldsViewModel : EntrySubViewModelBase,
        IHandle<EntryFieldExpandedMessage>
    {
        private readonly IEventAggregator _events;
        private readonly BindableCollection<EntryFieldItemViewModel> _items;

        /// <summary>
        /// Gets the field items.
        /// </summary>
        public IObservableCollection<EntryFieldItemViewModel> Items
        {
            get { return _items; }
        }

        public EntryFieldsViewModel(IEventAggregator events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            _events = events;
            _items = new BindableCollection<EntryFieldItemViewModel>();
            base.DisplayName = "Fields";
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
                .Select(x => new EntryFieldItemViewModel(_events)
                {
                    Key = x.Key,
                    Value = (string)x.Value,
                    IsProtected = IsProtected(x.Value),
                }));
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