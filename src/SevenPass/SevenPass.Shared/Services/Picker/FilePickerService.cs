using System;
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
    public class FilePickerService : IFilePickerService
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
            if (args.Kind != ActivationKind.PickFileContinuation)
                return;

            var pars = args as FileOpenPickerContinuationEventArgs;
            if (pars == null)
                return;

            object target;
            if (!pars.ContinuationData.TryGetValue("Target", out target))
                return;

            if (!(target is string))
                return;

            var value = (FilePickTargets)Enum.Parse(
                typeof(FilePickTargets), (string)target);
            await Continue(value, pars.Files[0]);
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

            switch (target)
            {
                case FilePickTargets.Databases:
                    picker.FileTypeFilter.Add(".kdbx");
                    break;

                case FilePickTargets.KeyFile:
                    picker.FileTypeFilter.Add("*");
                    break;
            }

#if WINDOWS_PHONE_APP
            picker.ContinuationData.Add(
                "Target", target.ToString());
            picker.PickSingleFileAndContinue();
#else
            var file = await picker.PickSingleFileAsync();
            await Continue(target, file);
#endif
        }

        private async Task Continue(FilePickTargets target, IStorageFile file)
        {
            switch (target)
            {
                case FilePickTargets.Databases:
                    await _registration.RegisterAsync(file);
                    break;

                case FilePickTargets.KeyFile:
                    var view = _rootFrame.Content as FrameworkElement;
                    if (view == null)
                        break;

                    var viewModel = view.DataContext as PasswordViewModel;
                    if (viewModel != null)
                        await viewModel.AddKeyFile(file);
                    break;
            }
        }
    }
}