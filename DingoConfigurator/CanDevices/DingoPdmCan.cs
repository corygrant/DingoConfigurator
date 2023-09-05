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

    public enum WiperMode
    {
        DigIn,
        IntIn,
        MixIn
    }

    public enum WiperState
    {
        Parked,
        Parking,
        SlowOn,
        FastOn,
        InterPause,
        InterOn,
        Wash,
        Swipe
    }

    public enum WiperSpeed
    {
        Park,
        Slow,
        Fast,
        Inter1,
        Inter2,
        Inter3,
        Inter4,
        Inter5,
        Inter6
    }

    public enum VarMap
    {
        None = 0,
        DigIn1,
        DigIn2,
        CANIn1,
        CANIn2,
        CANIn3,
        CANIn4,
        CANIn5,
        CANIn6,
        CANIn7,
        CANIn8,
        CANIn9,
        CANIn10,
        CANIn11,
        CANIn12,
        CANIn13,
        CANIn14,
        CANIn15,
        CANIn16,
        CANIn17,
        CANIn18,
        CANIn19,
        CANIn20,
        CANIn21,
        CANIn22,
        CANIn23,
        CANIn24,
        CANIn25,
        CANIn26,
        CANIn27,
        CANIn28,
        CANIn29,
        CANIn30,
        CANIn31,
        CANIn32,
        VirtIn1,
        VirtIn2,
        VirtIn3,
        VirtIn4,
        VirtIn5,
        VirtIn6,
        VirtIn7,
        VirtIn8,
        VirtIn9,
        VirtIn10,
        VirtIn11,
        VirtIn12,
        VirtIn13,
        VirtIn14,
        VirtIn15,
        VirtIn16,
        Output1,
        Output2,
        Output3,
        Output4,
        Output5,
        Output6,
        Output7,
        Output8,
        WiperSlow,
        WiperFast
    }

    public class DingoPdmOutput : NotifyPropertyChangedBase
    {
        private string _name;
        public string Name
        {
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

        private int _number;
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

        private double _current;
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

        private OutState _state;
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

        private double _currentLimit;
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

        private int _resetCount;
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
        private string _name;
        public string Name
        {
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

        private int _number;
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

        private bool _invertInput;
        public bool InvertInput
        {
            get => _invertInput;
            set
            {
                if (_invertInput != value)
                {
                    _invertInput = value;
                    OnPropertyChanged(nameof(InvertInput));
                }
            }
        }
    }

    public class DingoPdmCanInput : NotifyPropertyChangedBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _number;
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

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private int _lowByte;
        public int LowByte
        {
            get => _lowByte;
            set
            {
                if (_lowByte != value)
                {
                    _lowByte = value;
                    OnPropertyChanged(nameof(LowByte));
                }
            }
        }

        private int _highByte;
        public int HighByte
        {
            get => _highByte;
            set
            {
                if (_highByte != value)
                {
                    _highByte = value;
                    OnPropertyChanged(nameof(HighByte));
                }
            }
        }

        private Operator _operator;
        public Operator Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    OnPropertyChanged(nameof(Operator));
                }
            }
        }

        private int _onVal;
        public int OnVal
        {
            get => _onVal;
            set
            {
                if (_onVal != value)
                {
                    _onVal = value;
                    OnPropertyChanged(nameof(OnVal));
                }
            }
        }

        private InputMode _mode;
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }
    }

    public class DingoPdmVirtualInput : NotifyPropertyChangedBase
    {
        private string _name;
        public string Name
        {
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

        private int _number;
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

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private bool _not0;
        public bool Not0
        {
            get => _not0;
            set
            {
                if (_not0 != value)
                {
                    _not0 = value;
                    OnPropertyChanged(nameof(Not0));
                }
            }
        }

        private VarMap _var0;
        public VarMap Var0
        {
            get => _var0;
            set
            {
                if (_var0 != value)
                {
                    _var0 = value;
                    OnPropertyChanged(nameof(Var0));
                }
            }
        }

        private Conditional _cond0;
        public Conditional Cond0
        {
            get => _cond0;
            set
            {
                if (_cond0 != value)
                {
                    _cond0 = value;
                    OnPropertyChanged(nameof(Cond0));
                }
            }
        }

        private bool _not1;
        public bool Not1
        {
            get => _not1;
            set
            {
                if (_not1 != value)
                {
                    _not1 = value;
                    OnPropertyChanged(nameof(Not1));
                }
            }
        }

        private VarMap _var1;
        public VarMap Var1
        {
            get => _var1;
            set
            {
                if (_var1 != value)
                {
                    _var1 = value;
                    OnPropertyChanged(nameof(Var1));
                }
            }
        }

        private Conditional _cond1;
        public Conditional Cond1
        {
            get => _cond1;
            set
            {
                if (_cond1 != value)
                {
                    _cond1 = value;
                    OnPropertyChanged(nameof(Cond1));
                }
            }
        }

        private bool _not2;
        public bool Not2
        {
            get => _not2;
            set
            {
                if (_not2 != value)
                {
                    _not2 = value;
                    OnPropertyChanged(nameof(Not2));
                }
            }
        }

        private VarMap _var2;
        public VarMap Var2
        {
            get => _var2;
            set
            {
                if (_var2 != value)
                {
                    _var2 = value;
                    OnPropertyChanged(nameof(Var2));
                }
            }
        }

        private InputMode _mode;
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }
    }

    public class DingoPdmWiper : NotifyPropertyChangedBase
    {
        private string _name;
        public string Name
        {
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

        private int _number;
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

        private bool _slowState;
        public bool SlowState
        {
            get => _slowState;
            set
            {
                if (_slowState != value)
                {
                    _slowState = value;
                    OnPropertyChanged(nameof(SlowState));
                }
            }
        }

        private bool _fastState;
        public bool FastState
        {
            get => _fastState;
            set
            {
                if (_fastState != value)
                {
                    _fastState = value;
                    OnPropertyChanged(nameof(FastState));
                }
            }
        }

        private WiperMode _mode;
        public WiperMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        private WiperState _state;
        public WiperState State
        {
            get => _state;
            set
            {
                if(_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private WiperSpeed _speed;
        public WiperSpeed Speed
        {
            get => _speed;
            set
            {
                if (value != _speed)
                {
                    _speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }
        private VarMap _slowInput;
        public VarMap SlowInput
        {
            get => _slowInput;
            set
            {
                if (_slowInput != value)
                {
                    _slowInput = value;
                    OnPropertyChanged(nameof(SlowInput));
                }
            }
        }

        private VarMap _fastInput;
        public VarMap FastInput
        {
            get => _fastInput;
            set
            {
                if (_fastInput != value)
                {
                    _fastInput = value;
                    OnPropertyChanged(nameof(FastInput));
                }
            }
        }

        private VarMap _interInput;
        public VarMap InterInput
        {
            get => _interInput;
            set
            {
                if (_interInput != value)
                {
                    _interInput = value;
                    OnPropertyChanged(nameof(InterInput));
                }
            }
        }

        private VarMap _onInput;
        public VarMap OnInput
        {
            get => _onInput;
            set
            {
                if (_onInput != value)
                {
                    _onInput = value;
                    OnPropertyChanged(nameof(OnInput));
                }
            }
        }

        private VarMap _speedInput;
        public VarMap SpeedInput
        {
            get => _speedInput;
            set
            {
                if (_speedInput != value)
                {
                    _speedInput = value;
                    OnPropertyChanged(nameof(SpeedInput));
                }
            }
        }

        private VarMap _parkInput;
        public VarMap ParkInput
        {
            get => _parkInput;
            set
            {
                if (_parkInput != value)
                {
                    _parkInput = value;
                    OnPropertyChanged(nameof(ParkInput));
                }
            }
        }

        private bool _parkStopLevel;
        public bool ParkStopLevel
        {
            get => _parkStopLevel;
            set
            {
                if (_parkStopLevel != value)
                {
                    _parkStopLevel = value;
                    OnPropertyChanged(nameof(ParkStopLevel));
                }
            }
        }

        private VarMap _swipeInput;
        public VarMap SwipeInput
        {
            get => _swipeInput;
            set
            {
                if (_swipeInput != value)
                {
                    _swipeInput = value;
                    OnPropertyChanged(nameof(SwipeInput));
                }
            }
        }

        private VarMap _washInput;
        public VarMap WashInput
        {
            get => _washInput;
            set
            {
                if (_washInput != value)
                {
                    _washInput = value;
                    OnPropertyChanged(nameof(WashInput));
                }
            }
        }

        private int _washWipeCycles;
        public int WashWipeCycles
        {
            get => _washWipeCycles;
            set
            {
                if (_washWipeCycles != value)
                {
                    _washWipeCycles = value;
                    OnPropertyChanged(nameof(WashWipeCycles));
                }
            }
        }

        private int[] _speedMap;
        public int[] SpeedMap
        {
            get => _speedMap;
            set
            {
                if (_speedMap != value)
                {
                    _speedMap = value;
                    OnPropertyChanged(nameof(SpeedMap));
                }
            }
        }

        private int[] _intermitTime;
        public int[] IntermitTime
        {
            get => _intermitTime;
            set
            {
                if (_intermitTime != value)
                {
                    _intermitTime = value;
                    OnPropertyChanged(nameof(IntermitTime));
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

        private ObservableCollection<DingoPdmInput> _digitalInputs { get; set; }
        public ObservableCollection<DingoPdmInput> DigitalInputs
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

        private ObservableCollection<DingoPdmCanInput> _canInputs;
        public ObservableCollection<DingoPdmCanInput> CanInputs
        {
            get => _canInputs;
            private set
            {
                if (_canInputs != value)
                {
                    _canInputs = value;
                    OnPropertyChanged(nameof(CanInputs));
                }
            }
        }

        private ObservableCollection<DingoPdmVirtualInput> _virtualInputs;
        public ObservableCollection<DingoPdmVirtualInput> VirtualInputs
        {
            get => _virtualInputs;
            private set
            {
                if (_virtualInputs != value)
                {
                    _virtualInputs = value;
                    OnPropertyChanged(nameof(VirtualInputs));
                }
            }
        }

        private ObservableCollection<DingoPdmWiper> _wipers;
        public ObservableCollection<DingoPdmWiper> Wipers
        {
            get => _wipers;
            private set
            {
                if (_wipers != value)
                {
                    _wipers = value;
                    OnPropertyChanged(nameof(Wipers));
                }
            }
        }

        public DingoPdmCan(string name, int id)
        {
            Name = name;
            BaseId = id;
            DigitalInputs= new ObservableCollection<DingoPdmInput>();
            for (int i = 0; i < 2; i++)
            {
                DigitalInputs.Add(new DingoPdmInput());
                DigitalInputs[i].Number = i + 1;
            }

            TotalCurrent=0;
            BatteryVoltage=0;
            BoardTempC=0;

            Outputs = new ObservableCollection<DingoPdmOutput>();
            for(int i=0; i<8; i++)
            {
                Outputs.Add(new DingoPdmOutput());
                Outputs[i].Number = i + 1;
            }

            CanInputs = new ObservableCollection<DingoPdmCanInput>();
            for(int i=0; i<32; i++)
            {
                CanInputs.Add(new DingoPdmCanInput());
                CanInputs[i].Number = i + 1;
            }

            VirtualInputs = new ObservableCollection<DingoPdmVirtualInput>();
            for(int i=0; i<16; i++)
            {
                VirtualInputs.Add(new DingoPdmVirtualInput());
                VirtualInputs[i].Number = i + 1;
            }

            Wipers = new ObservableCollection<DingoPdmWiper>();
            Wipers.Add(new DingoPdmWiper());

            SubPages.Add(new CanDeviceSub("States", this));
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
            if ((id < BaseId) || (id > BaseId + 17)) return false;

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
            if (id == BaseId + 11) ReadMessage11(data);
            if (id == BaseId + 12) ReadMessage12(data);
            if (id == BaseId + 13) ReadMessage13(data);
            if (id == BaseId + 14) ReadMessage14(data);
            if (id == BaseId + 15) ReadMessage15(data);
            if (id == BaseId + 16) ReadMessage16(data);
            if (id == BaseId + 17) ReadMessage17(data);

            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }

        private void ReadMessage0(byte[] data)
        {
            DigitalInputs[0].State = Convert.ToBoolean(data[0] & 0x01);
            DigitalInputs[1].State = Convert.ToBoolean((data[0] >> 1) & 0x01);

            TotalCurrent = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            BatteryVoltage = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            BoardTempC = Convert.ToDouble((data[6] << 8) + data[7]);
            BoardTempF = BoardTempC * 1.8 + 32;

        }

        private void ReadMessage1(byte[] data)
        {
            
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

        }

        private void ReadMessage11(byte[] data)
        {
            CanInputs[0].Value = data[0];
            CanInputs[1].Value = data[1];
            CanInputs[2].Value = data[2];
            CanInputs[3].Value = data[3];
            CanInputs[4].Value = data[4];
            CanInputs[5].Value = data[5];
            CanInputs[6].Value = data[6];
            CanInputs[7].Value = data[7];
        }

        private void ReadMessage12(byte[] data)
        {
            CanInputs[8].Value = data[0];
            CanInputs[9].Value = data[1];
            CanInputs[10].Value = data[2];
            CanInputs[11].Value = data[3];
            CanInputs[12].Value = data[4];
            CanInputs[13].Value = data[5];
            CanInputs[14].Value = data[6];
            CanInputs[15].Value = data[7];
        }

        private void ReadMessage13(byte[] data)
        {
            CanInputs[16].Value = data[0];
            CanInputs[17].Value = data[1];
            CanInputs[18].Value = data[2];
            CanInputs[19].Value = data[3];
            CanInputs[20].Value = data[4];
            CanInputs[21].Value = data[5];
            CanInputs[22].Value = data[6];
            CanInputs[23].Value = data[7];
        }

        private void ReadMessage14(byte[] data)
        {
            CanInputs[24].Value = data[0];
            CanInputs[25].Value = data[1];
            CanInputs[26].Value = data[2];
            CanInputs[27].Value = data[3];
            CanInputs[28].Value = data[4];
            CanInputs[29].Value = data[5];
            CanInputs[30].Value = data[6];
            CanInputs[31].Value = data[7];
        }

        private void ReadMessage15(byte[] data)
        {
            VirtualInputs[0].Value = data[0];
            VirtualInputs[1].Value = data[1];
            VirtualInputs[2].Value = data[2];
            VirtualInputs[3].Value = data[3];
            VirtualInputs[4].Value = data[4];
            VirtualInputs[5].Value = data[5];
            VirtualInputs[6].Value = data[6];
            VirtualInputs[7].Value = data[7];
        }

        private void ReadMessage16(byte[] data)
        {
            VirtualInputs[8].Value = data[0];
            VirtualInputs[9].Value = data[1];
            VirtualInputs[10].Value = data[2];
            VirtualInputs[11].Value = data[3];
            VirtualInputs[12].Value = data[4];
            VirtualInputs[13].Value = data[5];
            VirtualInputs[14].Value = data[6];
            VirtualInputs[15].Value = data[7];
        }

        private void ReadMessage17(byte[] data)
        {
            Wipers[0].SlowState = Convert.ToBoolean(data[0] & 0x01);
            Wipers[0].FastState = Convert.ToBoolean((data[0] >> 1) & 0x01);
        }
    }
}
