using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using SevenPass.Entry.ViewModels;

namespace SevenPass.Entry.Views
{
    public sealed partial class BrowserView
    {
        private BrowserViewModel Model
        {
            get { return (BrowserViewModel)DataContext; }
        }

        public BrowserView()
        {
            InitializeComponent();
        }

        private async Task AutoFill(string value, params string[] names)
        {
            const string script = @"var inputFields = document.querySelectorAll(""input[name*='{0}']"");
for (var i = inputFields.length > 0; i--;) {{ inputFields[i].value = '{1}'; }}";

            if (string.IsNullOrEmpty(value))
                return;

            value = value.Replace(@"\", @"\\");

            foreach (var name in names)
            {
                await View.InvokeScriptAsync("eval", new[]
                {
                    string.Format(script, name, value)
                });
            }
        }

        private async void AutoType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            View.Focus(FocusState.Programmatic);

            value = value.Replace(@"\", @"\\");
            var script = string.Format(
                "document.activeElement.value='{0}';", value);
            await View.InvokeScriptAsync("eval", new[] {script});
        }

        private void OnBackClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            if (View.CanGoBack)
                View.GoBack();
        }

        private async void OnDomContentLoaded(WebView sender,
            WebViewDOMContentLoadedEventArgs args)
        {
            var model = Model;
            await AutoFill(model.UserName, "username", "email", "login");
            await AutoFill(model.Password, "password", "passwd");
        }

        private void OnForwardClicked(object sender, RoutedEventArgs e)
        {
            if (View.CanGoForward)
                View.GoForward();
        }

        private void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            View.Refresh();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlyoutBase.GetAttachedFlyout(View).Hide();
            var item = (BrowserViewModel.FieldViewModel)e.AddedItems[0];
            AutoType(item.Value);
        }

        private void OnStopClicked(object sender, RoutedEventArgs e)
        {
            View.Stop();
        }

        private void OnTypeField(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(View);
            Strings.SelectedItems.Clear();
        }

        private void OnTypePassword(object sender, RoutedEventArgs e)
        {
            AutoType(Model.Password);
        }

        private void OnTypeUserName(object sender, RoutedEventArgs e)
        {
            AutoType(Model.UserName);
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            View.Navigate(new Uri(Model.Url));
        }
    }
}