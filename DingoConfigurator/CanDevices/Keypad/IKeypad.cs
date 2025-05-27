using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.Keypad
{
    public interface IKeypad : ICanDevice
    {

        bool SendNewSetting();
        
    }
}
