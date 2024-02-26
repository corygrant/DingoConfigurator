using CanDevices;
using CanDevices.CanBoard;
using CanDevices.CanMsgLog;
using CanDevices.DingoDash;
using CanDevices.DingoPdm;
using CanDevices.SoftButtonBox;
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
            int sbbNum = 0;
            int logNum = 0;

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

                if (cd.GetType() == typeof(SoftButtonBox))
                {
                    sbbNum++;
                }

                if (cd.GetType() == typeof(CanMsgLog))
                {
                    logNum++;
                }
            }

            _config.pdm = new PdmConfig[pdmNum];
            _config.canBoard = new CanBoardConfig[cbNum];
            _config.dash = new DashConfig[dashNum];
            _config.sbb = new SoftButtonBoxConfig[sbbNum];
            _config.log = new CanMsgLogConfig[logNum];

            pdmNum = 0;
            cbNum = 0;
            dashNum = 0;
            sbbNum = 0;
            logNum = 0;

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

                if (cd.GetType() == typeof(SoftButtonBox))
                {
                    var sbb = (SoftButtonBox)cd;

                    _config.sbb[sbbNum] = new SoftButtonBoxConfig();

                    SoftButtonBoxConfigHandler.UpdateConfig(ref sbb, ref _config.sbb[sbbNum], sbbNum);

                    sbbNum++;
                }

                if (cd.GetType() == typeof(CanMsgLog))
                {
                    var log = (CanMsgLog)cd;

                    _config.log[logNum] = new CanMsgLogConfig();

                    CanMsgLogConfigHandler.UpdateConfig(ref log, ref _config.log[logNum], logNum);

                    logNum++;
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
            int sbbNum = 0;
            int logNum = 0;

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

                if (cd.GetType() == typeof(SoftButtonBox))
                {
                    var sbb = (SoftButtonBox)cd;

                    SoftButtonBoxConfigHandler.UpdateConfig(ref sbb, ref _config.sbb[sbbNum], sbbNum);

                    sbbNum++;
                }

                if (cd.GetType() == typeof(CanMsgLog))
                {
                    var log = (CanMsgLog)cd;

                    CanMsgLogConfigHandler.UpdateConfig(ref log, ref _config.log[logNum], logNum);

                    logNum++;
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
            int sbbNum = 0;
            int logNum = 0;

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

                if (cd.GetType() == typeof(SoftButtonBox))
                {
                    sbbNum++;
                }

                if (cd.GetType() == typeof(CanMsgLog))
                {
                    logNum++;
                }
            }

            return  (pdmNum == _config.pdm.Length) && 
                    (cbNum == _config.canBoard.Length) && 
                    (dashNum == _config.dash.Length) && 
                    (sbbNum == _config.sbb.Length) &&
                    (logNum == _config.log.Length);
        }
    }
}
