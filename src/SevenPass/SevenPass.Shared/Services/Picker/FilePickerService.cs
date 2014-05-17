using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SevenPass.Services.Databases;
using SevenPass.ViewModels;

namespace SevenPass.Services.Picker
{
    public sealed class FilePickerService : IFilePickerService
    {
        private readonly IRegisteredDbsService _registration;
        private readonly Frame _rootFrame;

        public FilePickerService(Frame rootFrame, IRegisteredDbsService registration)
        {
            if (rootFrame == null) throw new ArgumentNullException("rootFrame");
            if (registration == null) throw new ArgumentNullException("registration");

            _rootFrame = rootFrame;
            _registration = registration;
        }

        /// <summary>
        /// Handles continuation after file picking.
        /// </summary>
        /// <param name="args">The continuation event arguments.</param>
        public async Task ContinueAsync(IActivatedEventArgs args)
        {
#if !WINDOWS_PHONE_APP
            return;
#else
            switch (args.Kind)
            {
                case ActivationKind.PickFileContinuation:
                case ActivationKind.PickSaveFileContinuation:
                    break;

                default:
                    return;
            }

            var openPars = args as FileOpenPickerContinuationEventArgs;
            if (openPars != null)
            {
                object target;
                var all = openPars.ContinuationData.ToList();
                if (!openPars.ContinuationData.TryGetValue("Target", out target))
                    return;

                if (!(target is string))
                    return;

                var value = (FilePickTargets)Enum.Parse(
                    typeof(FilePickTargets), (string)target);
                await Continue(value, openPars.Files[0]);

                return;
            }

            var savePars = args as FileSavePickerContinuationEventArgs;
            if (savePars == null || savePars.File == null)
                return;

            object path;
            if (!savePars.ContinuationData.TryGetValue("Source", out path))
                return;

            if (!(path is string))
                return;

            var source = await StorageFile.GetFileFromPathAsync((string)path);
            await Continue(FilePickTargets.Attachments, source, savePars.File);
#endif
        }

        /// <summary>
        /// Shows the file picker UI.
        /// </summary>
        /// <param name="target">The target for the file.</param>
        public async Task PickAsync(FilePickTargets target)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
            };
            picker.FileTypeFilter.Add("*");

#if WINDOWS_PHONE_APP
            picker.ContinuationData.Add(
                "Target", target.ToString());
            picker.PickSingleFileAndContinue();
#else
            var file = await picker.PickSingleFileAsync();
            await Continue(target, file);
#endif
        }

        public async Task SaveAsync(IStorageFile file)
        {
            var picker = new FileSavePicker
            {
                SuggestedFileName = file.Name,
            };

            var extension = Path.GetExtension(file.Name);
            if (string.IsNullOrEmpty(extension))
                extension = ".unknown";

            picker.FileTypeChoices.Add("Attachment File",
                new List<string> {extension});


#if WINDOWS_PHONE_APP
            picker.ContinuationData.Add("Source", file.Path);
            picker.PickSaveFileAndContinue();
#else
            var target = await picker.PickSaveFileAsync();
            await Continue(FilePickTargets.Attachments, file, target);
#endif
        }

        private async Task Continue(FilePickTargets target,
            IStorageFile read, IStorageFile write = null)
        {
            switch (target)
            {
                case FilePickTargets.Databases:
                    await _registration.RegisterAsync(read);
                    break;

                case FilePickTargets.KeyFile:
                    var view = _rootFrame.Content as FrameworkElement;
                    if (view == null)
                        break;

                    var viewModel = view.DataContext as PasswordViewModel;
                    if (viewModel != null)
                        await viewModel.AddKeyFile(read);
                    break;

                case FilePickTargets.Attachments:
                    CachedFileManager.DeferUpdates(write);
                    await read.CopyAndReplaceAsync(write);
                    await CachedFileManager.CompleteUpdatesAsync(write);
                    break;
            }
        }
    }
}