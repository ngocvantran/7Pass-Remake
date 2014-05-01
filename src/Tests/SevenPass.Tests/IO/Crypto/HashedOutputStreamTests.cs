using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using SevenPass.IO.Crypto;
using Xunit;

namespace SevenPass.Tests.IO.Crypto
{
    public class HashedOutputStreamTests
    {
        [Fact]
        public async Task Should_produce_written_bytes_hash()
        {
            var data = CryptographicBuffer.GenerateRandom(2048);

            var expected = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .HashData(data);

            using (var file = new InMemoryRandomAccessStream())
            using (var hashed = new HashedOutputStream(file))
            {
                for (var i = 0U; i < 8; i++)
                {
                    var part = CryptographicBuffer.CreateFromByteArray(
                        data.ToArray(i*256U, 256));

                    await hashed.WriteAsync(part);
                }

                var hash = hashed.GetHashAndReset();
                Assert.Equal(expected.ToArray(), hash.ToArray());
            }
        }
    }
}