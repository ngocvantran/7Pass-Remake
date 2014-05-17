using System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace SevenPass.Views
{
    public sealed partial class DatabaseItemView
    {
        public DatabaseItemView()
        {
            InitializeComponent();
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(this);
        }
    }
}