using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class Flasher : NotifyPropertyChangedBase
    {
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

        private bool _value;
        [JsonIgnore]
        public bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

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

        private bool _single;
        public bool Single
        {
            get => _single;
            set
            {
                if (_single != value)
                {
                    _single = value;
                    OnPropertyChanged(nameof(Single));
                }
            }
        }

        private VarMap _input;
        public VarMap Input
        {
            get => _input;
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        private bool _inputValue;
        public bool InputValue
        {
            get => _inputValue;
            set
            {
                if (_inputValue != value)
                {
                    _inputValue = value;
                    OnPropertyChanged(nameof(InputValue));
                }
            }
        }

        private VarMap _output;
        public VarMap Output
        {
            get => _output;
            set
            {
                if (_output != value)
                {
                    _output = value;
                    OnPropertyChanged(nameof(Output));
                }
            }
        }

        private int _onTime;
        public int OnTime
        {
            get => _onTime;
            set
            {
                if (_onTime != value)
                {
                    _onTime = value;
                    OnPropertyChanged(nameof(OnTime));
                }
            }
        }

        private int _offTime;
        public int OffTime
        {
            get => _offTime;
            set
            {
                if (_offTime != value)
                {
                    _offTime = value;
                    OnPropertyChanged(nameof(OffTime));
                }
            }
        }
    }
}
