using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace SevenPass.Services.Picker
{
    public interface IFilePickerService
    {
        /// <summary>
        /// Handles continuation after open file picking.
        /// </summary>
        /// <param name="args">The continuation event arguments.</param>
        Task ContinueAsync(IActivatedEventArgs args);

        /// <summary>
        /// Shows the open file picker UI.
        /// </summary>
        /// <param name="target">The target for the file.</param>
        Task PickAsync(FilePickTargets target);

        /// <summary>
        /// Shows the save file picker UI.
        /// </summary>
        /// <param name="file">The file to save</param>
        Task SaveAsync(IStorageFile file);
    }
}