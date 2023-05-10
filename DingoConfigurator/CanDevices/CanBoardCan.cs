using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanBoardCan : ICanDevice
    {
        public int BaseId { get; private set; }

        public CanBoardCan(int baseId)
        {
            BaseId = baseId;
        }

        public bool Read(int id, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
