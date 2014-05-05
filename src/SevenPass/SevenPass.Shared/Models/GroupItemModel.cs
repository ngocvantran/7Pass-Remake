using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Models
{
    public class GroupItemModel : ItemModelBase
    {
        private readonly XElement _element;

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group notes.
        /// </summary>
        public string Notes { get; set; }

        public GroupItemModel(XElement element)
            : base(element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
            Name = (string)element.Element("Name");
            Notes = (string)element.Element("Notes");
        }

        public GroupItemModel() {}

        /// <summary>
        /// Lists the entries of this group.
        /// </summary>
        /// <returns>The entries.</returns>
        public List<EntryItemModel> ListEntries()
        {
            return _element
                .Elements("Entry")
                .Select(x => new EntryItemModel(x))
                .ToList();
        }

        /// <summary>
        /// Lists the child groups of this group.
        /// </summary>
        /// <returns>The child groups.</returns>
        public List<GroupItemModel> ListGroups()
        {
            return _element
                .Elements("Group")
                .Select(x => new GroupItemModel(x))
                .ToList();
        }
    }
}