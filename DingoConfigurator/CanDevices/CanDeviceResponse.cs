using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanDeviceResponse
    {
        public int Prefix { get; set; }
        public int Index { get; set; } //If used, ex: output 1 , output 2, etc
        public CanInterfaceData Data { get; set; }
        public Timer TimeSentTimer { get; set; }
        public int ReceiveAttempts { get; set; }
        public int DeviceBaseId { get; set; }
        public string MsgDescription { get; set; }
        public CanDeviceResponse()
        {
            Data = new CanInterfaceData();
        }
    }
}
