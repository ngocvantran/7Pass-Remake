using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.ViewModels
{
    public class GroupItemViewModel : ItemViewModelBase
    {
        private readonly XElement _element;

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string Name { get; set; }

        public GroupItemViewModel(XElement element)
            : base(element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
            Name = (string)element.Element("Name");
        }

        /// <summary>
        /// Lists the entries of this group.
        /// </summary>
        /// <returns>The entries.</returns>
        public List<EntryItemViewModel> ListEntries()
        {
            return _element
                .Elements("Entry")
                .Select(x => new EntryItemViewModel(x))
                .ToList();
        }

        /// <summary>
        /// Lists the child groups of this group.
        /// </summary>
        /// <returns>The child groups.</returns>
        public List<GroupItemViewModel> ListGroups()
        {
            return _element
                .Elements("Group")
                .Select(x => new GroupItemViewModel(x))
                .ToList();
        }
    }
}