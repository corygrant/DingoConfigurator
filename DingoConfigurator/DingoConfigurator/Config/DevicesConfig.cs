using CanDevices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DingoConfigurator.Config
{
    public class DevicesConfig
    {
        public PdmConfig[] pdm { get; set; }
        public CanBoardConfig[] canBoard { get; set; }
        public DashConfig[] dash { get; set; }
    }

    public static class DevicesConfigHandler
    {
        public static void Serialize(DevicesConfig config)
        {
            var jsonString = JsonSerializer.Serialize<DevicesConfig>(config);
            File.WriteAllText("D:\\GitHub\\DingoConfigurator\\DingoPDM_v7_Config.json", jsonString);
        }

        public static DevicesConfig Deserialize()
        {
            var jsonString = File.ReadAllText("D:\\GitHub\\DingoConfigurator\\RallyCar.json");
            return JsonSerializer.Deserialize<DevicesConfig>(jsonString);
        }
        public static void InitConfig(DevicesConfig config)
        {
            config.pdm = new PdmConfig[]
            {
                new PdmConfig(),
                new PdmConfig()
            };

            config.canBoard = new CanBoardConfig[]
            {
                new CanBoardConfig(),
                new CanBoardConfig()
            };

            config.dash = new DashConfig[]
            {
                new DashConfig()
            };
        }
    }
}
