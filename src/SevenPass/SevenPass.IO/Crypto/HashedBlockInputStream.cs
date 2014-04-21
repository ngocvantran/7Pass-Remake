using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace SevenPass.IO.Crypto
{
    /// <summary>
    /// Streams that write data with hash of the block
    /// to ensure data integrity of the input data.
    /// </summary>
    public class HashedBlockInputStream : IInputStream
    {
        private readonly IRandomAccessStream _buffer;
        private readonly DataReader _reader;
        private readonly HashAlgorithmProvider _sha;
        private readonly IInputStream _stream;

        private int _blockIndex;

        public HashedBlockInputStream(IInputStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            _buffer = new InMemoryRandomAccessStream();

            _reader = new DataReader(_stream)
            {
                ByteOrder = ByteOrder.LittleEndian,
            };

            _sha = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256);
        }

        public void Dispose()
        {
            _buffer.Dispose();
            _stream.Dispose();
        }

        /// <summary>
        /// Returns an asynchronous byte reader object.
        /// </summary>
        /// <param name="buffer">The buffer into which the asynchronous read operation places the bytes that are read.</param>
        /// <param name="count">The number of bytes to read that is less than or equal to the Capacity value.</param>
        /// <param name="options">Specifies the type of the asynchronous read operation.</param>
        /// <returns>
        /// The asynchronous operation.
        /// </returns>
        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(
            IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run<IBuffer, uint>(async (token, progress) =>
            {
                var index = 0U;
                var left = count;
                IBuffer result = null;

                do
                {
                    if (_buffer.Position == _buffer.Size)
                    {
                        if (!await ReadBlock())
                        {
                            if (result != null)
                            {
                                // Have partial requested data
                                return result
                                    .ToArray(0, (int)index)
                                    .AsBuffer();
                            }

                            // No data to return
                            return WindowsRuntimeBuffer.Create(0);
                        }
                    }

                    if (result == null)
                        result = new byte[count].AsBuffer();

                    var remaining = _buffer.Size - _buffer.Position;
                    var toRead = (uint)Math.Min(left, remaining);

                    buffer = await _buffer.ReadAsync(buffer, toRead);
                    buffer.CopyTo(0, result, index, toRead);

                    index += toRead;
                } while (index < count);

                // Has full requested data
                return result;
            });
        }

        /// <summary>
        /// Reads and verify hash of the next block.
        /// </summary>
        private async Task<bool> ReadBlock()
        {
            // Detect end of file
            var read = await _reader.LoadAsync(4);
            if (read == 0)
                return false;

            // Block Index
            var blockIndex = _reader.ReadInt32();
            if (blockIndex != _blockIndex)
            {
                throw new InvalidDataException(string.Format(
                    "Wrong block ID detected, expected: {0}, actual: {1}",
                    _blockIndex, blockIndex));
            }
            _blockIndex++;

            // Block hash
            var hash = WindowsRuntimeBuffer.Create(32);
            hash = await _stream.ReadAsync(hash, 32);

            read = await _reader.LoadAsync(4);
            if (read != 4)
            {
                throw new InvalidDataException(
                    "Data corruption detected (truncated data)");
            }

            // Validate block size (< 10MB)
            var blockSize = _reader.ReadInt32();
            if (blockSize == 0)
            {
                // Terminator block
                var isTerminator = hash
                    .ToArray()
                    .All(x => x == 0);

                if (!isTerminator)
                {
                    throw new InvalidDataException(
                        "Data corruption detected (invalid hash for terminator block)");
                }

                return false;
            }

            if (0 > blockSize || blockSize > 10485760)
            {
                throw new InvalidDataException(
                    "Data corruption detected (truncated data)");
            }

            // Check data truncate
            var loaded = await _reader.LoadAsync((uint)blockSize);
            if (loaded < blockSize)
            {
                throw new InvalidDataException(
                    "Data corruption detected (truncated data)");
            }

            var buffer = _reader.ReadBuffer((uint)blockSize);

            // Verify block integrity
            var actual = _sha.HashData(buffer);
            if (!CryptographicBuffer.Compare(hash, actual))
            {
                throw new InvalidDataException(
                    "Data corruption detected (content corrupted)");
            }

            await _buffer.WriteAsync(buffer);
            _buffer.Seek(0);

            return true;
        }
    }
}