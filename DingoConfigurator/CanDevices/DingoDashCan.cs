using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DingoDashCan : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
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
        public int BaseId
        {
            get => _baseId;
            private set
            {
                if (_baseId != value)
                {
                    _baseId = value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
        }

        private DateTime _lastRxTime { get; set; }
        public DateTime LastRxTime { get => _lastRxTime; }

        private bool _isConnected;
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

        public bool Read(int id, byte[] data)
        {
            if ((id < BaseId) || (id > BaseId + 10)) return false;

            //if (id == BaseId + 0) ReadMessage0(data);
            
            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }
    }
}
