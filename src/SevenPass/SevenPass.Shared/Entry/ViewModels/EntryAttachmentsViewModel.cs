using System;
using System.Linq;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentsViewModel : EntrySubViewModelBase
    {
        private readonly BindableCollection<EntryAttachmentViewModel> _items;
        private Visibility _listVisibility;
        private DataTransferManager _transferManager;

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        public IObservableCollection<EntryAttachmentViewModel> Items
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
                NotifyOfPropertyChange(() => NoAttachmentVisibility);
            }
        }

        /// <summary>
        /// Gets the visibility of empty data prompt.
        /// </summary>
        public Visibility NoAttachmentVisibility
        {
            get
            {
                return ListVisibility == Visibility.Visible
                    ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public EntryAttachmentsViewModel()
        {
            DisplayName = "Attachments";
            _items = new BindableCollection<EntryAttachmentViewModel>();
        }

        protected override void OnActivate()
        {
            _transferManager = DataTransferManager.GetForCurrentView();
            _transferManager.DataRequested += OnDataRequested;
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var sharing = _items.FirstOrDefault(x => x.IsSharing);
            if (sharing == null)
                return;

            sharing.IsSharing = false;
            var data = args.Request.Data;
            var defer = args.Request.GetDeferral();

            var file = await sharing.SaveToFile();
            data.Properties.Title = file.Name;
            data.SetStorageItems(new[] {file}, true);
            defer.Complete();
        }

        protected override void OnDeactivate(bool close)
        {
            _transferManager.DataRequested -= OnDataRequested;
        }

        protected override void Populate(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var attachments = element
                .Elements("Binary")
                .Select(x => new EntryAttachmentViewModel(x)
                {
                    Key = (string)x.Element("Key"),
                    Value = x.Element("Value"),
                })
                .ToList();

            var references = GetReferences(element);
            foreach (var attachment in attachments.ToList())
            {
                var value = attachment.Value;
                var reference = value.Attribute("Ref");
                if (reference == null)
                    continue;

                // Referenced binary, update the reference
                var map = references.Value;
                value = map != null
                    ? map[(string)reference].FirstOrDefault()
                    : null;

                if (value != null)
                {
                    attachment.Value = value;
                    continue;
                }

                // Broken reference, do not display
                attachments.Remove(attachment);
            }

            Items.AddRange(attachments);

            ListVisibility = Items.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private Lazy<ILookup<string, XElement>> GetReferences(XElement element)
        {
            return new Lazy<ILookup<string, XElement>>(() =>
            {
                var root = element.Parent;
                while (true)
                {
                    if (root == null)
                        return null;

                    if (root.Name == "KeePassFile")
                        break;

                    root = root.Parent;
                }

                return root
                    .Element("Meta")
                    .Element("Binaries")
                    .Elements("Binary")
                    .Select(x => new
                    {
                        Element = x,
                        Id = x.Attribute("ID"),
                    })
                    .Where(x => x.Id != null)
                    .ToLookup(x => (string)x.Id, x => x.Element);
            });
        }
    }
}