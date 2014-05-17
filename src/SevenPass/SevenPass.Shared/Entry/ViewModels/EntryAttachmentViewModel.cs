using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.System;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentViewModel
    {
        private readonly XElement _element;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the element containing the attachment value.
        /// </summary>
        public XElement Value { get; set; }

        public EntryAttachmentViewModel(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _element = element;
        }

        public void Delete()
        {
            // TODO
        }

        public async Task Open()
        {
            var file = await SaveToFile();
            await Launcher.LaunchFileAsync(file);
        }

        public void Save()
        {
            // TODO
        }

        public void Share()
        {
            // TODO
        }

        private async Task<IStorageFile> SaveToFile()
        {
            try
            {
                var target = await ApplicationData.Current.TemporaryFolder
                    .CreateFolderAsync("Attachments",
                        CreationCollisionOption.OpenIfExists);

                var file = await target.CreateFileAsync(Key,
                    CreationCollisionOption.ReplaceExisting);

                var value = Value;
                var compressed = value.Attribute("Compressed");
                var isCompressed = compressed != null && (bool)compressed;


                if (!isCompressed)
                {
                    var buffer = CryptographicBuffer
                        .DecodeFromBase64String(value.Value);
                    await FileIO.WriteBufferAsync(file, buffer);
                }
                else
                {
                    var bytes = Convert.FromBase64String(value.Value);

                    using (var buffer = new MemoryStream(bytes))
                    using (var gz = new GZipStream(buffer, CompressionMode.Decompress))
                    using (var output = await file.OpenStreamForWriteAsync())
                    {
                        await gz.CopyToAsync(output);
                    }
                }

                return file;
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}