using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanDeviceSub : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        [JsonIgnore]
        public virtual string Name
        {
            get => _name;
            set => _name = value;
        }

        private ICanDevice _canDevice;
        [JsonIgnore]
        public ICanDevice CanDevice
        {
            get => _canDevice;
            set => _canDevice = value;
        }

        [JsonIgnore]
        public virtual int BaseId
        {
            get => CanDevice?.BaseId ?? 0;
            set
            {
                if (CanDevice != null)
                    CanDevice.BaseId = value;
            }
        }

        private bool _isConnected;
        [JsonIgnore]
        public virtual bool IsConnected
        {
            get => CanDevice.IsConnected;
            protected set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        [JsonIgnore]
        public virtual DateTime LastRxTime => CanDevice.LastRxTime;

        public int TimerIntervalMs { get => 0; }

        public CanDeviceSub()
        {
            Name = "CanDeviceSub";
            CanDevice = null;
        }

        public CanDeviceSub(string name, ICanDevice canDevice)
        {
            Name = name;
            CanDevice = canDevice;
        }

        public virtual void UpdateIsConnected()
        {
        }

        public virtual bool InIdRange(int id)
        {
            return false;
        }

        public virtual void UpdateView()
        {
        
        }

        public virtual bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            return true;
        }

        public virtual void Clear()
        {

        }

        public virtual void UpdateProperty(string property)
        {
            OnPropertyChanged(property);
        }

        public virtual List<CanDeviceResponse> GetUploadMessages()
        {
            return null;
        }

        public virtual List<CanDeviceResponse> GetDownloadMessages()
        {
            return null;
        }
        public virtual List<CanDeviceResponse> GetUpdateMessages(int newId)
        {
            return null;
        }

        public virtual CanDeviceResponse GetBurnMessage()
        {
            return null;
        }

        public virtual CanDeviceResponse GetSleepMessage()
        {
            return null;
        }
        public virtual CanDeviceResponse GetVersionMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetTimerMessages()
        {
            return new List<CanDeviceResponse>();
        }
    }
}
