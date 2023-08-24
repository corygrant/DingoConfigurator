using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DashConfig
    {
        public DashConfig()
        {
            name = String.Empty;
            label = String.Empty;
        }

        public string name {  get; set; }
        public string label { get; set; }
    }
}
