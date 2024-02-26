using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.CanMsgLog
{
    public static class CanMsgLogConfigHandler
    {
        public static void UpdateConfig(ref CanMsgLog log, ref CanMsgLogConfig config, int logNum)
        {
            if (log == null) return;
            if (config == null) return;

            config.name = $"Log{logNum}";
            config.label = log.Name;
            config.baseCanId = log.BaseId;

        }

        public static void ApplyConfig(ref CanMsgLog log, CanMsgLogConfig config)
        {
            //Nothing to apply
        }
    }

    public class CanMsgLogConfig
    {
        public CanMsgLogConfig()
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
