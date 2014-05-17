using System;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentViewModel
    {
        private readonly XElement _element;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the element containing the attachment value.
        /// </summary>
        public XElement Value { get; set; }

        public EntryAttachmentViewModel(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
        }
    }
}