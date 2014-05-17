using System;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Messages;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryFieldViewModel : PropertyChangedBase,
        IHandle<BackButtonPressedMessage>
    {
        private readonly IEventAggregator _events;
        private readonly IActivate _parent;
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
                if (value == _isExpanded)
                    return;

                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
                NotifyOfPropertyChange(() => SummaryViewVisibility);
                NotifyOfPropertyChange(() => ExpandedViewVisibility);

                if (_isExpanded)
                {
                    _events.PublishOnUIThread(
                        new EntryFieldExpandedMessage(this));
                }
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

        public EntryFieldViewModel(IActivate parent,
            IEventAggregator events)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (events == null) throw new ArgumentNullException("events");

            _parent = parent;
            _events = events;
            _events.Subscribe(this);
        }

        /// <summary>
        /// Expands this view.
        /// </summary>
        public void Expand()
        {
            IsExpanded = true;
        }

        public void Handle(BackButtonPressedMessage message)
        {
            if (!_parent.IsActive || !IsExpanded)
                return;

            IsExpanded = false;
            message.IsHandled = true;
        }
    }
}