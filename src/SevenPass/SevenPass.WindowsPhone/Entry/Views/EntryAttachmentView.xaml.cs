using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace SevenPass.Entry.Views
{
    public sealed partial class EntryAttachmentView
    {
        public EntryAttachmentView()
        {
            InitializeComponent();
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}