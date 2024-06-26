﻿using System.Text.Json.Serialization;
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
