using System;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public abstract class EntrySubViewModelBase : Screen, IEntrySubViewModel
    {
        private XElement _entry;

        public virtual void Loads(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _entry = element;

            if (IsInitialized)
                Populate(_entry);
        }

        protected override void OnInitialize()
        {
            if (_entry != null)
                Populate(_entry);
        }

        /// <summary>
        /// Populate values from entry.
        /// </summary>
        /// <param name="element"></param>
        protected abstract void Populate(XElement element);
    }
}