using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using SevenPass.IO;
using SevenPass.IO.Crypto;
using Xunit;

namespace SevenPass.Tests.IO.Crypto
{
    public class HashedBlockFileFormatTests
    {
        [Fact]
        public async Task Should_detect_corrupt_block_length()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await CopyData(input, "IO.HashedBlockStream.bin");

                input.Seek(36);

                var writer = new DataWriter(input)
                {
                    ByteOrder = ByteOrder.LittleEndian,
                };
                writer.WriteInt32(-100);
                await writer.StoreAsync();

                input.Seek(0);
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => HashedBlockFileFormat.Read(input));
            }
        }

        [Fact]
        public async Task Should_detect_corrupt_data()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await CopyData(input, "IO.HashedBlockStream.bin");

                input.Seek(200);
                await input.WriteAsync(CryptographicBuffer.GenerateRandom(8));

                input.Seek(0);
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => HashedBlockFileFormat.Read(input));
            }
        }

        [Fact]
        public async Task Should_detect_truncated_stream()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await CopyData(input, "IO.HashedBlockStream.bin");
                input.Size -= 8;

                input.Seek(0);
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => HashedBlockFileFormat.Read(input));
            }
        }

        [Fact]
        public async Task Should_read_correctly_formatted_stream()
        {
            using (var input = new InMemoryRandomAccessStream())
            using (var expectedData = TestFiles.Read("IO.HashedBlockStream.Content.bin"))
            {
                await CopyData(input, "IO.HashedBlockStream.bin");

                input.Seek(0);
                expectedData.Seek(0);

                using (var actualData = await HashedBlockFileFormat.Read(input))
                {
                    Assert.Equal(expectedData.Size, (ulong)actualData.Length);

                    var actual = new byte[1024];
                    var expected = WindowsRuntimeBuffer.Create(actual.Length);

                    while (true)
                    {
                        expected = await expectedData.ReadAsync(expected);
                        var read = await actualData.ReadAsync(actual, 0, actual.Length);

                        Assert.Equal(expected.Length, (uint)read);

                        if (read == 0)
                            break;

                        Assert.Equal(expected.ToArray(), actual.Take(read));
                    }
                }
            }
        }

        [Fact]
        public async Task Should_verify_block_index()
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await CopyData(input, "IO.HashedBlockStream.bin");

                input.Seek(97390);
                var writer = new DataWriter(input);
                writer.WriteInt32(5);
                await writer.StoreAsync();

                input.Seek(0);
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => HashedBlockFileFormat.Read(input));
            }
        }

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
    }
}