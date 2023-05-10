using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DingoDashCan : ICanDevice
    {
        public int BaseId { get; private set; }

        public DingoDashCan(int baseId)
        {
            BaseId = baseId;
        }

        public bool Read(int id, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
