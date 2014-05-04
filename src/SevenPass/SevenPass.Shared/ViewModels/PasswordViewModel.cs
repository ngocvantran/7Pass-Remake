using System;
using System.Threading.Tasks;
using Windows.Storage;
using Caliburn.Micro;
using SevenPass.IO;
using SevenPass.Services.Picker;

namespace SevenPass.ViewModels
{
    public class PasswordViewModel : Screen
    {
        private readonly PasswordData _password;
        private readonly IFilePickerService _picker;

        public bool CanClearKeyfile
        {
            get { return _password.HasKeyFile; }
        }

        /// <summary>
        /// Determines whether user has provided a valid master key.
        /// </summary>
        public bool CanOpenDatabase
        {
            get { return _password.IsValid; }
        }

        /// <summary>
        /// Gets or sets the database ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the master password.
        /// </summary>
        public string Password
        {
            get { return _password.Password; }
            set
            {
                _password.Password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanOpenDatabase);
            }
        }

        public PasswordViewModel(IFilePickerService picker)
        {
            if (picker == null)
                throw new ArgumentNullException("picker");

            _picker = picker;
            _password = new PasswordData();
        }

        public async Task AddKeyFile(IStorageFile file)
        {
            using (var input = await file.OpenReadAsync())
                await _password.AddKeyFile(input);

            NotifyOfPropertyChange(() => CanOpenDatabase);
            NotifyOfPropertyChange(() => CanClearKeyfile);
        }

        /// <summary>
        /// Removes the keyfile.
        /// </summary>
        public void ClearKeyfile()
        {
            _password.ClearKeyfile();
            NotifyOfPropertyChange(() => CanOpenDatabase);
            NotifyOfPropertyChange(() => CanClearKeyfile);
        }

        public async Task OpenDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task PickKeyfile()
        {
            await _picker.PickAsync(
                FilePickTargets.KeyFile);
        }
    }
}