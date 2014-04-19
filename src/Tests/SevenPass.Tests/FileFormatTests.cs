using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using NUnit.Framework;
using SevenPass.IO;
using SevenPass.IO.Models;
using Buffer = Windows.Storage.Streams.Buffer;

namespace SevenPass.Tests
{
    [TestFixture]
    public class FileFormatTests
    {
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
            using (var database = TestFiles.Read("Demo7Pass.kdbx"))
            using (var file = new InMemoryRandomAccessStream())
            {
                IBuffer buffer = new Buffer(512);
                buffer = await database.ReadAsync(
                    buffer, 512, InputStreamOptions.None);
                
                await file.WriteAsync(buffer);
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
            using (var input = TestFiles.Read("Demo7Pass.kdbx"))
            {
                var result = await FileFormat.Headers(input);

                Assert.IsNotNull(result.Headers);
                Assert.AreEqual(FileFormats.Supported, result.Format);
            }
        }

        [Test]
        public async Task Headers_should_parse_fields()
        {
            using (var input = TestFiles.Read("Demo7Pass.kdbx"))
            {
                var result = await FileFormat.Headers(input);

                var headers = result.Headers;
                Assert.IsNotNull(headers);

                Assert.IsTrue(headers.UseGZip);
                Assert.AreEqual(6000, headers.TransformRounds);

                Assert.AreEqual(
                    "2B-46-56-39-9A-5B-DF-9F-DF-E9-E8-70-5A-34-B6-F4-84-F9-B1-B9-40-C3-D7-CF-B7-FF-EC-E3-B6-34-E0-AE",
                    BitConverter.ToString(headers.MasterSeed));
                Assert.AreEqual(
                    "95-25-F6-99-2B-EB-73-9C-BA-A7-3A-E6-E0-50-62-7F-CA-FF-37-8D-3C-D6-F6-C2-32-D2-0A-A9-2F-6D-09-27",
                    BitConverter.ToString(headers.TransformSeed));
                Assert.AreEqual(
                    "F3-60-C2-9E-1A-60-3A-65-48-CF-BB-28-DA-6F-FF-50",
                    BitConverter.ToString(headers.EncryptionIV));
                Assert.AreEqual(
                    "54-34-7F-E3-2F-3E-DB-CC-AE-1F-C6-0F-72-C1-1D-AF-D0-A7-24-87-B3-15-F9-B1-74-ED-10-73-ED-67-A6-E0",
                    BitConverter.ToString(headers.StartBytes));
                Assert.AreEqual(
                    "5B-A6-2E-1B-5D-5D-FB-CB-29-5E-F3-BD-2B-62-7E-74-B1-41-D7-DB-3E-19-59-FC-E5-39-34-2B-A3-76-21-21",
                    BitConverter.ToString(headers.Hash.ToArray()));
            }
        }
    }
}