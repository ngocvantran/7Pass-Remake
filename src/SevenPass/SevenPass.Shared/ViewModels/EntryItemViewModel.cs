using System;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Models;

namespace SevenPass.ViewModels
{
    public class EntryItemViewModel : PropertyChangedBase
    {
        private readonly EntryItemModel _entry;

        private bool _showPassword;

        /// <summary>
        /// Gets the entry UUID.
        /// </summary>
        public string Id
        {
            get { return _entry.Id; }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password
        {
            get { return _entry.Password; }
        }

        /// <summary>
        /// Gets the visibility of entry password.
        /// </summary>
        public Visibility PasswordVisibility
        {
            get
            {
                return _showPassword
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the entry title.
        /// </summary>
        public string Title
        {
            get { return _entry.Title; }
        }

        /// <summary>
        /// Gets the visibility of entry title.
        /// </summary>
        public Visibility TitleVisibility
        {
            get
            {
                return !_showPassword
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username
        {
            get { return _entry.Username; }
        }

        public EntryItemViewModel(EntryItemModel entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            _entry = entry;
        }

        /// <summary>
        /// Toggles the showing of title/password.
        /// </summary>
        public void TogglePassword()
        {
            _showPassword = !_showPassword;
            NotifyOfPropertyChange(() => TitleVisibility);
            NotifyOfPropertyChange(() => PasswordVisibility);
        }
    }
}