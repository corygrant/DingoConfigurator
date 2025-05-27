using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CanDevices.DingoPdm;

namespace CanDevices.Keypad
{
    public abstract class KeypadBase : CanDeviceSub
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

        private List<Keypad.ButtonBase> _buttons;
        public List<Keypad.ButtonBase> Buttons
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

        public KeypadBase(string name, ICanDevice canDevice) : base(name, canDevice)
        {
        }

        public static KeypadBase Create(KeypadModel model, string name, ICanDevice canDevice)
        {
            switch(model)
            {
                case KeypadModel.Blink2Key:
                case KeypadModel.Blink4Key:
                case KeypadModel.Blink5Key:
                case KeypadModel.Blink6Key:
                case KeypadModel.Blink8Key:
                case KeypadModel.Blink10Key:
                case KeypadModel.Blink12Key:
                case KeypadModel.Blink15Key:
                case KeypadModel.Blink13Key_2Dial:
                case KeypadModel.BlinkRacepad:
                    return new BlinkMarine.Keypad(model, name, canDevice);
                case KeypadModel.Grayhill6Key:
                case KeypadModel.Grayhill8Key:
                case KeypadModel.Grayhill15Key:
                case KeypadModel.Grayhill20Key:
                    return new Grayhill.Keypad(model, name, canDevice);
                default:
                    throw new NotImplementedException($"No keypad implementation for model {model}");
            }
        }

        protected virtual void ModelUpdate(KeypadModel model)
        {
            throw new NotImplementedException();
        }


        public override void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - LastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public virtual bool SendNewSetting()
        {
            throw new NotImplementedException();
        }

        public virtual List<CanDeviceResponse> RequestMsgs(int id)
        {
            throw new NotImplementedException();
        }

        public virtual bool Receive(byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual List<CanDeviceResponse> WriteMsgs(int id)
        {
            throw new NotImplementedException();
        }

    }
}
