using System;
using Windows.ApplicationModel.Resources;

namespace SevenPass
{
    public static class Resources
    {
        private static readonly ResourceLoader _resource;

        static Resources()
        {
            _resource = ResourceLoader
                .GetForViewIndependentUse();
        }

        public static string GetString(string name)
        {
            return _resource.GetString(name);
        }
    }
}