using System;
using System.Xml.Linq;
using SevenPass.Services.Cache;
using Xunit;

namespace SevenPass.Tests.Services
{
    public class CacheServiceTests
    {
        private readonly XDocument _doc;
        private readonly XElement _entry;
        private readonly XElement _group;
        private readonly CacheService _service;

        public CacheServiceTests()
        {
            _service = new CacheService();

            _entry = new XElement("Entry",
                new XElement("UUID", "e12"));
            _group = new XElement("Group",
                new XElement("UUID", "g02"),
                new XElement("Entry",
                    new XElement("UUID", "e21")),
                new XElement("Entry",
                    new XElement("UUID", "e22")));

            _doc = new XDocument(
                new XElement("KeePassFile",
                    new XElement("Root",
                        new XElement("Group",
                            new XElement("UUID", "g01"),
                            new XElement("Entry",
                                new XElement("UUID", "e11")),
                            _entry),
                        _group)));

            _service.Cache(new CachedDatabase
            {
                Document = _doc,
            });
        }

        [Fact]
        public void Clear_should_clear_the_cache()
        {
            _service.Clear();
            Assert.Null(_service.Database);
        }

        [Fact]
        public void GetEntry_should_exclude_history()
        {
            _entry.Add(new XElement("History",
                new XElement("Entry",
                    new XElement("UUID", "e33"))));

            RefreshCache();
            Assert.Null(_service.GetEntry("e33"));
        }

        [Fact]
        public void GetEntry_should_return_null_if_no_cache()
        {
            _service.Clear();
            Assert.Null(_service.GetEntry("e12"));
        }

        [Fact]
        public void GetEntry_should_return_null_if_not_found()
        {
            Assert.Null(_service.GetEntry("e33"));
        }

        [Fact]
        public void GetEntry_should_return_the_entry()
        {
            Assert.Same(_entry, _service.GetEntry("e12"));
        }

        [Fact]
        public void GetEntry_should_return_the_first_element_if_duplicated_UUID()
        {
            var ids = _doc.Descendants("UUID");
            foreach (var id in ids)
                id.SetValue("sameId");

            RefreshCache();
            var entry = _service.GetEntry("sameId");

            Assert.NotNull(entry);
            Assert.Equal("Entry", entry.Name);
        }

        [Fact]
        public void GetGroup_should_return_null_if_no_cache()
        {
            _service.Clear();
            Assert.Null(_service.GetGroup("g02"));
        }

        [Fact]
        public void GetGroup_should_return_null_if_not_found()
        {
            Assert.Null(_service.GetGroup("g03"));
        }

        [Fact]
        public void GetGroup_should_return_the_first_element_if_duplicated_UUID()
        {
            var ids = _doc.Descendants("UUID");
            foreach (var id in ids)
                id.SetValue("sameId");

            RefreshCache();
            var group = _service.GetGroup("sameId");

            Assert.NotNull(group);
            Assert.Equal("Group", group.Name);
        }

        [Fact]
        public void GetGroup_should_return_the_group()
        {
            Assert.Same(_group, _service.GetGroup("g02"));
        }

        [Fact]
        public void Root_should_be_null_if_no_cache()
        {
            Assert.NotNull(_service.Root);

            _service.Clear();
            Assert.Null(_service.Root);
        }

        [Fact]
        public void Root_should_provide_the_first_group_at_root()
        {
            var root = _service.Root;
            Assert.NotNull(root);
            Assert.Equal("Group", root.Name);
            Assert.Equal("g01", (string)root.Element("UUID"));
        }

        [Fact]
        public void Should_provides_the_cached_database()
        {
            var db = new CachedDatabase
            {
                Document = new XDocument()
            };
            _service.Cache(db);

            Assert.Same(db, _service.Database);
        }

        private void RefreshCache()
        {
            _service.Cache(new CachedDatabase
            {
                Document = _doc,
            });
        }
    }
}