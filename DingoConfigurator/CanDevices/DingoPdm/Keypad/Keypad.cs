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
using System.Collections.Specialized;
using System.Globalization;

namespace CanDevices.DingoPdm
{
    public class Keypad : CanDeviceSub
    {
        private List<Button> _buttons;
        public List<Button> Buttons
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

        private List<Dial> _dials;
        public List<Dial> Dials
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

        private int _timeout;
        [JsonPropertyName("timeout")]
        public int Timeout
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

        private int _backlightBrightness;
        [JsonPropertyName("backlightBrightness")]
        public int BacklightBrightness
        {
            get => _backlightBrightness;
            set
            {
                if (_backlightBrightness != value)
                {
                    _backlightBrightness = value;
                    OnPropertyChanged(nameof(BacklightBrightness));
                }
            }
        }

        private int _buttonBrightness;
        [JsonPropertyName("buttonBrightness")]
        public int ButtonBrightness
        {
            get => _buttonBrightness;
            set
            {
                if (_buttonBrightness != value)
                {
                    _buttonBrightness = value;
                    OnPropertyChanged(nameof(ButtonBrightness));
                }
            }
        }

        private int _dimBacklightBrightness;
        [JsonPropertyName("dimBacklightBrightness")]
        public int DimBacklightBrightness
        {
            get => _dimBacklightBrightness;
            set
            {
                if (_dimBacklightBrightness != value)
                {
                    _dimBacklightBrightness = value;
                    OnPropertyChanged(nameof(DimBacklightBrightness));
                }
            }
        }

        private VarMap _dimmingVar;
        [JsonPropertyName("dimmingVar")]
        public VarMap DimmingVar
        {
            get => _dimmingVar;
            set
            {
                if (_dimmingVar != value)
                {
                    _dimmingVar = value;
                    OnPropertyChanged(nameof(DimmingVar));
                }
            }
        }

        private int _dimButtonBrightness;
        [JsonPropertyName("dimButtonBrightness")]
        public int DimButtonBrightness
        {
            get => _dimButtonBrightness;
            set
            {
                if (_dimButtonBrightness != value)
                {
                    _dimButtonBrightness = value;
                    OnPropertyChanged(nameof(DimButtonBrightness));
                }
            }
        }

        private KeypadModel _model;
        [JsonPropertyName("model")]
        public KeypadModel Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    ModelUpdate(_model);
                    OnPropertyChanged(nameof(Model));
                }
            }
        }

        protected int _baseId;
        [JsonPropertyName("baseId")]
        public override int BaseId
        {
            get => _baseId;
            set
            {
                if (_baseId != value)
                {
                    _baseId = value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
        }

        protected bool _isConnected;
        [JsonIgnore]
        public override bool IsConnected
        {
            get => _isConnected;
            protected set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        protected DateTime _lastRxTime { get; set; }
        [JsonIgnore]
        public override DateTime LastRxTime { get => _lastRxTime; }

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

        private int _currentBrightness;
        [JsonIgnore]
        public int CurrentBrightness
        {
            get => _currentBrightness;
            set
            {
                if (_currentBrightness != value)
                {
                    _currentBrightness = value;
                    OnPropertyChanged(nameof(CurrentBrightness));
                }
            }
        }

        private BlinkMarineBacklightColor _currentBacklightColor;
        [JsonIgnore]
        public BlinkMarineBacklightColor CurrentBacklightColor
        {
            get => _currentBacklightColor;
            set
            {
                if (_currentBacklightColor != value)
                {
                    _currentBacklightColor = value;
                    OnPropertyChanged(nameof(CurrentBacklightColor));
                }
            }
        }

        
        public Keypad(KeypadModel model, int num, string name, ICanDevice canDevice) : base(name, canDevice)
        {
            BaseId = 0x15;

            Number = num;

            _buttons = new List<Button>();
            _dials = new List<Dial>();

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

        protected void ModelUpdate(KeypadModel model)
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
                case KeypadModel.Grayhill6Key:
                    btnCount = 6;
                    dialCount = 0;
                    break;
                case KeypadModel.Grayhill8Key:
                    btnCount = 8;
                    dialCount = 0;
                    break;
                case KeypadModel.Grayhill15Key:
                    btnCount = 15;
                    dialCount = 0;
                    break;
                case KeypadModel.Grayhill20Key:
                    btnCount = 20;
                    dialCount = 0;
                    break;
                default:
                    throw new NotImplementedException($"No keypad implementation for model {model}");
            }

            for (int i = 0; i < btnCount; i++)
                Buttons.Add(new Button(Number, i + 1));

            for (int i = 0; i < dialCount; i++)
                Dials.Add(new Dial(Number, i + 1));
        }

        public override void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - LastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public override void Clear()
        {
            _buttons.Clear();
            _dials.Clear();
            _messageHandlers.Clear();
        }

        public override bool InIdRange(int id)
        {
            if (!Enabled)
                return false;

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

                //Only reset Rx timer if the msg comes from the keypad, not the PDM
                if (FromKeypad.TryGetValue((MsgId)msgOffset, out bool fromKeypad))
                {
                    if (fromKeypad)
                        _lastRxTime = DateTime.Now;
                }

                return true;
            }

            return false;
        }

        public void SetButtonState(int index, bool state)
        {
            if (index < 0 || index > (Buttons.Count - 1))
                return;
            Buttons[index].State = state;
        }

        public void SetDialTicks(int index, int ticks)
        {
            if (index < 0 || index > (Dials.Count - 1))
                return;
            Dials[index].Ticks = ticks;
        }

        private void NMT(byte[] data)
        {
            if (data[0] == 1)
                StartReceived = true;
        }

        private void ButtonState(byte[] data)
        {
            /*
             * Use CAN data from PDM, not from the keypad
             * 
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
            */
        }

        private void SetLed(byte[] data)
        {
            Buttons[0].ActiveColor.Red = Convert.ToBoolean(data[0] & 0x01);
            Buttons[1].ActiveColor.Red = Convert.ToBoolean((data[0] >> 1) & 0x01);
            Buttons[2].ActiveColor.Red = Convert.ToBoolean((data[0] >> 2) & 0x01);
            Buttons[3].ActiveColor.Red = Convert.ToBoolean((data[0] >> 3) & 0x01);
            Buttons[4].ActiveColor.Red = Convert.ToBoolean((data[0] >> 4) & 0x01);
            Buttons[5].ActiveColor.Red = Convert.ToBoolean((data[0] >> 5) & 0x01);
            Buttons[6].ActiveColor.Red = Convert.ToBoolean((data[0] >> 6) & 0x01);
            Buttons[7].ActiveColor.Red = Convert.ToBoolean((data[0] >> 7) & 0x01);
            Buttons[8].ActiveColor.Red = Convert.ToBoolean(data[1] & 0x01);
            Buttons[9].ActiveColor.Red = Convert.ToBoolean((data[1] >> 1) & 0x01);
            Buttons[10].ActiveColor.Red = Convert.ToBoolean((data[1] >> 2) & 0x01);
            Buttons[11].ActiveColor.Red = Convert.ToBoolean((data[1] >> 3) & 0x01);

            Buttons[0].ActiveColor.Green = Convert.ToBoolean((data[1] >> 4) & 0x01);
            Buttons[1].ActiveColor.Green = Convert.ToBoolean((data[1] >> 5) & 0x01);
            Buttons[2].ActiveColor.Green = Convert.ToBoolean((data[1] >> 6) & 0x01);
            Buttons[3].ActiveColor.Green = Convert.ToBoolean((data[1] >> 7) & 0x01);
            Buttons[4].ActiveColor.Green = Convert.ToBoolean(data[2] & 0x01);
            Buttons[5].ActiveColor.Green = Convert.ToBoolean((data[2] >> 1) & 0x01);
            Buttons[6].ActiveColor.Green = Convert.ToBoolean((data[2] >> 2) & 0x01);
            Buttons[7].ActiveColor.Green = Convert.ToBoolean((data[2] >> 3) & 0x01);
            Buttons[8].ActiveColor.Green = Convert.ToBoolean((data[2] >> 4) & 0x01);
            Buttons[9].ActiveColor.Green = Convert.ToBoolean((data[2] >> 5) & 0x01);
            Buttons[10].ActiveColor.Green = Convert.ToBoolean((data[2] >> 6) & 0x01);
            Buttons[11].ActiveColor.Green = Convert.ToBoolean((data[2] >> 7) & 0x01);

            Buttons[0].ActiveColor.Blue = Convert.ToBoolean(data[3] & 0x01);
            Buttons[1].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 1) & 0x01);
            Buttons[2].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 2) & 0x01);
            Buttons[3].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 3) & 0x01);
            Buttons[4].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 4) & 0x01);
            Buttons[5].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 5) & 0x01);
            Buttons[6].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 6) & 0x01);
            Buttons[7].ActiveColor.Blue = Convert.ToBoolean((data[3] >> 7) & 0x01);
            Buttons[8].ActiveColor.Blue = Convert.ToBoolean(data[4] & 0x01);
            Buttons[9].ActiveColor.Blue = Convert.ToBoolean((data[4] >> 1) & 0x01);
            Buttons[10].ActiveColor.Blue = Convert.ToBoolean((data[4] >> 2) & 0x01);
            Buttons[11].ActiveColor.Blue = Convert.ToBoolean((data[4] >> 3) & 0x01);
        }

        private void DialStateA(byte[] data)
        {
            
        }

        private void SetLedBlink(byte[] data)
        {
            Buttons[0].ActiveColor.RedFlash = Convert.ToBoolean(data[0] & 0x01);
            Buttons[1].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 1) & 0x01);
            Buttons[2].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 2) & 0x01);
            Buttons[3].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 3) & 0x01);
            Buttons[4].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 4) & 0x01);
            Buttons[5].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 5) & 0x01);
            Buttons[6].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 6) & 0x01);
            Buttons[7].ActiveColor.RedFlash = Convert.ToBoolean((data[0] >> 7) & 0x01);
            Buttons[8].ActiveColor.RedFlash = Convert.ToBoolean(data[1] & 0x01);
            Buttons[9].ActiveColor.RedFlash = Convert.ToBoolean((data[1] >> 1) & 0x01);
            Buttons[10].ActiveColor.RedFlash = Convert.ToBoolean((data[1] >> 2) & 0x01);
            Buttons[11].ActiveColor.RedFlash = Convert.ToBoolean((data[1] >> 3) & 0x01);

            Buttons[0].ActiveColor.GreenFlash = Convert.ToBoolean((data[1] >> 4) & 0x01);
            Buttons[1].ActiveColor.GreenFlash = Convert.ToBoolean((data[1] >> 5) & 0x01);
            Buttons[2].ActiveColor.GreenFlash = Convert.ToBoolean((data[1] >> 6) & 0x01);
            Buttons[3].ActiveColor.GreenFlash = Convert.ToBoolean((data[1] >> 7) & 0x01);
            Buttons[4].ActiveColor.GreenFlash = Convert.ToBoolean(data[2] & 0x01);
            Buttons[5].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 1) & 0x01);
            Buttons[6].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 2) & 0x01);
            Buttons[7].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 3) & 0x01);
            Buttons[8].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 4) & 0x01);
            Buttons[9].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 5) & 0x01);
            Buttons[10].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 6) & 0x01);
            Buttons[11].ActiveColor.GreenFlash = Convert.ToBoolean((data[2] >> 7) & 0x01);

            Buttons[0].ActiveColor.BlueFlash = Convert.ToBoolean(data[3] & 0x01);
            Buttons[1].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 1) & 0x01);
            Buttons[2].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 2) & 0x01);
            Buttons[3].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 3) & 0x01);
            Buttons[4].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 4) & 0x01);
            Buttons[5].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 5) & 0x01);
            Buttons[6].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 6) & 0x01);
            Buttons[7].ActiveColor.BlueFlash = Convert.ToBoolean((data[3] >> 7) & 0x01);
            Buttons[8].ActiveColor.BlueFlash = Convert.ToBoolean(data[4] & 0x01);
            Buttons[9].ActiveColor.BlueFlash = Convert.ToBoolean((data[4] >> 1) & 0x01);
            Buttons[10].ActiveColor.BlueFlash = Convert.ToBoolean((data[4] >> 2) & 0x01);
            Buttons[11].ActiveColor.BlueFlash = Convert.ToBoolean((data[4] >> 3) & 0x01);
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
            CurrentBrightness = data[0] / 0x3F;
            CurrentBacklightColor = (BlinkMarineBacklightColor)data[1];
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

        public List<CanDeviceResponse> RequestMsgs(int id)
        {
            List<CanDeviceResponse> requests = new List<CanDeviceResponse>();

            requests.Add(new CanDeviceResponse
            {
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

        public List<CanDeviceResponse> WriteMsgs(int id)
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
                },
                MsgDescription = $"Keypad{Number}"
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
                },
                MsgDescription = $"KeypadLed{Number}"
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

        public bool Receive(byte[] data)
        {
            if (data.Length != 6) return false;

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
            data[3] = Convert.ToByte((Convert.ToByte(BaseId) & 0x7F) + (Convert.ToByte(TimeoutEnabled) << 7));
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

        private static readonly IReadOnlyDictionary<MsgId, bool> FromKeypad = new Dictionary<MsgId, bool>
        {
            { MsgId.NMT, false },
            { MsgId.ButtonState, true },
            { MsgId.SetLed, false },
            { MsgId.DialStateA, true },
            { MsgId.SetLedBlink, false },
            { MsgId.DialStateB, true },
            { MsgId.LedBrightness, false },
            { MsgId.AnalogInput, true },
            { MsgId.Backlight, false },
            { MsgId.SdoResponse, true },
            { MsgId.SdoRequest, false },
            { MsgId.Heartbeat, true }
        };

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
