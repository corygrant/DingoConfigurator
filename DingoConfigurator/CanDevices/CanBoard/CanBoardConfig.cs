using CanDevices.DingoPdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.CanBoard
{
    public static class CanBoardConfigHandler
    {
        public static void UpdateConfig(ref CanBoardCan cb, ref CanBoardConfig config, int cbNum)
        {
            if (cb == null) return;
            if (config == null) return;

            config.name = $"CANBoard{cbNum}";
            config.label = cb.Name;
            config.baseCanId = cb.BaseId;

        }

        public static void ApplyConfig(ref CanBoardCan cb, CanBoardConfig config)
        {
            //Nothing to apply
        }
    }

    public class CanBoardConfig
    {
        public CanBoardConfig()
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
