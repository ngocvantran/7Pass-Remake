using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
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
            if (System.Diagnostics.Debugger.IsAttached)
                LogManager.GetLog = x => new DebugLog(x);

            RegisterServices();
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
            _container.RegisterNavigationService(rootFrame);
            _events = _container.GetInstance<IEventAggregator>();

            var messages = new GlobalMessagesService();
            _events.Subscribe(messages);
            _container.Instance(messages);

#if WINDOWS_PHONE_APP
            _container
                .GetInstance<BackButtonHandler>()
                .Initialize();
#endif
        }

        private void RegisterServices()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container
                .AssemblyContainingType<MainViewModel>()
                .RegisterViewModels();

#if WINDOWS_PHONE_APP
            _container.Singleton<BackButtonHandler>();
#endif
            
            _container.Instance(AutoMaps.Initialize());
            _container.Singleton<ICacheService, CacheService>();
            _container.Singleton<IFilePickerService, FilePickerService>();
            _container.Singleton<IRegisteredDbsService, RegisteredDbsService>();

            _container.PerRequest<IEntrySubViewModel, EntryDetailsViewModel>();
            _container.PerRequest<IEntrySubViewModel, EntryNotesViewModel>();
            _container.PerRequest<IEntrySubViewModel, EntryAttachmentsViewModel>();
            _container.PerRequest<IEntrySubViewModel, EntryFieldsViewModel>();
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