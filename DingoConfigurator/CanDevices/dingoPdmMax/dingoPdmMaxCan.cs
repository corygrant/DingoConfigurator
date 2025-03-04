using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices.dingoPdmMax
{
    public class dingoPdmMaxCan : DingoPdmCan
    {
        protected override int _minMajorVersion { get; } = 0;
        protected override int _minMinorVersion { get; } = 4;
        protected override int _minBuildVersion { get; } = 7;

        protected override int _numOutputs { get; } = 4;

        public dingoPdmMaxCan(string name, int baseId) : base(name, baseId)
        {

        }

        protected override void ReadMessage2(byte[] data)
        {
            //Unused with Max
        }
        protected override void ReadMessage3(byte[] data)
        {
            Outputs[0].State = (OutState)((data[0] & 0x0F));
            Outputs[1].State = (OutState)(data[0] >> 4);
            Outputs[2].State = (OutState)((data[1] & 0x0F));
            Outputs[3].State = (OutState)(data[1] >> 4);
            
            Wipers[0].SlowState = Convert.ToBoolean(data[4] & 0x01);
            Wipers[0].FastState = Convert.ToBoolean((data[4] >> 1) & 0x01);
            Wipers[0].State = (WiperState)(data[5] >> 4);
            Wipers[0].Speed = (WiperSpeed)(data[5] & 0x0F);

            Flashers[0].Value = Convert.ToBoolean(data[6] & 0x01) && Flashers[0].Enabled;
            Flashers[1].Value = Convert.ToBoolean((data[6] >> 1) & 0x01) && Flashers[1].Enabled;
            Flashers[2].Value = Convert.ToBoolean((data[6] >> 2) & 0x01) && Flashers[2].Enabled;
            Flashers[3].Value = Convert.ToBoolean((data[6] >> 3) & 0x01) && Flashers[3].Enabled;

            //TODO: remove Inputvalue from Flasher. It is not used
            //Flashers[0].InputValue = Convert.ToBoolean((data[6] >> 4) & 0x01);
            //Flashers[1].InputValue = Convert.ToBoolean((data[6] >> 5) & 0x01);
            //Flashers[2].InputValue = Convert.ToBoolean((data[6] >> 6) & 0x01);
            //Flashers[3].InputValue = Convert.ToBoolean((data[6] >> 7) & 0x01);
        }

        protected override void ReadMessage4(byte[] data)
        {
            Outputs[0].ResetCount = data[0];
            Outputs[1].ResetCount = data[1];
            Outputs[2].ResetCount = data[2];
            Outputs[3].ResetCount = data[3];
        }

        protected override void ReadMessage15(byte[] data)
        {
            Outputs[0].CurrentDutyCycle = data[0];
            Outputs[1].CurrentDutyCycle = data[1];
            Outputs[2].CurrentDutyCycle = data[2];
            Outputs[3].CurrentDutyCycle = data[3];
        }

    }
}
