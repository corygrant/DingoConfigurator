using CanDevices.CanBoard;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace CanDevices.DingoPdm
{
    public class Output : NotifyPropertyChangedBase
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

        private int _currentLimit;
        [JsonPropertyName("currentLimit")]
        public int CurrentLimit
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
        [JsonPropertyName("resetCountLimit")]
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
        [JsonPropertyName("resetMode")]
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

        private double _resetTime;
        [JsonPropertyName("resetTime")]
        public double ResetTime
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

        private int _inrushCurrentLimit;
        [JsonPropertyName("inrushCurrentLimit")]
        public int InrushCurrentLimit
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

        private double _inrushTime;
        [JsonPropertyName("inrushTime")]
        public double InrushTime
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
        [JsonPropertyName("input")]
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

        public static byte[] Request(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Outputs);
            data[1] = Convert.ToByte((index & 0x0F) << 4);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 8) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            Input = (VarMap)(data[2]);
            CurrentLimit = data[3];
            ResetCountLimit = (data[4] & 0xF0) >> 4;
            ResetMode = (ResetMode)(data[4] & 0x0F);
            ResetTime = data[5] / 10.0;
            InrushCurrentLimit = data[6];
            InrushTime = data[7] / 10.0;

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Outputs);
            data[1] = Convert.ToByte((((Number - 1) & 0x0F) << 4) +
                      (Convert.ToByte(Enabled) & 0x01));
            data[2] = Convert.ToByte(Input);
            data[3] = Convert.ToByte(CurrentLimit);
            data[4] = Convert.ToByte((Convert.ToByte(ResetCountLimit) << 4) +
                      (Convert.ToByte(ResetMode) & 0x0F));
            data[5] = Convert.ToByte(ResetTime * 10);
            data[6] = Convert.ToByte(InrushCurrentLimit);
            data[7] = Convert.ToByte(InrushTime * 10);
            return data;
        }

    }

    public class CurrentLimitValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 20.00) return new ValidationResult(false, "Value must less than or equal to 20.0");
            return new ValidationResult(true, null);
        }
    }

    public class ResetLimitValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 15.00) return new ValidationResult(false, "Value must less than or equal to 15");
            return new ValidationResult(true, null);
        }
    }

    public class ResetTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 25.50) return new ValidationResult(false, "Value must less than or equal to 25.5");
            return new ValidationResult(true, null);
        }
    }

    public class InrushLimitValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 250.00) return new ValidationResult(false, "Value must less than or equal to 250.0");
            return new ValidationResult(true, null);
        }
    }

    public class InrushTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 25.50) return new ValidationResult(false, "Value must less than or equal to 25.5");
            return new ValidationResult(true, null);
        }
    }
}
