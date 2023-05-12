using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DingoDashCan : ICanDevice
    {
        public string Name { get; set; }
        public int BaseId { get; private set; }
        private DateTime _lastRxTime { get; set; }
        public DateTime LastRxTime { get => _lastRxTime; }
        public bool IsConnected
        {
            get
            {
                TimeSpan timeSpan = DateTime.Now - _lastRxTime;
                return (timeSpan.TotalMilliseconds < 500);
            }
        }

        public DingoDashCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
        }

        public bool Read(int id, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
