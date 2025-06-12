using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CanDevices.DingoPdm;
using CanInterfaces;

namespace CanDevices.DingoPdm
{
    public class Button : ButtonBase
    {

        private int _numValColors;
        [JsonPropertyName("numValColors")]
        public int NumValColors
        {
            get => _numValColors;
            set
            {
                if (_numValColors != value)
                {
                    _numValColors = value;
                    OnPropertyChanged(nameof(NumValColors));
                }
            }
        }

        private BlinkMarineButtonColor[] _valColors;
        [JsonPropertyName("valColors")]
        public BlinkMarineButtonColor[] ValColors
        {
            get => _valColors;
            set
            {
                if (_valColors != value)
                {
                    _valColors = value;
                    OnPropertyChanged(nameof(ValColors));
                }
            }
        }

        private BlinkMarineButtonColor _faultColor;
        [JsonPropertyName("faultColor")]
        public BlinkMarineButtonColor FaultColor
        {
            get => _faultColor;
            set
            {
                if (_faultColor != value)
                {
                    _faultColor = value;
                    OnPropertyChanged(nameof(FaultColor));
                }
            }
        }

        private BlinkMarineButtonColor[] _blinkingColors;
        [JsonPropertyName("blinkingColors")]
        public BlinkMarineButtonColor[] BlinkingColors
        {
            get => _blinkingColors;
            set
            {
                if (_blinkingColors != value)
                {
                    _blinkingColors = value;
                    OnPropertyChanged(nameof(BlinkingColors));
                }
            }
        }

        private BlinkMarineButtonColor _faultBlinkingColor;
        [JsonPropertyName("faultBlinkingColor")]
        public BlinkMarineButtonColor FaultBlinkingColor
        {
            get => _faultBlinkingColor;
            set
            {
                if (_faultBlinkingColor != value)
                {
                    _faultBlinkingColor = value;
                    OnPropertyChanged(nameof(FaultBlinkingColor));
                }
            }
        }

        private BlinkMarineButtonColor _activeColor;

        private Brush _activeBrush;
        [JsonIgnore]
        public Brush ActiveBrush
        {
            get => _activeBrush;
            set
            {
                if (_activeBrush != value)
                {
                    _activeBrush = value;
                    OnPropertyChanged(nameof(ActiveBrush));
                }
            }
        }


        private BlinkMarineButtonColor _activeBlinkingColor;

        private Brush _activeBlinkingBrush;
        [JsonIgnore]
        public Brush ActiveBlinkingBrush
        {
            get => _activeBlinkingBrush;
            set
            {
                if (_activeBlinkingBrush != value)
                {
                    _activeBlinkingBrush = value;
                    OnPropertyChanged(nameof(ActiveBlinkingBrush));
                }
            }
        }

        public void SetActiveColorRed(bool val)
        {
            if (val)
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) | (1 << 0)); // Set bit 0
            else
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) & ~(1 << 0)); // Clear bit 0
        }

        public void SetActiveColorGreen(bool val)
        {
            if (val)
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) | (1 << 1)); // Set bit 1
            else
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) & ~(1 << 1)); // Clear bit 1
        }

        public void SetActiveColorBlue(bool val)
        {
            if (val)
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) | (1 << 2)); // Set bit 2
            else
                _activeColor = (BlinkMarineButtonColor)(((int)_activeColor) & ~(1 << 2)); // Clear bit 2
        }

        public void UpdateActiveColor()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => UpdateActiveColor());
                return;
            }

            var brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);

            var color = Colors.Black;
            if (_activeColor != BlinkMarineButtonColor.Off)
                color = ButtonColorToColor(_activeColor);
            else
                color = BacklightColorToColor(BacklightColor);

            var blinkColor = color;
            if (_activeBlinkingColor != BlinkMarineButtonColor.Off)
            {
                blinkColor = ButtonColorToColor((BlinkMarineButtonColor)(Convert.ToByte(_activeColor) ^ Convert.ToByte(_activeBlinkingColor)));
            }


            brush.GradientStops.Add(new GradientStop(color, 0.5));
            brush.GradientStops.Add(new GradientStop(blinkColor, 0.5));
            ActiveBrush = brush;
        }

        protected Color ButtonColorToColor(BlinkMarineButtonColor color)
        {
            switch (color)
            {
                case BlinkMarineButtonColor.Off:
                    return Colors.Gray;

                case BlinkMarineButtonColor.Red:
                    return Colors.Red;

                case BlinkMarineButtonColor.Green:
                    return Colors.Lime;

                case BlinkMarineButtonColor.Blue:
                    return Colors.Blue;

                case BlinkMarineButtonColor.Orange:
                    return Colors.Orange;

                case BlinkMarineButtonColor.Violet:
                    return Colors.DarkViolet;

                case BlinkMarineButtonColor.Cyan:
                    return Colors.Cyan;

                case BlinkMarineButtonColor.White:
                    return Colors.White;

                default:
                    return Colors.Black;
            }
        }

        protected Color BacklightColorToColor(BlinkMarineBacklightColor color)
        {
            switch (color)
            {
                case BlinkMarineBacklightColor.Off:
                    return Colors.Gray;

                case BlinkMarineBacklightColor.Red:
                    return Colors.Red;

                case BlinkMarineBacklightColor.Green:
                    return Colors.Lime;

                case BlinkMarineBacklightColor.Blue:
                    return Colors.Blue;

                case BlinkMarineBacklightColor.Yellow:
                    return Colors.Yellow;

                case BlinkMarineBacklightColor.Cyan:
                    return Colors.Cyan;

                case BlinkMarineBacklightColor.Violet:
                    return Colors.DarkViolet;

                case BlinkMarineBacklightColor.White:
                    return Colors.White;

                case BlinkMarineBacklightColor.Amber:
                    return Colors.Gold;

                case BlinkMarineBacklightColor.YellowGreen:
                    return Colors.GreenYellow;
            }

            return Colors.Black;
        }

        public void SetActiveBlinkingColorRed(bool val)
        {
            if (val)
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) | (1 << 0)); // Set bit 0
            else
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) & ~(1 << 0)); // Clear bit 0
        }

        public void SetActiveBlinkingColorGreen(bool val)
        {
            if (val)
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) | (1 << 1)); // Set bit 1
            else
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) & ~(1 << 1)); // Clear bit 1
        }

        public void SetActiveBlinkingColorBlue(bool val)
        {
            if (val)
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) | (1 << 2)); // Set bit 2
            else
                _activeBlinkingColor = (BlinkMarineButtonColor)(((int)_activeBlinkingColor) & ~(1 << 2)); // Clear bit 2
        }


        private BlinkMarineBacklightColor _backlightColor;
        [JsonIgnore]
        public BlinkMarineBacklightColor BacklightColor
        {
            get => _backlightColor;
            set
            {
                if (_backlightColor != value)
                {
                    _backlightColor = value;
                    OnPropertyChanged(nameof(BacklightColor));
                }
            }
        }

        public Button()
        {
            _valColors = new BlinkMarineButtonColor[4];
            _blinkingColors = new BlinkMarineButtonColor[4];
        }

        public Button(int keypadNumber, int number) : base(keypadNumber, number)
        {
            _valColors = new BlinkMarineButtonColor[4];
            _blinkingColors = new BlinkMarineButtonColor[4];
        }

        public override List<CanDeviceResponse> RequestMsgs(int id)
        {
            List<CanDeviceResponse> requests = new List<CanDeviceResponse>
            {
                new CanDeviceResponse
                {
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadButton),
                    Index = ((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3),
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Request()
                    },
                    MsgDescription = $"Keypad{KeypadNumber} Button{Number}"
                },
                new CanDeviceResponse
                {
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadButtonLed),
                    Index = ((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3),
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = RequestLed()
                    },
                    MsgDescription = $"Keypad{KeypadNumber} ButtonLed{Number}"
                }
            };

            return requests;
        }

        public override bool Receive(byte[] data)
        {
            var prefix = (MessagePrefix)(data[0] - 128);
            switch (prefix)
            {
                case MessagePrefix.KeypadButton:
                    return ReceiveButton(data);
                case MessagePrefix.KeypadButtonLed:
                    return ReceiveLed(data);
                default:
                    return false;
            }
        }

        public override List<CanDeviceResponse> WriteMsgs(int id)
        {
            List<CanDeviceResponse> requests = new List<CanDeviceResponse>
            {
                new CanDeviceResponse
                {
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadButton),
                    Index = ((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3),
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = Write()
                    },
                    MsgDescription = $"Keypad{KeypadNumber} Button{Number}"
                },
                new CanDeviceResponse
                {
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadButtonLed),
                    Index = ((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3),
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = WriteLed()
                    },
                    MsgDescription = $"Keypad{KeypadNumber} ButtonLed{Number}"
                }
            };

            return requests;
        }

        private byte[] Request()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButton);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3));
            return data;
        }

        private bool ReceiveButton(byte[] data)
        {
            if (data.Length != 8) return false;

            Enabled = Convert.ToBoolean(data[2] & 0x01);
            HasDial = Convert.ToBoolean((data[2] & 0x02) >> 1);
            Mode = (InputMode)((data[2] & 0x0C) >> 2);

            ValVars[0] = (VarMap)data[3];
            ValVars[1] = (VarMap)data[4];
            ValVars[2] = (VarMap)data[5];
            ValVars[3] = (VarMap)data[6];
            FaultVar = (VarMap)data[7];

            return true;
        }

        private byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButton);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3));
            data[2] = Convert.ToByte(Convert.ToByte(Enabled) +
                                        (Convert.ToByte(HasDial) << 1) +
                                        (Convert.ToByte(Mode) << 2));
            data[3] = Convert.ToByte(ValVars[0]);
            data[4] = Convert.ToByte(ValVars[1]);
            data[5] = Convert.ToByte(ValVars[2]);
            data[6] = Convert.ToByte(ValVars[3]);
            data[7] = Convert.ToByte(FaultVar);

            return data;
        }

        private byte[] RequestLed()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButtonLed);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3));
            return data;
        }

        private bool ReceiveLed(byte[] data)
        {
            if (data.Length != 8) return false;

            ValColors[0] = (BlinkMarineButtonColor)(data[2] & 0x0F);
            ValColors[1] = (BlinkMarineButtonColor)((data[2] & 0xF0) >> 4);
            ValColors[2] = (BlinkMarineButtonColor)(data[3] & 0x0F);
            ValColors[3] = (BlinkMarineButtonColor)((data[3] & 0xF0) >> 4);
            FaultColor = (BlinkMarineButtonColor)(data[4] & 0x0F);
            NumValColors = (data[4] & 0xF0) >> 4;
            BlinkingColors[0] = (BlinkMarineButtonColor)(data[5] & 0x0F);
            BlinkingColors[1] = (BlinkMarineButtonColor)((data[5] & 0xF0) >> 4);
            BlinkingColors[2] = (BlinkMarineButtonColor)(data[6] & 0x0F);
            BlinkingColors[3] = (BlinkMarineButtonColor)((data[6] & 0xF0) >> 4);
            FaultBlinkingColor = (BlinkMarineButtonColor)(data[7] & 0x0F);

            return true;
        }

        private byte[] WriteLed()
        {
            NumValColors = 4;

            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButtonLed);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) << 3));
            data[2] = Convert.ToByte((Convert.ToByte(ValColors[0]) & 0x0F) +
                                      (Convert.ToByte(ValColors[1]) << 4));
            data[3] = Convert.ToByte((Convert.ToByte(ValColors[2]) & 0x0F) +
                                      (Convert.ToByte(ValColors[3]) << 4));
            data[4] = Convert.ToByte((Convert.ToByte(FaultColor) & 0x0F) +
                                      (Convert.ToByte(NumValColors) << 4));
            data[5] = Convert.ToByte((Convert.ToByte(BlinkingColors[0]) & 0x0F) +
                                      (Convert.ToByte(BlinkingColors[1]) << 4));
            data[6] = Convert.ToByte((Convert.ToByte(BlinkingColors[2]) & 0x0F) +
                                      (Convert.ToByte(BlinkingColors[3]) << 4));
            data[7] = Convert.ToByte((Convert.ToByte(FaultBlinkingColor) & 0x0F));

            return data;
        }
    }


}
