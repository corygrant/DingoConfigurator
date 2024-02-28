using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class Output : NotifyPropertyChangedBase
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

        private double _current;
        [JsonIgnore]
        public double Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        private OutState _state;
        [JsonIgnore]
        public OutState State
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

        private double _currentLimit;
        public double CurrentLimit
        {
            get => _currentLimit;
            set
            {
                if (_currentLimit != value)
                {
                    _currentLimit = value;
                    OnPropertyChanged(nameof(CurrentLimit));
                }
            }
        }

        private int _resetCount;
        [JsonIgnore]
        public int ResetCount
        {
            get => _resetCount;
            set
            {
                if (_resetCount != value)
                {
                    _resetCount = value;
                    OnPropertyChanged(nameof(ResetCount));
                }
            }
        }

        private int _resetCountLimit;
        public int ResetCountLimit
        {
            get => _resetCountLimit;
            set
            {
                if (_resetCountLimit != value)
                {
                    _resetCountLimit = value;
                    OnPropertyChanged(nameof(ResetCountLimit));
                }
            }
        }

        private ResetMode _resetMode;
        public ResetMode ResetMode
        {
            get => _resetMode;
            set
            {
                if (value != _resetMode)
                {
                    _resetMode = value;
                    OnPropertyChanged(nameof(ResetMode));
                }
            }
        }

        private int _resetTime;
        public int ResetTime
        {
            get => _resetTime;
            set
            {
                if (_resetTime != value)
                {
                    _resetTime = value;
                    OnPropertyChanged(nameof(ResetTime));
                }
            }
        }

        private double _inrushCurrentLimit;
        public double InrushCurrentLimit
        {
            get => _inrushCurrentLimit;
            set
            {
                if (value != _inrushCurrentLimit)
                {
                    _inrushCurrentLimit = value;
                    OnPropertyChanged(nameof(InrushCurrentLimit));
                }
            }
        }

        private int _inrushTime;
        public int InrushTime
        {
            get => _inrushTime;
            set
            {
                if (value != _inrushTime)
                {
                    _inrushTime = value;
                    OnPropertyChanged(nameof(InrushTime));
                }
            }
        }

        private VarMap _input;
        public VarMap Input
        {
            get => _input;
            set
            {
                if (value != _input)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

    }
}
