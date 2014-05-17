using System;
using SevenPass.Entry.ViewModels;

namespace SevenPass.Messages
{
    public class EntryFieldExpandedMessage
    {
        private readonly EntryFieldItemViewModel _item;

        /// <summary>
        /// Gets the item that was expanded.
        /// </summary>
        public EntryFieldItemViewModel Item
        {
            get { return _item; }
        }

        public EntryFieldExpandedMessage(EntryFieldItemViewModel item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
        }
    }
}