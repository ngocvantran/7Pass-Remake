using System;
using System.Linq;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentsViewModel : EntrySubViewModelBase
    {
        private readonly BindableCollection<EntryAttachmentViewModel> _items;

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        public IObservableCollection<EntryAttachmentViewModel> Items
        {
            get { return _items; }
        }

        public EntryAttachmentsViewModel()
        {
            DisplayName = "Attachments";
            _items = new BindableCollection<EntryAttachmentViewModel>();
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
        }
    }
}