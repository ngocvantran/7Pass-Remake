using System;
using System.Threading.Tasks;
using SevenPass.IO;
using SevenPass.IO.Models;
using Xunit;

namespace SevenPass.Tests.IO
{
    public class IntegrationTests
    {
        [Fact]
        public async Task Database_decryption()
        {
            using (var kdbx = TestFiles.Read("IO.Demo7Pass.kdbx"))
            {
                // Headers
                var result = await FileFormat.Headers(kdbx);
                Assert.Equal(FileFormats.Supported, result.Format);

                var headers = result.Headers;
                Assert.NotNull(headers);

                // Master Key
                var password = new PasswordData
                {
                    Password = "demo",
                };
                var masterKey = await password
                    .GetMasterKey(headers);

                // Decrypt
                using (var decrypted = await FileFormat
                    .Decrypt(kdbx, masterKey, headers))
                {
                    // Start bytes
                    await FileFormat.VerifyStartBytes(
                        decrypted, headers);

                    // Parse content
                    var doc = FileFormat.ParseContent(
                        decrypted, headers.UseGZip);

                    FileFormat.VerifyHeaders(headers, doc);
                }
            }
        }
    }
}