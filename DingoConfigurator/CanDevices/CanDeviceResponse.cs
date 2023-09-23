using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{
    public class CanDeviceResponse
    {
        public CanInterfaceData Data { get; set; }
        public bool Sent { get; set; }
        public bool Received { get; set; }
        public DateTime TimeSent { get; set; }

        public CanDeviceResponse()
        {
            Data = new CanInterfaceData();
        }
    }
}
