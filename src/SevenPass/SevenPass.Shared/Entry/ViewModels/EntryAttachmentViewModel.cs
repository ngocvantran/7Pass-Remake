using System;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the element containing the attachment value.
        /// </summary>
        public XElement Value { get; set; }
    }
}