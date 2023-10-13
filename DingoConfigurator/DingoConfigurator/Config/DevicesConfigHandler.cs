using CanDevices;
using CanDevices.CanBoard;
using CanDevices.DingoDash;
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
            Clear();
        }

        public void Clear()
        {
            Config = new DevicesConfig();
            Opened = false;
        }
        public bool NewSaveFile(string path, ICanDevice[] devices)
        {
            int pdmNum = 0;
            int cbNum = 0;
            int dashNum = 0;

            foreach (var cd in devices)
            {
                if (cd.GetType() == typeof(DingoPdmCan))
                {
                    pdmNum++;
                }

                if (cd.GetType() == typeof(CanBoardCan))
                {
                    cbNum++;
                }

                if (cd.GetType() == typeof(DingoDashCan))
                {
                    dashNum++;
                }
            }

            _config.pdm = new PdmConfig[pdmNum];
            _config.canBoard = new CanBoardConfig[cbNum];
            _config.dash = new DashConfig[dashNum];

            pdmNum = 0;
            cbNum = 0;
            dashNum = 0;

            foreach (var cd in devices)
            {
                if (cd.GetType() == typeof(DingoPdmCan))
                {
                    var pdm = (DingoPdmCan)cd;

                    _config.pdm[pdmNum] = new PdmConfig();

                    PdmConfigHandler.UpdateConfig(ref pdm, ref _config.pdm[pdmNum], pdmNum);

                    pdmNum++;
                }

                if (cd.GetType() == typeof(CanBoardCan))
                {
                    var cb = (CanBoardCan)cd;

                    _config.canBoard[cbNum] = new CanBoardConfig();

                    CanBoardConfigHandler.UpdateConfig(ref cb, ref _config.canBoard[cbNum], cbNum);

                    cbNum++;
                }

                if (cd.GetType() == typeof(DingoDashCan))
                {
                    var dash = (DingoDashCan)cd;

                    _config.dash[dashNum] = new DashConfig();

                    DingoDashConfigHandler.UpdateConfig(ref dash, ref _config.dash[dashNum], dashNum);

                    dashNum++;
                }
            }

            SaveFile(path);

            Opened = true;

            return true;
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

                if (cd.GetType() == typeof(CanBoardCan))
                {
                    var cb = (CanBoardCan)cd;

                    CanBoardConfigHandler.UpdateConfig(ref cb, ref _config.canBoard[cbNum], cbNum);

                    cbNum++;
                }

                if (cd.GetType() == typeof(DingoDashCan))
                {
                    var dash = (DingoDashCan)cd;

                    DingoDashConfigHandler.UpdateConfig(ref dash, ref _config.dash[dashNum], dashNum);

                    dashNum++;
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

        public bool CheckDeviceCount(ICanDevice[] devices)
        {
            int pdmNum = 0;
            int cbNum = 0;
            int dashNum = 0;

            foreach (var cd in devices)
            {
                if (cd.GetType() == typeof(DingoPdmCan))
                {
                    pdmNum++;
                }

                if (cd.GetType() == typeof(CanBoardCan))
                {
                    cbNum++;
                }

                if (cd.GetType() == typeof(DingoDashCan))
                {
                    dashNum++;
                }
            }

            return (pdmNum == _config.pdm.Length) && (cbNum == _config.canBoard.Length) && (dashNum == _config.dash.Length);
        }
    }
}
