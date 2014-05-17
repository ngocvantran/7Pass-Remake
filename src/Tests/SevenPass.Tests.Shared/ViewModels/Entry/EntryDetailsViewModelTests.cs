using System;
using System.Xml.Linq;
using SevenPass.Entry.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels.Entry
{
    public class EntryDetailsViewModelTests
        : EntrySubViewTestsBase<EntryDetailsViewModel>
    {
        public EntryDetailsViewModelTests()
            : base(new XElement("Entry",
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
                    new XElement("Key", "Secret"),
                    new XElement("Value", "secret")),
                new XElement("String",
                    new XElement("Key", "URL"),
                    new XElement("Value", "http://localhost/{S:Secret}/1")))) {}

        [Fact]
        public void Should_replace_fields_in_url()
        {
            Populate();

            Assert.Equal("http://localhost/secret/1", ViewModel.Url);
        }

        protected override void AssertValues(EntryDetailsViewModel viewModel)
        {
            Assert.NotNull(viewModel.Url);
            Assert.Equal("demo", viewModel.Password);
            Assert.Equal("Demo Entry", viewModel.Title);
            Assert.Equal("Demo User", viewModel.UserName);
        }

        protected override object GetLoadedIndicator(EntryDetailsViewModel viewModel)
        {
            return viewModel.Password;
        }
    }
}