using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanDevices.CanBoard;
using System.Windows;
using CanDevices.DingoPdm;
using System.Text.Json.Serialization;

namespace CanDevices.Keypad.BlinkMarine
{
    public class Keypad : KeypadBase
    {
        private List<BlinkMarine.Button> _buttons;
        public new List<BlinkMarine.Button> Buttons
        {
            get => _buttons;
            set
            {
                if (_buttons != value)
                {
                    _buttons = value;
                    OnPropertyChanged(nameof(Buttons));
                }
            }
        }

        private List<BlinkMarine.Dial> _dials;
        public List<BlinkMarine.Dial> Dials
        {
            get => _dials;
            set
            {
                if (_dials != value)
                {
                    _dials = value;
                    OnPropertyChanged(nameof(Dials));
                }
            }
        }

        private bool _startReceived;
        public bool StartReceived
        {
            get => _startReceived;
            set
            {
                if (_startReceived != value)
                {
                    _startReceived = value;
                    OnPropertyChanged(nameof(StartReceived));
                }
            }
        }

        private readonly Dictionary<int, Action<byte[]>> _messageHandlers;

        
        private BlinkMarineBacklightColor _backlightColor;
        [JsonPropertyName("backlightColor")]
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

        
        public Keypad()
        {
            _buttons = new List<BlinkMarine.Button>();
            _dials = new List<BlinkMarine.Dial>();

            _messageHandlers = new Dictionary<int, Action<byte[]>>
            {
                { Convert.ToInt16(MsgId.NMT), NMT },
                { Convert.ToInt16(MsgId.ButtonState), ButtonState },
                { Convert.ToInt16(MsgId.SetLed), SetLed },
                { Convert.ToInt16(MsgId.DialStateA), DialStateA },
                { Convert.ToInt16(MsgId.SetLedBlink), SetLedBlink },
                { Convert.ToInt16(MsgId.DialStateB), DialStateB },
                { Convert.ToInt16(MsgId.LedBrightness), LedBrightness },
                { Convert.ToInt16(MsgId.AnalogInput), AnalogInput },
                { Convert.ToInt16(MsgId.Backlight), Backlight },
                { Convert.ToInt16(MsgId.SdoResponse), SdoResponse },
                { Convert.ToInt16(MsgId.SdoRequest), SdoRequest },
                { Convert.ToInt16(MsgId.Heartbeat), Heartbeat }
            };
        }

        public new void Clear()
        {
            _buttons.Clear();
            _dials.Clear();
            _messageHandlers.Clear();
        }

        public new bool InIdRange(int id)
        {
            // Define a HashSet for quick lookup
            var validIds = new HashSet<int>
            {
                _baseId + Convert.ToInt16(MsgId.NMT),
                _baseId + Convert.ToInt16(MsgId.ButtonState),
                _baseId + Convert.ToInt16(MsgId.DialStateA),
                _baseId + Convert.ToInt16(MsgId.SetLedBlink),
                _baseId + Convert.ToInt16(MsgId.DialStateB),
                _baseId + Convert.ToInt16(MsgId.LedBrightness),
                _baseId + Convert.ToInt16(MsgId.AnalogInput),
                _baseId + Convert.ToInt16(MsgId.Backlight),
                _baseId + Convert.ToInt16(MsgId.SdoResponse),
                _baseId + Convert.ToInt16(MsgId.SdoRequest),
                _baseId + Convert.ToInt16(MsgId.Heartbeat)
            };

            // Check if the id exists in the set
            return validIds.Contains(id);
        }


        public new bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if(!InIdRange(id))
            {
                return false;
            }

            // Calculate the message offset from base ID
            int msgOffset = id - _baseId;

            if (_messageHandlers.TryGetValue(msgOffset, out var handler))
            {
                handler(data);
                return true;
            }

            return false;
        }

        private void NMT(byte[] data)
        {
            if (data[0] == 1)
                StartReceived = true;
        }

        private void ButtonState(byte[] data)
        {
            
        }

        private void SetLed(byte[] data)
        {

        }

        private void DialStateA(byte[] data)
        {
        }

        private void SetLedBlink(byte[] data)
        {
        }

        private void DialStateB(byte[] data)
        {
        }

        private void LedBrightness(byte[] data)
        {
        }

        private void AnalogInput(byte[] data)
        {
        }

        private void Backlight(byte[] data)
        {
        }

        private void SdoResponse(byte[] data)
        {
        }

        private void SdoRequest(byte[] data)
        {
        }

        private void Heartbeat(byte[] data)
        {

        }

        public static byte[] Request(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Keypad);
            data[1] = Convert.ToByte(index);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 5) return false;

            Enabled = Convert.ToBoolean(data[2] & 0x01);
            BaseId = data[3] & 0x7F;
            TimeoutEnabled = Convert.ToBoolean((data[3] & 0x80) >> 7);
            Timeout = data[4] * 100;

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Keypad);
            data[1] = Convert.ToByte(Number - 1);
            data[2] = Convert.ToByte(Enabled);
            data[3] = Convert.ToByte((BaseId & 0x7F) + Convert.ToByte(TimeoutEnabled) << 7);
            return data;
        }

        public static byte[] RequestLed(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadLed);
            data[1] = Convert.ToByte(index);
            return data;
        }

        public bool ReceiveLed(byte[] data)
        {
            if (data.Length != 8) return false;

            BacklightBrightness = data[2];
            BacklightColor = (BlinkMarineBacklightColor)data[3];
            DimBacklightBrightness = data[4];
            DimmingVar = (VarMap)data[5];
            ButtonBrightness = data[6];
            DimButtonBrightness = data[7];

            return true;
        }

        public byte[] WriteLed()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadLed);
            data[1] = Convert.ToByte(Number - 1);
            data[2] = Convert.ToByte(BacklightBrightness);
            data[3] = Convert.ToByte(BacklightColor);
            data[4] = Convert.ToByte(DimBacklightBrightness);
            data[5] = Convert.ToByte(DimmingVar);
            data[6] = Convert.ToByte(ButtonBrightness);
            data[7] = Convert.ToByte(DimButtonBrightness);

            return data;
        }

        

        private enum MsgId
        {
            NMT = 0x00,
            ButtonState = 0x180,
            SetLed = 0x200,
            DialStateA = 0x280,
            SetLedBlink = 0x300,
            DialStateB = 0x380,
            LedBrightness = 0x400,
            AnalogInput = 0x480,
            Backlight = 0x500,
            SdoResponse = 0x580,
            SdoRequest = 0x600,
            Heartbeat = 0x700
        }
    }

    
}
