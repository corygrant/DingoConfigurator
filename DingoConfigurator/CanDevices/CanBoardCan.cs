using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class CanBoardCan : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name= value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _baseId;
        public int BaseId { 
            get => _baseId;
            private set
            {
                if(_baseId != value)
                {
                    _baseId= value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
        }

        private DateTime _lastRxTime;
        public DateTime LastRxTime { get => _lastRxTime; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        private List<double> _analogInMV = new List<double>(new double[5]);
        public List<double> AnalogInMV { 
            get => _analogInMV; 
            private set
            {
                if(_analogInMV!= value)
                {
                    _analogInMV= value;
                    OnPropertyChanged(nameof(AnalogInMV));
                }
            }
        }

        private double _boardTempC;
        public double BoardTempC { 
            get => _boardTempC; 
            private set
            {
                if(_boardTempC!= value)
                {
                    _boardTempC= value;
                    OnPropertyChanged(nameof(BoardTempC));
                }
            }
        }

        private List<int> _rotarySwitchPos = new List<int>(new int[5]);
        public List<int> RotarySwitchPos { 
            get => _rotarySwitchPos; 
            private set
            {
                if (_rotarySwitchPos!= value)
                {
                    _rotarySwitchPos= value;
                    OnPropertyChanged(nameof(RotarySwitchPos));
                }
            }
        }

        private List<bool> _digitalIn = new List<bool>(new bool[8]);
        public List<bool> DigitalIn { 
            get => _digitalIn; 
            private set
            {
                if(_digitalIn!= value)
                {
                    _digitalIn= value;
                    OnPropertyChanged(nameof(DigitalIn));
                }
            }
        }

        private List<bool> _analogDigitalIn = new List<bool>(new bool[5]);   
        public List<bool> AnalogDigitalIn { 
            get => _analogDigitalIn;
            private set
            {
                if(_analogDigitalIn != value)
                {
                    _analogDigitalIn= value;
                    OnPropertyChanged(nameof(AnalogDigitalIn));
                }
            }
        }

        private List<bool> _digitalOut = new List<bool>(new bool[4]);
        public List<bool> DigitalOut { 
            get => _digitalOut; private set
            {
                if (_digitalOut != value)
                {
                    _digitalOut = value;
                    OnPropertyChanged(nameof(DigitalOut));
                }
            }
        }

        private int _heartbeat;
        public int Heartbeat
        {
            get { return _heartbeat; }
            set { 
                if (_heartbeat != value)
                {
                    _heartbeat = value;
                    OnPropertyChanged(nameof(Heartbeat));
                }
            }
        }


        public CanBoardCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
        }

        public void UpdateIsConnected()
        {
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public bool Read(int id, byte[] data)
        {
            if ((id < BaseId) || (id > BaseId + 2)) return false;

            if (id == BaseId + 0) ReadMessage0(data);
            if (id == BaseId + 1) ReadMessage1(data);
            if (id == BaseId + 2) ReadMessage2(data);

            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }

        private void ReadMessage0(byte[] data)
        {
            AnalogInMV[0] = Convert.ToDouble((data[1] << 8) + data[0]);
            AnalogInMV[1] = Convert.ToDouble((data[3] << 8) + data[2]);
            AnalogInMV[2] = Convert.ToDouble((data[5] << 8) + data[4]);
            AnalogInMV[3] = Convert.ToDouble((data[7] << 8) + data[6]);
        }

        private void ReadMessage1(byte[] data)
        {
            AnalogInMV[4] = Convert.ToDouble((data[1] << 8) + data[0]);
            //Byte 2 empty
            //Byte 3 empty
            //Byte 4 empty
            //Byte 5 empty
            BoardTempC = Convert.ToDouble(((data[7] << 8) + data[6]) / 100.0);
        }

        private void ReadMessage2(byte[] data)
        {
            RotarySwitchPos[0] = Convert.ToInt16(data[0] & 0x0F);
            RotarySwitchPos[1] = Convert.ToInt16(data[0] & 0xF0);
            RotarySwitchPos[2] = Convert.ToInt16(data[1] & 0x0F);
            RotarySwitchPos[3] = Convert.ToInt16(data[1] & 0xF0);
            RotarySwitchPos[4] = Convert.ToInt16(data[2] & 0x0F);

            DigitalIn[0] = Convert.ToBoolean(data[4] & 0x1);
            DigitalIn[1] = Convert.ToBoolean((data[4] & 0x2) >> 1);
            DigitalIn[2] = Convert.ToBoolean((data[4] & 0x4) >> 2);
            DigitalIn[3] = Convert.ToBoolean((data[4] & 0x8) >> 3);
            DigitalIn[4] = Convert.ToBoolean((data[4] & 0x10) >> 4);
            DigitalIn[5] = Convert.ToBoolean((data[4] & 0x20) >> 5);
            DigitalIn[6] = Convert.ToBoolean((data[4] & 0x40) >> 6);
            DigitalIn[7] = Convert.ToBoolean((data[4] & 0x80) >> 7);

            AnalogDigitalIn[0] = Convert.ToBoolean(data[5] & 0x1);
            AnalogDigitalIn[1] = Convert.ToBoolean((data[5] & 0x2) >> 1);
            AnalogDigitalIn[2] = Convert.ToBoolean((data[5] & 0x4) >> 2);
            AnalogDigitalIn[3] = Convert.ToBoolean((data[5] & 0x8) >> 3);
            AnalogDigitalIn[4] = Convert.ToBoolean((data[5] & 0x10) >> 4);

            DigitalOut[0] = Convert.ToBoolean(data[6] & 0x1);
            DigitalOut[1] = Convert.ToBoolean((data[6] & 0x2) >> 1);
            DigitalOut[2] = Convert.ToBoolean((data[6] & 0x4) >> 2);
            DigitalOut[3] = Convert.ToBoolean((data[6] & 0x8) >> 3);

            Heartbeat = Convert.ToInt16(data[7]);
        }
    }
}
