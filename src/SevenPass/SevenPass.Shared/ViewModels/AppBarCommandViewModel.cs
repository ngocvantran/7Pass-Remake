using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace SevenPass.ViewModels
{
    public abstract class AppBarCommandViewModel : PropertyChangedBase
    {
        private IconElement _icon;
        private bool _isEnabled;
        private bool _isPrimary;
        private string _label;
        private Visibility _visibility;

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public IconElement Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the command is available.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this command is a primary or secondary command.
        /// </summary>
        public bool IsPrimary
        {
            get { return _isPrimary; }
            set
            {
                _isPrimary = value;
                NotifyOfPropertyChange(() => IsPrimary);
            }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyOfPropertyChange(() => Label);
            }
        }

        /// <summary>
        /// Gets or sets the button visibility.
        /// </summary>
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        protected AppBarCommandViewModel()
        {
            _isPrimary = true;
            _isEnabled = true;
            _visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invokes this command.
        /// </summary>
        public abstract void Invoke();

        /// <summary>
        /// Updates the command state.
        /// </summary>
        public virtual void UpdateState() {}
    }
}