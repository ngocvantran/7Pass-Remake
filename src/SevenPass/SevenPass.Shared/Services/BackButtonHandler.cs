#if WINDOWS_PHONE_APP
using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.Messages;

namespace SevenPass.Services
{
    public class BackButtonHandler
    {
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigation;
        private readonly Frame _rootFrame;

        public BackButtonHandler(Frame rootFrame,
            INavigationService navigation, IEventAggregator events)
        {
            if (rootFrame == null) throw new ArgumentNullException("rootFrame");
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (events == null) throw new ArgumentNullException("events");

            _events = events;
            _rootFrame = rootFrame;
            _navigation = navigation;
        }

        public void Initialize()
        {
            _rootFrame.Unloaded += OnUnloaded;
            HardwareButtons.BackPressed += OnBackPressed;
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var message = new BackButtonPressedMessage();
            _events.PublishOnUIThread(message);

            e.Handled = message.IsHandled;
            if (message.IsHandled || !_navigation.CanGoBack)
                return;

            e.Handled = true;
            _navigation.GoBack();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _rootFrame.Unloaded -= OnUnloaded;
            HardwareButtons.BackPressed -= OnBackPressed;
        }
    }
}
#endif