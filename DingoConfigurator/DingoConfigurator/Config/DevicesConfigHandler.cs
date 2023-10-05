using CanDevices;
using CanDevices.DingoPdm;
using System;
using System.IO;
using System.Text.Json;

namespace DingoConfigurator.Config
{
    public class DevicesConfigHandler
    {
        public DevicesConfig Config { get; private set; }
        public bool Opened { get; private set; }

        public DevicesConfigHandler()
        {
            Config = new DevicesConfig();
            Opened = false;
        }

        public bool Save(string path, ICanDevice[] devices)
        {
            int pdmNum = 0;
            int cbNum = 0;
            int dashNum = 0;
            foreach (var cd in devices)
            {

                if (cd.GetType() == typeof(DingoPdmCan))
                {
                    var pdm = (DingoPdmCan)cd;

                    Config.pdm[pdmNum].name = $"PDM{pdmNum}";
                    Config.pdm[pdmNum].label = pdm.Name;

                    for (int i = 0; i < Config.pdm[pdmNum].input.Length; i++)
                    {
                        Config.pdm[pdmNum].input[i].name = $"DigitalInput{i}";
                        Config.pdm[pdmNum].input[i].label = pdm.DigitalInputs[i].Name;
                        Config.pdm[pdmNum].input[i].enabled = true;
                        Config.pdm[pdmNum].input[i].mode = 0;
                        Config.pdm[pdmNum].input[i].invertInput = pdm.DigitalInputs[i].InvertInput;
                        Config.pdm[pdmNum].input[i].debounceTime = 20;
                    }

                    for (int i = 0; i < Config.pdm[pdmNum].virtualInput.Length; i++)
                    {
                        Config.pdm[pdmNum].virtualInput[i].name = $"VirtualInput{i}";
                        Config.pdm[pdmNum].virtualInput[i].label = pdm.VirtualInputs[i].Name;
                        Config.pdm[pdmNum].virtualInput[i].enabled = pdm.VirtualInputs[i].Enabled;
                        Config.pdm[pdmNum].virtualInput[i].not0 = pdm.VirtualInputs[i].Not0;
                        Config.pdm[pdmNum].virtualInput[i].var0 = pdm.VirtualInputs[i].Var0;
                        Config.pdm[pdmNum].virtualInput[i].cond0 = pdm.VirtualInputs[i].Cond0;
                        Config.pdm[pdmNum].virtualInput[i].not1 = pdm.VirtualInputs[i].Not1;
                        Config.pdm[pdmNum].virtualInput[i].var1 = pdm.VirtualInputs[i].Var1;
                        Config.pdm[pdmNum].virtualInput[i].cond1 = pdm.VirtualInputs[i].Cond1;
                        Config.pdm[pdmNum].virtualInput[i].not2 = pdm.VirtualInputs[i].Not2;
                        Config.pdm[pdmNum].virtualInput[i].var2 = pdm.VirtualInputs[i].Var2;
                        Config.pdm[pdmNum].virtualInput[i].mode = pdm.VirtualInputs[i].Mode;
                    }

                    for (int i = 0; i < Config.pdm[pdmNum].output.Length; i++)
                    {
                        Config.pdm[pdmNum].output[i].name = $"Output{i}";
                        Config.pdm[pdmNum].output[i].label = pdm.Outputs[i].Name;
                        Config.pdm[pdmNum].output[i].enabled = true;
                        Config.pdm[pdmNum].output[i].input = 0;
                        Config.pdm[pdmNum].output[i].currentLimit = pdm.Outputs[i].CurrentLimit;
                        Config.pdm[pdmNum].output[i].inrushLimit = 0;
                        Config.pdm[pdmNum].output[i].inrushTime = 0;
                        Config.pdm[pdmNum].output[i].resetMode = 0;
                        Config.pdm[pdmNum].output[i].resetTime = 0;
                        Config.pdm[pdmNum].output[i].resetLimit = 0;
                    }

                    Config.pdm[pdmNum].wiper.enabled = pdm.Wipers[0].Enabled;
                    Config.pdm[pdmNum].wiper.lowSpeedInput = pdm.Wipers[0].SlowInput;
                    Config.pdm[pdmNum].wiper.highSpeedInput = pdm.Wipers[0].FastInput;
                    Config.pdm[pdmNum].wiper.parkInput = pdm.Wipers[0].ParkInput;
                    Config.pdm[pdmNum].wiper.parkStopLevel = pdm.Wipers[0].ParkStopLevel;
                    Config.pdm[pdmNum].wiper.washInput = pdm.Wipers[0].WashInput;
                    Config.pdm[pdmNum].wiper.washCycles = pdm.Wipers[0].WashWipeCycles;
                    Config.pdm[pdmNum].wiper.intermitInput = pdm.Wipers[0].InterInput;
                    Config.pdm[pdmNum].wiper.speedInput = pdm.Wipers[0].SpeedInput;
                    Config.pdm[pdmNum].wiper.intermitTime = pdm.Wipers[0].IntermitTime;
                    Config.pdm[pdmNum].wiper.speedMap = pdm.Wipers[0].SpeedMap;

                    for (int i = 0; i < Config.pdm[pdmNum].flasher.Length; i++)
                    {
                        Config.pdm[pdmNum].flasher[i].name = $"Flasher{i}";
                        Config.pdm[pdmNum].flasher[i].label = pdm.Flashers[i].Name;
                        Config.pdm[pdmNum].flasher[i].enabled = pdm.Flashers[i].Enabled;
                        Config.pdm[pdmNum].flasher[i].input = pdm.Flashers[i].Input;
                        Config.pdm[pdmNum].flasher[i].flashOnTime = pdm.Flashers[i].OnTime;
                        Config.pdm[pdmNum].flasher[i].flashOffTime = pdm.Flashers[i].OffTime;
                        Config.pdm[pdmNum].flasher[i].singleCycle = Convert.ToInt16(pdm.Flashers[i].Single);
                        Config.pdm[pdmNum].flasher[i].output = pdm.Flashers[i].Output;
                    }

                    Config.pdm[pdmNum].starter.enabled = pdm.StarterDisable[0].Enabled;
                    Config.pdm[pdmNum].starter.input = pdm.StarterDisable[0].Input;
                    Config.pdm[pdmNum].starter.disableOut[0] = pdm.StarterDisable[0].Output1;
                    Config.pdm[pdmNum].starter.disableOut[1] = pdm.StarterDisable[0].Output2;
                    Config.pdm[pdmNum].starter.disableOut[2] = pdm.StarterDisable[0].Output3;
                    Config.pdm[pdmNum].starter.disableOut[3] = pdm.StarterDisable[0].Output4;
                    Config.pdm[pdmNum].starter.disableOut[4] = pdm.StarterDisable[0].Output5;
                    Config.pdm[pdmNum].starter.disableOut[5] = pdm.StarterDisable[0].Output6;
                    Config.pdm[pdmNum].starter.disableOut[6] = pdm.StarterDisable[0].Output7;
                    Config.pdm[pdmNum].starter.disableOut[7] = pdm.StarterDisable[0].Output8;

                    for (int i = 0; i < Config.pdm[pdmNum].canInput.Length; i++)
                    {
                        Config.pdm[pdmNum].canInput[i].name = $"CanInput{i}";
                        Config.pdm[pdmNum].canInput[i].label = pdm.CanInputs[i].Name;
                        Config.pdm[pdmNum].canInput[i].enabled = pdm.CanInputs[i].Enabled;
                        Config.pdm[pdmNum].canInput[i].id = pdm.CanInputs[i].Id;
                        Config.pdm[pdmNum].canInput[i].lowByte = pdm.CanInputs[i].LowByte;
                        Config.pdm[pdmNum].canInput[i].highByte = pdm.CanInputs[i].HighByte;
                        Config.pdm[pdmNum].canInput[i].oper = pdm.CanInputs[i].Operator;
                        Config.pdm[pdmNum].canInput[i].onValue = pdm.CanInputs[i].OnVal;
                        Config.pdm[pdmNum].canInput[i].mode = pdm.CanInputs[i].Mode;
                    }

                    Config.pdm[pdmNum].canOutput.enable = true;
                    Config.pdm[pdmNum].canOutput.baseId = pdm.BaseId;
                    Config.pdm[pdmNum].canOutput.updateTime = 0;

                    pdmNum++;
                }
            }

            Serialize(path);

            return true;
        }

        public bool Serialize(string filename)
        {
            var jsonString = JsonSerializer.Serialize<DevicesConfig>(Config);
            File.WriteAllText(filename, jsonString);

            //Catch file exceptions and return false
            return true;
        }

        public bool Deserialize(string filename)
        {
            var jsonString = File.ReadAllText(filename);
            Config = JsonSerializer.Deserialize<DevicesConfig>(jsonString);

            //Catch file exceptions and return false
            return true;
        }
    }
}
