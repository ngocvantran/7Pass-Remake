using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.ViewModels;

namespace SevenPass.Entry.ViewModels
{
    public interface IEntrySubViewModel : IHaveDisplayName
    {
        /// <summary>
        /// Gets the app bar commands.
        /// </summary>
        /// <returns>The app bar commands.</returns>
        IEnumerable<AppBarCommandViewModel> GetCommands();

        /// <summary>
        /// Loads the entry details.
        /// </summary>
        /// <param name="element">The Entry element.</param>
        void Loads(XElement element);
    }
}