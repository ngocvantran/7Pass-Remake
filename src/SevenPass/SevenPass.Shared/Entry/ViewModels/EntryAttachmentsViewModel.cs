using System;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentsViewModel : EntrySubViewModelBase
    {
        public EntryAttachmentsViewModel()
        {
            DisplayName = "Attachments";
        }

        protected override void Populate(XElement element) {}
    }
}