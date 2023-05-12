using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanBoardCan : ICanDevice
    {
        public string Name { get; set; }
        public int BaseId { get; private set; }
        private DateTime _lastRxTime { get; set; }
        public DateTime LastRxTime { get => _lastRxTime; }
        public bool IsConnected
        {
            get
            {
                TimeSpan timeSpan = DateTime.Now - _lastRxTime;
                return (timeSpan.TotalMilliseconds < 500);
            }
        }

        private List<double> _analogInMV { get; set; }
        public List<double> AnalogInMV { get => _analogInMV; }

        public double BoardTempC { get; private set; }

        private List<int> _rotarySwitchPos { get; set; }
        public List<int> RotarySwitchPos { get => _rotarySwitchPos; }

        private List<bool> _digitalIn { get; set; }
        public List<bool> DigitalIn { get => _digitalIn; }

        private List<bool> _analogDigitalIn { get; set; }   
        public List<bool> AnalogDigitalIn { get => _analogDigitalIn; }

        private List<bool> _digitalOut { get; set; }
        public List<bool> DigitalOut { get => _digitalOut; }

        public int Heartbeat;


        public CanBoardCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
            _lastRxTime = DateTime.MinValue;
            _analogInMV = new List<double>(new double[5]);
            BoardTempC = 0.0;
            _rotarySwitchPos = new List<int>(new int[5]);
            _digitalIn = new List<bool> (new bool[8]);
            _analogDigitalIn = new List<bool>(new bool[5]);
            _digitalOut = new List<bool>(new bool[4]);
        }

        public bool Read(int id, byte[] data)
        {
            if ((id < BaseId) || (id > BaseId + 2)) return false;

            if (id == BaseId + 0) ReadMessage0(data);
            if (id == BaseId + 1) ReadMessage1(data);
            if (id == BaseId + 2) ReadMessage2(data);

            _lastRxTime = DateTime.Now;

            return true;
        }

        private void ReadMessage0(byte[] data)
        {
            _analogInMV[0] = Convert.ToDouble((data[1] << 8) + data[0]);
            _analogInMV[1] = Convert.ToDouble((data[3] << 8) + data[2]);
            _analogInMV[2] = Convert.ToDouble((data[5] << 8) + data[4]);
            _analogInMV[3] = Convert.ToDouble((data[7] << 8) + data[6]);
        }

        private void ReadMessage1(byte[] data)
        {
            _analogInMV[4] = Convert.ToDouble((data[1] << 8) + data[0]);
            //Byte 2 empty
            //Byte 3 empty
            //Byte 4 empty
            //Byte 5 empty
            BoardTempC = Convert.ToDouble(((data[7] << 8) + data[6]) / 100.0);
        }

        private void ReadMessage2(byte[] data)
        {
            _rotarySwitchPos[0] = Convert.ToInt16(data[0] & 0x0F);
            _rotarySwitchPos[1] = Convert.ToInt16(data[0] & 0xF0);
            _rotarySwitchPos[2] = Convert.ToInt16(data[1] & 0x0F);
            _rotarySwitchPos[3] = Convert.ToInt16(data[1] & 0xF0);
            _rotarySwitchPos[4] = Convert.ToInt16(data[2] & 0x0F);

            _digitalIn[0] = Convert.ToBoolean(data[4] & 0x1);
            _digitalIn[1] = Convert.ToBoolean((data[4] & 0x2) >> 1);
            _digitalIn[2] = Convert.ToBoolean((data[4] & 0x4) >> 2);
            _digitalIn[3] = Convert.ToBoolean((data[4] & 0x8) >> 3);
            _digitalIn[4] = Convert.ToBoolean((data[4] & 0x10) >> 4);
            _digitalIn[5] = Convert.ToBoolean((data[4] & 0x20) >> 5);
            _digitalIn[6] = Convert.ToBoolean((data[4] & 0x40) >> 6);
            _digitalIn[7] = Convert.ToBoolean((data[4] & 0x80) >> 7);

            _analogDigitalIn[0] = Convert.ToBoolean(data[5] & 0x1);
            _analogDigitalIn[1] = Convert.ToBoolean((data[5] & 0x2) >> 1);
            _analogDigitalIn[2] = Convert.ToBoolean((data[5] & 0x4) >> 2);
            _analogDigitalIn[3] = Convert.ToBoolean((data[5] & 0x8) >> 3);
            _analogDigitalIn[4] = Convert.ToBoolean((data[5] & 0x10) >> 4);

            _digitalOut[0] = Convert.ToBoolean(data[6] & 0x1);
            _digitalOut[1] = Convert.ToBoolean((data[6] & 0x2) >> 1);
            _digitalOut[2] = Convert.ToBoolean((data[6] & 0x4) >> 2);
            _digitalOut[3] = Convert.ToBoolean((data[6] & 0x8) >> 3);

            Heartbeat = Convert.ToInt16(data[7]);
        }
    }
}
