using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public class CanDataEventArgs : EventArgs
    {
        public CanDataEventArgs(CanData CanData)
        {
            this.canData = CanData;
        }

        public CanData canData { get; private set; }
    }

    public class CanData
    {
        public int Id { get; set; }
        public int Len { get; set; }
        public byte[] Payload { get; set; }
    }
}
