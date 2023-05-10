using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    internal interface ICanDevice
    {
        int BaseId { get;}
        bool Read(int id, byte[] data);
    }
}
