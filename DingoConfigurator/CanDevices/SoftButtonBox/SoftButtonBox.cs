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

        private bool _emulationEnabled = true;
        public bool EmulationEnabled
        {
            get => _emulationEnabled;
            set
            {
                if (_emulationEnabled != value)
                {
                    _emulationEnabled = value;
                    OnPropertyChanged(nameof(EmulationEnabled));
                }
            }
        }

        private IKeypadEmulator _keypadEmulator;
        [JsonIgnore]
        public IKeypadEmulator KeypadEmulator => _keypadEmulator;

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

        public int GetTimerIntervalMs()
        {
            return EmulationEnabled ? 100 : 0;
        }

        public List<CanDeviceResponse> GetTimerMessages()
        {
            var messages = new List<CanDeviceResponse>();
            
            if (EmulationEnabled && _keypadEmulator != null)
            {
                messages.AddRange(_keypadEmulator.GenerateButtonStateMessages());
                messages.AddRange(_keypadEmulator.GenerateDialStateMessages());
                messages.AddRange(_keypadEmulator.GenerateHeartbeatMessage());
            }
            
            return messages;
        }

        public bool InIdRange(int id)
        {
            return (id >= BaseId) && (id <= BaseId + 0x7FF);
        }

        public bool IsPriorityMsg(int id)
        {
            return false;
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if (!InIdRange(id)) return false;

            if (EmulationEnabled && _keypadEmulator != null)
            {
                _keypadEmulator.ProcessIncomingMessage(id, data);
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
     
        }
    }
}
