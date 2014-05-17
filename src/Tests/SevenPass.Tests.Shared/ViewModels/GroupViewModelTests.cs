using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SevenPass.Models;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;
using SevenPass.Entry.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels
{
    public class GroupViewModelTests
    {
        private readonly MockNavigationService _navigation;
        private readonly GroupViewModel _viewModel;

        public GroupViewModelTests()
        {
            _navigation = new MockNavigationService();
            _viewModel = new GroupViewModel(
                new MockCacheService(), _navigation)
            {
                Id = MockCacheService.GROUP_ID,
            };
        }

        [Fact]
        public void Should_open_child_group_on_select()
        {
            _viewModel.SelectedItem = new GroupItemViewModel(new GroupItemModel
            {
                Id = MockCacheService.CHILD_GROUP_ID,
            });

            Assert.Equal(typeof(GroupViewModel), _navigation.Target);
        }

        [Fact]
        public void Should_open_entry_on_select()
        {
            _viewModel.SelectedItem = new EntryItemViewModel(new EntryItemModel
            {
                Id = MockCacheService.ENTRY_ID,
            });

            Assert.Equal(typeof(EntryViewModel), _navigation.Target);
        }

        [Fact]
        public async Task Should_populate_items_on_initialize()
        {
            await _viewModel.Initialize();

            var group = Assert.Single(_viewModel.Items
                .OfType<GroupItemViewModel>());
            Assert.Equal("Child Group", group.Name);

            var entry = Assert.Single(_viewModel.Items
                .OfType<EntryItemViewModel>());
            Assert.Equal("Demo Entry", entry.Title);
        }

        [Fact]
        public async Task Should_populate_names_on_initialize()
        {
            await _viewModel.Initialize();
            Assert.Equal("Demo DB", _viewModel.DatabaseName);
            Assert.Equal("Root Group", _viewModel.DisplayName);
        }

        public class MockCacheService : ICacheService
        {
            public const string CHILD_GROUP_ID = "kaLNzo6afkKGD1dJiKTXFA==";
            public const string ENTRY_ID = "1gwdeQjEhUeTV4/Ihg4c3g==";
            public const string GROUP_ID = "SnMTc/hDbkKKeEIv3n1qwA==";

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
                throw new NotSupportedException();
            }

            public XElement GetGroup(string uuid)
            {
                Assert.Equal(GROUP_ID, uuid);

                return new XElement("Group",
                    new XElement("UUID", GROUP_ID),
                    new XElement("Name", "Root Group"),
                    new XElement("Group",
                        new XElement("UUID", CHILD_GROUP_ID),
                        new XElement("Name", "Child Group")),
                    new XElement("Entry",
                        new XElement("UUID", ENTRY_ID),
                        new XElement("String",
                            new XElement("Key", "Title"),
                            new XElement("Value", "Demo Entry"))));
            }
        }
    }
}