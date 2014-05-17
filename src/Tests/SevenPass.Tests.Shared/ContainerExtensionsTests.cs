using System;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using SevenPass.ViewModels;
using Xunit;

namespace SevenPass.Tests
{
    public class ContainerExtensionsTests
    {
        private readonly SimpleContainer _container;

        public ContainerExtensionsTests()
        {
            _container = new SimpleContainer();
        }

        [Fact]
        public void RegisterInstances_should_register_per_request_all_types_implementing_the_service()
        {
            _container
                .AssemblyContainingType<MainViewModel>()
                .RegisterInstances<IEntrySubViewModel>();

            AssertRegistered<IEntrySubViewModel>();
        }

        [Fact]
        public void RegisterViewModels_should_register_all_view_models()
        {
            _container
                .AssemblyContainingType<MainViewModel>()
                .RegisterViewModels();

            AssertRegistered<MainViewModel>();
            AssertRegistered<EntryViewModel>();
        }

        private void AssertRegistered<T>()
        {
            Assert.True(_container.HasHandler(typeof(T), null));
        }
    }
}