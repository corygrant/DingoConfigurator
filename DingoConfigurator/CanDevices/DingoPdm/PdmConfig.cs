using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{
    public class PdmConfig
    {
        public PdmConfig()
        {
            deviceConfig = new DeviceConfig();

            output = new OutputConfig[]
            {
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig(),
            new OutputConfig()
            };

            input = new InputConfig[]
            {
            new InputConfig(),
            new InputConfig()
            };

            virtualInput = new VirtualInputConfig[]
            {
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig(),
            new VirtualInputConfig()
            };

            canInput = new CanInputConfig[]
            {
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig(),
            new CanInputConfig()
            };

            flasher = new FlasherConfig[]
            {
            new FlasherConfig(),
            new FlasherConfig(),
            new FlasherConfig(),
            new FlasherConfig()
            };

            canOutput = new CanOutputConfig();
            starter = new StarterDisableConfig();
            starter.disableOut = new bool[8];
            wiper = new WiperConfig();

        }

        public string name { get; set; }
        public string label { get; set; }
        public DeviceConfig deviceConfig { get; set; }
        public InputConfig[] input { get; set; }
        public VirtualInputConfig[] virtualInput { get; set; }
        public OutputConfig[] output { get; set; }
        public WiperConfig wiper { get; set; }
        public FlasherConfig[] flasher { get; set; }
        public StarterDisableConfig starter { get; set; }
        public CanInputConfig[] canInput { get; set; }
        public CanOutputConfig canOutput { get; set; }
    }

    public class DeviceConfig
    {
        public string hwVersion { get; set; }
        public CanSpeed canSpeed { get; set; }
    }

    public class WiperConfig
    {
        public bool enabled { get; set; }
        public VarMap lowSpeedInput { get; set; }
        public VarMap highSpeedInput { get; set; }
        public VarMap parkInput { get; set; }
        public bool parkStopLevel { get; set; }
        public VarMap washInput { get; set; }
        public int washCycles { get; set; }
        public VarMap intermitInput { get; set; }
        public VarMap speedInput { get; set; }
        public int[] intermitTime { get; set; }
        public int[] speedMap { get; set; }
    }

    public class StarterDisableConfig
    {
        public bool enabled { get; set; }
        public VarMap input { get; set; }
        public bool[] disableOut { get; set; }
    }

    public class CanOutputConfig
    {
        public bool enable { get; set; }
        public int baseId { get; set; }
        public int updateTime { get; set; }
    }

    public class InputConfig
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public InputMode mode { get; set; }
        public bool invertInput { get; set; }
        public int debounceTime { get; set; }
    }

    public class VirtualInputConfig
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public bool not0 { get; set; }
        public VarMap var0 { get; set; }
        public Conditional cond0 { get; set; }
        public bool not1 { get; set; }
        public VarMap var1 { get; set; }
        public Conditional cond1 { get; set; }
        public bool not2 { get; set; }
        public VarMap var2 { get; set; }
        public InputMode mode { get; set; }
    }

    public class OutputConfig
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public VarMap input { get; set; }
        public double currentLimit { get; set; }
        public double inrushLimit { get; set; }
        public int inrushTime { get; set; }
        public ResetMode resetMode { get; set; }
        public int resetTime { get; set; }
        public int resetLimit { get; set; }
    }

    public class FlasherConfig
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public VarMap input { get; set; }
        public int flashOnTime { get; set; }
        public int flashOffTime { get; set; }
        public int singleCycle { get; set; }
        public VarMap output { get; set; }
    }

    public class CanInputConfig
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public int id { get; set; }
        public int lowByte { get; set; }
        public int highByte { get; set; }
        public Operator oper { get; set; }
        public int onValue { get; set; }
        public InputMode mode { get; set; }
    }

}
