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
using System.Collections.ObjectModel;

namespace CanDevices.DingoPdm
{
    public class Keypad : CanDeviceSub
    {
        private ObservableCollection<Button> _allButtons;
        [JsonPropertyName("buttons")]
        public ObservableCollection<Button> AllButtons
        {
            get => _allButtons;
            set
            {
                if (_allButtons != value)
                {
                    _allButtons = value;
                    OnPropertyChanged(nameof(AllButtons));
                    UpdateVisibleButtons();
                    OnPropertyChanged(nameof(VisibleButtons));
                }
            }
        }

        private int _numButtons;
        [JsonIgnore]
        public int NumButtons
        {
            get => _numButtons;
            set
            {
                if (_numButtons != value)
                {
                    _numButtons = value;
                    OnPropertyChanged(nameof(NumButtons));
                    UpdateVisibleButtons();
                    OnPropertyChanged(nameof(VisibleButtons));
                }
            }
        }

        private ObservableCollection<Button> _visibleButtons;
        [JsonIgnore]
        public ObservableCollection<Button> VisibleButtons
        {
            get
            {
                if (_visibleButtons == null)
                {
                    _visibleButtons = new ObservableCollection<Button>();
                    UpdateVisibleButtons();
                }
                else if (_visibleButtons.Count == 0 && NumButtons > 0 && AllButtons != null)
                {
                    // Trigger update if collection is empty but should have buttons and AllButtons exists
                    UpdateVisibleButtons();
                }
                else if (AllButtons == null && NumButtons > 0)
                {
                    // Initialize AllButtons if it's null (should only happen in non-deserialization scenarios)
                    _allButtons = new ObservableCollection<Button>();
                    for(int i = 0; i < 20; i++)
                    {
                        _allButtons.Add(new Button(Number > 0 ? Number : 1, i + 1));
                    }
                    UpdateVisibleButtons();
                }
                return _visibleButtons;
            }
        }

        private void UpdateVisibleButtons()
        {
            if (_visibleButtons == null || AllButtons == null) return;
            
            // Clear and repopulate the visible buttons collection
            _visibleButtons.Clear();
            for (int i = 0; i < Math.Min(NumButtons, AllButtons.Count); i++)
            {
                _visibleButtons.Add(AllButtons[i]);
            }
        }

        private ObservableCollection<Dial> _dials;
        [JsonPropertyName("dials")]
        public ObservableCollection<Dial> Dials
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

        private int _numDials;
        [JsonIgnore]
        public int NumDials
        {
            get => _numDials;
            set
            {
                if (_numDials != value)
                {
                    _numDials = value;
                    OnPropertyChanged(nameof(NumDials));
                }
            }
        }

        private bool _startReceived;
        [JsonIgnore]
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
                    
                    // Update button keypad numbers after deserialization
                    if (_allButtons != null)
                    {
                        foreach (var button in _allButtons)
                        {
                            button.KeypadNumber = value;
                        }
                    }
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

        private int _baseId;
        [JsonPropertyName("baseId")]
        public int KeypadBaseId
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

        [JsonIgnore]
        public override int BaseId
        {
            get => _baseId;
            set => _baseId = value;
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

        private bool _colorsEnabled;
        [JsonIgnore]
        public bool ColorsEnabled
        {
            get => _colorsEnabled;
            set
            {
                if (_colorsEnabled != value)
                {
                    _colorsEnabled = value;
                    OnPropertyChanged(nameof(ColorsEnabled));
                }
            }
        }

        private string _keypadName;
        [JsonPropertyName("name")]
        public string KeypadName
        {
            get => _keypadName;
            set
            {
                if (_keypadName != value)
                {
                    _keypadName = value;
                    OnPropertyChanged(nameof(KeypadName));
                }
            }
        }

        private bool _visible = true;
        [JsonPropertyName("visible")]
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged(nameof(Visible));
                }
            }
        }

        [JsonIgnore]
        public override string Name
        {
            get => _keypadName;
            set => _keypadName = value;
        }

        private readonly Dictionary<int, Action<byte[]>> _messageHandlers;

        public Keypad()
        {
            _allButtons = new ObservableCollection<Button>();
            for(int i = 0; i < 20; i++)
            {
                _allButtons.Add(new Button(1, i + 1));
            }

            _dials = new ObservableCollection<Dial>();
            NumButtons = 20; // Default to showing all buttons
            
            // Initialize VisibleButtons collection
            _visibleButtons = new ObservableCollection<Button>();
            UpdateVisibleButtons();

           
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

        public Keypad(int number, string name, ICanDevice canDevice) : base(name, canDevice)
        {

            Number = number;

            _allButtons = new ObservableCollection<Button>();
            for(int i = 0; i < 20; i++)
            {
                _allButtons.Add(new Button(Number, i + 1));
            }


            _dials = new ObservableCollection<Dial>();
            NumButtons = 20; // Default to showing all buttons
            
            // Initialize VisibleButtons collection
            _visibleButtons = new ObservableCollection<Button>();
            UpdateVisibleButtons();

           
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
            switch (model)
            {
                case KeypadModel.Blink2Key:
                    NumButtons = 2;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink4Key:
                    NumButtons = 4;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink5Key:
                    NumButtons = 5;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink6Key:
                    NumButtons = 6;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink8Key:
                    NumButtons = 8;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink10Key:
                    NumButtons = 10;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink12Key:
                    NumButtons = 12;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Blink15Key:
                    NumButtons = 15;
                    NumDials = 0;
                    ColorsEnabled = true;
                    break; 
                case KeypadModel.Blink13Key_2Dial:
                    NumButtons = 13;
                    NumDials = 2;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.BlinkRacepad:
                    NumButtons = 12;
                    NumDials = 4;
                    ColorsEnabled = true;
                    break;
                case KeypadModel.Grayhill6Key:
                    NumButtons = 6;
                    NumDials = 0;
                    ColorsEnabled = false;
                    break;
                case KeypadModel.Grayhill8Key:
                    NumButtons = 8;
                    NumDials = 0;
                    ColorsEnabled = false;
                    break;
                case KeypadModel.Grayhill15Key:
                    NumButtons = 15;
                    NumDials = 0;
                    ColorsEnabled = false;
                    break;
                case KeypadModel.Grayhill20Key:
                    NumButtons = 20;
                    NumDials = 0;
                    ColorsEnabled = false;
                    break;
                default:
                    throw new NotImplementedException($"No keypad implementation for model {model}");
            }

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
            _messageHandlers.Clear();
        }

        public override bool InIdRange(int id)
        {
            if (!Enabled)
                return false;

            // Define a HashSet for quick lookup
            var validIds = new HashSet<int>
            {
                BaseId + Convert.ToInt16(MsgId.NMT),
                BaseId + Convert.ToInt16(MsgId.ButtonState),
                BaseId + Convert.ToInt16(MsgId.SetLed),
                BaseId + Convert.ToInt16(MsgId.DialStateA),
                BaseId + Convert.ToInt16(MsgId.SetLedBlink),
                BaseId + Convert.ToInt16(MsgId.DialStateB),
                BaseId + Convert.ToInt16(MsgId.LedBrightness),
                BaseId + Convert.ToInt16(MsgId.AnalogInput),
                BaseId + Convert.ToInt16(MsgId.Backlight),
                BaseId + Convert.ToInt16(MsgId.SdoResponse),
                BaseId + Convert.ToInt16(MsgId.SdoRequest),
                BaseId + Convert.ToInt16(MsgId.Heartbeat)
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
            int msgOffset = id - BaseId;

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
            if (index < 0 || index > (NumButtons - 1))
                return;
            AllButtons[index].State = state;
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
            
        }

        private void SetLed(byte[] data)
        {

            //TODO: Color has to be set based on BlinkMarine model
            
            AllButtons[0].SetActiveColorRed(Convert.ToBoolean(data[0] & 0x01));
            AllButtons[1].SetActiveColorRed(Convert.ToBoolean((data[0] >> 1) & 0x01));
            AllButtons[2].SetActiveColorRed(Convert.ToBoolean((data[0] >> 2) & 0x01));
            AllButtons[3].SetActiveColorRed(Convert.ToBoolean((data[0] >> 3) & 0x01));
            AllButtons[4].SetActiveColorRed(Convert.ToBoolean((data[0] >> 4) & 0x01));
            AllButtons[5].SetActiveColorRed(Convert.ToBoolean((data[0] >> 5) & 0x01));
            AllButtons[6].SetActiveColorRed(Convert.ToBoolean((data[0] >> 6) & 0x01));
            AllButtons[7].SetActiveColorRed(Convert.ToBoolean((data[0] >> 7) & 0x01));
            AllButtons[8].SetActiveColorRed(Convert.ToBoolean(data[1] & 0x01));
            AllButtons[9].SetActiveColorRed(Convert.ToBoolean((data[1] >> 1) & 0x01));
            AllButtons[10].SetActiveColorRed(Convert.ToBoolean((data[1] >> 2) & 0x01));
            AllButtons[11].SetActiveColorRed(Convert.ToBoolean((data[1] >> 3) & 0x01));
            AllButtons[12].SetActiveColorRed(false);
            AllButtons[13].SetActiveColorRed(false);
            AllButtons[14].SetActiveColorRed(false);
            AllButtons[15].SetActiveColorRed(false);
            AllButtons[16].SetActiveColorRed(false);
            AllButtons[17].SetActiveColorRed(false);
            AllButtons[18].SetActiveColorRed(false);
            AllButtons[19].SetActiveColorRed(false);

            AllButtons[0].SetActiveColorGreen(Convert.ToBoolean((data[1] >> 4) & 0x01));
            AllButtons[1].SetActiveColorGreen(Convert.ToBoolean((data[1] >> 5) & 0x01));
            AllButtons[2].SetActiveColorGreen(Convert.ToBoolean((data[1] >> 6) & 0x01));
            AllButtons[3].SetActiveColorGreen(Convert.ToBoolean((data[1] >> 7) & 0x01));
            AllButtons[4].SetActiveColorGreen(Convert.ToBoolean(data[2] & 0x01));
            AllButtons[5].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 1) & 0x01));
            AllButtons[6].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 2) & 0x01));
            AllButtons[7].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 3) & 0x01));
            AllButtons[8].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 4) & 0x01));
            AllButtons[9].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 5) & 0x01));
            AllButtons[10].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 6) & 0x01));
            AllButtons[11].SetActiveColorGreen(Convert.ToBoolean((data[2] >> 7) & 0x01));
            AllButtons[12].SetActiveColorGreen(false);
            AllButtons[13].SetActiveColorGreen(false);
            AllButtons[14].SetActiveColorGreen(false);
            AllButtons[15].SetActiveColorGreen(false);
            AllButtons[16].SetActiveColorGreen(false);
            AllButtons[17].SetActiveColorGreen(false);
            AllButtons[18].SetActiveColorGreen(false);
            AllButtons[19].SetActiveColorGreen(false);

            AllButtons[0].SetActiveColorBlue(Convert.ToBoolean(data[3] & 0x01));
            AllButtons[1].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 1) & 0x01));
            AllButtons[2].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 2) & 0x01));
            AllButtons[3].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 3) & 0x01));
            AllButtons[4].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 4) & 0x01));
            AllButtons[5].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 5) & 0x01));
            AllButtons[6].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 6) & 0x01));
            AllButtons[7].SetActiveColorBlue(Convert.ToBoolean((data[3] >> 7) & 0x01));
            AllButtons[8].SetActiveColorBlue(Convert.ToBoolean(data[4] & 0x01));
            AllButtons[9].SetActiveColorBlue(Convert.ToBoolean((data[4] >> 1) & 0x01));
            AllButtons[10].SetActiveColorBlue(Convert.ToBoolean((data[4] >> 2) & 0x01));
            AllButtons[11].SetActiveColorBlue(Convert.ToBoolean((data[4] >> 3) & 0x01));
            AllButtons[12].SetActiveColorBlue(false);
            AllButtons[13].SetActiveColorBlue(false);
            AllButtons[14].SetActiveColorBlue(false);
            AllButtons[15].SetActiveColorBlue(false);
            AllButtons[16].SetActiveColorBlue(false);
            AllButtons[17].SetActiveColorBlue(false);
            AllButtons[18].SetActiveColorBlue(false);
            AllButtons[19].SetActiveColorBlue(false);

            foreach (var btn in AllButtons)
            {
                btn.UpdateActiveColor();
            }
            
        }

        private void DialStateA(byte[] data)
        {
            
        }

        private void SetLedBlink(byte[] data)
        {
            //TODO: Color has to be set based on BlinkMarine model

            AllButtons[0].SetActiveBlinkingColorRed(Convert.ToBoolean(data[0] & 0x01));
            AllButtons[1].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 1) & 0x01));
            AllButtons[2].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 2) & 0x01));
            AllButtons[3].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 3) & 0x01));
            AllButtons[4].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 4) & 0x01));
            AllButtons[5].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 5) & 0x01));
            AllButtons[6].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 6) & 0x01));
            AllButtons[7].SetActiveBlinkingColorRed(Convert.ToBoolean((data[0] >> 7) & 0x01));
            AllButtons[8].SetActiveBlinkingColorRed(Convert.ToBoolean(data[1] & 0x01));
            AllButtons[9].SetActiveBlinkingColorRed(Convert.ToBoolean((data[1] >> 1) & 0x01));
            AllButtons[10].SetActiveBlinkingColorRed(Convert.ToBoolean((data[1] >> 2) & 0x01));
            AllButtons[11].SetActiveBlinkingColorRed(Convert.ToBoolean((data[1] >> 3) & 0x01));
            AllButtons[12].SetActiveBlinkingColorRed(false);
            AllButtons[13].SetActiveBlinkingColorRed(false);
            AllButtons[14].SetActiveBlinkingColorRed(false);
            AllButtons[15].SetActiveBlinkingColorRed(false);
            AllButtons[16].SetActiveBlinkingColorRed(false);
            AllButtons[17].SetActiveBlinkingColorRed(false);
            AllButtons[18].SetActiveBlinkingColorRed(false);
            AllButtons[19].SetActiveBlinkingColorRed(false);

            AllButtons[0].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[1] >> 4) & 0x01));
            AllButtons[1].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[1] >> 5) & 0x01));
            AllButtons[2].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[1] >> 6) & 0x01));
            AllButtons[3].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[1] >> 7) & 0x01));
            AllButtons[4].SetActiveBlinkingColorGreen(Convert.ToBoolean(data[2] & 0x01));
            AllButtons[5].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 1) & 0x01));
            AllButtons[6].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 2) & 0x01));
            AllButtons[7].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 3) & 0x01));
            AllButtons[8].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 4) & 0x01));
            AllButtons[9].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 5) & 0x01));
            AllButtons[10].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 6) & 0x01));
            AllButtons[11].SetActiveBlinkingColorGreen(Convert.ToBoolean((data[2] >> 7) & 0x01));
            AllButtons[12].SetActiveBlinkingColorGreen(false);
            AllButtons[13].SetActiveBlinkingColorGreen(false);
            AllButtons[14].SetActiveBlinkingColorGreen(false);
            AllButtons[15].SetActiveBlinkingColorGreen(false);
            AllButtons[16].SetActiveBlinkingColorGreen(false);
            AllButtons[17].SetActiveBlinkingColorGreen(false);
            AllButtons[18].SetActiveBlinkingColorGreen(false);
            AllButtons[19].SetActiveBlinkingColorGreen(false);

            AllButtons[0].SetActiveBlinkingColorBlue(Convert.ToBoolean(data[3] & 0x01));
            AllButtons[1].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 1) & 0x01));
            AllButtons[2].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 2) & 0x01));
            AllButtons[3].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 3) & 0x01));
            AllButtons[4].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 4) & 0x01));
            AllButtons[5].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 5) & 0x01));
            AllButtons[6].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 6) & 0x01));
            AllButtons[7].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[3] >> 7) & 0x01));
            AllButtons[8].SetActiveBlinkingColorBlue(Convert.ToBoolean(data[4] & 0x01));
            AllButtons[9].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[4] >> 1) & 0x01));
            AllButtons[10].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[4] >> 2) & 0x01));
            AllButtons[11].SetActiveBlinkingColorBlue(Convert.ToBoolean((data[4] >> 3) & 0x01));
            AllButtons[12].SetActiveBlinkingColorBlue(false);
            AllButtons[13].SetActiveBlinkingColorBlue(false);
            AllButtons[14].SetActiveBlinkingColorBlue(false);
            AllButtons[15].SetActiveBlinkingColorBlue(false);
            AllButtons[16].SetActiveBlinkingColorBlue(false);
            AllButtons[17].SetActiveBlinkingColorBlue(false);
            AllButtons[18].SetActiveBlinkingColorBlue(false);
            AllButtons[19].SetActiveBlinkingColorBlue(false);

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

            foreach(var btn in AllButtons)
            {
                btn.BacklightColor = CurrentBacklightColor;
            }
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

            foreach (var button in VisibleButtons)
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
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadDial),
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

            foreach (var button in VisibleButtons)
            {
                foreach (var response in button.WriteMsgs(id))
                {
                    requests.Add(response);
                }
            }

            foreach (var dial in Dials)
            {
                requests.Add(new CanDeviceResponse
                {
                    Prefix = Convert.ToInt16(MessagePrefix.KeypadDial),
                    Index = dial.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 5,
                        Payload = dial.Write(Number, dial.Number - 1)
                    },
                    MsgDescription = $"Dial{Number}"
                });
            }

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
            Timeout = data[4] / 10;
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
            data[4] = Convert.ToByte(Convert.ToDouble(Timeout) * 10.0);
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
