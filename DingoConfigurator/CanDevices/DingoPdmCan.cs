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

    public enum MessagePrefix
    {
        Burn = 'B',
        Version = 'V',
        CAN = 'C',
        Input = 'I',
        Output = 'O',
        VirtualInput = 'U',
        Flasher = 'H',
        Wiper = 'W',
        WiperSpeed = 'P',
        WiperDelay = 'Y',
        StarterDisable = 'D',
        CANInput = 'N'
    }

    public class DingoPdmOutput : NotifyPropertyChangedBase
    {

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

        private int _resetCountLimit;
        public int ResetCountLimit
        {
            get => _resetCountLimit;
            set
            {
                if (_resetCountLimit != value)
                {
                    _resetCountLimit = value;
                    OnPropertyChanged(nameof(ResetCountLimit));
                }
            }
        }

        private ResetMode _resetMode;
        public ResetMode ResetMode
        {
            get => _resetMode;
            set
            {
                if(value != _resetMode)
                {
                    _resetMode = value;
                    OnPropertyChanged(nameof(ResetMode));
                }
            }
        }

        private int _resetTime;
        public int ResetTime
        {
            get => _resetTime;
            set
            {
                if (_resetTime != value)
                {
                    _resetTime = value;
                    OnPropertyChanged(nameof(ResetTime));
                }
            }
        }

        private double _inrushCurrentLimit;
        public double InrushCurrentLimit
        {
            get => _inrushCurrentLimit;
            set
            {
                if(value != _inrushCurrentLimit)
                {
                    _inrushCurrentLimit = value;
                    OnPropertyChanged(nameof(InrushCurrentLimit));
                }
            }
        }

        private int _inrushTime;
        public int InrushTime
        {
            get => _inrushTime;
            set
            {
                if(value != _inrushTime)
                {
                    _inrushTime = value;
                    OnPropertyChanged(nameof(InrushTime));
                }
            }
        }

        private VarMap _input;
        public VarMap Input
        {
            get => _input;
            set
            {
                if(value != _input)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        public DingoPdmOutput()
        {
            _enabled = false;
            _number = 0;
            _state = OutState.Off;
            _current = 0;
            _currentLimit = 0;
            _resetCount = 0;
        }
    }

    public class DingoPdmInput : NotifyPropertyChangedBase
    {
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

        private InputMode _mode;
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if ( _mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        private int _debounceTime;
        public int DebounceTime
        {
            get => _debounceTime;
            set
            {
                if ( _debounceTime != value)
                {
                    _debounceTime = value;
                    OnPropertyChanged(nameof(DebounceTime));
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

        public DingoPdmWiper()
        {
            SpeedMap = new int[8];
            IntermitTime = new int[6];
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

    public class DingoPdmFlasher : NotifyPropertyChangedBase
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

        private bool _single;
        public bool Single
        {
            get => _single;
            set
            {
                if (_single != value)
                {
                    _single = value;
                    OnPropertyChanged(nameof(Single));
                }
            }
        }

        private VarMap _input;
        public VarMap Input
        {
            get => _input;
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        private VarMap _output;
        public VarMap Output
        {
            get => _output;
            set
            {
                if (_output != value)
                {
                    _output = value;
                    OnPropertyChanged(nameof(Output));
                }
            }
        }

        private int _onTime;
        public int OnTime
        {
            get => _onTime;
            set
            {
                if (_onTime != value)
                {
                    _onTime = value;
                    OnPropertyChanged(nameof(OnTime));
                }
            }
        }

        private int _offTime;
        public int OffTime
        {
            get => _offTime;
            set
            {
                if (_offTime != value)
                {
                    _offTime = value;
                    OnPropertyChanged(nameof(OffTime));
                }
            }
        }
    }

    public class DingoPdmStarterDisable : NotifyPropertyChangedBase
    {
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
        private VarMap _input;
        public VarMap Input
        {
            get => _input;
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        private bool _output1;
        public bool Output1
        {
            get => _output1;
            set
            {
                if (_output1 != value)
                {
                    _output1 = value;
                    OnPropertyChanged(nameof(Output1));
                }
            }
        }

        private bool _output2;
        public bool Output2
        {
            get => _output2;
            set
            {
                if (_output2 != value)
                {
                    _output2 = value;
                    OnPropertyChanged(nameof(Output2));
                }
            }
        }

        private bool _output3;
        public bool Output3
        {
            get => _output3;
            set
            {
                if (_output3 != value)
                {
                    _output3 = value;
                    OnPropertyChanged(nameof(Output3));
                }
            }
        }

        private bool _output4;
        public bool Output4
        {
            get => _output4;
            set
            {
                if (_output4 != value)
                {
                    _output4 = value;
                    OnPropertyChanged(nameof(Output4));
                }
            }
        }

        private bool _output5;
        public bool Output5
        {
            get => _output5;
            set
            {
                if (_output5 != value)
                {
                    _output5 = value;
                    OnPropertyChanged(nameof(Output5));
                }
            }
        }

        private bool _output6;
        public bool Output6
        {
            get => _output6;
            set
            {
                if (_output6 != value)
                {
                    _output6 = value;
                    OnPropertyChanged(nameof(Output6));
                }
            }
        }

        private bool _output7;
        public bool Output7
        {
            get => _output7;
            set
            {
                if (_output7 != value)
                {
                    _output7 = value;
                    OnPropertyChanged(nameof(Output7));
                }
            }
        }

        private bool _output8;
        public bool Output8
        {
            get => _output8;
            set
            {
                if (_output8 != value)
                {
                    _output8 = value;
                    OnPropertyChanged(nameof(Output8));
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

        private string _version;
        public string Version
        {
            get => _version;
            private set
            {
                if (value != _version)
                {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
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

        private ObservableCollection<DingoPdmFlasher> _flashers;
        public ObservableCollection<DingoPdmFlasher> Flashers
        {
            get => _flashers;
            private set
            {
                if (_flashers != value)
                {
                    _flashers = value;
                    OnPropertyChanged(nameof(Flashers));
                }
            }
        }

        private ObservableCollection<DingoPdmStarterDisable> _starterDisable;
        public ObservableCollection<DingoPdmStarterDisable> StarterDisable
        {
            get => _starterDisable;
            set
            {
                if(value != _starterDisable)
                {
                    _starterDisable = value;
                    OnPropertyChanged(nameof(StarterDisable));
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

            Wipers = new ObservableCollection<DingoPdmWiper>
            {
                new DingoPdmWiper()
            };

            Flashers = new ObservableCollection<DingoPdmFlasher>();
            for(int i=0; i<4; i++)
            {
                Flashers.Add(new DingoPdmFlasher());
                Flashers[i].Number = i + 1;
            }

            StarterDisable = new ObservableCollection<DingoPdmStarterDisable>
            {
                new DingoPdmStarterDisable()
            };

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
            if ((id < BaseId) || (id > BaseId + 30)) 
                return false;

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

            if (id == BaseId + 30) ReadSettingsResponse(data);

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

        private void ReadSettingsResponse(byte[] data)
        {
            //Response is lowercase version of set/get prefix
            MessagePrefix prefix = (MessagePrefix)Char.ToUpper(Convert.ToChar(data[0]));

            int index = 0;

            switch (prefix)
            {
                case MessagePrefix.Version:
                    Version = $"V{data[1]}.{data[2]}.{(data[3] << 8) + (data[4])}";
                    break;

                case MessagePrefix.CAN:
                    BaseId = (data[2] << 8) + data[3];

                    break;

                case MessagePrefix.Input:
                    index = (data[1] & 0xF0) >> 4;
                    if (index >= 0 && index < 2)
                    {
                        DigitalInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        DigitalInputs[index].InvertInput = Convert.ToBoolean((data[1] & 0x08) >> 3);
                        DigitalInputs[index].Mode = (InputMode)((data[1] & 0x06) >> 1);
                        DigitalInputs[index].DebounceTime = data[2] * 10;
                    }
                    break;

                case MessagePrefix.Output:
                    index = (data[1] & 0xF0) >> 4;
                    if (index >= 0 && index < 8)
                    {
                        Outputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Outputs[index].Input = (VarMap)(data[2]);
                        Outputs[index].CurrentLimit = data[3] / 10;
                        Outputs[index].ResetCountLimit = (data[4] & 0xF0) >> 4;
                        Outputs[index].ResetMode = (ResetMode)(data[4] & 0x0F);
                        Outputs[index].ResetTime = data[5] * 10;
                        Outputs[index].InrushCurrentLimit = data[6] / 10;
                        Outputs[index].InrushTime = data[7] * 10;
                    }
                    break;

                case MessagePrefix.VirtualInput:
                    index = data[2];
                    if (index >= 0 && index < 16)
                    {
                        VirtualInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        VirtualInputs[index].Not0 = Convert.ToBoolean((data[1] & 0x02) >> 1);
                        VirtualInputs[index].Not1 = Convert.ToBoolean((data[1] & 0x04) >> 2);
                        VirtualInputs[index].Not2 = Convert.ToBoolean((data[1] & 0x08) >> 3);
                        VirtualInputs[index].Var0 = (VarMap)data[3];
                        VirtualInputs[index].Var1 = (VarMap)data[4];
                        VirtualInputs[index].Var2 = (VarMap)data[5];
                        VirtualInputs[index].Mode = (InputMode)((data[6] & 0xC0) >> 6);
                        VirtualInputs[index].Cond0 = (Conditional)(data[6] & 0x03);
                        VirtualInputs[index].Cond1 = (Conditional)((data[6] & 0x0C) >> 2);
                    }
                    break;

                case MessagePrefix.Flasher:
                    index = (data[1] & 0xF0) >> 4;
                    if (index >= 0 && index < 16)
                    {
                        Flashers[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Flashers[index].Single = Convert.ToBoolean((data[1] & 0x02) >> 1);
                        Flashers[index].Input = (VarMap)(data[2]);
                        Flashers[index].Output = (VarMap)(data[3] + 50);
                        Flashers[index].OnTime = data[4] * 10;
                        Flashers[index].OffTime = data[5] * 10;
                    }
                    break;

                case MessagePrefix.Wiper:
                    //Wipers[0].Enabled = (data[1] & 0x01);
                    Wipers[0].Mode = (WiperMode)((data[1] & 0x03) >> 1);
                    Wipers[0].ParkStopLevel = Convert.ToBoolean((data[1] & 0x04) >> 3);
                    Wipers[0].WashWipeCycles = (data[1] & 0xF0) >> 4;
                    Wipers[0].SlowInput = (VarMap)data[2];
                    Wipers[0].FastInput = (VarMap)data[3];
                    Wipers[0].InterInput = (VarMap)data[4];
                    Wipers[0].OnInput = (VarMap)data[5];
                    Wipers[0].ParkInput = (VarMap)data[6];
                    Wipers[0].WashInput = (VarMap)data[7];
                    break;

                case MessagePrefix.WiperSpeed:
                    Wipers[0].SwipeInput = (VarMap)data[1];
                    Wipers[0].SpeedInput = (VarMap)data[2];
                    Wipers[0].SpeedMap[0] = (data[3] & 0x0F);
                    Wipers[0].SpeedMap[1] = (data[3] & 0xF0) >> 4;
                    Wipers[0].SpeedMap[2] = (data[4] & 0x0F);
                    Wipers[0].SpeedMap[3] = (data[4] & 0xF0) >> 4;
                    Wipers[0].SpeedMap[4] = (data[5] & 0x0F);
                    Wipers[0].SpeedMap[5] = (data[5] & 0xF0) >> 4;
                    Wipers[0].SpeedMap[6] = (data[6] & 0x0F);
                    Wipers[0].SpeedMap[7] = (data[6] & 0xF0) >> 4;
                    break;

                case MessagePrefix.WiperDelay:
                    Wipers[0].IntermitTime[0] = data[1] * 10;
                    Wipers[0].IntermitTime[1] = data[2] * 10;
                    Wipers[0].IntermitTime[2] = data[3] * 10;
                    Wipers[0].IntermitTime[3] = data[4] * 10;
                    Wipers[0].IntermitTime[4] = data[5] * 10;
                    Wipers[0].IntermitTime[5] = data[6] * 10;
                    break;

                case MessagePrefix.StarterDisable:
                    StarterDisable[0].Enabled = Convert.ToBoolean(data[1] & 0x01);
                    StarterDisable[0].Input = (VarMap)(data[2]);
                    StarterDisable[0].Output1 = Convert.ToBoolean(data[3] & 0x01);
                    StarterDisable[0].Output2 = Convert.ToBoolean((data[3] & 0x02) >> 1);
                    StarterDisable[0].Output3 = Convert.ToBoolean((data[3] & 0x04) >> 2);
                    StarterDisable[0].Output4 = Convert.ToBoolean((data[3] & 0x08) >> 3);
                    StarterDisable[0].Output5 = Convert.ToBoolean((data[3] & 0x10) >> 4);
                    StarterDisable[0].Output6 = Convert.ToBoolean((data[3] & 0x20) >> 5);
                    StarterDisable[0].Output7 = Convert.ToBoolean((data[3] & 0x40) >> 6);
                    StarterDisable[0].Output8 = Convert.ToBoolean((data[3] & 0x80) >> 7);
                    break;

                case MessagePrefix.CANInput:
                    index = data[2];
                    if (index >= 0 && index < 32)
                    {
                        CanInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        CanInputs[index].Mode = (InputMode)((data[1] & 0x06) >> 1);
                        CanInputs[index].Operator = (Operator)((data[1] & 0xF0) >> 4);
                        CanInputs[index].Id = (data[3] << 8) + data[4];
                        CanInputs[index].HighByte = (data[5] & 0xF0) >> 4;
                        CanInputs[index].LowByte = (data[5] & 0x0F);
                        CanInputs[index].OnVal = data[6];
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetConfigFile(PdmConfig config)
        {
            for (int i = 0; i < DigitalInputs.Count; i++)
            {
                DigitalInputs[i].Name = config.input[i].label;
                DigitalInputs[i].Enabled = config.input[i].enabled;
                DigitalInputs[i].Mode = config.input[i].mode;
                DigitalInputs[i].InvertInput = config.input[i].invertInput;
                DigitalInputs[i].DebounceTime = config.input[i].debounceTime;
            }

            for (int i = 0; i < Outputs.Count; i++)
            {
                Outputs[i].Name = config.output[i].label;
                Outputs[i].Enabled = config.output[i].enabled;
                Outputs[i].Input = config.output[i].input;
                Outputs[i].CurrentLimit = config.output[i].currentLimit;
                Outputs[i].InrushCurrentLimit = config.output[i].inrushLimit;
                Outputs[i].InrushTime = config.output[i].inrushTime;
                Outputs[i].ResetMode = config.output[i].resetMode;
                Outputs[i].ResetCountLimit = config.output[i].resetLimit;
            }

            for (int i = 0; i < VirtualInputs.Count; i++)
            {
                VirtualInputs[i].Name = config.virtualInput[i].label;
                VirtualInputs[i].Enabled = config.virtualInput[i].enabled;
                VirtualInputs[i].Not0 = config.virtualInput[i].not0;
                VirtualInputs[i].Var0 = config.virtualInput[i].var0;
                VirtualInputs[i].Cond0 = config.virtualInput[i].cond0;
                VirtualInputs[i].Not1 = config.virtualInput[i].not1;
                VirtualInputs[i].Var1 = config.virtualInput[i].var1;
                VirtualInputs[i].Cond1 = config.virtualInput[i].cond1;
                VirtualInputs[i].Not2 = config.virtualInput[i].not2;
                VirtualInputs[i].Var2 = config.virtualInput[i].var2;
                VirtualInputs[i].Mode = config.virtualInput[i].mode;
            }

            for (int i = 0; i < CanInputs.Count; i++)
            {
                CanInputs[i].Name = config.canInput[i].label;
                CanInputs[i].Enabled = config.canInput[i].enabled;
                CanInputs[i].Id = config.canInput[i].id;
                CanInputs[i].HighByte = config.canInput[i].highByte;
                CanInputs[i].LowByte = config.canInput[i].lowByte;
                CanInputs[i].Operator = config.canInput[i].oper;
                CanInputs[i].OnVal = config.canInput[i].onValue;
                CanInputs[i].Mode = config.canInput[i].mode;
            }

            for (int i = 0; i < Flashers.Count; i++)
            {
                Flashers[i].Name = config.flasher[i].label;
                Flashers[i].Enabled = config.flasher[i].enabled;
                Flashers[i].Input = config.flasher[i].input;
                Flashers[i].OnTime = config.flasher[i].flashOnTime;
                Flashers[i].OffTime = config.flasher[i].flashOffTime;
                Flashers[i].Single = Convert.ToBoolean(config.flasher[i].singleCycle);
                Flashers[i].Output = config.flasher[i].output;
            }

            StarterDisable[0].Enabled = config.starter.enabled;
            StarterDisable[0].Input = config.starter.input;
            StarterDisable[0].Output1 = config.starter.disableOut[0];
            StarterDisable[0].Output2 = config.starter.disableOut[1];
            StarterDisable[0].Output3 = config.starter.disableOut[2];
            StarterDisable[0].Output4 = config.starter.disableOut[3];
            StarterDisable[0].Output5 = config.starter.disableOut[4];
            StarterDisable[0].Output6 = config.starter.disableOut[5];
            StarterDisable[0].Output7 = config.starter.disableOut[6];
            StarterDisable[0].Output8 = config.starter.disableOut[7];

            Wipers[0].Enabled = config.wiper.enabled;
            Wipers[0].SlowInput = config.wiper.lowSpeedInput;
            Wipers[0].FastInput = config.wiper.highSpeedInput;
            Wipers[0].ParkInput = config.wiper.parkInput;
            Wipers[0].ParkStopLevel = config.wiper.parkStopLevel;
            Wipers[0].WashInput = config.wiper.washInput;
            Wipers[0].WashWipeCycles = config.wiper.washCycles;
            Wipers[0].InterInput = config.wiper.intermitInput;
            Wipers[0].SpeedInput = config.wiper.speedInput;
            Wipers[0].IntermitTime = config.wiper.intermitTime;
            Wipers[0].SpeedMap = config.wiper.speedMap;
        }
    }
}
