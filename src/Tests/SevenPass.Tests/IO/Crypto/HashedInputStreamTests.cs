using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using SevenPass.IO;
using SevenPass.IO.Crypto;
using Xunit;

namespace SevenPass.Tests.IO.Crypto
{
    public class HashedInputStreamTests
    {
        [Theory]
        [InlineData(256)]
        [InlineData(300)]
        public async Task Should_produce_read_bytes_hash(int bufferSize)
        {
            var data = CryptographicBuffer.GenerateRandom(2048);

            var expected = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .HashData(data);

            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(data);
                file.Seek(0);

                var buffer = WindowsRuntimeBuffer.Create(bufferSize);

                using (var hashed = new HashedInputStream(file))
                {
                    for (var i = 0; i < 8; i++)
                    {
                        await hashed.ReadAsync(
                            buffer, buffer.Capacity);
                    }

                    var hash = hashed.GetHashAndReset();
                    Assert.Equal(expected.ToArray(), hash.ToArray());
                }
            }
        }
    }
}