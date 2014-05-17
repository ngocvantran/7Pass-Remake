using System;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using SevenPass.Services.Cache;
using Xunit;

namespace SevenPass.Tests.ViewModels.Entry
{
    public class EntryViewModelTests
    {
        private readonly XElement _entry;
        private readonly MockEntrySubViewModel _subModel;
        private readonly EntryViewModel _viewModel;

        public EntryViewModelTests()
        {
            _entry = new XElement("Entry");
            _subModel = new MockEntrySubViewModel();

            _viewModel = new EntryViewModel(
                new MockCacheService(_entry), new EventAggregator(),
                new IEntrySubViewModel[] {_subModel})
            {
                Id = MockCacheService.ID,
            };
        }

        [Fact]
        public void Initialize_should_populate_sub_view_models()
        {
            ScreenExtensions.TryActivate(_viewModel);
            Assert.Same(_entry, _subModel.Element);
        }

        public class MockCacheService : ICacheService
        {
            public const string ID = "NK4XTExcnk+wrek5ojwJfQ==";
            private readonly XElement _entry;

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

            public MockCacheService(XElement entry)
            {
                if (entry == null)
                    throw new ArgumentNullException("entry");

                _entry = entry;
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
                return _entry;
            }

            public XElement GetGroup(string uuid)
            {
                throw new NotSupportedException();
            }
        }

        public class MockEntrySubViewModel : IEntrySubViewModel
        {
            public string DisplayName { get; set; }

            public XElement Element { get; set; }

            public void Loads(XElement element)
            {
                Element = element;
            }
        }
    }
}