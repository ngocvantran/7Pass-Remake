using System;
using System.Xml.Linq;
using SevenPass.Models;
using Xunit;

namespace SevenPass.Tests.Models
{
    public class EntryItemModelTests
    {
        private readonly EntryItemModel _entry;
        private readonly XElement _element;

        public EntryItemModelTests()
        {
            _element = XElement.Parse(@"<Entry>
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
    <Value>
      This entry has a protected fields which is encrypted.
      7Pass supports protected fields and display them correctly.
    </Value>
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
  <History>
    <Entry>
      <UUID>c2+BwE1hjUWGHo1QnydKGQ==</UUID>
      <IconID>0</IconID>
      <ForegroundColor />
      <BackgroundColor />
      <OverrideURL />
      <Tags />
      <Times>
        <LastModificationTime>2010-12-06T15:22:26Z</LastModificationTime>
        <CreationTime>2010-12-06T15:19:56Z</CreationTime>
        <LastAccessTime>2010-12-06T15:22:26Z</LastAccessTime>
        <ExpiryTime>2011-05-05T15:19:56Z</ExpiryTime>
        <Expires>True</Expires>
        <UsageCount>1</UsageCount>
        <LocationChanged>2010-12-06T15:19:56Z</LocationChanged>
      </Times>
      <String>
        <Key>Address</Key>
        <Value Protected='True'>KVztXeS4AF9l+AEeYzXdVqCAxBZoZ18CAqInI03X9w==</Value>
      </String>
      <String>
        <Key>CCV</Key>
        <Value Protected='True'>Dkov</Value>
      </String>
      <String>
        <Key>Credit Card</Key>
        <Value Protected='True'>zrMRO6bY5iSW+++wAmTbNqZAng==</Value>
      </String>
      <String>
        <Key>Notes</Key>
        <Value>
          This entry has a protected fields which is encrypted.
          7Pass supports protected fields and display them correctly.
        </Value>
      </String>
      <String>
        <Key>Password</Key>
        <Value Protected='True'>a3bSiWrlH912A33MjAKTqguXnHA=</Value>
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
    </Entry>
  </History>
</Entry>");

            _entry = new EntryItemModel(_element);
        }

        [Fact]
        public void Should_track_element()
        {
            Assert.Same(_element, _entry.Element);
        }

        [Fact]
        public void Should_parse_entry_id()
        {
            Assert.Equal("c2+BwE1hjUWGHo1QnydKGQ==", _entry.Id);
        }

        [Fact]
        public void Should_parse_password()
        {
            /* 
             * Password would be decrypted during the decryption process.
             * The entry should simply parse the content of the string
             */

            Assert.Equal("99vDEC8eGw5FgteIXYQnfQN4UE4=", _entry.Password);
        }

        [Fact]
        public void Should_parse_title()
        {
            Assert.Equal("Protected Fields", _entry.Title);
        }

        [Fact]
        public void Should_parse_username()
        {
            Assert.Equal("SomeUser", _entry.Username);
        }
    }
}