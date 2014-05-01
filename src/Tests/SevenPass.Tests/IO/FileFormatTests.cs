using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using SevenPass.IO;
using SevenPass.IO.Models;
using Xunit;

namespace SevenPass.Tests.IO
{
    public class FileFormatTests
    {
        [Fact]
        public async Task Decrypt_should_decrypt_content()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.kdbx"))
            {
                input.Seek(222);

                var masterSeed = CryptographicBuffer.DecodeFromHexString(
                    "2b4656399a5bdf9fdfe9e8705a34b6f484f9b1b940c3d7cfb7ffece3b634e0ae");
                var masterKey = CryptographicBuffer.DecodeFromHexString(
                    "87730050341ff55c46421f2f2a5f4e1e018d0443d19cacc8682f128f1874d0a4");
                var encryptionIV = CryptographicBuffer.DecodeFromHexString(
                    "f360c29e1a603a6548cfbb28da6fff50");

                using (var decrypted = await FileFormat.Decrypt(input,
                    masterKey, masterSeed, encryptionIV))
                {
                    var buffer = WindowsRuntimeBuffer.Create(32);
                    buffer = await decrypted.ReadAsync(buffer, 32);

                    Assert.Equal(
                        "54347fe32f3edbccae1fc60f72c11dafd0a72487b315f9b174ed1073ed67a6e0",
                        CryptographicBuffer.EncodeToHexString(buffer));
                }
            }
        }

        [Fact]
        public async Task Headers_should_detect_1x_file_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A65FB4BB5"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.Null(result.Headers);
                Assert.Equal(FileFormats.KeePass1x, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_new_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                // Schema: 4.01
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A67FB4BB501000400"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.Null(result.Headers);
                Assert.Equal(FileFormats.NewVersion, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_not_supported_files()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(
                    CryptographicBuffer.GenerateRandom(1024));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.Null(result.Headers);
                Assert.Equal(FileFormats.NotSupported, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_old_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                // Schema; 2.01
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A67FB4BB501000200"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.Null(result.Headers);
                Assert.Equal(FileFormats.OldVersion, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_partial_support_format()
        {
            using (var database = TestFiles.Read("IO.Demo7Pass.kdbx"))
            using (var file = new InMemoryRandomAccessStream())
            {
                var buffer = WindowsRuntimeBuffer.Create(512);
                buffer = await database.ReadAsync(
                    buffer, 512, InputStreamOptions.None);

                await file.WriteAsync(buffer);
                file.Seek(8);

                // Schema; 3.Max
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("FFFF0300"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.NotNull(result.Headers);
                Assert.Equal(FileFormats.PartialSupported, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_pre_release_format()
        {
            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(CryptographicBuffer
                    .DecodeFromHexString("03D9A29A66FB4BB5"));

                file.Seek(0);
                var result = await FileFormat.Headers(file);

                Assert.Null(result.Headers);
                Assert.Equal(FileFormats.OldVersion, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_detect_supported_format()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.kdbx"))
            {
                var result = await FileFormat.Headers(input);

                Assert.NotNull(result.Headers);
                Assert.Equal(FileFormats.Supported, result.Format);
            }
        }

        [Fact]
        public async Task Headers_should_parse_fields()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.kdbx"))
            {
                var result = await FileFormat.Headers(input);

                var headers = result.Headers;
                Assert.NotNull(headers);

                Assert.True(headers.UseGZip);
                Assert.Equal(6000U, headers.TransformRounds);

                Assert.Equal(
                    "2b4656399a5bdf9fdfe9e8705a34b6f484f9b1b940c3d7cfb7ffece3b634e0ae",
                    CryptographicBuffer.EncodeToHexString(headers.MasterSeed));
                Assert.Equal(
                    "9525f6992beb739cbaa73ae6e050627fcaff378d3cd6f6c232d20aa92f6d0927",
                    CryptographicBuffer.EncodeToHexString(headers.TransformSeed));
                Assert.Equal(
                    "f360c29e1a603a6548cfbb28da6fff50",
                    CryptographicBuffer.EncodeToHexString(headers.EncryptionIV));
                Assert.Equal(
                    "54347fe32f3edbccae1fc60f72c11dafd0a72487b315f9b174ed1073ed67a6e0",
                    CryptographicBuffer.EncodeToHexString(headers.StartBytes));
                Assert.Equal(
                    "5ba62e1b5d5dfbcb295ef3bd2b627e74b141d7db3e1959fce539342ba3762121",
                    CryptographicBuffer.EncodeToHexString(headers.Hash));
            }
        }

        [Fact]
        public void ParseContent_should_decompress_content()
        {
            using (var decrypted = TestFiles.Read("IO.Demo7Pass.Decrypted.bin"))
            {
                var doc = FileFormat
                    .ParseContent(decrypted, true);
                Assert.NotNull(doc);

                var root = doc.Root;
                Assert.NotNull(root);
                Assert.Equal("KeePassFile", root.Name.LocalName);
            }
        }

        [Fact]
        public void VerifyHeaders_should_detect_corrupted_hash()
        {
            var doc = new XDocument(
                new XElement("KeePassFile",
                    new XElement("Meta",
                        new XElement("HeaderHash", "not a hash"))));

            var hash = CryptographicBuffer.GenerateRandom(32);
            Assert.False(FileFormat.VerifyHeaders(hash, doc));
        }

        [Fact]
        public void VerifyHeaders_should_detect_corrupted_headers()
        {
            const string value = "W6YuG11d+8spXvO9K2J+dLFB19s+GVn85Tk0K6N2ISE=";

            var doc = new XDocument(
                new XElement("KeePassFile",
                    new XElement("Meta",
                        new XElement("HeaderHash", value))));

            var hash = CryptographicBuffer.GenerateRandom(32);
            Assert.False(FileFormat.VerifyHeaders(hash, doc));
        }

        [Fact]
        public void VerifyHeaders_should_detect_empty_document()
        {
            var doc = new XDocument();
            var hash = CryptographicBuffer.GenerateRandom(32);
            Assert.False(FileFormat.VerifyHeaders(hash, doc));
        }

        [Fact]
        public void VerifyHeaders_should_detect_missing_hash()
        {
            var doc = new XDocument(
                new XElement("KeePassFile",
                    new XElement("Meta")));

            var hash = CryptographicBuffer.GenerateRandom(32);
            Assert.False(FileFormat.VerifyHeaders(hash, doc));
        }

        [Fact]
        public void VerifyHeaders_should_detect_valid_headers()
        {
            const string value = "W6YuG11d+8spXvO9K2J+dLFB19s+GVn85Tk0K6N2ISE=";

            var doc = new XDocument(
                new XElement("KeePassFile",
                    new XElement("Meta",
                        new XElement("HeaderHash", value))));

            var hash = CryptographicBuffer
                .DecodeFromBase64String(value);

            Assert.True(FileFormat.VerifyHeaders(hash, doc));
        }

        [Fact]
        public async Task VerifyStartBytes_should_return_false_when_not_match()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await input.WriteAsync(CryptographicBuffer
                    .GenerateRandom(32));

                input.Seek(0);
                var match = await FileFormat.VerifyStartBytes(input,
                    CryptographicBuffer.GenerateRandom(32));

                Assert.False(match);
            }
        }

        [Fact]
        public async Task VerifyStartBytes_should_return_false_when_reach_end_of_stream()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                var bytes = CryptographicBuffer.GenerateRandom(32);
                await input.WriteAsync(bytes);

                input.Seek(0);
                input.Size = 16;

                var match = await FileFormat
                    .VerifyStartBytes(input, bytes);

                Assert.False(match);
            }
        }

        [Fact]
        public async Task VerifyStartBytes_should_return_true_when_match()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                var bytes = CryptographicBuffer.GenerateRandom(32);
                await input.WriteAsync(bytes);

                input.Seek(0);
                var match = await FileFormat
                    .VerifyStartBytes(input, bytes);

                Assert.True(match);
            }
        }
    }
}