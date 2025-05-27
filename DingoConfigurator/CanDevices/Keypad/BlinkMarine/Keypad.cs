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
using CanInterfaces;
using System.Reflection;

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

        
        public Keypad(KeypadModel model, string name, ICanDevice canDevice) : base(name, canDevice)
        {
            BaseId = 0x15;

            _buttons = new List<BlinkMarine.Button>();
            _dials = new List<BlinkMarine.Dial>();

            ModelUpdate(model);

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

        protected override void ModelUpdate(KeypadModel model)
        {
            Buttons.Clear();
            Dials.Clear();

            int btnCount = 0;
            int dialCount = 0;
            switch (model)
            {
                case KeypadModel.Blink2Key:
                    btnCount = 2;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink4Key:
                    btnCount = 4;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink6Key:
                    btnCount = 6;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink8Key:
                    btnCount = 8;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink10Key:
                    btnCount = 10;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink12Key:
                    btnCount = 12;
                    dialCount = 0;
                    break;
                case KeypadModel.Blink15Key:
                    btnCount = 15;
                    dialCount = 0;
                    break; 
                case KeypadModel.Blink13Key_2Dial:
                    btnCount = 13;
                    dialCount = 2;
                    break;
                case KeypadModel.BlinkRacepad:
                    btnCount = 12;
                    dialCount = 4;
                    break;
            }

            for (int i = 0; i < btnCount; i++)
                Buttons.Add(new BlinkMarine.Button(Number, i));

            for (int i = 0; i < dialCount; i++)
                Dials.Add(new BlinkMarine.Dial(Number, i + 1));
        }

        public override void Clear()
        {
            _buttons.Clear();
            _dials.Clear();
            _messageHandlers.Clear();
        }

        public override bool InIdRange(int id)
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


        public override bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
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
            Buttons[0].State = Convert.ToBoolean(data[0] & 0x01);
            Buttons[1].State = Convert.ToBoolean((data[0] & 0x02) >> 1);
            Buttons[2].State = Convert.ToBoolean((data[0] & 0x04) >> 2);
            Buttons[3].State = Convert.ToBoolean((data[0] & 0x08) >> 3);
            Buttons[4].State = Convert.ToBoolean((data[0] & 0x10) >> 4);
            Buttons[5].State = Convert.ToBoolean((data[0] & 0x20) >> 5);
            Buttons[6].State = Convert.ToBoolean((data[0] & 0x40) >> 6);
            Buttons[7].State = Convert.ToBoolean((data[0] & 0x80) >> 7);
            Buttons[8].State = Convert.ToBoolean(data[1] & 0x01);
            Buttons[9].State = Convert.ToBoolean((data[1] & 0x02) >> 1);
            Buttons[10].State = Convert.ToBoolean((data[1] & 0x04) >> 2);
            Buttons[11].State = Convert.ToBoolean((data[1] & 0x08) >> 3);
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

        public override List<CanDeviceResponse> RequestMsgs(int id)
        {
            List<CanDeviceResponse> requests = new List<CanDeviceResponse>();

            requests.Add(new CanDeviceResponse
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
            });

            requests.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = Convert.ToInt16(MessagePrefix.KeypadLed),
                Index = Number - 1,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 2,
                    Payload = RequestLed()
                },
                MsgDescription = $"KeypadLed{Number}"
            });

            foreach (var button in Buttons)
            {
                foreach (var response in button.RequestMsgs(id))
                {
                    requests.Add(response);
                }
            }

            foreach (var dial in Dials)
            {
                requests.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadLed),
                    Index = dial.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Dial.Request(Number, dial.Number - 1)
                    },
                    MsgDescription = $"Dial{Number}"
                });
            }

            return requests;
        }

        public override List<CanDeviceResponse> WriteMsgs(int id)
        {
            List<CanDeviceResponse> requests = new List<CanDeviceResponse>();

            requests.Add(new CanDeviceResponse
            {
                Prefix = Convert.ToInt16(MessagePrefix.Keypad),
                Index = Number - 1,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 6,
                    Payload = Write()
                }
            });

            requests.Add(new CanDeviceResponse
            {
                Prefix = Convert.ToInt16(MessagePrefix.KeypadLed),
                Index = Number - 1,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 8,
                    Payload = WriteLed()
                }
            });

            return requests;
        }

        public byte[] Request()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Keypad);
            data[1] = Convert.ToByte(Number-1);
            return data;
        }

        public override bool Receive(byte[] data)
        {
            if (data.Length != 5) return false;

            Enabled = Convert.ToBoolean(data[2] & 0x01);
            BaseId = data[3] & 0x7F;
            TimeoutEnabled = Convert.ToBoolean((data[3] & 0x80) >> 7);
            Timeout = data[4] * 100;
            Model = (KeypadModel)data[5];
            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.Keypad);
            data[1] = Convert.ToByte(Number - 1);
            data[2] = Convert.ToByte(Enabled);
            data[3] = Convert.ToByte((BaseId & 0x7F) + Convert.ToByte(TimeoutEnabled) << 7);
            data[4] = Convert.ToByte(Timeout / 100);
            data[5] = Convert.ToByte(Model);
            return data;
        }

        public byte[] RequestLed()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.KeypadLed);
            data[1] = Convert.ToByte(Number - 1);
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
