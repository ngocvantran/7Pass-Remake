using System;
using Windows.UI.Xaml;
using Caliburn.Micro;

namespace SevenPass.Entry.ViewModels
{
    public class EntryFieldItemViewModel : PropertyChangedBase
    {
        private bool _isExpanded;
        private bool _isProtected;
        private string _key;
        private string _value;

        /// <summary>
        /// Gets the visibility of the expanded view.
        /// </summary>
        public Visibility ExpandedViewVisibility
        {
            get
            {
                return IsExpanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the visibility of the non-protected value text box.
        /// </summary>
        public Visibility ExposedValueVisibility
        {
            get
            {
                return IsProtected
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets value indicating whether this field is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
                NotifyOfPropertyChange(() => SummaryViewVisibility);
                NotifyOfPropertyChange(() => ExpandedViewVisibility);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is protected.
        /// </summary>
        public bool IsProtected
        {
            get { return _isProtected; }
            set
            {
                _isProtected = value;
                NotifyOfPropertyChange(() => IsProtected);
                NotifyOfPropertyChange(() => ExposedValueVisibility);
                NotifyOfPropertyChange(() => ProtectedValueVisibility);
            }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                NotifyOfPropertyChange(() => Key);
            }
        }

        /// <summary>
        /// Gets the visibility of the protected value password box.
        /// </summary>
        public Visibility ProtectedValueVisibility
        {
            get
            {
                return IsProtected
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public Visibility SummaryViewVisibility
        {
            get
            {
                return IsExpanded
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }
    }
}