using System;
using Windows.Foundation;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace SevenPass.IO.Crypto
{
    public class HashedInputStream : IInputStream
    {
        private readonly CryptographicHash _sha;
        private readonly IInputStream _stream;

        public HashedInputStream(IInputStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            _sha = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .CreateHash();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        /// <summary>
        /// Gets hash of written bytes and reset.
        /// </summary>
        /// <returns>Hash of the written bytes.</returns>
        public IBuffer GetHashAndReset()
        {
            return _sha.GetValueAndReset();
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
            var result = _stream.ReadAsync(buffer, count, options);

            var complete = result.Completed;
            result.Completed = (info, state) =>
            {
                if (state == AsyncStatus.Completed)
                    _sha.Append(info.GetResults());

                if (complete != null)
                    complete(info, state);
            };

            return result;
        }
    }
}