using System;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Models
{
    public sealed class EntryItemModel : ItemModelBase
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the entry title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        public EntryItemModel() {}

        public EntryItemModel(XElement element)
            : base(element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var strings = element
                .Elements("String")
                .ToLookup(x => (string)x.Element("Key"),
                    x => (string)x.Element("Value"));

            Title = strings["Title"].FirstOrDefault();
            Username = strings["UserName"].FirstOrDefault();
            Password = strings["Password"].FirstOrDefault();
        }
    }
}