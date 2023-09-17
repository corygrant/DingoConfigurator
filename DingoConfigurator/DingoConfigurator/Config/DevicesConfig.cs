using CanDevices;
using CanDevices.DingoPdm;
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
        public DevicesConfig()
        {
            pdm = new PdmConfig[]
            {
                new PdmConfig(),
                new PdmConfig()
            };

            canBoard = new CanBoardConfig[]
            {
                new CanBoardConfig(),
                new CanBoardConfig()
            };

            dash = new DashConfig[]
            {
                new DashConfig()
            };
        }

        public PdmConfig[] pdm { get; set; }
        public CanBoardConfig[] canBoard { get; set; }
        public DashConfig[] dash { get; set; }
    }

    public static class DevicesConfigHandler
    {
        public static void Serialize(DevicesConfig config, string filename)
        {
            var jsonString = JsonSerializer.Serialize<DevicesConfig>(config);
            File.WriteAllText(filename, jsonString);
        }

        public static DevicesConfig Deserialize(string filename)
        {
            var jsonString = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<DevicesConfig>(jsonString);
        }
    }
}
