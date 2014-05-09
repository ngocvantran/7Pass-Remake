using System;
using System.IO;
using System.Reflection;

namespace SevenPass.Tests
{
    public static class TestIO
    {
        private static readonly Lazy<string> _baseDirectory;

        public static string Directory
        {
            get { return _baseDirectory.Value; }
        }

        static TestIO()
        {
            _baseDirectory = new Lazy<string>(() =>
            {
                var asm = typeof(TestIO).GetTypeInfo().Assembly;
                var property = typeof(Assembly).GetRuntimeProperty("CodeBase");
                var codeBase = new Uri((string)property.GetValue(asm));

                return Path.GetDirectoryName(codeBase.LocalPath);
            });
        }
    }
}