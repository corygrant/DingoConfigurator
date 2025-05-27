using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

        BlinkMarineButtonColor[] _valColors;
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

        BlinkMarineButtonColor _faultColor;
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

        BlinkMarineButtonColor[] _blinkingColors;
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

        BlinkMarineButtonColor _faultBlinkingColor;
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

        public Button(int keypadNum, int buttonNum):base(keypadNum, buttonNum)
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
                    Sent = false,
                    Received = false,
                    Prefix = Convert.ToInt16(MessagePrefix.Keypad),
                    Index = Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Request()
                    },
                    MsgDescription = $"Keypad{Number}"
                },
                new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = Convert.ToInt16(MessagePrefix.Keypad),
                    Index = Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = RequestLed()
                    },
                    MsgDescription = $"Keypad{Number}"
                }
            };

            return requests;
        }

        public override bool Receive(byte[] data)
        {
            MessagePrefix prefix = (MessagePrefix)data[0];
            switch(prefix)
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
                    Sent = false,
                    Received = false,
                    Prefix = Convert.ToInt16(MessagePrefix.Keypad),
                    Index = Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Write()
                    },
                    MsgDescription = $"Keypad{Number}"
                },
                new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = Convert.ToInt16(MessagePrefix.Keypad),
                    Index = Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = WriteLed()
                    },
                    MsgDescription = $"Keypad{Number}"
                }
            };

            return requests;
        }

        private byte[] Request()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButton);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) & 0xF8) >> 3);
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
            data[1] = Convert.ToByte(((KeypadNumber-1) & 0x07) + ((Number - 1) & 0xF8) >> 3);
            data[2] = Convert.ToByte(   Convert.ToByte(Enabled) + 
                                        Convert.ToByte(HasDial) << 1 + 
                                        Convert.ToByte(Mode) << 2);
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
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) & 0xF8) >> 3);
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
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadButtonLed);
            data[1] = Convert.ToByte(((KeypadNumber - 1) & 0x07) + ((Number - 1) & 0xF8) >> 3);
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
