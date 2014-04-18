using System;
using System.IO;
using SevenPass.IO.Models;

namespace SevenPass.IO
{
    public static class FileFormat
    {
        /// <summary>
        /// Reads the meta data from the specified stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>
        /// The file meta data, or <c>null</c> if the input stream
        /// is not a valid KeePass 2.x database.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="input"/> cannot be <c>null</c>.
        /// </exception>
        public static FileMetadata Metadata(Stream input)
        {
            throw new NotImplementedException();
        }
    }
}