using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace CanDevices.DingoPdm
{
    public class Flasher : NotifyPropertyChangedBase
    {
        private string _name;
        [JsonPropertyName("name")]
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
        [JsonPropertyName("number")]
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
        [JsonPropertyName("enabled")]
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
        [JsonPropertyName("single")]
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
        [JsonPropertyName("input")]
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
        [JsonPropertyName("inputValue")]
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
        [JsonPropertyName("output")]
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
        [JsonPropertyName("onTime")]
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
        [JsonPropertyName("offTime")]
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

    public class FlashTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 3000) return new ValidationResult(false, "Value must less than or equal to 3000");
            return new ValidationResult(true, null);
        }
    }
}
