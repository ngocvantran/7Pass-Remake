using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace SevenPass.Views
{
    public sealed partial class PasswordView
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Ring.IsActive = false;
            FlyoutBase.GetAttachedFlyout(this).Hide();
            base.OnNavigatingFrom(e);
        }

        private void OnOpenDatabase(object sender, RoutedEventArgs e)
        {
            Ring.IsActive = true;
            FlyoutBase.ShowAttachedFlyout(this);
        }

        private void OnPasswordLoaded(object sender, RoutedEventArgs e)
        {
            Password.Focus(FocusState.Programmatic);
        }
    }
}