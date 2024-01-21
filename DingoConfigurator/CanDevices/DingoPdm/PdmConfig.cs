using CanDevices.CanBoard;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CanDevices.DingoPdm
{
    public static class PdmConfigHandler
    {
        public static void UpdateConfig(ref DingoPdmCan pdm, ref PdmConfig config, int pdmNum)
        {
            if (pdm == null) return;
            if(config == null) return;

            config.name = $"PDM{pdmNum}";
            config.label = pdm.Name;

            for (int i = 0; i < config.input.Length; i++)
            {
                config.input[i].name = $"DigitalInput{i}";
                config.input[i].label = pdm.DigitalInputs[i].Name;
                config.input[i].enabled = true;
                config.input[i].mode = 0;
                config.input[i].invertInput = pdm.DigitalInputs[i].InvertInput;
                config.input[i].debounceTime = 20;
                config.input[i].pull = pdm.DigitalInputs[i].Pull;
            }

            for (int i = 0; i < config.virtualInput.Length; i++)
            {
                config.virtualInput[i].name = $"VirtualInput{i}";
                config.virtualInput[i].label = pdm.VirtualInputs[i].Name;
                config.virtualInput[i].enabled = pdm.VirtualInputs[i].Enabled;
                config.virtualInput[i].not0 = pdm.VirtualInputs[i].Not0;
                config.virtualInput[i].var0 = pdm.VirtualInputs[i].Var0;
                config.virtualInput[i].cond0 = pdm.VirtualInputs[i].Cond0;
                config.virtualInput[i].not1 = pdm.VirtualInputs[i].Not1;
                config.virtualInput[i].var1 = pdm.VirtualInputs[i].Var1;
                config.virtualInput[i].cond1 = pdm.VirtualInputs[i].Cond1;
                config.virtualInput[i].not2 = pdm.VirtualInputs[i].Not2;
                config.virtualInput[i].var2 = pdm.VirtualInputs[i].Var2;
                config.virtualInput[i].mode = pdm.VirtualInputs[i].Mode;
            }

            for (int i = 0; i < config.output.Length; i++)
            {
                config.output[i].name = $"Output{i}";
                config.output[i].label = pdm.Outputs[i].Name;
                config.output[i].enabled = true;
                config.output[i].input = 0;
                config.output[i].currentLimit = pdm.Outputs[i].CurrentLimit;
                config.output[i].inrushLimit = 0;
                config.output[i].inrushTime = 0;
                config.output[i].resetMode = 0;
                config.output[i].resetTime = 0;
                config.output[i].resetLimit = 0;
            }

            config.wiper.enabled = pdm.Wipers[0].Enabled;
            config.wiper.lowSpeedInput = pdm.Wipers[0].SlowInput;
            config.wiper.highSpeedInput = pdm.Wipers[0].FastInput;
            config.wiper.parkInput = pdm.Wipers[0].ParkInput;
            config.wiper.parkStopLevel = pdm.Wipers[0].ParkStopLevel;
            config.wiper.washInput = pdm.Wipers[0].WashInput;
            config.wiper.washCycles = pdm.Wipers[0].WashWipeCycles;
            config.wiper.intermitInput = pdm.Wipers[0].InterInput;
            config.wiper.speedInput = pdm.Wipers[0].SpeedInput;
            config.wiper.intermitTime = pdm.Wipers[0].IntermitTime;
            config.wiper.speedMap = pdm.Wipers[0].SpeedMap;

            for (int i = 0; i < config.flasher.Length; i++)
            {
                config.flasher[i].name = $"Flasher{i}";
                config.flasher[i].label = pdm.Flashers[i].Name;
                config.flasher[i].enabled = pdm.Flashers[i].Enabled;
                config.flasher[i].input = pdm.Flashers[i].Input;
                config.flasher[i].flashOnTime = pdm.Flashers[i].OnTime;
                config.flasher[i].flashOffTime = pdm.Flashers[i].OffTime;
                config.flasher[i].singleCycle = Convert.ToInt16(pdm.Flashers[i].Single);
                config.flasher[i].output = pdm.Flashers[i].Output;
            }

            config.starter.enabled = pdm.StarterDisable[0].Enabled;
            config.starter.input = pdm.StarterDisable[0].Input;
            config.starter.disableOut[0] = pdm.StarterDisable[0].Output1;
            config.starter.disableOut[1] = pdm.StarterDisable[0].Output2;
            config.starter.disableOut[2] = pdm.StarterDisable[0].Output3;
            config.starter.disableOut[3] = pdm.StarterDisable[0].Output4;
            config.starter.disableOut[4] = pdm.StarterDisable[0].Output5;
            config.starter.disableOut[5] = pdm.StarterDisable[0].Output6;
            config.starter.disableOut[6] = pdm.StarterDisable[0].Output7;
            config.starter.disableOut[7] = pdm.StarterDisable[0].Output8;

            for (int i = 0; i < config.canInput.Length; i++)
            {
                config.canInput[i].name = $"CanInput{i}";
                config.canInput[i].label = pdm.CanInputs[i].Name;
                config.canInput[i].enabled = pdm.CanInputs[i].Enabled;
                config.canInput[i].id = pdm.CanInputs[i].Id;
                config.canInput[i].lowByte = pdm.CanInputs[i].LowByte;
                config.canInput[i].highByte = pdm.CanInputs[i].HighByte;
                config.canInput[i].oper = pdm.CanInputs[i].Operator;
                config.canInput[i].onValue = pdm.CanInputs[i].OnVal;
                config.canInput[i].mode = pdm.CanInputs[i].Mode;
            }

            config.canOutput.enable = true;
            config.canOutput.baseId = pdm.BaseId;
            config.canOutput.updateTime = 0;
        }

        public static void ApplyConfig(ref DingoPdmCan pdm, PdmConfig config)
        {
            if (pdm == null) return;

            int index = 0;

            foreach (var di in pdm.DigitalInputs)
            {
                di.Name = config.input[index].label;
                di.Enabled = config.input[index].enabled;
                di.Mode = config.input[index].mode;
                di.InvertInput = config.input[index].invertInput;
                di.DebounceTime = config.input[index].debounceTime;
                di.Pull = config.input[index].pull;

                index++;
            }

            index = 0;

            foreach (var output in pdm.Outputs)
            {
                output.Name = config.output[index].label;
                output.Enabled = config.output[index].enabled;
                output.Input = config.output[index].input;
                output.CurrentLimit = config.output[index].currentLimit;
                output.InrushCurrentLimit = config.output[index].inrushLimit;
                output.InrushTime = config.output[index].inrushTime;
                output.ResetMode = config.output[index].resetMode;
                output.ResetTime = config.output[index].resetTime;
                output.ResetCountLimit = config.output[index].resetLimit;

                index++;
            }

            index = 0;

            foreach (var canIn in pdm.CanInputs)
            {
                canIn.Name = config.canInput[index].label;
                canIn.Enabled = config.canInput[index].enabled;
                canIn.Id = config.canInput[index].id;
                canIn.LowByte = config.canInput[index].lowByte;
                canIn.HighByte = config.canInput[index].highByte;
                canIn.Operator = config.canInput[index].oper;
                canIn.OnVal = config.canInput[index].onValue;
                canIn.Mode = config.canInput[index].mode;

                index++;
            }

            index = 0;

            foreach (var virtIn in pdm.VirtualInputs)
            {
                virtIn.Name = config.virtualInput[index].label;
                virtIn.Enabled = config.virtualInput[index].enabled;
                virtIn.Not0 = config.virtualInput[index].not0;
                virtIn.Var0 = config.virtualInput[index].var0;
                virtIn.Cond0 = config.virtualInput[index].cond0;
                virtIn.Not1 = config.virtualInput[index].not1;
                virtIn.Var1 = config.virtualInput[index].var1;
                virtIn.Not2 = config.virtualInput[index].not2;
                virtIn.Var2 = config.virtualInput[index].var2;
                virtIn.Mode = config.virtualInput[index].mode;

                index++;
            }

            index = 0;

            pdm.Wipers[0].Enabled = config.wiper.enabled;
            pdm.Wipers[0].SlowInput = config.wiper.lowSpeedInput;
            pdm.Wipers[0].FastInput = config.wiper.highSpeedInput;
            pdm.Wipers[0].ParkInput = config.wiper.parkInput;
            pdm.Wipers[0].ParkStopLevel = config.wiper.parkStopLevel;
            pdm.Wipers[0].WashInput = config.wiper.washInput;
            pdm.Wipers[0].WashWipeCycles = config.wiper.washCycles;
            pdm.Wipers[0].InterInput = config.wiper.intermitInput;
            pdm.Wipers[0].SpeedInput = config.wiper.speedInput;
            pdm.Wipers[0].IntermitTime = config.wiper.intermitTime;
            pdm.Wipers[0].SpeedMap = config.wiper.speedMap;

            foreach (var flash in pdm.Flashers)
            {
                flash.Name = config.flasher[index].label;
                flash.Enabled = config.flasher[index].enabled;
                flash.Input = config.flasher[index].input;
                flash.OnTime = config.flasher[index].flashOnTime;
                flash.OffTime = config.flasher[index].flashOffTime;
                flash.Single = config.flasher[index].singleCycle == 1;
                flash.Output = config.flasher[index].output;

                index++;
            }

            pdm.StarterDisable[0].Enabled = config.starter.enabled;
            pdm.StarterDisable[0].Input = config.starter.input;
            pdm.StarterDisable[0].Output1 = config.starter.disableOut[0];
            pdm.StarterDisable[0].Output2 = config.starter.disableOut[1];
            pdm.StarterDisable[0].Output3 = config.starter.disableOut[2];
            pdm.StarterDisable[0].Output4 = config.starter.disableOut[3];
            pdm.StarterDisable[0].Output5 = config.starter.disableOut[4];
            pdm.StarterDisable[0].Output6 = config.starter.disableOut[5];
            pdm.StarterDisable[0].Output7 = config.starter.disableOut[6];
            pdm.StarterDisable[0].Output8 = config.starter.disableOut[7];
        }
    }

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
        public InputPull pull { get; set; }
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
