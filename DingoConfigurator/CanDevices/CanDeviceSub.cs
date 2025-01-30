using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanDeviceSub : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private ICanDevice _canDevice;
        public ICanDevice CanDevice
        {
            get => _canDevice;
            set => _canDevice = value;
        }

        public int BaseId
        {
            get => CanDevice.BaseId;
            set => BaseId = value;
        } 

        public bool IsConnected
        {
            get => CanDevice.IsConnected;
        }

        public DateTime LastRxTime => CanDevice.LastRxTime;

        public CanDeviceSub(string name, ICanDevice canDevice)
        {
            Name = name;
            CanDevice = canDevice;
        }

        public void UpdateIsConnected()
        {
        }

        public bool IsPriorityMsg(int id)
        {
            return false;
        }
        public bool InIdRange(int id)
        {
            return false;
        }

        public void UpdateView()
        {
        
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<Guid,CanDeviceResponse> queue)
        {
            return true;
        }

        public void Clear()
        {

        }

        public void UpdateProperty(string property)
        {
            OnPropertyChanged(property);
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

        public CanDeviceResponse GetSleepMessage()
        {
            return null;
        }
    }
}
