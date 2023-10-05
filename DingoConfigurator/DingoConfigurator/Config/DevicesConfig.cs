using CanDevices;
using CanDevices.DingoPdm;
using CanDevices.CanBoard;
using CanDevices.DingoDash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
