using System;
using System.IO;
using System.Reflection;
using Windows.Storage.Streams;

namespace SevenPass.Tests
{
    internal static class TestFiles
    {
        public static IRandomAccessStream Read(string name)
        {
            var assembly = typeof(TestFiles).GetTypeInfo().Assembly;
            var keyFile = assembly.GetManifestResourceStream(
                "SevenPass.Tests." + name);

            return keyFile.AsRandomAccessStream();
        }
    }
}