using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public enum OutState
    {
        Off,
        On,
        Overcurrent,
        Fault
    }

    public class DingoPdmOutput : NotifyPropertyChangedBase
    {
        private int _number { get; set; }
        public int Number
        {
            get => _number;
            set
            {
                if(_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private double _current { get; set; }
        public double Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        private OutState _state { get; set; }
        public OutState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private double _currentLimit { get; set; }
        public double CurrentLimit
        {
            get => _currentLimit;
            set
            {
                if (_currentLimit != value)
                {
                    _currentLimit = value;
                    OnPropertyChanged(nameof(CurrentLimit));
                }
            }
        }

        private int _resetCount { get; set; }
        public int ResetCount
        {
            get => _resetCount;
            set
            {
                if (_resetCount != value)
                {
                    _resetCount = value;
                    OnPropertyChanged(nameof(ResetCount));
                }
            }
        }

        public DingoPdmOutput()
        {
            _number = 0;
            _state = OutState.Off;
            _current = 0;
            _currentLimit = 0;
            _resetCount = 0;
        }
    }

    public class DingoPdmInput : NotifyPropertyChangedBase
    {
        private int _number { get; set; }
        public int Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                if( _state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
    }

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

        private List<CanDeviceSub> _subPages = new List<CanDeviceSub>();
        public List<CanDeviceSub> SubPages
        {
            get => _subPages;
            private set
            {
                if(_subPages != value)
                {
                    _subPages = value;
                    OnPropertyChanged(nameof(SubPages));
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
                    foreach(var subPage in _subPages)
                    {
                        subPage.UpdateProperty(nameof(IsConnected));
                    }
                }
            } 
        }

        private new ObservableCollection<DingoPdmInput> _digitalInputs { get; set; }
        public new ObservableCollection<DingoPdmInput> DigitalInputs
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

        private ObservableCollection<DingoPdmOutput> _outputs;
        public ObservableCollection<DingoPdmOutput> Outputs
        {
            get => _outputs;
            private set
            {
                if(_outputs != value)
                {
                    _outputs = value;
                    OnPropertyChanged(nameof(Outputs));
                }
            }
        }
        


        public DingoPdmCan(string name, int id)
        {
            Name = name;
            BaseId = id;
            DigitalInputs= new ObservableCollection<DingoPdmInput>();
            for (int i = 0; i < 8; i++)
            {
                DigitalInputs.Add(new DingoPdmInput());
                DigitalInputs[i].Number = i + 1;
            }

            DeviceState = new DevState();
            TotalCurrent=0;
            BatteryVoltage=0;
            BoardTempC=0;
            Outputs = new ObservableCollection<DingoPdmOutput>();
            for(int i=0; i<12; i++)
            {
                Outputs.Add(new DingoPdmOutput());
                Outputs[i].Number = i + 1;
            }

            SubPages.Add(new CanDeviceSub("Settings", this));
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
                DigitalInputs[i].State = Convert.ToBoolean(data[i]);
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
            Outputs[0].Current = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[1].Current = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[2].Current = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[3].Current = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage3(byte[] data)
        {
            Outputs[4].Current = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[5].Current = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[6].Current = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[7].Current = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage4(byte[] data)
        {
            Outputs[8].Current  = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[9].Current  = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[10].Current = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[11].Current = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage5(byte[] data)
        {
            Outputs[0].State = (OutState)((data[0] & 0x0F));
            Outputs[1].State = (OutState)((data[0] & 0xF0) >> 4);
            Outputs[2].State = (OutState)((data[1] & 0x0F));
            Outputs[3].State = (OutState)((data[1] & 0xF0) >> 4);
            Outputs[4].State = (OutState)((data[2] & 0x0F));
            Outputs[5].State = (OutState)((data[2] & 0xF0) >> 4);
            Outputs[6].State = (OutState)((data[3] & 0x0F));
            Outputs[7].State  = (OutState)((data[3] & 0xF0) >> 4);
            Outputs[8].State = (OutState)((data[4] & 0x0F));
            Outputs[9].State = (OutState)((data[4] & 0xF0) >> 4);
            Outputs[10].State = (OutState)((data[5] & 0x0F));
            Outputs[11].State = (OutState)((data[5] & 0xF0) >> 4);
        }

        private void ReadMessage6(byte[] data)
        {
            Outputs[0].CurrentLimit = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[1].CurrentLimit = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[2].CurrentLimit = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[3].CurrentLimit = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage7(byte[] data)
        {
            Outputs[4].CurrentLimit = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[5].CurrentLimit = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[6].CurrentLimit = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[7].CurrentLimit = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage8(byte[] data)
        {
            Outputs[8].CurrentLimit  = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[9].CurrentLimit  = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[10].CurrentLimit = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[11].CurrentLimit = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage9(byte[] data)
        {
            Outputs[0].ResetCount = data[0];
            Outputs[1].ResetCount = data[1];
            Outputs[2].ResetCount = data[2];
            Outputs[3].ResetCount = data[3];
            Outputs[4].ResetCount = data[4];
            Outputs[5].ResetCount = data[5];
            Outputs[6].ResetCount = data[6];
            Outputs[7].ResetCount = data[7];
        }

        private void ReadMessage10(byte[] data)
        {
            Outputs[8].ResetCount = data[0];
            Outputs[9].ResetCount = data[1];
            Outputs[10].ResetCount = data[2];
            Outputs[11].ResetCount = data[3];
        }
    }
}
