using System;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels
{
    public class EntryViewModelTests
    {
        private readonly EntryViewModel _viewModel;

        public EntryViewModelTests()
        {
            _viewModel = new EntryViewModel(
                new MockCacheService())
            {
                Id = MockCacheService.ID,
            };
        }

        [Fact]
        public void Initialize_should_populate_fields()
        {
            ScreenExtensions.TryActivate(_viewModel);

            Assert.Equal("demo", _viewModel.Password);
            Assert.Equal("Demo Entry", _viewModel.Title);
            Assert.Equal("Demo User", _viewModel.UserName);
            Assert.Equal("Demo DB", _viewModel.DatabaseName);
            Assert.Equal("http://localhost/", _viewModel.Url);
        }

        public class MockCacheService : ICacheService
        {
            public const string ID = "NK4XTExcnk+wrek5ojwJfQ==";

            public CachedDatabase Database
            {
                get
                {
                    return new CachedDatabase
                    {
                        Name = "Demo DB",
                    };
                }
            }

            public XElement Root
            {
                get { throw new NotSupportedException(); }
            }

            public void Cache(CachedDatabase database)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public XElement GetEntry(string uuid)
            {
                Assert.Equal(ID, uuid);

                return new XElement("Entry",
                    new XElement("UUID", ID),
                    new XElement("String",
                        new XElement("Key", "Title"),
                        new XElement("Value", "Demo Entry")),
                    new XElement("String",
                        new XElement("Key", "UserName"),
                        new XElement("Value", "Demo User")),
                    new XElement("String",
                        new XElement("Key", "Password"),
                        new XElement("Value", "demo")),
                    new XElement("String",
                        new XElement("Key", "URL"),
                        new XElement("Value", "http://localhost/")));
            }

            public XElement GetGroup(string uuid)
            {
                throw new NotSupportedException();
            }
        }
    }
}