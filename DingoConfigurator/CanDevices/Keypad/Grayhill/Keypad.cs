using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.Keypad.Grayhill
{
    public class Keypad : KeypadBase
    {
        public new void Clear()
        {

        }

        public new bool InIdRange(int id)
        {
            return false;
        }

        public new bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if (!InIdRange(id))
            {
                return false;
            }



            return true;
        }
    }
}
