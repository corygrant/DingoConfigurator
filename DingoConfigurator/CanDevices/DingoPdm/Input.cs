using System.Reflection;
using System;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace CanDevices.DingoPdm
{
    public class Input : NotifyPropertyChangedBase
    {
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

        private bool _state;
        [JsonIgnore]
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
        [JsonPropertyName("invertInput")]
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

        private int _debounceTime;
        [JsonPropertyName("debounceTime")]
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
        [JsonPropertyName("pull")]
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
            data[0] = Convert.ToByte(MessagePrefix.Inputs);
            data[1] = Convert.ToByte((index & 0x0F) << 4);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 4) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            InvertInput = Convert.ToBoolean((data[1] & 0x08) >> 3);
            Mode = (InputMode)((data[1] & 0x06) >> 1);
            DebounceTime = data[2] * 10;
            Pull = (InputPull)(data[3] & 0x03);

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Inputs);
            data[1] = Convert.ToByte((((Number - 1) & 0x0F) << 4) +
                      ((Convert.ToByte(InvertInput) & 0x01) << 3) +
                      ((Convert.ToByte(Mode) & 0x03) << 1) +
                      (Convert.ToByte(Enabled) & 0x01));
            data[2] = Convert.ToByte(DebounceTime / 10);
            data[3] = Convert.ToByte((byte)Pull & 0x03);
            return data;
        }

    }

    public class DebounceTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 500.00) return new ValidationResult(false, "Value must less than or equal to 500.0");
            return new ValidationResult(true, null);
        }
    }
}
