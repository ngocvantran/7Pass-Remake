using System;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public class EntryAttachmentsViewModel : EntrySubViewModelBase
    {
        public EntryAttachmentsViewModel()
        {
            base.DisplayName = "Attachments";
        }

        protected override void Populate(XElement element) {}
    }
}