using CanDevices.DingoPdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.dingoPdmMax
{
    public class dingoPdmMaxPlot : DingoPdmPlot
    {
        public dingoPdmMaxPlot(string name, ICanDevice canDevice) : base(name, canDevice)
        {
        }
    }
}
