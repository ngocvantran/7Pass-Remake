using System;
using Windows.Foundation;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace SevenPass.IO.Crypto
{
    /// <summary>
    /// Calculates the hash of a stream after reading/writing.
    /// Adapted from KeePassLib.Cryptography.HashingStreamEx from KeePass source code.
    /// </summary>
    public class HashingStreamEx : IInputStream, IOutputStream
    {
        private readonly bool _forWriting;
        private readonly CryptographicHash _sha;
        private readonly IRandomAccessStream _stream;

        /// <summary>
        /// Gets the stream hash, only available after stream has been disposed.
        /// </summary>
        public IBuffer Hash { get; private set; }

        public HashingStreamEx(IRandomAccessStream stream, bool forWriting)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            _forWriting = forWriting;
            _sha = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .CreateHash();
        }

        public void Dispose()
        {
            _stream.Dispose();

            if (Hash == null)
                Hash = _sha.GetValueAndReset();
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            return _stream.FlushAsync();
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(
            IBuffer buffer, uint count, InputStreamOptions options)
        {
            if (_forWriting)
            {
                throw new InvalidOperationException(
                    "Stream configured for writing only.");
            }

            var progress = _stream.ReadAsync(buffer, count, options);

            var complete = progress.Completed;
            progress.Completed = (info, status) =>
            {
                if (status == AsyncStatus.Completed)
                    _sha.Append(buffer);

                complete(info, status);
            };

            return progress;
        }

        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            if (!_forWriting)
            {
                throw new InvalidOperationException(
                    "Stream configured for reading only.");
            }

            _sha.Append(buffer);
            return _stream.WriteAsync(buffer);
        }
    }
}