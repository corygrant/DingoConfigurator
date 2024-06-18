using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public class CanDataEventArgs : EventArgs
    {
        public CanDataEventArgs(CanInterfaceData CanData)
        {
            this.canData = CanData;
        }

        public CanInterfaceData canData { get; private set; }
    }

    public class CanInterfaceData
    {
        public int Id { get; set; }
        public int Len { get; set; }
        public byte[] Payload { get; set; }
    }

    public enum CanInterfaceBaudRate
    {
        BAUD_1M,
        BAUD_500K,
        BAUD_250K
    }
}
