using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class CanInput : NotifyPropertyChangedBase
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

        private int _value;
        [JsonIgnore]
        public int Value
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

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private int _lowByte;
        public int LowByte
        {
            get => _lowByte;
            set
            {
                if (_lowByte != value)
                {
                    _lowByte = value;
                    OnPropertyChanged(nameof(LowByte));
                }
            }
        }

        private int _highByte;
        public int HighByte
        {
            get => _highByte;
            set
            {
                if (_highByte != value)
                {
                    _highByte = value;
                    OnPropertyChanged(nameof(HighByte));
                }
            }
        }

        private Operator _operator;
        public Operator Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    OnPropertyChanged(nameof(Operator));
                }
            }
        }

        private int _onVal;
        public int OnVal
        {
            get => _onVal;
            set
            {
                if (_onVal != value)
                {
                    _onVal = value;
                    OnPropertyChanged(nameof(OnVal));
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
    }
}
