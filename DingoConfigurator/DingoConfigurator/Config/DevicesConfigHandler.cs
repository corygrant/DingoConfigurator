using CanDevices;
using CanDevices.DingoPdm;
using System;
using System.IO;
using System.Text.Json;

namespace DingoConfigurator.Config
{
    public class DevicesConfigHandler
    {
        private DevicesConfig _config;
        public DevicesConfig Config { get => _config; private set { _config = value; }}
        public bool Opened { get; private set; }

        public DevicesConfigHandler()
        {
            Config = new DevicesConfig();
            Opened = false;
        }

        public bool UpdateSaveFile(string path, ICanDevice[] devices)
        {
            int pdmNum = 0;
            int cbNum = 0;
            int dashNum = 0;
            foreach (var cd in devices)
            {

                if (cd.GetType() == typeof(DingoPdmCan))
                {
                    var pdm = (DingoPdmCan)cd;

                    PdmConfigHandler.UpdateConfig(ref pdm, ref _config.pdm[pdmNum], pdmNum);

                    pdmNum++;
                }
            }

            SaveFile(path);

            return true;
        }

        public bool SaveFile(string filename)
        {
            var jsonString = JsonSerializer.Serialize<DevicesConfig>(Config);
            File.WriteAllText(filename, jsonString);

            //Catch file exceptions and return false
            return true;
        }

        public bool OpenFile(string filename)
        {
            var jsonString = File.ReadAllText(filename);
            Config = JsonSerializer.Deserialize<DevicesConfig>(jsonString);

            Opened = true;

            //Catch file exceptions and return false
            return true;
        }
    }
}
