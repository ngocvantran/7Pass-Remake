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
        private readonly XElement _element;
        private readonly T _viewModel;

        protected EntrySubViewTestsBase(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
            _viewModel = new T();
        }

        [Fact]
        public void Initialize_should_populate_fields()
        {
            _viewModel.Loads(_element);
            ScreenExtensions.TryActivate(_viewModel);

            AssertValues(_viewModel);
        }

        [Fact]
        public void Loads_should_not_populate_fields_if_not_initialized()
        {
            _viewModel.Loads(_element);
            Assert.Null(GetLoadedIndicator(_viewModel));
        }

        [Fact]
        public void Loads_should_populate_fields_if_already_initialized()
        {
            ScreenExtensions.TryActivate(_viewModel);
            _viewModel.Loads(_element);

            Assert.NotNull(GetLoadedIndicator(_viewModel));
        }

        protected abstract void AssertValues(T viewModel);

        protected abstract object GetLoadedIndicator(T viewModel);
    }
}