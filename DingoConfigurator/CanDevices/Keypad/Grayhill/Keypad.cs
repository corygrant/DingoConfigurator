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
        public Keypad(KeypadModel model)
        {
            
        }
        public override void Clear()
        {

        }

        public override bool InIdRange(int id)
        {
            return false;
        }

        public override bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if (!InIdRange(id))
            {
                return false;
            }



            return true;
        }
    }
}
