using CanDevices.DingoPdm;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace CanDevices.DingoPdm
{
    public class Wiper : NotifyPropertyChangedBase
    {
        public Wiper()
        {
            SpeedMap = new WiperSpeed[8];
            IntermitTime = new double[6];
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

        private bool _slowState;
        [JsonIgnore]
        public bool SlowState
        {
            get => _slowState;
            set
            {
                if (_slowState != value)
                {
                    _slowState = value;
                    OnPropertyChanged(nameof(SlowState));
                }
            }
        }

        private bool _fastState;
        [JsonIgnore]
        public bool FastState
        {
            get => _fastState;
            set
            {
                if (_fastState != value)
                {
                    _fastState = value;
                    OnPropertyChanged(nameof(FastState));
                }
            }
        }

        private WiperMode _mode;
        [JsonPropertyName("mode")]
        public WiperMode Mode
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

        private WiperState _state;
        [JsonIgnore]
        public WiperState State
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

        private WiperSpeed _speed;
        [JsonIgnore]
        public WiperSpeed Speed
        {
            get => _speed;
            set
            {
                if (value != _speed)
                {
                    _speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }
        private VarMap _slowInput;
        [JsonPropertyName("slowInput")]
        public VarMap SlowInput
        {
            get => _slowInput;
            set
            {
                if (_slowInput != value)
                {
                    _slowInput = value;
                    OnPropertyChanged(nameof(SlowInput));
                }
            }
        }

        private VarMap _fastInput;
        [JsonPropertyName("fastInput")]
        public VarMap FastInput
        {
            get => _fastInput;
            set
            {
                if (_fastInput != value)
                {
                    _fastInput = value;
                    OnPropertyChanged(nameof(FastInput));
                }
            }
        }

        private VarMap _interInput;
        [JsonPropertyName("interInput")]
        public VarMap InterInput
        {
            get => _interInput;
            set
            {
                if (_interInput != value)
                {
                    _interInput = value;
                    OnPropertyChanged(nameof(InterInput));
                }
            }
        }

        private VarMap _onInput;
        [JsonPropertyName("onInput")]   
        public VarMap OnInput
        {
            get => _onInput;
            set
            {
                if (_onInput != value)
                {
                    _onInput = value;
                    OnPropertyChanged(nameof(OnInput));
                }
            }
        }

        private VarMap _speedInput;
        [JsonPropertyName("speedInput")]
        public VarMap SpeedInput
        {
            get => _speedInput;
            set
            {
                if (_speedInput != value)
                {
                    _speedInput = value;
                    OnPropertyChanged(nameof(SpeedInput));
                }
            }
        }

        private VarMap _parkInput;
        [JsonPropertyName("parkInput")]
        public VarMap ParkInput
        {
            get => _parkInput;
            set
            {
                if (_parkInput != value)
                {
                    _parkInput = value;
                    OnPropertyChanged(nameof(ParkInput));
                }
            }
        }

        private bool _parkStopLevel;
        [JsonPropertyName("parkStopLevel")]
        public bool ParkStopLevel
        {
            get => _parkStopLevel;
            set
            {
                if (_parkStopLevel != value)
                {
                    _parkStopLevel = value;
                    OnPropertyChanged(nameof(ParkStopLevel));
                }
            }
        }

        private VarMap _swipeInput;
        [JsonPropertyName("swipeInput")]
        public VarMap SwipeInput
        {
            get => _swipeInput;
            set
            {
                if (_swipeInput != value)
                {
                    _swipeInput = value;
                    OnPropertyChanged(nameof(SwipeInput));
                }
            }
        }

        private VarMap _washInput;
        [JsonPropertyName("washInput")]
        public VarMap WashInput
        {
            get => _washInput;
            set
            {
                if (_washInput != value)
                {
                    _washInput = value;
                    OnPropertyChanged(nameof(WashInput));
                }
            }
        }

        private int _washWipeCycles;
        [JsonPropertyName("washWipeCycles")]
        public int WashWipeCycles
        {
            get => _washWipeCycles;
            set
            {
                if (_washWipeCycles != value)
                {
                    _washWipeCycles = value;
                    OnPropertyChanged(nameof(WashWipeCycles));
                }
            }
        }

        private WiperSpeed[] _speedMap;
        [JsonPropertyName("speedMap")]
        public WiperSpeed[] SpeedMap
        {
            get => _speedMap;
            set
            {
                if (_speedMap != value)
                {
                    _speedMap = value;
                    OnPropertyChanged(nameof(SpeedMap));
                }
            }
        }

        private double[] _intermitTime;
        [JsonPropertyName("intermitTime")]
        public double[] IntermitTime
        {
            get => _intermitTime;
            set
            {
                if (_intermitTime != value)
                {
                    _intermitTime = value;
                    OnPropertyChanged(nameof(IntermitTime));
                }
            }
        }

        public static byte[] Request()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Wiper);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 8) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            Mode = (WiperMode)((data[1] & 0x06) >> 1);
            ParkStopLevel = Convert.ToBoolean((data[1] & 0x08) >> 3);
            WashWipeCycles = (data[1] & 0xF0) >> 4;
            SlowInput = (VarMap)data[2];
            FastInput = (VarMap)data[3];
            InterInput = (VarMap)data[4];
            OnInput = (VarMap)data[5];
            ParkInput = (VarMap)data[6];
            WashInput = (VarMap)data[7];

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Wiper);
            data[1] = Convert.ToByte(((Convert.ToByte(WashWipeCycles) & 0x0F) << 4) +
                      ((Convert.ToByte(ParkStopLevel) & 0x01) << 3) +
                      ((Convert.ToByte(Mode) & 0x03) << 1) +
                      (Convert.ToByte(Enabled) & 0x01));
            data[2] = Convert.ToByte(SlowInput);
            data[3] = Convert.ToByte(FastInput);
            data[4] = Convert.ToByte(InterInput);
            data[5] = Convert.ToByte(OnInput);
            data[6] = Convert.ToByte(ParkInput);
            data[7] = Convert.ToByte(WashInput);
            return data;
        }

        public static byte[] RequestSpeed()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.WiperSpeed);
            return data;
        }

        public bool ReceiveSpeed(byte[] data)
        {
            if (data.Length != 7) return false;

            SwipeInput = (VarMap)data[1];
            SpeedInput = (VarMap)data[2];
            SpeedMap[0] = (WiperSpeed)(data[3] & 0x0F);
            SpeedMap[1] = (WiperSpeed)((data[3] & 0xF0) >> 4);
            SpeedMap[2] = (WiperSpeed)(data[4] & 0x0F);
            SpeedMap[3] = (WiperSpeed)((data[4] & 0xF0) >> 4);
            SpeedMap[4] = (WiperSpeed)(data[5] & 0x0F);
            SpeedMap[5] = (WiperSpeed)((data[5] & 0xF0) >> 4);
            SpeedMap[6] = (WiperSpeed)(data[6] & 0x0F);
            SpeedMap[7] = (WiperSpeed)((data[6] & 0xF0) >> 4);

            return true;
        }

        public byte[] WriteSpeed()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.WiperSpeed);
            data[1] = Convert.ToByte(SwipeInput);
            data[2] = Convert.ToByte(SpeedInput);
            data[3] = Convert.ToByte(((Convert.ToByte(SpeedMap[1]) & 0x0F) << 4) +
                      (Convert.ToByte(SpeedMap[0]) & 0x0F));
            data[4] = Convert.ToByte(((Convert.ToByte(SpeedMap[3]) & 0x0F) << 4) +
                      (Convert.ToByte(SpeedMap[2]) & 0x0F));
            data[5] = Convert.ToByte(((Convert.ToByte(SpeedMap[4]) & 0x0F) << 4) +
                      (Convert.ToByte(SpeedMap[5]) & 0x0F));
            data[6] = Convert.ToByte(((Convert.ToByte(SpeedMap[7]) & 0x0F) << 4) +
                      (Convert.ToByte(SpeedMap[6]) & 0x0F));
                    
            return data;
        }

        public static byte[] RequestDelays()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.WiperDelays);
            return data;
        }

        public bool ReceiveDelays(byte[] data)
        {
            if (data.Length != 7) return false;

            IntermitTime[0] = data[1] / 10.0;
            IntermitTime[1] = data[2] / 10.0;
            IntermitTime[2] = data[3] / 10.0;
            IntermitTime[3] = data[4] / 10.0;
            IntermitTime[4] = data[5] / 10.0;
            IntermitTime[5] = data[6] / 10.0;

            return true;
        }

        public byte[] WriteDelays()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.WiperDelays);
            data[1] = Convert.ToByte(IntermitTime[0] * 10);
            data[2] = Convert.ToByte(IntermitTime[1] * 10);
            data[3] = Convert.ToByte(IntermitTime[2] * 10);
            data[4] = Convert.ToByte(IntermitTime[3] * 10);
            data[5] = Convert.ToByte(IntermitTime[4] * 10);
            data[6] = Convert.ToByte(IntermitTime[5] * 10);
                   
            return data;
        }
    }
    public class WipeCyclesValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 0.00) return new ValidationResult(false, "Value must be zero or greater");
            if (proposedValue > 15) return new ValidationResult(false, "Value must less than or equal to 15");
            return new ValidationResult(true, null);
        }
    }

    public class IntermitTimeValidationRule : ValidationRule
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
