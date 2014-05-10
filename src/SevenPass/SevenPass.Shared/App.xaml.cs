using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.Services;
using SevenPass.Services.Cache;
using SevenPass.Services.Databases;
using SevenPass.Services.Picker;
using SevenPass.ViewModels;
using SevenPass.Views;

namespace SevenPass
{
    public sealed partial class App
    {
        private WinRTContainer _container;
        private IEventAggregator _events;
        private INavigationService _navigation;

        public App()
        {
            InitializeComponent();
        }

        protected override void BuildUp(object instance)
        {
            TryRegister(instance);
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container.PerRequest<MainViewModel>();
            _container.PerRequest<DatabaseViewModel>();
            _container.PerRequest<PasswordViewModel>();

            _container.Instance(AutoMaps.Initialize());
            _container.Singleton<ICacheService, CacheService>();
            _container.Singleton<IFilePickerService, FilePickerService>();
            _container.Singleton<IRegisteredDbsService, RegisteredDbsService>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container
                .GetAllInstances(service)
                .Select(TryRegister);
        }

        protected override object GetInstance(Type service, string key)
        {
            return TryRegister(_container.GetInstance(service, key));
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            _container
                .GetInstance<IFilePickerService>()
                .ContinueAsync(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<MainView>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.Instance(rootFrame);
            _events = _container.GetInstance<IEventAggregator>();
            _navigation = _container.RegisterNavigationService(rootFrame);

            var messages = new GlobalMessagesService();
            _events.Subscribe(messages);
            _container.Instance(messages);

#if WINDOWS_PHONE_APP
            EventHandler<Windows.Phone.UI.Input.BackPressedEventArgs> handler = (sender, e) =>
            {
                if (!_navigation.CanGoBack)
                    return;

                e.Handled = true;
                _navigation.GoBack();
            };

            rootFrame.Loaded += (sender, args) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += handler;
            };

            rootFrame.Unloaded += (sender, args) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= handler;
            };
#endif
        }

        private object TryRegister(object instance)
        {
            var handler = instance as IHandle;
            if (handler != null)
                _events.Subscribe(instance);

            return instance;
        }
    }
}