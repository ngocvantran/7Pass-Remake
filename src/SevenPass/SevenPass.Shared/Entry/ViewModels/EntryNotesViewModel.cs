using System;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Entry.ViewModels
{
    public class EntryNotesViewModel : EntrySubViewModelBase
    {
        private string _notes;

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public EntryNotesViewModel()
        {
            base.DisplayName = "Notes";
        }

        protected override void Populate(XElement element)
        {
            Notes = element
                .Elements("String")
                .Where(x => (string)x.Element("Key") == "Notes")
                .Select(x => (string)x.Element("Value"))
                .FirstOrDefault();
        }
    }
}