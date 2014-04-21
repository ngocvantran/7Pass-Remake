using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using SevenPass.IO.Crypto;
using SevenPass.IO.Models;

namespace SevenPass.IO
{
    public static class FileFormat
    {
        /// <summary>
        /// Decrypts the specified input stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="masterSeed">The master seed.</param>
        /// <param name="masterKey">The master key.</param>
        /// <param name="encryptionIV">The encryption initialization vector.</param>
        /// <returns>The decrypted buffer.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="input"/>, <paramref name="masterSeed"/>, <paramref name="masterKey"/>
        /// and <paramref name="encryptionIV"/> cannot be <c>null</c>.
        /// </exception>
        public static async Task<IInputStream> Decrypt(IRandomAccessStream input,
            IBuffer masterSeed, IBuffer masterKey, IBuffer encryptionIV)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (masterSeed == null) throw new ArgumentNullException("masterSeed");
            if (masterKey == null) throw new ArgumentNullException("masterKey");
            if (encryptionIV == null) throw new ArgumentNullException("encryptionIV");

            var sha = HashAlgorithmProvider
                .OpenAlgorithm(HashAlgorithmNames.Sha256)
                .CreateHash();

            sha.Append(masterSeed);
            sha.Append(masterKey);

            var seed = sha.GetValueAndReset();
            var aes = SymmetricKeyAlgorithmProvider
                .OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7)
                .CreateSymmetricKey(seed);

            var buffer = WindowsRuntimeBuffer.Create(
                (int)(input.Size - input.Position));
            buffer = await input.ReadAsync(buffer, buffer.Capacity);
            buffer = CryptographicEngine.Decrypt(aes, buffer, encryptionIV);

            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(buffer);
            stream.Seek(0);

            return stream;
        }

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

            var hash = new HashedInputStream(input);
            var buffer = WindowsRuntimeBuffer.Create(128);

            // Signature
            buffer = await hash.ReadAsync(buffer, 8);

            var format = CheckSignature(buffer);
            if (format != FileFormats.Supported)
            {
                return new ReadHeaderResult
                {
                    Format = format,
                };
            }

            // Schema version
            buffer = await hash.ReadAsync(buffer, 4);

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
            var headers = await GetHeaders(hash, buffer);
            headers.Hash = hash.GetHashAndReset();

            return new ReadHeaderResult
            {
                Format = format,
                Headers = headers,
            };
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
        /// Parse the headers fields.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="buffer">The header bytes reader.</param>
        /// <returns>The file headers.</returns>
        private static async Task<FileHeaders> GetHeaders(
            IInputStream input, IBuffer buffer)
        {
            var result = new FileHeaders();

            while (true)
            {
                buffer = await input.ReadAsync(buffer, 3);
                var field = (HeaderFields)buffer.GetByte(0);
                var size = BitConverter.ToUInt16(buffer.ToArray(1, 2), 0);

                if (size > 0)
                    buffer = await input.ReadAsync(buffer, size);

                switch (field)
                {
                    case HeaderFields.EndOfHeader:
                        return result;

                    case HeaderFields.CompressionFlags:
                        result.UseGZip = buffer.GetByte(0) == 1;
                        break;

                    case HeaderFields.EncryptionIV:
                        result.EncryptionIV = buffer
                            .ToArray().AsBuffer();
                        break;

                    case HeaderFields.MasterSeed:
                        result.MasterSeed = buffer
                            .ToArray().AsBuffer();
                        break;

                    case HeaderFields.StreamStartBytes:
                        result.StartBytes = buffer
                            .ToArray().AsBuffer();
                        break;

                    case HeaderFields.TransformSeed:
                        result.TransformSeed = buffer
                            .ToArray().AsBuffer();
                        break;

                    case HeaderFields.TransformRounds:
                        result.TransformRounds = BitConverter.ToUInt64(
                            buffer.ToArray(), 0);
                        break;
                }
            }
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