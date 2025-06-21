using System;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using CanDevices.DingoPdm;

namespace CanDevices.SoftButtonBox
{
    public class SoftButtonBox : NotifyPropertyChangedBase, ICanDevice
    {
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

        private int _baseId;
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
                    if (_keypadEmulator != null)
                    {
                        _keypadEmulator.BaseCanId = value;
                    }
                }
            }
        }

        private DateTime _lastRxTime { get; set; }
        [JsonIgnore]
        public DateTime LastRxTime { get => _lastRxTime; }

        private bool _isConnected;
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

        private KeypadModel _keypadModel = KeypadModel.Blink12Key;
        [JsonPropertyName("keypadModel")]
        public KeypadModel KeypadModel
        {
            get => _keypadModel;
            set
            {
                if (_keypadModel != value)
                {
                    _keypadModel = value;
                    OnPropertyChanged(nameof(KeypadModel));
                    InitializeKeypadEmulator();
                }
            }
        }


        private IKeypadEmulator _keypadEmulator;
        [JsonIgnore]
        public IKeypadEmulator KeypadEmulator => _keypadEmulator;

        private int _ledStatesChanged = 0;
        [JsonIgnore]
        public int LedStatesChanged => _ledStatesChanged;

        private int _timerIntervalMs = 100;
        [JsonPropertyName("timerIntervalMs")]
        public int TimerIntervalMs { 
            get => _timerIntervalMs; 
            set
            {
                if (_timerIntervalMs != value)
                {
                    _timerIntervalMs = value;
                    OnPropertyChanged(nameof(TimerIntervalMs));
                }
            }
        }

        public SoftButtonBox(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
            InitializeKeypadEmulator();
        }

        private void InitializeKeypadEmulator()
        {
            try
            {
                _keypadEmulator = KeypadEmulatorFactory.CreateEmulator(KeypadModel, BaseId);
            }
            catch (Exception ex)
            {
                _keypadEmulator = null;
            }
        }

        public CanDeviceResponse GetBurnMessage()
        {
            return null;
        }

        public CanDeviceResponse GetSleepMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetDownloadMessages()
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

        public List<CanDeviceResponse> GetTimerMessages()
        {
            var messages = new List<CanDeviceResponse>();
            
            if (_keypadEmulator != null)
            {
                messages.AddRange(_keypadEmulator.GenerateButtonStateMessages());
                messages.AddRange(_keypadEmulator.GenerateDialStateMessages());
                messages.AddRange(_keypadEmulator.GenerateHeartbeatMessage());
            }
            
            return messages;
        }

        public bool InIdRange(int id)
        {
            if (_keypadEmulator == null)
            {
                return false;
            }
            return _keypadEmulator.InIdRange(id);
        }


        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if (!InIdRange(id)) return false;

            if (_keypadEmulator != null)
            {
                bool wasLedMessage = _keypadEmulator.IsLedMessage(id);
                _keypadEmulator.ProcessIncomingMessage(id, data);
                
                if (wasLedMessage)
                {
                    // Fire property change to notify UI of LED updates
                    _ledStatesChanged++;
                    OnPropertyChanged(nameof(LedStatesChanged));
                }
            }

            _lastRxTime = DateTime.Now;
            UpdateIsConnected();

            return true;
        }

        public bool[] GetButtonStates()
        {
            return _keypadEmulator?.ButtonStates ?? new bool[0];
        }

        public int[] GetDialValues()
        {
            return _keypadEmulator?.DialValues ?? new int[0];
        }

        public void UpdateIsConnected()
        {
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public void Clear()
        {
            _keypadEmulator?.Reset();
        }

        public void SetButtonState(int buttonIndex, bool pressed)
        {
            _keypadEmulator?.SetButtonState(buttonIndex, pressed);
        }

        public void SetDialValue(int dialIndex, int value)
        {
            _keypadEmulator?.SetDialValue(dialIndex, value);
        }

        public string GetLedColorName(int buttonIndex)
        {
            return _keypadEmulator?.GetLedColorName(buttonIndex) ?? "Off";
        }

        public string GetLedBlinkColorName(int buttonIndex)
        {
            return _keypadEmulator?.GetLedBlinkColorName(buttonIndex) ?? "Off";
        }
    }
}
