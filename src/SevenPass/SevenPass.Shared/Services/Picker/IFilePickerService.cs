using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace SevenPass.Services.Picker
{
    public interface IFilePickerService
    {
        /// <summary>
        /// Handles continuation after file picking.
        /// </summary>
        /// <param name="args">The continuation event arguments.</param>
        Task ContinueAsync(IActivatedEventArgs args);

        /// <summary>
        /// Shows the file picker UI.
        /// </summary>
        /// <param name="target">The target for the file.</param>
        Task PickAsync(FilePickTargets target);
    }
}