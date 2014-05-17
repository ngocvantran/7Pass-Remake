using System;
using Windows.UI.Xaml;

namespace SevenPass.Views
{
    public sealed partial class PasswordView
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        private void OnPasswordLoaded(object sender, RoutedEventArgs e)
        {
            Password.Focus(FocusState.Programmatic);
        }
    }
}