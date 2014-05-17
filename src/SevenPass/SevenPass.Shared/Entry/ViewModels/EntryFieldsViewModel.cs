using System;
using System.Linq;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    // TODO: collapse selected item on Back button press

    public class EntryFieldsViewModel : EntrySubViewModelBase
    {
        private readonly BindableCollection<EntryFieldItemViewModel> _items;

        private EntryFieldItemViewModel _selectedItem;

        /// <summary>
        /// Gets the field items.
        /// </summary>
        public IObservableCollection<EntryFieldItemViewModel> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public EntryFieldItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != null)
                    _selectedItem.IsExpanded = false;

                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);

                if (value != null)
                    value.IsExpanded = true;
            }
        }

        public EntryFieldsViewModel()
        {
            _items = new BindableCollection<EntryFieldItemViewModel>();
            base.DisplayName = "Fields";
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
                .Select(x => new EntryFieldItemViewModel
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