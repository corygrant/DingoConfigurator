using CanDevices.DingoPdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.CanBoard
{
    public static class CanBoardConfigHandler
    {
        public static void UpdateConfig(ref CanBoardCan cb, ref CanBoardConfig config, int cbNum)
        {
            if (cb == null) return;
            if (config == null) return;

            config.name = $"CANBoard{cbNum}";
            config.label = cb.Name;
            config.baseCanId = cb.BaseId;

            for (int i = 0; i < config.analogInput.Length; i++)
            {
                config.analogInput[i].name = $"AnalogInput{i}";
                config.analogInput[i].label = cb.AnalogIn[i].Name;
            }

            for (int i = 0; i < config.digitalInput.Length; i++)
            {
                config.digitalInput[i].name = $"DigitalInput{i}";
                config.digitalInput[i].label = cb.DigitalIn[i].Name;
            }

            for (int i = 0; i < config.output.Length; i++)
            {
                config.output[i].name = $"DigitalOutput{i}";
                config.output[i].label = cb.DigitalOut[i].Name;
            }
        }

        public static void ApplyConfig(ref CanBoardCan cb, CanBoardConfig config)
        {
            if (cb == null) return;

            int index = 0;

            foreach (var ai in cb.AnalogIn)
            {
                ai.Name = config.analogInput[index].label;
                index++;
            }

            index = 0;

            foreach (var di in cb.DigitalIn)
            {
                di.Name = config.digitalInput[index].label;
                index++;
            }

            index = 0;

            foreach (var output in cb.DigitalOut)
            {
                output.Name = config.output[index].label;
                index++;
            }
        }
    }

    public class CanBoardConfig
    {
        public CanBoardConfig()
        {
            name = String.Empty;
            label = String.Empty;
            baseCanId = 0;

            analogInput = new IOConfig[]
            {
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig()
            };

            digitalInput = new IOConfig[]
            {
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig()
            };

            output = new IOConfig[]
            {
                new IOConfig(),
                new IOConfig(),
                new IOConfig(),
                new IOConfig()
            };
        }

        public string name { get; set; }
        public string label { get; set; }
        public int baseCanId { get; set; }

        public IOConfig[] analogInput { get; set;}
        public IOConfig[] digitalInput { get; set;}
        public IOConfig[] output { get; set;}
    }

    public class IOConfig
    {
        public IOConfig()
        {
            label = String.Empty;
            name = String.Empty;
        }

        public string label { get; set; }
        public string name { get; set; }
    }

}
