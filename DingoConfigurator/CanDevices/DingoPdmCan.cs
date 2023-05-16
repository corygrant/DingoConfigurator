using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public class DingoPdmCan : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        public string Name { 
            get => _name; 
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _baseId;
        public int BaseId
        {
            get => _baseId;
            private set
            {
                if (_baseId != value)
                {
                    _baseId = value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
        }

        private DateTime _lastRxTime { get; set; }
        public DateTime LastRxTime { get => _lastRxTime;}

        private bool _isConnected;
        public bool IsConnected {
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

        private List<bool> _digitalInputs { get; set; }
        public List<bool> DigitalInputs
        {
            get => _digitalInputs;
            set
            {
                if (_digitalInputs != value)
                {
                    _digitalInputs = value;
                    OnPropertyChanged(nameof(DigitalInputs));
                }
            }
        }

        public enum DevState
        {
            Initialize,
            Running,
            Configure,
            Error
        }

        private DevState _deviceState;
        public DevState DeviceState { 
            get => _deviceState;
            private set
            {
                if (_deviceState != value)
                {
                    _deviceState = value;
                    OnPropertyChanged(nameof(DevState));
                }
            }
        }

        private double _totalCurrent;
        public double TotalCurrent { 
            get => _totalCurrent; 
            private set
            {
                if (_totalCurrent != value)
                {
                    _totalCurrent = value;
                    OnPropertyChanged(nameof(TotalCurrent));
                }
            }
        }

        private double _batteryVoltage;
        public double BatteryVoltage
        {
            get => _batteryVoltage;
            private set
            {
                if (_batteryVoltage != value)
                {
                    _batteryVoltage = value;
                    OnPropertyChanged(nameof(BatteryVoltage));
                }
            }
        }

        private double _boardTempC;
        public double BoardTempC
        {
            get => _boardTempC;
            private set
            {
                if (_boardTempC != value)
                {
                    _boardTempC = value;
                    OnPropertyChanged(nameof(BoardTempC));
                }
            }
        }

        private double _boardTempF;
        public double BoardTempF
        {
            get => _boardTempF;
            private set
            {
                if (_boardTempF != value)
                {
                    _boardTempF = value;
                    OnPropertyChanged(nameof(BoardTempF));
                }
            }
        }

        private List<double> _outputCurrent { get; set; }
        public List<double> OutputCurrent
        {
            get => _outputCurrent;
            private set
            {
                if (_outputCurrent != value)
                {
                    _outputCurrent = value;
                    OnPropertyChanged(nameof(OutputCurrent));
                }
            }
        }

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
        public List<OutState> OutputState
        {
            get => _outputState;
            private set
            {
                if (_outputState != value)
                {
                    _outputState = value;
                    OnPropertyChanged(nameof(OutputState));
                }
            }
        }

        private List<double> _outputCurrentLimit { get; set; }
        public List<double> OutputCurrentLimit
        {
            get => _outputCurrentLimit;
            private set
            {
                if (_outputCurrentLimit != value)
                {
                    _outputCurrentLimit = value;
                    OnPropertyChanged(nameof(OutputCurrentLimit));
                }
            }
        }

        private List<int> _outputResetCount { get; set; }
        public List<int> OutputResetCount
        {
            get => _outputResetCount;
            private set
            {
                if (_outputResetCount != value)
                {
                    _outputResetCount = value;
                    OnPropertyChanged(nameof(OutputResetCount));
                }
            }
        }


        public DingoPdmCan(string name, int id)
        {
            Name = name;
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

        public void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
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

            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

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
