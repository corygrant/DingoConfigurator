using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CanDevices
{
    public enum CanSpeed
    {
        CAN_BITRATE_10K = 0,
        CAN_BITRATE_20K = 1,
        CAN_BITRATE_50K = 2,
        CAN_BITRATE_100K = 3,
        CAN_BITRATE_125K = 4,
        CAN_BITRATE_250K = 5,
        CAN_BITRATE_500K = 6,
        CAN_BITRATE_750K = 7,
        CAN_BITRATE_1000K = 8,
        CAN_BITRATE_INVALID
    }

    public enum InputMode
    {
        MODE_NUM,
        MODE_MOMENTARY,
        MODE_LATCHING
    }

    public enum Conditional
    {
        AND,
        OR,
        NOR
    }

    public enum Operator
    {
        EQUAL,
        GREATER_THAN,
        LESS_THAN,
        BITWISE_AND,
        BITWISE_NAND
    }

    public enum ResetMode
    {
        RESET_NONE,
        RESET_COUNT,
        RESET_ENDLESS
    }

    public static class PdmConfigHandler
    {
        public static void InitConfig(PdmConfig config)
        {
            config.deviceConfig = new DeviceConfig();

            config.output = new Output[]
            {
                new Output(),
                new Output(),
                new Output(),
                new Output(),
                new Output(),
                new Output(),
                new Output(),
                new Output()
            };

            config.input = new Input[]
            {
                new Input(),
                new Input()
            };

            config.virtualInput = new VirtualInput[] 
            {
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput(),
                new VirtualInput()
            };

            config.canInput = new CanInput[]
            {
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput(),
                new CanInput()
            };

            config.flasher = new Flasher[]
            {
                new Flasher(),
                new Flasher(),
                new Flasher(),
                new Flasher()
            };

            config.canOutput = new CanOutput();            
            config.starter = new Starter();
            config.wiper = new Wiper();

        }
    }

    public class PdmConfig
    {
        public PdmConfig()
        {
            PdmConfigHandler.InitConfig(this);
        }

        public string name { get; set; }
        public string label { get; set; }
        public DeviceConfig deviceConfig { get; set; }
        public Input[] input { get; set; }
        public VirtualInput[] virtualInput { get; set; }
        public Output[] output { get; set; }
        public Wiper wiper { get; set; }
        public Flasher[] flasher { get; set; }
        public Starter starter { get; set; }
        public CanInput[] canInput { get; set; }
        public CanOutput canOutput { get; set; }
    }

    public class DeviceConfig
    {
        public string hwVersion { get; set; }
        public CanSpeed canSpeed { get; set; }
    }

    public class Wiper
    {
        public bool enabled { get; set; }
        public int lowSpeedInput { get; set; }
        public int highSpeedInput { get; set; }
        public int parkInput { get; set; }
        public bool parkStopLevel { get; set; }
        public int washInput { get; set; }
        public int washTime { get; set; }
        public int intermitInput { get; set; }
        public int intermitSelect { get; set; }
        public int intermitTime1 { get; set; }
        public int intermitTime2 { get; set; }
        public int intermitTime3 { get; set; }
        public int intermitTime4 { get; set; }
        public int intermitTime5 { get; set; }
        public int intermitTime6 { get; set; }
        public int intermitTime7 { get; set; }
        public int intermitTime8 { get; set; }
    }

    public class Starter
    {
        public bool enabled { get; set; }
        public int input { get; set; }
        public bool[] disableOut { get; set; }
    }

    public class CanOutput
    {
        public bool enable { get; set; }
        public int baseId { get; set; }
        public int updateTime { get; set; }
    }

    public class Input
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public InputMode mode { get; set; }
        public bool invertInput { get; set; }
        public int debounceTime { get; set; }
    }

    public class VirtualInput
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public bool not0 { get; set; }
        public int var0 { get; set; }
        public Conditional cond0 { get; set; }
        public bool not1 { get; set; }
        public int var1 { get; set; }
        public Conditional cond1 { get; set; }
        public bool not2 { get; set; }
        public int var2 { get; set; }
        public InputMode mode { get; set; }
    }

    public class Output
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public int input { get; set; }
        public double currentLimit { get; set; }
        public double inrushLimit { get; set; }
        public int inrushTime { get; set; }
        public ResetMode resetMode { get; set; }
        public int resetTime { get; set; }
        public int resetLimit { get; set; }
    }

    public class Flasher
    {
        public string name { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public int input { get; set; }
        public int flashOnTime { get; set; }
        public int flashOffTime { get; set; }
        public int singleCycle { get; set; }
        public int output { get; set; }
    }

    public class CanInput
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
