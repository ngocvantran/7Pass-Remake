using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using SevenPass.IO.Models;

namespace SevenPass.IO
{
    public static class FileFormat
    {
        /// <summary>
        /// Reads the headers of the specified database file stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>The read result.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="input"/> cannot be <c>null</c>.
        /// </exception>
        public static async Task<ReadHeaderResult> Headers(IInputStream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            IBuffer buffer = new Windows.Storage.Streams.Buffer(2048);

            // Signature
            buffer = await input.ReadAsync(buffer,
                8, InputStreamOptions.None);
            var format = CheckSignature(buffer);
            if (format != FileFormats.Supported)
            {
                return new ReadHeaderResult
                {
                    Format = format,
                };
            }

            // Schema version
            buffer = await input.ReadAsync(
                buffer, 4, InputStreamOptions.None);
            var version = GetVersion(buffer);
            format = CheckCompatibility(version);
            switch (format)
            {
                case FileFormats.Supported:
                case FileFormats.PartialSupported:
                    break;

                default:
                    return new ReadHeaderResult
                    {
                        Format = format,
                    };
            }

            // Fields
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the compatibility with the specified database schema version.
        /// </summary>
        /// <param name="schema">The database schema version.</param>
        /// <returns>The compatibility.</returns>
        private static FileFormats CheckCompatibility(Version schema)
        {
            if (schema.Major < 3)
                return FileFormats.OldVersion;

            if (schema.Major > 3)
                return FileFormats.NewVersion;

            return schema.Minor > 1
                ? FileFormats.PartialSupported
                : FileFormats.Supported;
        }

        /// <summary>
        /// Gets the file format of the specified stream based on file signature.
        /// </summary>
        /// <param name="buffer">The signature bytes buffer.</param>
        /// <returns>The detected database file format.</returns>
        private static FileFormats CheckSignature(IBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            // KeePass 1.x
            var oldSignature = CryptographicBuffer
                .DecodeFromHexString("03D9A29A65FB4BB5");
            if (CryptographicBuffer.Compare(buffer, oldSignature))
                return FileFormats.KeePass1x;

            // KeePass 2.x pre-release
            var preRelease = CryptographicBuffer
                .DecodeFromHexString("03D9A29A66FB4BB5");
            if (CryptographicBuffer.Compare(buffer, preRelease))
                return FileFormats.OldVersion;

            // KeePass 2.x
            var current = CryptographicBuffer
                .DecodeFromHexString("03D9A29A67FB4BB5");
            if (!CryptographicBuffer.Compare(buffer, current))
                return FileFormats.NotSupported;

            return FileFormats.Supported;
        }

        /// <summary>
        /// Gets the database schema version.
        /// </summary>
        /// <param name="buffer">The version bytes buffer</param>
        /// <returns>The database schema version.</returns>
        private static Version GetVersion(IBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            var bytes = buffer.ToArray(0, 4);
            var minor = BitConverter.ToUInt16(bytes, 0);
            var major = BitConverter.ToUInt16(bytes, 2);

            return new Version(major, minor);
        }
    }
}