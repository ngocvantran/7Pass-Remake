using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using NUnit.Framework;
using SevenPass.IO;
using SevenPass.IO.Crypto;

namespace SevenPass.Tests.Crypto
{
    [TestFixture]
    public class HashedInputStreamTests
    {
        [Test]
        public async Task Should_produce_read_bytes_hash(
            [Values(256U, 300U)] uint bufferSize)
        {
            var data = CryptographicBuffer.GenerateRandom(2048);

            var expected = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .HashData(data);

            using (var file = new InMemoryRandomAccessStream())
            {
                await file.WriteAsync(data);
                file.Seek(0);

                var buffer = new Windows.Storage.Streams.Buffer(bufferSize);

                using (var hashed = new HashedInputStream(file))
                {
                    for (var i = 0; i < 8; i++)
                    {
                        await hashed.ReadAsync(
                            buffer, buffer.Capacity);
                    }

                    var hash = hashed.GetHashAndReset();
                    Assert.AreEqual(expected.ToArray(), hash.ToArray());
                }
            }
        }
    }
}