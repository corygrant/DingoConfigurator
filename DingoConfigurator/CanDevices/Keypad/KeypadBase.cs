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
    public class KeypadBase : NotifyPropertyChangedBase, IKeypad
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

        protected string _name;
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
        protected int _baseId;
        [JsonPropertyName("baseId")]
        public int BaseId
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

        protected DateTime _lastRxTime { get; set; }
        [JsonIgnore]
        public DateTime LastRxTime { get => _lastRxTime; }

        protected bool _isConnected;
        [JsonIgnore]
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public CanDeviceResponse GetBurnMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetDownloadMessages()
        {
            return null;
        }

        public CanDeviceResponse GetSleepMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetUpdateMessages(int newId)
        {
            return null;
        }

        public List<CanDeviceResponse> GetUploadMessages()
        {
            return null;
        }

        public CanDeviceResponse GetVersionMessage()
        {
            return null;
        }

        public bool InIdRange(int id)
        {
            return false;
        }

        public bool IsPriorityMsg(int id)
        {
            return false;
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            throw new NotImplementedException();
        }

        public void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - LastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public bool SendNewSetting()
        {
            throw new NotImplementedException();
        }
    }
}
