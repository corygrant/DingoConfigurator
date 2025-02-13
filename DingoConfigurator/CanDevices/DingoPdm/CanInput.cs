using System;
using System.Reflection;
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

        private bool _timeoutEnabled;
        [JsonPropertyName("timeoutEnabled")]
        public bool TimeoutEnabled
        {
            get => _timeoutEnabled;
            set
            {
                if (_timeoutEnabled != value)
                {
                    _timeoutEnabled = value;
                    OnPropertyChanged(nameof(TimeoutEnabled));
                }
            }
        }

        private double _timeout;
        [JsonPropertyName("timeout")]
        public double Timeout
        {
            get => _timeout;
            set
            {
                if (_timeout != value)
                {
                    _timeout = value;
                    OnPropertyChanged(nameof(Timeout));
                }
            }
        }

        private bool _ide;
        [JsonPropertyName("ide")]
        public bool Ide
        {
            get => _ide;
            set
            {
                if (_ide != value)
                {
                    _ide = value;
                    OnPropertyChanged(nameof(Ide));
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

                Ide = (_id > 2047);
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

        public static byte[] Request(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.CanInputs);
            data[1] = Convert.ToByte(index);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 7) return false;

            Enabled = Convert.ToBoolean(data[2] & 0x01);
            Mode = (InputMode)((data[2] & 0x06) >> 1);
            TimeoutEnabled = Convert.ToBoolean((data[2] & 0x08) >> 3);
            Operator = (Operator)((data[2] & 0xF0) >> 4);
            DLC = (data[3] & 0xF0) >> 4;
            StartingByte = (data[3] & 0x0F);
            OnVal = (data[4] << 8) + data[5];
            Timeout = data[6] / 10.0;
            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.CanInputs);
            data[1] = Convert.ToByte(Number - 1);
            data[2] = Convert.ToByte(((Convert.ToByte(Operator) & 0x0F) << 4) +
                      ((Convert.ToByte(Mode) & 0x03) << 1) +
                      Convert.ToByte((Convert.ToByte(TimeoutEnabled) << 3)) +
                      (Convert.ToByte(Enabled) & 0x01));
            data[3] = Convert.ToByte(((DLC & 0x0F) << 4) +
                      (Convert.ToByte(StartingByte) & 0x0F));
            data[4] = Convert.ToByte((OnVal & 0xFF00) >> 8); 
            data[5] = Convert.ToByte(OnVal & 0x00FF);
            data[6] = Convert.ToByte((Timeout * 10));
            return data;
        }

        public static byte[] RequestId(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.CanInputsId);
            data[1] = Convert.ToByte(index);
            return data;
        }

        public bool ReceiveId(byte[] data)
        {
            if (data.Length != 8) return false;

            Ide = Convert.ToBoolean((data[2] & 0x08) >> 3);

            if (Ide)
            {
                Id = ((data[4] & 0x1F) << 24) + (data[5] << 16) + (data[6] << 8) + data[7];
            }
            else
            {
                Id = ((data[2] & 0x07) << 8) + data[3];
            }

            return true;
        }

        public byte[] WriteId()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.CanInputsId);
            data[1] = Convert.ToByte(Number - 1);
            data[2] = Convert.ToByte((Convert.ToByte(Ide) << 3) +
                      ((Id >> 8) & 0x07));
            data[3] = Convert.ToByte(Id & 0xFF);
            data[4] = Convert.ToByte((Id >> 24) & 0x1F);
            data[5] = Convert.ToByte((Id >> 16) & 0xFF);
            data[6] = Convert.ToByte((Id >> 8) & 0xFF);
            data[7] = Convert.ToByte(Id & 0xFF);
            return data;
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
            if (proposedValue > 536870911) return new ValidationResult(false, "Value must less than or equal to 536870911");
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

    public class TimeoutValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 25.5) return new ValidationResult(false, "Value must less than or equal to 25.5");
            return new ValidationResult(true, null);
        }
    }
}
