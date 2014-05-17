using System;
using System.Linq;
using System.Xml.Linq;
using SevenPass.Entry.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels.Entry
{
    public class EntryAttachmentsViewModelTests
        : EntrySubViewTestsBase<EntryAttachmentsViewModel>
    {
        public EntryAttachmentsViewModelTests()
            : base(CreateElement()) {}

        [Fact]
        public void Should_get_correct_Value_element()
        {
            Populate();

            var attachments = ViewModel.Items
                .ToDictionary(x => x.Key, x => x.Value);

            var thumbnail = attachments["thumbnail.png"];
            Assert.Equal("wjHliQR9Qk+C62qV/yBAEA==", thumbnail.Value);

            var reference = attachments["reference.ini"];
            Assert.Equal("pOm2+267l02EebFHq4KHIA==", reference.Value);
        }

        [Fact]
        public void Should_parse_file_names()
        {
            Populate();

            var names = ViewModel.Items
                .Select(x => x.Key)
                .ToList();

            Assert.Contains("thumbnail.png", names);
            Assert.Contains("reference.ini", names);
        }

        protected override void AssertValues(EntryAttachmentsViewModel viewModel)
        {
            Assert.Equal(2, viewModel.Items.Count);
        }

        protected override object GetLoadedIndicator(EntryAttachmentsViewModel viewModel)
        {
            return viewModel.Items;
        }

        private static XElement CreateElement()
        {
            var entry = new XElement("Entry",
                new XElement("Binary",
                    new XElement("Key", "thumbnail.png"),
                    new XElement("Value",
                        new XAttribute("Protected", "True"),
                        new XAttribute("Compressed", "True"),
                        "wjHliQR9Qk+C62qV/yBAEA==")),
                new XElement("Binary",
                    new XElement("Key", "reference.ini"),
                    new XElement("Value",
                        new XAttribute("Ref", "10"))));

            var root = new XElement("KeePassFile",
                new XElement("Meta",
                    new XElement("Binaries",
                        new XElement("Binary",
                            new XAttribute("ID", "10"),
                            "pOm2+267l02EebFHq4KHIA=="))),
                new XElement("Root",
                    new XElement("Group", entry)));

            return entry;
        }
    }
}