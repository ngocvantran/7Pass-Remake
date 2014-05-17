using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;

namespace SevenPass.Tests.ViewModels
{
    public class MockNavigationService : INavigationService
    {
        public event NavigatedEventHandler Navigated;
        public event NavigatingCancelEventHandler Navigating;
        public event NavigationFailedEventHandler NavigationFailed;
        public event NavigationStoppedEventHandler NavigationStopped;
        public IList<PageStackEntry> BackStack { get; private set; }
        public bool CanGoBack { get; set; }
        public bool CanGoForward { get; set; }
        public Type CurrentSourcePageType { get; set; }
        public IList<PageStackEntry> ForwardStack { get; private set; }
        public object Parameters { get; private set; }
        public Type SourcePageType { get; set; }
        public Type Target { get; private set; }

        public MockNavigationService()
        {
            BackStack = new List<PageStackEntry>();
            ForwardStack = new List<PageStackEntry>();

            ViewLocator.LocateTypeForModelType = (type, o, arg3) => type;
            ViewLocator.DeterminePackUriFromType = (modelType, viewType) =>
            {
                var asmName = viewType
                    .GetTypeInfo().Assembly
                    .GetAssemblyName();

                return viewType.FullName
                    .Replace(asmName, string.Empty)
                    .Replace(".", "/") + ".xaml";
            };
        }

        public void GoBack()
        {
            throw new NotSupportedException();
        }

        public void GoForward()
        {
            throw new NotSupportedException();
        }

        public bool Navigate(Type sourcePageType)
        {
            Parameters = null;
            Target = sourcePageType;

            return true;
        }

        public bool Navigate(Type sourcePageType, object parameter)
        {
            Parameters = parameter;
            Target = sourcePageType;

            return true;
        }

        public bool ResumeState()
        {
            throw new NotSupportedException();
        }

        public bool SuspendState()
        {
            throw new NotSupportedException();
        }
    }
}