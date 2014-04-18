using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;
using SevenPass.Views;

namespace SevenPass
{
    public sealed partial class App
    {
        private WinRTContainer _container;
        private INavigationService _navigation;

        public App()
        {
            InitializeComponent();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container.PerRequest<MainViewModel>();
            _container.PerRequest<DatabaseViewModel>();
            _container.Singleton<ICacheService, CacheService>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<MainView>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _navigation = _container.RegisterNavigationService(rootFrame);

#if WINDOWS_PHONE_APP
            rootFrame.Loaded += (sender, args) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += OnHardwareBackPressed;
            };
            rootFrame.Unloaded += (sender, args) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= OnHardwareBackPressed;
            };
        }

        private void OnHardwareBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!_navigation.CanGoBack)
                return;

            e.Handled = true;
            _navigation.GoBack();
#endif
        }
    }
}