using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.SoftButtonBox
{
    public static class SoftButtonBoxConfigHandler
    {
        public static void UpdateConfig(ref SoftButtonBox sbb, ref SoftButtonBoxConfig config, int sbbNum)
        {
            if (sbb == null) return;
            if (config == null) return;

            config.name = $"SoftButtonBox{sbbNum}";
            config.label = sbb.Name;
            config.baseCanId = sbb.BaseId;

        }

        public static void ApplyConfig(ref SoftButtonBox sbb, SoftButtonBoxConfig config)
        {
            //Nothing to apply
        }
    }

    public class SoftButtonBoxConfig
    {
        public SoftButtonBoxConfig()
        {
            name = String.Empty;
            label = String.Empty;
            baseCanId = 0;
        }

        public string name { get; set; }
        public string label { get; set; }
        public int baseCanId { get; set; }
    }
}
