namespace CanDevices.DingoPdm
{
    public class Input : NotifyPropertyChangedBase
    {
        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private bool _invertInput;
        public bool InvertInput
        {
            get => _invertInput;
            set
            {
                if (_invertInput != value)
                {
                    _invertInput = value;
                    OnPropertyChanged(nameof(InvertInput));
                }
            }
        }

        private InputMode _mode;
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        private int _debounceTime;
        public int DebounceTime
        {
            get => _debounceTime;
            set
            {
                if (_debounceTime != value)
                {
                    _debounceTime = value;
                    OnPropertyChanged(nameof(DebounceTime));
                }
            }
        }

        private InputPull _inputPull;
        public InputPull Pull
        {
            get => _inputPull;
            set
            {
                if (_inputPull != value)
                {
                    _inputPull = value;
                    OnPropertyChanged(nameof(InputPull));
                }
            }
        }
    }
}
