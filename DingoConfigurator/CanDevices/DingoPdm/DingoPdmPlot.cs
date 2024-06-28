using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{

    public class DingoPdmPlot : CanDeviceSub
    {
        public DingoPdmPlot(string name, ICanDevice canDevice) : base(name, canDevice)
        {
        }
    }
}
