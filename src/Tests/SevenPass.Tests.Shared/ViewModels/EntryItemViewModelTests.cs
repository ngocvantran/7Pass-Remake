using System;
using Windows.UI.Xaml;
using SevenPass.Models;
using SevenPass.ViewModels;
using Xunit;

namespace SevenPass.Tests.ViewModels
{
    public class EntryItemViewModelTests
    {
        private readonly EntryItemModel _model;
        private readonly EntryItemViewModel _viewModel;

        public EntryItemViewModelTests()
        {
            _model = new EntryItemModel();
            _viewModel = new EntryItemViewModel(_model);
        }

        [Fact]
        public void Should_delegate_entry_details()
        {
            _model.Id = "TEST_UUID";
            _model.Title = "test entry";
            _model.Username = "test user";
            _model.Password = "test password";

            Assert.Equal(_model.Id, _viewModel.Id);
            Assert.Equal(_model.Title, _viewModel.Title);
            Assert.Equal(_model.Username, _viewModel.Username);
            Assert.Equal(_model.Password, _viewModel.Password);
        }

        [Fact]
        public void Should_hide_password_by_default()
        {
            Assert.Equal(Visibility.Visible, _viewModel.TitleVisibility);
            Assert.Equal(Visibility.Collapsed, _viewModel.PasswordVisibility);
        }

        [Fact]
        public void TogglePassword_should_toggle_password_and_title_visibility()
        {
            // Show password
            _viewModel.TogglePassword();
            Assert.Equal(Visibility.Collapsed, _viewModel.TitleVisibility);
            Assert.Equal(Visibility.Visible, _viewModel.PasswordVisibility);

            // Hide password
            _viewModel.TogglePassword();
            Assert.Equal(Visibility.Visible, _viewModel.TitleVisibility);
            Assert.Equal(Visibility.Collapsed, _viewModel.PasswordVisibility);
        }
    }
}