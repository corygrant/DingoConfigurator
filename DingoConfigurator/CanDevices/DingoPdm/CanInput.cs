using System;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace CanDevices.DingoPdm
{
    public class CanInput : NotifyPropertyChangedBase
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

        private int _id;
        [JsonPropertyName("id")]
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
        [Obsolete]
        [JsonPropertyName("lowByte")]
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
        [Obsolete]
        [JsonPropertyName("highByte")]
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

        private int _startingByte;
        [JsonPropertyName("startingByte")]
        public int StartingByte
        {
            get => _startingByte;
            set
            {
                if (_startingByte != value)
                {
                    _startingByte = value;
                    OnPropertyChanged(nameof(StartingByte));
                }
            }
        }

        private int _dlc;
        [JsonPropertyName("dlc")]
        public int DLC
        {
            get => _dlc;
            set
            {
                if (_dlc != value)
                {
                    _dlc = value;
                    OnPropertyChanged(nameof(DLC));
                }
            }
        }

        private Operator _operator;
        [JsonPropertyName("operator")]
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
        [JsonPropertyName("onVal")]
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
        [JsonPropertyName("mode")]
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

    public class CanIdValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 2047) return new ValidationResult(false, "Value must less than or equal to 2047");
            return new ValidationResult(true, null);
        }
    }

    public class ByteValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 8) return new ValidationResult(false, "Value must less than or equal to 8");
            return new ValidationResult(true, null);
        }
    }

    public class ArgValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 65535) return new ValidationResult(false, "Value must less than or equal to 65535");
            return new ValidationResult(true, null);
        }
    }
}
