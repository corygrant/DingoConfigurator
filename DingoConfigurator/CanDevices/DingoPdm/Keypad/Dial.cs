using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CanDevices.DingoPdm;

namespace CanDevices.DingoPdm
{
    public class Dial : NotifyPropertyChangedBase
    {
        private int _keypadNumber;
        [JsonPropertyName("keypadNumber")]
        public int KeypadNumber
        {
            get => _keypadNumber;
            set
            {
                if (_keypadNumber != value)
                {
                    _keypadNumber = value;
                    OnPropertyChanged(nameof(KeypadNumber));
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

        private int _minLed;
        [JsonPropertyName("minLed")]
        public int MinLed
        {
            get => _minLed;
            set
            {
                if (_minLed != value)
                {
                    _minLed = value;
                    OnPropertyChanged(nameof(MinLed));
                }
            }
        }

        private int _maxLed;
        [JsonPropertyName("maxLed")]
        public int MaxLed
        {
            get => _maxLed;
            set
            {
                if (_maxLed != value)
                {
                    _maxLed = value;
                    OnPropertyChanged(nameof(MaxLed));
                }
            }
        }

        private int _ledOffset;
        [JsonPropertyName("ledOffset")]
        public int LedOffset
        {
            get => _ledOffset;
            set
            {
                if (_ledOffset != value)
                {
                    _ledOffset = value;
                    OnPropertyChanged(nameof(LedOffset));
                }
            }
        }

        private int _ticks;
        [JsonIgnore]
        public int Ticks
        {
            get => _ticks;
            set
            {
                if (_ticks != value)
                {
                    _ticks = value;
                    OnPropertyChanged(nameof(Ticks));
                }
            }
        }

        private bool clockwise;
        [JsonIgnore]
        public bool Clockwise
        {
            get => clockwise;
            set
            {
                if (clockwise != value)
                {
                    clockwise = value;
                    OnPropertyChanged(nameof(Clockwise));
                }
            }
        }

        private bool _counterClockwise;
        [JsonIgnore]
        public bool CounterClockwise
        {
            get => _counterClockwise;
            set
            {
                if (_counterClockwise != value)
                {
                    _counterClockwise = value;
                    OnPropertyChanged(nameof(CounterClockwise));
                }
            }
        }

        private int _encoderTicks;
        [JsonIgnore]
        public int EncoderTicks
        {
            get => _encoderTicks;
            set
            {
                if (_encoderTicks != value)
                {
                    _encoderTicks = value;
                    OnPropertyChanged(nameof(EncoderTicks));
                }
            }
        }

        private int _maxEncoderTicks;
        [JsonIgnore]
        public int MaxEncoderTicks
        {
            get => _maxEncoderTicks;
            set
            {
                if (_maxEncoderTicks != value)
                {
                    _maxEncoderTicks = value;
                    OnPropertyChanged(nameof(MaxEncoderTicks));
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

        public Dial():this(1,1) {  }

        public Dial(int keypadNumber, int number)
        {
            KeypadNumber = keypadNumber;
            Number = number;
        }

        public static byte[] Request(int keypadIndex, int dialIndex)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadDial);
            data[1] = Convert.ToByte((keypadIndex & 0x07) + (dialIndex & 0xF8) >> 3);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 5) return false;

            MinLed = data[2];
            MaxLed = data[3];
            LedOffset = data[4];

            return true;
        }

        public byte[] Write(int keypadIndex, int dialIndex)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadDial);
            data[1] = Convert.ToByte((keypadIndex & 0x07) + (dialIndex & 0xF8) >> 3);
            data[2] = Convert.ToByte(MinLed);
            data[3] = Convert.ToByte(MaxLed);
            data[4] = Convert.ToByte(LedOffset);

            return data;
        }
    }
}
