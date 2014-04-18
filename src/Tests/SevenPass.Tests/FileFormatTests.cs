using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using NUnit.Framework;
using SevenPass.IO;
using SevenPass.IO.Models;

namespace SevenPass.Tests
{
    [TestFixture]
    public class FileFormatTests
    {
        [Test]
        public async Task Headers_should_detect_not_supported_files()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(
                    CryptographicBuffer.GenerateRandom(1024));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNull(result.Headers);
                Assert.AreEqual(FileFormats.NotSupported, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_1x_file_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A65FB4BB5"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNull(result.Headers);
                Assert.AreEqual(FileFormats.KeePass1x, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_pre_release_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A66FB4BB5"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNull(result.Headers);
                Assert.AreEqual(FileFormats.OldVersion, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_supported_format()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var database = assembly.GetManifestResourceStream(
                "SevenPass.Tests.Demo7Pass.kdbx");

            using (var input = database.AsRandomAccessStream())
            {
                var result = await FileFormat.Headers(input);

                Assert.IsNotNull(result.Headers);
                Assert.AreEqual(FileFormats.Supported, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_old_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                // Schema; 2.01
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A67FB4BB501000200"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNull(result.Headers);
                Assert.AreEqual(FileFormats.OldVersion, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_partial_support_format()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var database = assembly.GetManifestResourceStream(
                "SevenPass.Tests.Demo7Pass.kdbx");

            using (var file = new InMemoryRandomAccessStream())
            {
                var temp = new byte[512];
                database.Read(temp, 0, temp.Length);

                await file.WriteAsync(CryptographicBuffer
                    .CreateFromByteArray(temp));
                
                file.Seek(8);

                // Schema; 3.Max
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("FFFF0300"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNotNull(result.Headers);
                Assert.AreEqual(FileFormats.PartialSupported, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_detect_new_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                // Schema: 4.01
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A67FB4BB501000400"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.IsNull(result.Headers);
                Assert.AreEqual(FileFormats.NewVersion, result.Format);
            }
        }
    }
}