using System;
using SevenPass.Models;

namespace SevenPass.ViewModels
{
    public sealed class GroupItemViewModel
    {
        private readonly GroupItemModel _group;

        /// <summary>
        /// Gets the group UUID.
        /// </summary>
        public string Id
        {
            get { return _group.Id; }
        }

        /// <summary>
        /// Gets the group name.
        /// </summary>
        public string Name
        {
            get { return _group.Name; }
        }

        /// <summary>
        /// Gets the group notes.
        /// </summary>
        public string Notes
        {
            get { return _group.Notes; }
        }

        public GroupItemViewModel(GroupItemModel group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            _group = group;
        }
    }
}