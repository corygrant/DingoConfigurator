using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DingoPdmCan : ICanDevice
    {
        public int BaseId { get; private set; }
        private List<bool> _digitalInputs { get; set; }
        public List<bool> DigitalInputs => _digitalInputs;

        public enum DevState
        {
            Initialize,
            Running,
            Configure,
            Error
        }
        public DevState DeviceState { get; private set; }
        public double TotalCurrent { get; private set; }
        public double BatteryVoltage { get; private set; }
        public double BoardTempC { get; private set; }
        public double BoardTempF { get; private set; }
        private List<double> _outputCurrent { get; set; }
        public List<double> OutputCurrent { get => _outputCurrent; }
        public enum OutState 
        { 
            Off,
            On,
            InRush,
            ShortCircuit,
            Overcurrent,
            Fault,
            Suspended,
            TurningOff,
            TurningOn,
            InRushing,
            ShortCircuiting,
            Overcurrenting,
            Faulting,
            Suspending
        }
        private List<OutState> _outputState { get; set; }
        public List<OutState> OutputState { get => _outputState; }
        private List<double> _outputCurrentLimit { get; set; }
        public List<double> OutputCurrentLimit { get => _outputCurrentLimit; }
        private List<int> _outputResetCount { get; set; }
        public List<int> OutputResetCount { get => _outputResetCount; }

        public DingoPdmCan(int id)
        {
            BaseId = id;
            _digitalInputs= new List<bool>(new bool[8]);
            DeviceState= new DevState();
            TotalCurrent=0;
            BatteryVoltage=0;
            BoardTempC=0;
            _outputCurrent= new List<double>(new double[12]);
            _outputState = new List<OutState>(new OutState[12]);
            _outputCurrentLimit = new List<double>(new double[12]);
            _outputResetCount = new List<int>(new int[12]);
        }

        public bool Read(int id, byte[] data)
        {
            if ((id < BaseId) || (id > BaseId + 10)) return false;

            if (id == BaseId + 0) ReadMessage0(data);
            if (id == BaseId + 1) ReadMessage1(data);
            if (id == BaseId + 2) ReadMessage2(data);
            if (id == BaseId + 3) ReadMessage3(data);
            if (id == BaseId + 4) ReadMessage4(data);
            if (id == BaseId + 5) ReadMessage5(data);
            if (id == BaseId + 6) ReadMessage6(data);
            if (id == BaseId + 7) ReadMessage7(data);
            if (id == BaseId + 8) ReadMessage8(data);
            if (id == BaseId + 9) ReadMessage9(data);
            if (id == BaseId + 10) ReadMessage10(data);

            return true;
        }

        private void ReadMessage0(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                _digitalInputs[i] = Convert.ToBoolean(data[i]);
            }
        }

        private void ReadMessage1(byte[] data)
        {
            DeviceState = (DevState)data[0];
            //Byte 1 = empty
            TotalCurrent = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            BatteryVoltage= Convert.ToDouble(((data[4]<< 8) + data[5]) / 10.0);
            BoardTempC = Convert.ToDouble((data[6] << 8) + data[7]);
            BoardTempF = BoardTempC * 1.8 + 32;
        }

        private void ReadMessage2(byte[] data)
        {
            _outputCurrent[0] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrent[1] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrent[2] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrent[3] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage3(byte[] data)
        {
            _outputCurrent[4] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrent[5] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrent[6] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrent[7] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage4(byte[] data)
        {
            _outputCurrent[8] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrent[9] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrent[10] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrent[11] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage5(byte[] data)
        {
            _outputState[0] = (OutState)((data[0] & 0xF0) >> 4);
            _outputState[1] = (OutState)((data[0] & 0x0F));
            _outputState[2] = (OutState)((data[1] & 0xF0) >> 4);
            _outputState[3] = (OutState)((data[1] & 0x0F));
            _outputState[4] = (OutState)((data[2] & 0xF0) >> 4);
            _outputState[5] = (OutState)((data[2] & 0x0F));
            _outputState[6] = (OutState)((data[3] & 0xF0) >> 4);
            _outputState[7] = (OutState)((data[3] & 0x0F));
            _outputState[8] = (OutState)((data[4] & 0xF0) >> 4);
            _outputState[9] = (OutState)((data[4] & 0x0F));
            _outputState[10] = (OutState)((data[5] & 0xF0) >> 4);
            _outputState[11] = (OutState)((data[5] & 0x0F));
        }

        private void ReadMessage6(byte[] data)
        {
            _outputCurrentLimit[0] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrentLimit[1] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrentLimit[2] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrentLimit[3] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage7(byte[] data)
        {
            _outputCurrentLimit[4] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrentLimit[5] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrentLimit[6] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrentLimit[7] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage8(byte[] data)
        {
            _outputCurrentLimit[8] = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            _outputCurrentLimit[9] = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            _outputCurrentLimit[10] = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            _outputCurrentLimit[11] = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage9(byte[] data)
        {
            _outputResetCount[0] = data[0];
            _outputResetCount[1] = data[1];
            _outputResetCount[2] = data[2];
            _outputResetCount[3] = data[3];
            _outputResetCount[4] = data[4];
            _outputResetCount[5] = data[5];
            _outputResetCount[6] = data[6];
            _outputResetCount[7] = data[7];
        }

        private void ReadMessage10(byte[] data)
        {
            _outputResetCount[8] = data[0];
            _outputResetCount[9] = data[1];
            _outputResetCount[10] = data[2];
            _outputResetCount[11] = data[3];
        }
    }
}
