using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices.DingoDash
{
    public class DingoDashCan : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        [JsonPropertyName("name")]
        public string Name { 
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

        public DingoDashCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
        }

        public void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public bool IsPriorityMsg(int id)
        {
            return false;
        }
        public bool InIdRange(int id)
        {
            return (id >= BaseId) && (id <= BaseId + 10);
        }

        public void UpdateView()
        {
         
        }

        public bool Read(int id, byte[] data, ref List<CanDeviceResponse> queue)
        {
            if ((id < BaseId) || (id > BaseId + 10)) return false;

            //if (id == BaseId + 0) ReadMessage0(data);
            
            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }

        public void Clear()
        {
            
        }

        public List<CanDeviceResponse> GetUploadMessages()
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

        public CanDeviceResponse GetBurnMessage()
        {
            return null;
        }
    }
}
