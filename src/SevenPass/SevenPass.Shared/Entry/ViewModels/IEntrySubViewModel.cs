using System;
using System.Xml.Linq;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public interface IEntrySubViewModel : IHaveDisplayName
    {
        /// <summary>
        /// Loads the entry details.
        /// </summary>
        /// <param name="element">The Entry element.</param>
        void Loads(XElement element);
    }
}