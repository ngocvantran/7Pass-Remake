using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace SevenPass.Views
{
    /// <summary>
    /// Helper to binds a a <see cref="Hub"/> to a data source.
    /// Adaptered from source by Nick
    /// http://nicksnettravels.builttoroam.com/post/2013/11/13/Databinding-with-the-Windows-81-Hub-control.aspx
    /// </summary>
    public class HubBinder : DependencyObject
    {
        public static readonly DependencyProperty BoundSourceProperty = DependencyProperty
            .RegisterAttached("BoundSource", typeof(INotifyCollectionChanged), typeof(HubBinder),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.RegisterAttached(
            "DataSource", typeof(object), typeof(HubBinder), new PropertyMetadata(null, DataSourceChanged));

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
            "HeaderTemplate", typeof(DataTemplate), typeof(HubBinder), new PropertyMetadata(null, HeaderTemplateChanged));

        public static readonly DependencyProperty SectionTemplateProperty = DependencyProperty.RegisterAttached(
            "SectionTemplate", typeof(DataTemplate), typeof(HubBinder),
            new PropertyMetadata(null, SectionTemplateChanged));

        public static object GetDataSource(UIElement element)
        {
            return element.GetValue(DataSourceProperty);
        }

        public static DataTemplate GetHeaderTemplate(UIElement element)
        {
            return element.GetValue(HeaderTemplateProperty) as DataTemplate;
        }

        public static DataTemplate GetSectionTemplate(UIElement element)
        {
            return element.GetValue(SectionTemplateProperty) as DataTemplate;
        }

        public static void SetDataSource(UIElement element, object value)
        {
            element.SetValue(DataSourceProperty, value);
        }

        public static void SetHeaderTemplate(UIElement element, DataTemplate value)
        {
            element.SetValue(HeaderTemplateProperty, value);
        }

        public static void SetSectionTemplate(UIElement element, DataTemplate value)
        {
            element.SetValue(SectionTemplateProperty, value);
        }

        private static void DataSourceChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var hub = d as Hub;
            if (hub == null)
                return;

            var data = e.NewValue as IList;
            if (data == null)
                return;

            var binder = hub.GetValue(BoundSourceProperty) as HubListMonitor;
            if (binder != null)
                binder.Dispose();

            binder = new HubListMonitor(hub, data);
            hub.SetValue(BoundSourceProperty, binder);

            binder.Apply();
        }

        private static void HeaderTemplateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var hub = d as Hub;
            if (hub == null)
                return;

            var template = e.NewValue as DataTemplate;
            if (template == null)
                return;

            foreach (var hubSection in hub.Sections)
                hubSection.HeaderTemplate = template;
        }

        private static void SectionTemplateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var hub = d as Hub;
            if (hub == null)
                return;

            var template = e.NewValue as DataTemplate;
            if (template == null)
                return;

            foreach (var hubSection in hub.Sections)
                hubSection.ContentTemplate = template;
        }

        private class HubListMonitor : IDisposable
        {
            private readonly INotifyCollectionChanged _bindable;
            private readonly Hub _hub;
            private readonly IList _source;

            public HubListMonitor(Hub hub, IList source)
            {
                if (hub == null) throw new ArgumentNullException("hub");
                if (source == null) throw new ArgumentNullException("source");

                _hub = hub;
                _source = source;

                _bindable = source as INotifyCollectionChanged;
                if (_bindable != null)
                    _bindable.CollectionChanged += OnCollectionChanged;

                _hub.SectionsInViewChanged += OnSectionsInViewChanged;
            }

            public void Apply()
            {
                _hub.Sections.Clear();
                var header = GetHeaderTemplate(_hub);
                var template = GetSectionTemplate(_hub);

                foreach (var section in _source)
                {
                    _hub.Sections.Add(new HubSection
                    {
                        DataContext = section,
                        HeaderTemplate = header,
                        ContentTemplate = template,
                    });
                }

                UpdateActiveState();
            }

            public void Dispose()
            {
                if (_bindable != null)
                    _bindable.CollectionChanged -= OnCollectionChanged;

                _hub.SectionsInViewChanged -= OnSectionsInViewChanged;
            }

            private void UpdateActiveState()
            {
                var actives = _hub.SectionsInView.ToList();

                actives
                    .Select(x => x.DataContext)
                    .Apply(ScreenExtensions.TryActivate);

                _hub.Sections
                    .Except(actives)
                    .Select(x => x.DataContext)
                    .Apply(x => ScreenExtensions.TryDeactivate(x, false));
            }

            private void OnCollectionChanged(object sender,
                NotifyCollectionChangedEventArgs e)
            {
                _hub.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, Apply);
            }

            private void OnSectionsInViewChanged(object sender,
                SectionsInViewChangedEventArgs e)
            {
                UpdateActiveState();
            }
        }
    }
}