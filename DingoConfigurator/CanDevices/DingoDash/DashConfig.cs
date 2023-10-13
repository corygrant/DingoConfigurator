using CanDevices.CanBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.DingoDash
{
    public static class DingoDashConfigHandler
    {
        public static void UpdateConfig(ref DingoDashCan dash, ref DashConfig config, int dashNum)
        {
            if (dash == null) return;
            if (config == null) return;

            config.name = $"Dash{dashNum}";
            config.label = dash.Name;
            config.baseCanId = dash.BaseId;

        }

        public static void ApplyConfig(ref DingoDashCan cb, DashConfig config)
        {
            //Nothing to apply
        }
    }

    public class DashConfig
    {
        public DashConfig()
        {
            name = String.Empty;
            label = String.Empty;
            baseCanId = 0;
        }

        public string name {  get; set; }
        public string label { get; set; }
        public int baseCanId { get; set; }
    }
}
