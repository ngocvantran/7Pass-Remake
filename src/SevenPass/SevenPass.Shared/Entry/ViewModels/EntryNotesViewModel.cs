using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SevenPass.ViewModels;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryNotesViewModel : EntrySubViewModelBase
    {
        private readonly AppBarCommandViewModel[] _cmds;
        private float _fontSize;
        private string _notes;

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public float FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                NotifyOfPropertyChange(() => FontSize);

                _cmds.Apply(x => x.UpdateState());
            }
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public EntryNotesViewModel()
        {
            _fontSize = 15;
            DisplayName = "Notes";

            _cmds = new AppBarCommandViewModel[]
            {
                new IncreaseFontSizeCommand(this),
                new DecreaseFontSizeCommand(this),
            };
        }

        public override IEnumerable<AppBarCommandViewModel> GetCommands()
        {
            return _cmds;
        }

        protected override void OnActivate()
        {
            _cmds.Apply(x => x.Visibility = Visibility.Visible);
        }

        protected override void OnDeactivate(bool close)
        {
            _cmds.Apply(x => x.Visibility = Visibility.Collapsed);
        }

        protected override void Populate(XElement element)
        {
            Notes = element
                .Elements("String")
                .Where(x => (string)x.Element("Key") == "Notes")
                .Select(x => (string)x.Element("Value"))
                .FirstOrDefault();
        }

        public sealed class DecreaseFontSizeCommand : AppBarCommandViewModel
        {
            private readonly EntryNotesViewModel _parent;

            public DecreaseFontSizeCommand(EntryNotesViewModel parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");

                _parent = parent;
                Label = "decrease";
                Icon = new SymbolIcon(Symbol.FontDecrease);

                UpdateState();
            }

            public override void Invoke()
            {
                if (!IsEnabled)
                    return;
                _parent.FontSize -= 5;
            }

            public override void UpdateState()
            {
                IsEnabled = _parent.FontSize > 15;
            }
        }

        public sealed class IncreaseFontSizeCommand : AppBarCommandViewModel
        {
            private readonly EntryNotesViewModel _parent;

            public IncreaseFontSizeCommand(EntryNotesViewModel parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");

                _parent = parent;
                Label = "increase";
                Icon = new SymbolIcon(Symbol.FontIncrease);

                UpdateState();
            }

            public override void Invoke()
            {
                if (!IsEnabled)
                    return;

                _parent.FontSize += 5;
            }

            public override void UpdateState()
            {
                IsEnabled = _parent.FontSize < 50;
            }
        }
    }
}