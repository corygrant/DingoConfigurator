using System;
using System.Reflection;
using System.Security.Policy;
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

        private double _onTime;
        [JsonPropertyName("onTime")]
        public double OnTime
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

        private double _offTime;
        [JsonPropertyName("offTime")]
        public double OffTime
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

        private bool _plot;
        [JsonPropertyName("plot")]
        public bool Plot
        {
            get => _plot;
            set
            {
                if (_plot != value)
                {
                    _plot = value;
                    OnPropertyChanged(nameof(Plot));
                }
            }
        }

        public static byte[] Request(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Flashers);
            data[1] = Convert.ToByte((index & 0x0F) << 4);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 6) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            Single = Convert.ToBoolean((data[1] & 0x02) >> 1);
            Input = (VarMap)(data[2]);
            OnTime = data[4] / 10.0;
            OffTime = data[5] / 10.0;

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Flashers);
            data[1] = Convert.ToByte((((Number - 1) & 0x0F) << 4) +
                      (Convert.ToByte(Single) << 1) +
                      (Convert.ToByte(Enabled)));
            data[2] = Convert.ToByte(Input);
            data[3] = 0;
            data[4] = Convert.ToByte(OnTime * 10);
            data[5] = Convert.ToByte(OffTime * 10);
            return data;
        }
    }

    public class FlashTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new LoggingValidationResult(new ValidationResult(false, "Entry is required"));
            if (!double.TryParse(input, out proposedValue)) return new LoggingValidationResult(new ValidationResult(false, "Response is invalid"));
            if (proposedValue < 0.00) return new LoggingValidationResult(new ValidationResult(false, "Value must be zero or greater"));
            if (proposedValue > 25.5) return new LoggingValidationResult(new ValidationResult(false, "Value must less than or equal to 25.5"));
            return new ValidationResult(true, null);
        }
    }
}
