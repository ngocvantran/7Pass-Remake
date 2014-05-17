using System;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public class EntryFieldsViewModel : Screen, IEntrySubViewModel
    {
        public EntryFieldsViewModel()
        {
            base.DisplayName = "Fields";
        }

        /// <summary>
        /// Loads the entry details.
        /// </summary>
        /// <param name="element">The Entry element.</param>
        public void Loads(XElement element) {}
    }
}