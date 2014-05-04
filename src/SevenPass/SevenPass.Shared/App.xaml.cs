using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.Services;
using SevenPass.Services.Cache;
using SevenPass.Services.Databases;
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

            _container.Instance(AutoMaps.Initialize());
            _container.Singleton<ICacheService, CacheService>();
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

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<MainView>();
        }

        private object TryRegister(object instance)
        {
            var handler = instance as IHandle;
            if (handler != null)
                _events.Subscribe(instance);

            return instance;
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _events = _container.GetInstance<IEventAggregator>();
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

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args.Kind != ActivationKind.PickFileContinuation)
                return;

            var pars = (FileOpenPickerContinuationEventArgs)args;
            _container
                .GetInstance<IRegisteredDbsService>()
                .RegisterAsync(pars.Files[0]);
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