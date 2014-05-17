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
                    new XElement("Key", "URL"),
                    new XElement("Value", "http://localhost/")))) {}

        protected override void AssertValues(EntryDetailsViewModel viewModel)
        {
            Assert.Equal("demo", viewModel.Password);
            Assert.Equal("Demo Entry", viewModel.Title);
            Assert.Equal("Demo User", viewModel.UserName);
            Assert.Equal("http://localhost/", viewModel.Url);
        }

        protected override object GetLoadedIndicator(EntryDetailsViewModel viewModel)
        {
            return viewModel.Password;
        }
    }
}