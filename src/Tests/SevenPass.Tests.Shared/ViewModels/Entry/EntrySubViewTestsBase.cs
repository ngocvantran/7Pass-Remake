using System;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels.Entry
{
    public abstract class EntrySubViewTestsBase<T>
        where T : EntrySubViewModelBase, new()
    {
        protected readonly XElement Element;
        protected readonly T ViewModel;

        protected EntrySubViewTestsBase(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Element = element;
            ViewModel = new T();
        }

        [Fact]
        public void Initialize_should_populate_fields()
        {
            Populate();
            AssertValues(ViewModel);
        }

        [Fact]
        public void Loads_should_not_populate_fields_if_not_initialized()
        {
            ViewModel.Loads(Element);
            Assert.Null(GetLoadedIndicator(ViewModel));
        }

        [Fact]
        public void Loads_should_populate_fields_if_already_initialized()
        {
            ScreenExtensions.TryActivate(ViewModel);
            ViewModel.Loads(Element);

            Assert.NotNull(GetLoadedIndicator(ViewModel));
        }

        protected abstract void AssertValues(T viewModel);

        protected abstract object GetLoadedIndicator(T viewModel);

        protected void Populate()
        {
            ViewModel.Loads(Element);
            ScreenExtensions.TryActivate(ViewModel);
        }
    }
}