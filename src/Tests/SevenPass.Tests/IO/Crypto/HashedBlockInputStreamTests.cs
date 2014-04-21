using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using NUnit.Framework;
using SevenPass.IO;
using SevenPass.IO.Crypto;

namespace SevenPass.Tests.IO.Crypto
{
    [TestFixture]
    public class HashedBlockInputStreamTests
    {
        private static async Task CopyData(IOutputStream output, string name)
        {
            using (var input = TestFiles.Read(name))
            {
                var buffer = WindowsRuntimeBuffer.Create(1024);

                do
                {
                    buffer = await input.ReadAsync(
                        buffer, buffer.Capacity);
                    await output.WriteAsync(buffer);
                } while (buffer.Length > 0);
            }
        }

        private static async Task AssertEqual(IInputStream expectedStream,
            IInputStream actualStream)
        {
            var actual = WindowsRuntimeBuffer.Create(1024);
            var expected = WindowsRuntimeBuffer.Create(1024);

            do
            {
                actual = await actualStream.ReadAsync(
                    actual, actual.Capacity);

                expected = await expectedStream.ReadAsync(
                    expected, expected.Capacity);

                Assert.AreEqual(
                    CryptographicBuffer.EncodeToHexString(expected),
                    CryptographicBuffer.EncodeToHexString(actual));
            } while (actual.Length > 0);
        }

        private static async Task ReadAll(IInputStream stream)
        {
            var buffer = WindowsRuntimeBuffer.Create(1024);

            do
            {
                buffer = await stream.ReadAsync(
                    buffer, buffer.Capacity);
            } while (buffer.Length > 0);
        }

        private static void AssertThrows(Func<Task> task)
        {
            Assert.Throws<InvalidDataException>(() =>
                task().GetAwaiter().GetResult());
        }

        [Test]
        public async Task Should_detect_corrupt_block_length()
        {
            using (var hashed = new InMemoryRandomAccessStream())
            using (var unhasher = new HashedBlockInputStream(hashed))
            {
                await CopyData(hashed, "IO.HashedBlockStream.bin");

                hashed.Seek(36);

                var writer = new DataWriter(hashed)
                {
                    ByteOrder = ByteOrder.LittleEndian,
                };
                writer.WriteInt32(-100);
                await writer.StoreAsync();

                hashed.Seek(0);
                AssertThrows(() => ReadAll(unhasher));
            }
        }

        [Test]
        public async Task Should_detect_corrupt_data()
        {
            using (var hashed = new InMemoryRandomAccessStream())
            using (var unhasher = new HashedBlockInputStream(hashed))
            {
                await CopyData(hashed, "IO.HashedBlockStream.bin");

                hashed.Seek(200);
                await hashed.WriteAsync(CryptographicBuffer.GenerateRandom(8));

                hashed.Seek(0);
                AssertThrows(() => ReadAll(unhasher));
            }
        }

        [Test]
        public async Task Should_detect_truncated_stream()
        {
            using (var hashed = new InMemoryRandomAccessStream())
            using (var unhasher = new HashedBlockInputStream(hashed))
            {
                await CopyData(hashed, "IO.HashedBlockStream.bin");
                hashed.Size -= 8;

                hashed.Seek(0);
                AssertThrows(() => ReadAll(unhasher));
            }
        }

        [Test]
        public async Task Should_read_correctly_formatted_stream()
        {
            using (var hashed = new InMemoryRandomAccessStream())
            using (var expected = new InMemoryRandomAccessStream())
            using (var unhasher = new HashedBlockInputStream(hashed))
            {
                await CopyData(hashed, "IO.HashedBlockStream.bin");
                await CopyData(expected, "IO.HashedBlockStream.Content.bin");

                hashed.Seek(0);
                expected.Seek(0);

                await AssertEqual(expected, unhasher);
            }
        }

        [Test]
        public async Task Should_verify_block_index()
        {
            using (var hashed = new InMemoryRandomAccessStream())
            using (var unhasher = new HashedBlockInputStream(hashed))
            {
                await CopyData(hashed, "IO.HashedBlockStream.bin");

                hashed.Seek(97390);
                var writer = new DataWriter(hashed);
                writer.WriteInt32(5);
                await writer.StoreAsync();

                hashed.Seek(0);
                AssertThrows(() => ReadAll(unhasher));
            }
        }
    }
}