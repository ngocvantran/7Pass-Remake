using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Caliburn.Micro;
using SevenPass.ViewModels;

namespace SevenPass.Views
{
    public static class AppBarBinder
    {
        public static readonly DependencyProperty CommandsProperty = DependencyProperty.RegisterAttached(
            "Commands", typeof(object), typeof(AppBarBinder), new PropertyMetadata(null, OnCommandsChanged));

        public static readonly DependencyProperty MonitorProperty = DependencyProperty.RegisterAttached(
            "Monitor", typeof(CommandsListMonitor), typeof(AppBarBinder),
            new PropertyMetadata(default(CommandsListMonitor)));

        public static object GetCommands(DependencyObject element)
        {
            return element.GetValue(CommandsProperty);
        }

        public static void SetCommands(DependencyObject element, object value)
        {
            element.SetValue(CommandsProperty, value);
        }

        private static void OnCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as CommandBar;
            if (bar == null)
                return;

            var source = e.NewValue as IObservableCollection<AppBarCommandViewModel>;
            if (source == null)
                return;

            var monitor = bar.GetValue(MonitorProperty) as CommandsListMonitor;
            if (monitor != null)
                monitor.Dispose();

            monitor = new CommandsListMonitor(bar, source);
            bar.SetValue(MonitorProperty, monitor);

            monitor.Apply();
        }

        public class CommandsListMonitor : IDisposable
        {
            private readonly CommandBar _bar;
            private readonly IObservableCollection<AppBarCommandViewModel> _source;

            public CommandsListMonitor(CommandBar bar,
                IObservableCollection<AppBarCommandViewModel> source)
            {
                if (bar == null) throw new ArgumentNullException("bar");
                if (source == null) throw new ArgumentNullException("source");

                _bar = bar;
                _source = source;

                _bar.Unloaded += OnUnloaded;
                _source.CollectionChanged += OnCollectionChanged;
            }

            public void Apply()
            {
                ReleaseItems();

                var all = _bar.PrimaryCommands
                    .Concat(_bar.SecondaryCommands)
                    .OfType<AppBarButton>()
                    .Where(x => x.DataContext != null)
                    .ToLookup(x => x.DataContext);

                _bar.PrimaryCommands.Clear();
                _bar.SecondaryCommands.Clear();

                foreach (var model in _source)
                {
                    var command = all[model]
                        .FirstOrDefault();

                    if (command == null)
                    {
                        command = new AppBarButton
                        {
                            DataContext = model,
                        };

                        command.SetBinding(AppBarButton.LabelProperty,
                            new Binding {Path = new PropertyPath("Label")});
                        command.SetBinding(AppBarButton.IconProperty,
                            new Binding {Path = new PropertyPath("Icon")});
                        command.SetBinding(UIElement.VisibilityProperty,
                            new Binding {Path = new PropertyPath("Visibility")});
                        command.SetBinding(Control.IsEnabledProperty,
                            new Binding {Path = new PropertyPath("IsEnabled")});
                        Message.SetAttach(command, "Invoke()");
                    }

                    if (model.IsPrimary)
                        _bar.PrimaryCommands.Add(command);
                    else
                        _bar.SecondaryCommands.Add(command);
                }

                MonitorItems();
                UpdateAppBarVisibility();
            }

            public void Dispose()
            {
                ReleaseItems();

                _bar.Unloaded -= OnUnloaded;
                _source.CollectionChanged -= OnCollectionChanged;
            }

            private void MonitorItems()
            {
                var all = _bar.PrimaryCommands
                    .Concat(_bar.SecondaryCommands)
                    .OfType<AppBarButton>()
                    .Select(x => x.DataContext)
                    .OfType<AppBarCommandViewModel>();

                foreach (var item in all)
                    item.PropertyChanged += OnPropertyChanged;
            }

            private void ReleaseItems()
            {
                var all = _bar.PrimaryCommands
                    .Concat(_bar.SecondaryCommands)
                    .OfType<AppBarButton>()
                    .Select(x => x.DataContext)
                    .OfType<AppBarCommandViewModel>();

                foreach (var item in all)
                    item.PropertyChanged -= OnPropertyChanged;
            }

            private void UpdateAppBarVisibility()
            {
                var isVisible = _bar.PrimaryCommands
                    .OfType<AppBarButton>()
                    .Any(x => x.Visibility == Visibility.Visible);

                _bar.Visibility = isVisible
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            private void OnCollectionChanged(object sender,
                NotifyCollectionChangedEventArgs e)
            {
                Apply();
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case "Visibility":
                        UpdateAppBarVisibility();
                        break;

                    case "IsPrimary":
                        Apply();
                        break;
                }
            }

            private void OnUnloaded(object sender, RoutedEventArgs e)
            {
                Dispose();
            }
        }
    }
}