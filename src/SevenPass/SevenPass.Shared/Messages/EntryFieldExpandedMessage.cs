using System;
using SevenPass.Entry.ViewModels;

namespace SevenPass.Messages
{
    public sealed class EntryFieldExpandedMessage
    {
        private readonly EntryFieldViewModel _item;

        /// <summary>
        /// Gets the item that was expanded.
        /// </summary>
        public EntryFieldViewModel Item
        {
            get { return _item; }
        }

        public EntryFieldExpandedMessage(EntryFieldViewModel item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
        }
    }
}