using CanDevices.CanBoard;
using System;
using System.Collections.Generic;
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

        private bool _pwmEnabled;
        [JsonPropertyName("pwmEnabled")]
        public bool PwmEnabled
        {
            get => _pwmEnabled;
            set
            {
                if (value != _pwmEnabled)
                {
                    _pwmEnabled = value;
                    OnPropertyChanged(nameof(PwmEnabled));
                }
            }
        }

        private bool _softStartEnabled;
        [JsonPropertyName("softStartEnabled")]
        public bool SoftStartEnabled
        {
            get => _softStartEnabled;
            set
            {
                if (value != _softStartEnabled)
                {
                    _softStartEnabled = value;
                    OnPropertyChanged(nameof(SoftStartEnabled));
                }
            }
        }

        private bool _variableDutyCycle;
        [JsonPropertyName("variableDutyCycle")]
        public bool VariableDutyCycle
        {
            get => _variableDutyCycle;
            set
            {
                if (value != _variableDutyCycle)
                {
                    _variableDutyCycle = value;
                    OnPropertyChanged(nameof(VariableDutyCycle));
                }
            }
        }

        private VarMap _dutyCycleInput;
        [JsonPropertyName("dutyCycleInput")]
        public VarMap DutyCycleInput
        {
            get => _dutyCycleInput;
            set
            {
                if (value != _dutyCycleInput)
                {
                    _dutyCycleInput = value;
                    OnPropertyChanged(nameof(DutyCycleInput));
                }
            }
        }

        private int _fixedDutyCycle;
        [JsonPropertyName("fixedDutyCycle")]
        public int FixedDutyCycle
        {
            get => _fixedDutyCycle;
            set
            {
                if (value != _fixedDutyCycle)
                {
                    _fixedDutyCycle = value;
                    OnPropertyChanged(nameof(FixedDutyCycle));
                }
            }
        }

        private int _pwmFrequency;
        [JsonPropertyName("pwmFrequency")]
        public int PwmFrequency
        {
            get => _pwmFrequency;
            set
            {
                if (value != _pwmFrequency)
                {
                    _pwmFrequency = value;
                    OnPropertyChanged(nameof(PwmFrequency));
                }
            }
        }

        private int _softStartRampTime;
        [JsonPropertyName("softStartRampTime")]
        public int SoftStartRampTime
        {
            get => _softStartRampTime;
            set
            {
                if (value != _softStartRampTime)
                {
                    _softStartRampTime = value;
                    OnPropertyChanged(nameof(SoftStartRampTime));
                }
            }
        }

        private int _dutyCycleDenominator;
        [JsonPropertyName("dutyCycleDenominator")]
        public int DutyCycleDenominator
        {
            get => _dutyCycleDenominator;
            set
            {
                if (value != _dutyCycleDenominator)
                {
                    _dutyCycleDenominator = value;
                    OnPropertyChanged(nameof(DutyCycleDenominator));
                }
            }
        }

        private double _currentDutyCycle;
        [JsonIgnore]
        public double CurrentDutyCycle
        {
            get => _currentDutyCycle;
            set
            {
                if (value != _currentDutyCycle)
                {
                    _currentDutyCycle = value;
                    CalculatedPower = CalcPower(CurrentDutyCycle);
                    OnPropertyChanged(nameof(CurrentDutyCycle));
                }
            }
        }

        private double _calculatedPower;
        [JsonIgnore]
        public double CalculatedPower
        {
            get => _calculatedPower;
            set
            {
                if (value != _calculatedPower)
                {
                    _calculatedPower = value;
                    OnPropertyChanged(nameof(CalculatedPower));
                }
            }
        }

        private double CalcPower(double dc)
        {
            return (dc / 100) * Current;
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

        public static byte[] RequestPwm(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.OutputsPwm);
            data[1] = Convert.ToByte((index & 0x0F) << 4);
            return data;
        }

        public bool ReceivePwm(byte[] data)
        {
            if (data.Length != 8) return false;

            PwmEnabled = Convert.ToBoolean(data[1] & 0x01);
            SoftStartEnabled = Convert.ToBoolean((data[1] & 0x02) >> 1);
            VariableDutyCycle = Convert.ToBoolean((data[1] & 0x04) >> 2);
            DutyCycleInput = (VarMap)data[2];
            PwmFrequency = (data[3] << 1) + (data[4] & 0x01);
            FixedDutyCycle = (data[4] & 0xFE) >> 1;
            SoftStartRampTime = (data[5] << 8) + data[6];
            DutyCycleDenominator = data[7];

            return true;
        }

        public byte[] WritePwm()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.OutputsPwm);
            data[1] = Convert.ToByte((((Number - 1) & 0x0F) << 4) +
                      ((Convert.ToByte(VariableDutyCycle) & 0x01) << 2) +
                      ((Convert.ToByte(SoftStartEnabled) & 0x01) << 1) +
                      (Convert.ToByte(PwmEnabled) & 0x01));
            data[2] = Convert.ToByte(DutyCycleInput);
            data[3] = Convert.ToByte(PwmFrequency >> 1);
            data[4] = Convert.ToByte((PwmFrequency & 0x01) +
                      ((FixedDutyCycle & 0x7F) << 1));
            data[5] = Convert.ToByte(SoftStartRampTime >> 8);
            data[6] = Convert.ToByte(SoftStartRampTime & 0xFF);
            data[7] = Convert.ToByte(DutyCycleDenominator);

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
    public class DutyCycleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 1) return new ValidationResult(false, "Value must be 1 or greater");
            if (proposedValue > 100) return new ValidationResult(false, "Value must less than or equal to 100");
            return new ValidationResult(true, null);
        }
    }

    public class FrequencyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 1) return new ValidationResult(false, "Value must be 1 or greater");
            if (proposedValue > 400) return new ValidationResult(false, "Value must less than or equal to 400");
            return new ValidationResult(true, null);
        }
    }

    public class SoftStartTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 1) return new ValidationResult(false, "Value must be 1 or greater");
            if (proposedValue > 2000) return new ValidationResult(false, "Value must less than or equal to 2000");
            return new ValidationResult(true, null);
        }
    }

    public class DutyCycleDenomValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double proposedValue;
            string input = value.ToString();
            if (input == string.Empty) return new ValidationResult(false, "Entry is required");
            if (!double.TryParse(input, out proposedValue)) return new ValidationResult(false, "Response is invalid");
            if (proposedValue < 1) return new ValidationResult(false, "Value must be 1 or greater");
            if (proposedValue > 5000) return new ValidationResult(false, "Value must less than or equal to 5000");
            return new ValidationResult(true, null);
        }
    }
}
