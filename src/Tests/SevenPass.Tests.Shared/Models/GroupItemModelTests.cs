using System;
using System.Linq;
using System.Xml.Linq;
using SevenPass.Models;
using Xunit;

namespace SevenPass.Tests.Models
{
    public class GroupItemModelTests
    {
        private readonly XElement _element;
        private readonly GroupItemModel _group;

        public GroupItemModelTests()
        {
            _element = XElement.Parse(@"<Group>
    <UUID>GO5heTuMikaOm0x+OtJ0Hg==</UUID>
    <Name>General</Name>
    <Notes>Notes for General group.</Notes>
    <IconID>48</IconID>
    <Times>
        <LastModificationTime>2013-09-30T14:57:10Z</LastModificationTime>
        <CreationTime>2010-12-06T15:19:29Z</CreationTime>
        <LastAccessTime>2013-09-30T14:57:10Z</LastAccessTime>
        <ExpiryTime>2013-09-29T16:00:00Z</ExpiryTime>
        <Expires>False</Expires>
        <UsageCount>15</UsageCount>
        <LocationChanged>2010-12-06T15:19:29Z</LocationChanged>
    </Times>
    <IsExpanded>True</IsExpanded>
    <DefaultAutoTypeSequence>asdasd</DefaultAutoTypeSequence>
    <EnableAutoType>true</EnableAutoType>
    <EnableSearching>false</EnableSearching>
    <LastTopVisibleEntry>mJPnOQNd5kSKLbRj20Fw6g==</LastTopVisibleEntry>
    <Entry>
        <UUID>c2+BwE1hjUWGHo1QnydKGQ==</UUID>
        <IconID>0</IconID>
        <ForegroundColor />
        <BackgroundColor />
        <OverrideURL />
        <Tags />
        <Times>
            <LastModificationTime>2011-07-02T15:03:52Z</LastModificationTime>
            <CreationTime>2010-12-06T15:19:56Z</CreationTime>
            <LastAccessTime>2013-09-30T14:56:49Z</LastAccessTime>
            <ExpiryTime>2011-05-05T15:19:56Z</ExpiryTime>
            <Expires>False</Expires>
            <UsageCount>3</UsageCount>
            <LocationChanged>2010-12-06T15:19:56Z</LocationChanged>
        </Times>
        <String>
            <Key>Address</Key>
            <Value Protected='True'>JrlyF2YLe/YUT7E2oH0rRuPx/C8H64HNXnr3H+VElg==</Value>
        </String>
        <String>
            <Key>CCV</Key>
            <Value Protected='True'>LASl</Value>
        </String>
        <String>
            <Key>Credit Card</Key>
            <Value Protected='True'>HSaLhBSIYWr54WeKv6616YBeDg==</Value>
        </String>
        <String>
            <Key>Notes</Key>
            <Value>This entry has a protected fields which is encrypted.
7Pass supports protected fields and display them correctly.</Value>
        </String>
        <String>
            <Key>Password</Key>
            <Value Protected='True'>99vDEC8eGw5FgteIXYQnfQN4UE4=</Value>
        </String>
        <String>
            <Key>Title</Key>
            <Value>Protected Fields</Value>
        </String>
        <String>
            <Key>URL</Key>
            <Value />
        </String>
        <String>
            <Key>UserName</Key>
            <Value>SomeUser</Value>
        </String>
        <AutoType>
            <Enabled>True</Enabled>
            <DataTransferObfuscation>0</DataTransferObfuscation>
        </AutoType>
        <History />
    </Entry>
    <Entry>
        <UUID>mJPnOQNd5kSKLbRj20Fw6g==</UUID>
        <IconID>0</IconID>
        <CustomIconUUID>wjHliQR9Qk+C62qV/yBAEA==</CustomIconUUID>
        <ForegroundColor>#FF0080</ForegroundColor>
        <BackgroundColor>#0000FF</BackgroundColor>
        <OverrideURL>asdas das dasd asd as das das d</OverrideURL>
        <Tags>asda asd a dads;asdad</Tags>
        <Times>
            <LastModificationTime>2013-09-30T14:47:34Z</LastModificationTime>
            <CreationTime>2010-12-06T15:22:29Z</CreationTime>
            <LastAccessTime>2013-09-30T14:56:59Z</LastAccessTime>
            <ExpiryTime>2011-05-05T15:22:29Z</ExpiryTime>
            <Expires>False</Expires>
            <UsageCount>18</UsageCount>
            <LocationChanged>2010-12-06T15:22:29Z</LocationChanged>
        </Times>
        <String>
            <Key>Notes</Key>
            <Value>This entry has URL with fields in its URL.
7Pass parses it correctly and provide the user with the formatted URL.

Please do not use URLs with your password because the URL may be sent unencrypted over the network.</Value>
        </String>
        <String>
            <Key>Password</Key>
            <Value Protected='True'>22U6J74KFNSkoLsTHFI72JZdGFo=</Value>
        </String>
        <String>
            <Key>Secret</Key>
            <Value Protected='True'>fY1SDAYvpujL</Value>
        </String>
        <String>
            <Key>Title</Key>
            <Value>Fields In Url</Value>
        </String>
        <String>
            <Key>URL</Key>
            <Value>http://www.somesite.com/User/{S:UserID}/{S:Secret}</Value>
        </String>
        <String>
            <Key>UserID</Key>
            <Value>123456</Value>
        </String>
        <String>
            <Key>UserName</Key>
            <Value>Some User</Value>
        </String>
        <Binary>
            <Key>IMG_0010.jpg</Key>
            <Value Ref='0' />
        </Binary>
        <Binary>
            <Key>README</Key>
            <Value Ref='1' />
        </Binary>
        <AutoType>
            <Enabled>True</Enabled>
            <DataTransferObfuscation>0</DataTransferObfuscation>
        </AutoType>
        <History />
    </Entry>
    <Group>
        <UUID>6+o9z+jKD0Ovzw+Me88SoA==</UUID>
        <Name>test</Name>
        <Notes />
        <IconID>0</IconID>
        <Times>
            <LastModificationTime>2011-06-23T14:04:11Z</LastModificationTime>
            <CreationTime>2011-06-23T14:04:11Z</CreationTime>
            <LastAccessTime>2011-06-23T14:04:11Z</LastAccessTime>
            <ExpiryTime>2011-06-23T14:04:11Z</ExpiryTime>
            <Expires>False</Expires>
            <UsageCount>0</UsageCount>
            <LocationChanged>2011-06-23T14:04:11Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
    </Group>
    <Group>
        <UUID>StleL7AiDkaCWYVyjSvqUw==</UUID>
        <Name>gfdg</Name>
        <Notes />
        <IconID>0</IconID>
        <Times>
            <LastModificationTime>2011-07-02T15:02:45Z</LastModificationTime>
            <CreationTime>2011-07-02T15:02:45Z</CreationTime>
            <LastAccessTime>2011-07-02T15:02:45Z</LastAccessTime>
            <ExpiryTime>2011-07-02T15:02:45Z</ExpiryTime>
            <Expires>False</Expires>
            <UsageCount>0</UsageCount>
            <LocationChanged>2011-07-02T15:02:45Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
    </Group>
</Group>");
            _group = new GroupItemModel(_element);
        }

        [Fact]
        public void ListEntries_should_parse_entries()
        {
            Assert.Equal(new[]
            {
                "Protected Fields",
                "Fields In Url"
            }, _group
                .ListEntries()
                .Select(x => x.Title));
        }

        [Fact]
        public void ListGroups_should_parse_groups()
        {
            Assert.Equal(new[] {"test", "gfdg"}, _group
                .ListGroups()
                .Select(x => x.Name));
        }

        [Fact]
        public void Should_parse_group_id()
        {
            Assert.Equal("GO5heTuMikaOm0x+OtJ0Hg==", _group.Id);
        }

        [Fact]
        public void Should_parse_group_name()
        {
            Assert.Equal("General", _group.Name);
        }

        [Fact]
        public void Should_parse_notes()
        {
            Assert.Equal("Notes for General group.", _group.Notes);
        }

        [Fact]
        public void Should_track_element()
        {
            Assert.Same(_element, _group.Element);
        }
    }
}