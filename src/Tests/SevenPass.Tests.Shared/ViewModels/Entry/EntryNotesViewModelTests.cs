using System;
using System.Xml.Linq;
using SevenPass.Entry.ViewModels;
using SevenPass.Tests.ViewModels.Entry;
using Xunit;

namespace SevenPass.Tests.ViewModels
{
    public class EntryNotesViewModelTests
        : EntrySubViewTestsBase<EntryNotesViewModel>
    {
        public EntryNotesViewModelTests()
            : base(new XElement("Entry",
                new XElement("String",
                    new XElement("Key", "Notes"),
                    new XElement("Value", "Entry Notes")))) {}

        protected override void AssertValues(EntryNotesViewModel viewModel)
        {
            Assert.Equal("Entry Notes", viewModel.Notes);
        }

        protected override object GetLoadedIndicator(EntryNotesViewModel viewModel)
        {
            return viewModel.Notes;
        }
    }
}