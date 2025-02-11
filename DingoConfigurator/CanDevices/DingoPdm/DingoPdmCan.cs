using CanInterfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CanDevices.DingoPdm
{
    public class DingoPdmCan : NotifyPropertyChangedBase, ICanDevice
    {
        protected virtual int _minMajorVersion { get; } = 0;
        protected virtual int _minMinorVersion { get; } = 4;
        protected virtual int _minBuildVersion { get; } = 0;

        protected virtual int _numDigitalInputs { get; } = 2;
        protected virtual int _numOutputs { get; } = 8;
        protected virtual int _numCanInputs { get; } = 32;
        protected virtual int _numVirtualInputs { get; } = 16;
        protected virtual int _numFlashers { get; } = 4;

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected int _pdmType;

        protected string _name;
        [JsonPropertyName("name")]
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

        protected int _baseId;
        [JsonPropertyName("baseId")]
        public int BaseId
        {
            get => _baseId;
            set
            {
                if (_baseId != value)
                {
                    _baseId = value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
        }

        protected List<CanDeviceSub> _subPages = new List<CanDeviceSub>();
        [JsonIgnore]
        public List<CanDeviceSub> SubPages
        {
            get => _subPages;
            protected set
            {
                if (_subPages != value)
                {
                    _subPages = value;
                    OnPropertyChanged(nameof(SubPages));
                }
            }
        }

        protected DateTime _lastRxTime { get; set; }
        [JsonIgnore]
        public DateTime LastRxTime { get => _lastRxTime; }

        protected bool _isConnected;
        [JsonIgnore]
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    Clear();

                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    foreach (var subPage in _subPages)
                    {
                        subPage.UpdateProperty(nameof(IsConnected));
                    }
                }
            }
        }

        protected ObservableCollection<Input> _digitalInputs { get; set; }
        [JsonPropertyName("digitalInputs")]
        public ObservableCollection<Input> DigitalInputs
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

        protected DeviceState _deviceState;
        [JsonIgnore]
        public DeviceState DeviceState
        {
            get => _deviceState;
            set
            {
                if (_deviceState != value)
                {
                    _deviceState = value;
                    OnPropertyChanged(nameof(DeviceState));
                }
            }
        }

        protected double _totalCurrent;
        [JsonIgnore]
        public double TotalCurrent
        {
            get => _totalCurrent;
            protected set
            {
                if (_totalCurrent != value)
                {
                    _totalCurrent = value;
                    OnPropertyChanged(nameof(TotalCurrent));
                }
            }
        }

        protected double _batteryVoltage;
        [JsonIgnore]
        public double BatteryVoltage
        {
            get => _batteryVoltage;
            protected set
            {
                if (_batteryVoltage != value)
                {
                    _batteryVoltage = value;
                    OnPropertyChanged(nameof(BatteryVoltage));
                }
            }
        }

        protected double _boardTempC;
        [JsonIgnore]
        public double BoardTempC
        {
            get => _boardTempC;
            protected set
            {
                if (_boardTempC != value)
                {
                    _boardTempC = value;
                    OnPropertyChanged(nameof(BoardTempC));
                }
            }
        }

        protected double _boardTempF;
        [JsonIgnore]
        public double BoardTempF
        {
            get => _boardTempF;
            protected set
            {
                if (_boardTempF != value)
                {
                    _boardTempF = value;
                    OnPropertyChanged(nameof(BoardTempF));
                }
            }
        }

        protected string _version;
        [JsonIgnore]
        public string Version
        {
            get => _version;
            protected set
            {
                if (value != _version)
                {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }

        protected CanSpeed _baudRate;
        public CanSpeed BaudRate
        {
            get { return _baudRate; }
            set
            {
                _baudRate = value;
                OnPropertyChanged(nameof(BaudRate));
            }
        }

        [JsonIgnore]
        public IEnumerable<CanSpeed> BaudRates
        {
            get
            {
                return (IEnumerable<CanSpeed>)System.Enum.GetValues(typeof(CanSpeed));
            }
        }

        protected ObservableCollection<Output> _outputs;
        [JsonPropertyName("outputs")]
        public ObservableCollection<Output> Outputs
        {
            get => _outputs;
            set
            {
                if (_outputs != value)
                {
                    _outputs = value;
                    OnPropertyChanged(nameof(Outputs));
                }
            }
        }

        protected ObservableCollection<CanInput> _canInputs;
        [JsonPropertyName("canInputs")]
        public ObservableCollection<CanInput> CanInputs
        {
            get => _canInputs;
            set
            {
                if (_canInputs != value)
                {
                    _canInputs = value;
                    OnPropertyChanged(nameof(CanInputs));
                }
            }
        }

        protected ObservableCollection<VirtualInput> _virtualInputs;
        [JsonPropertyName("virtualInputs")]
        public ObservableCollection<VirtualInput> VirtualInputs
        {
            get => _virtualInputs;
            set
            {
                if (_virtualInputs != value)
                {
                    _virtualInputs = value;
                    OnPropertyChanged(nameof(VirtualInputs));
                }
            }
        }

        protected ObservableCollection<Wiper> _wipers;
        [JsonPropertyName("wipers")]
        public ObservableCollection<Wiper> Wipers
        {
            get => _wipers;
            set
            {
                if (_wipers != value)
                {
                    _wipers = value;
                    OnPropertyChanged(nameof(Wipers));
                }
            }
        }

        protected ObservableCollection<Flasher> _flashers;
        [JsonPropertyName("flashers")]
        public ObservableCollection<Flasher> Flashers
        {
            get => _flashers;
            set
            {
                if (_flashers != value)
                {
                    _flashers = value;
                    OnPropertyChanged(nameof(Flashers));
                }
            }
        }

        protected ObservableCollection<StarterDisable> _starterDisable;
        [JsonPropertyName("starterDisable")]
        public ObservableCollection<StarterDisable> StarterDisable
        {
            get => _starterDisable;
            set
            {
                if (value != _starterDisable)
                {
                    _starterDisable = value;
                    OnPropertyChanged(nameof(StarterDisable));
                }
            }
        }

        public DingoPdmCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;
            DigitalInputs = new ObservableCollection<Input>();
            for (int i = 0; i < _numDigitalInputs; i++)
            {
                DigitalInputs.Add(new Input());
                DigitalInputs[i].Number = i + 1;
            }

            TotalCurrent = 0;
            BatteryVoltage = 0;
            BoardTempC = 0;

            Outputs = new ObservableCollection<Output>();
            for (int i = 0; i < _numOutputs; i++)
            {
                Outputs.Add(new Output());
                Outputs[i].Number = i + 1;
            }

            CanInputs = new ObservableCollection<CanInput>();
            for (int i = 0; i < _numCanInputs; i++)
            {
                CanInputs.Add(new CanInput());
                CanInputs[i].Number = i + 1;
            }

            VirtualInputs = new ObservableCollection<VirtualInput>();
            for (int i = 0; i < _numVirtualInputs; i++)
            {
                VirtualInputs.Add(new VirtualInput());
                VirtualInputs[i].Number = i + 1;
            }

            Wipers = new ObservableCollection<Wiper>
            {
                new Wiper()
            };

            Flashers = new ObservableCollection<Flasher>();
            for (int i = 0; i < _numFlashers; i++)
            {
                Flashers.Add(new Flasher());
                Flashers[i].Number = i + 1;
            }

            StarterDisable = new ObservableCollection<StarterDisable>
            {
                new StarterDisable()
            };

            SubPages.Add(new CanDeviceSub("Settings", this));
            SubPages.Add(new DingoPdmPlot("Plots", this));
        }

        public void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - LastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public bool IsPriorityMsg(int id)
        {
            return ((id == BaseId + 30) || (id == BaseId + 31));
        }

        public bool InIdRange(int id)
        {
            return (id >= BaseId) && (id <= BaseId + 31) ;
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if ((id < BaseId) || (id > BaseId + 31)) 
                return false;

            if (id == BaseId + 0) ReadMessage0(data);
            if (id == BaseId + 1) ReadMessage1(data);
            if (id == BaseId + 2) ReadMessage2(data);
            if (id == BaseId + 3) ReadMessage3(data);
            if (id == BaseId + 4) ReadMessage4(data);
            if (id == BaseId + 5) ReadMessage5(data);

            if (id == BaseId + 30)
            {
                ReadSettingsResponse(data, queue);
            }

            if (id == BaseId + 31)
            {
                ReadInfoWarnErrorMessage(data);
            }

            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }

        protected void ReadMessage0(byte[] data)
        {
            DigitalInputs[0].State = Convert.ToBoolean(data[0] & 0x01);
            DigitalInputs[1].State = Convert.ToBoolean((data[0] >> 1) & 0x01);

            DeviceState = (DeviceState)(data[1] & 0x0F);
            _pdmType = (data[1] >> 4) & 0x0F;

            TotalCurrent = Convert.ToDouble(((data[3] << 8) + data[2]) / 10.0);
            BatteryVoltage = Convert.ToDouble(((data[5] << 8) + data[4]) / 10.0);
            BoardTempC = Math.Round(Convert.ToDouble((data[7] << 8) + data[6]));
            BoardTempF = Math.Round(BoardTempC * 1.8 + 32);

        }

        protected void ReadMessage1(byte[] data)
        {
            Outputs[0].Current = Convert.ToDouble(((data[1] << 8) + data[0]) / 10.0);
            Outputs[1].Current = Convert.ToDouble(((data[3] << 8) + data[2]) / 10.0);
            Outputs[2].Current = Convert.ToDouble(((data[5] << 8) + data[4]) / 10.0);
            Outputs[3].Current = Convert.ToDouble(((data[7] << 8) + data[6]) / 10.0);
        }

        protected virtual void ReadMessage2(byte[] data)
        {
            Outputs[4].Current = Convert.ToDouble(((data[1] << 8) + data[0]) / 10.0);
            Outputs[5].Current = Convert.ToDouble(((data[3] << 8) + data[2]) / 10.0);
            Outputs[6].Current = Convert.ToDouble(((data[5] << 8) + data[4]) / 10.0);
            Outputs[7].Current = Convert.ToDouble(((data[7] << 8) + data[6]) / 10.0);
        }

        protected virtual void ReadMessage3(byte[] data)
        {
            Outputs[0].State = (OutState)((data[0] & 0x0F));
            Outputs[1].State = (OutState)(data[0] >> 4);
            Outputs[2].State = (OutState)((data[1] & 0x0F));
            Outputs[3].State = (OutState)(data[1] >> 4);
            Outputs[4].State = (OutState)((data[2] & 0x0F));
            Outputs[5].State = (OutState)(data[2] >> 4);
            Outputs[6].State = (OutState)((data[3] & 0x0F));
            Outputs[7].State = (OutState)(data[3] >> 4);

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

        protected virtual void ReadMessage4(byte[] data)
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

        protected void ReadMessage5(byte[] data)
        {
            CanInputs[0].Value = data[0] & 0x01;
            CanInputs[1].Value = (data[0] >> 1) & 0x01;
            CanInputs[2].Value = (data[0] >> 2) & 0x01;
            CanInputs[3].Value = (data[0] >> 3) & 0x01;
            CanInputs[4].Value = (data[0] >> 4) & 0x01;
            CanInputs[5].Value = (data[0] >> 5) & 0x01;
            CanInputs[6].Value = (data[0] >> 6) & 0x01;
            CanInputs[7].Value = (data[0] >> 7) & 0x01;

            CanInputs[8].Value = data[1] & 0x01;
            CanInputs[9].Value = (data[1] >> 1) & 0x01;
            CanInputs[10].Value = (data[1] >> 2) & 0x01;
            CanInputs[11].Value = (data[1] >> 3) & 0x01;
            CanInputs[12].Value = (data[1] >> 4) & 0x01;
            CanInputs[13].Value = (data[1] >> 5) & 0x01;
            CanInputs[14].Value = (data[1] >> 6) & 0x01;
            CanInputs[15].Value = (data[1] >> 7) & 0x01;

            CanInputs[16].Value = data[2] & 0x01;
            CanInputs[17].Value = (data[2] >> 1) & 0x01;
            CanInputs[18].Value = (data[2] >> 2) & 0x01;
            CanInputs[19].Value = (data[2] >> 3) & 0x01;
            CanInputs[20].Value = (data[2] >> 4) & 0x01;
            CanInputs[21].Value = (data[2] >> 5) & 0x01;
            CanInputs[22].Value = (data[2] >> 6) & 0x01;
            CanInputs[23].Value = (data[2] >> 7) & 0x01;

            CanInputs[24].Value = data[3] & 0x01;
            CanInputs[25].Value = (data[3] >> 1) & 0x01;
            CanInputs[26].Value = (data[3] >> 2) & 0x01;
            CanInputs[27].Value = (data[3] >> 3) & 0x01;
            CanInputs[28].Value = (data[3] >> 4) & 0x01;
            CanInputs[29].Value = (data[3] >> 5) & 0x01;
            CanInputs[30].Value = (data[3] >> 6) & 0x01;
            CanInputs[31].Value = (data[3] >> 7) & 0x01;

            VirtualInputs[0].Value = data[4] & 0x01;
            VirtualInputs[1].Value = (data[4] >> 1) & 0x01;
            VirtualInputs[2].Value = (data[4] >> 2) & 0x01;
            VirtualInputs[3].Value = (data[4] >> 3) & 0x01;
            VirtualInputs[4].Value = (data[4] >> 4) & 0x01;
            VirtualInputs[5].Value = (data[4] >> 5) & 0x01;
            VirtualInputs[6].Value = (data[4] >> 6) & 0x01;
            VirtualInputs[7].Value = (data[4] >> 7) & 0x01;

            VirtualInputs[8].Value = data[5] & 0x01;
            VirtualInputs[9].Value = (data[5] >> 1) & 0x01;
            VirtualInputs[10].Value = (data[5] >> 2) & 0x01;
            VirtualInputs[11].Value = (data[5] >> 3) & 0x01;
            VirtualInputs[12].Value = (data[5] >> 4) & 0x01;
            VirtualInputs[13].Value = (data[5] >> 5) & 0x01;
            VirtualInputs[14].Value = (data[5] >> 6) & 0x01;
            VirtualInputs[15].Value = (data[5] >> 7) & 0x01;
        }


        protected void ReadSettingsResponse(byte[] data, ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            //Response is lowercase version of set/get prefix
            MessagePrefix prefix = (MessagePrefix)Char.ToUpper(Convert.ToChar(data[0]));

            int index = 0;

            var key = (BaseId, (int)prefix, 0);
            CanDeviceResponse response;

            switch (prefix)
            {
                case MessagePrefix.Version:
                    Version = $"V{data[1]}.{data[2]}.{(data[3] << 8) + (data[4])}";

                    if (!CheckVersion(data[1], data[2], (data[3] << 8) + (data[4])))
                    {
                        Logger.Error($"{Name} ID: {BaseId}, Firmware needs to be updated. V{_minMajorVersion}.{_minMinorVersion}.{_minBuildVersion} or greater");
                    }

                    key = (BaseId, (int)MessagePrefix.Version, 0);
                    if(queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

                    break;

                case MessagePrefix.CAN:
                    BaseId = (data[2] << 8) + data[3];
                    BaudRate = (CanSpeed)((data[1] & 0xF0)>>4);

                    key = (BaseId, (int)MessagePrefix.CAN, 0);
                    if (queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

                    break;

                case MessagePrefix.Input:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numDigitalInputs)
                    {
                        DigitalInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        DigitalInputs[index].InvertInput = Convert.ToBoolean((data[1] & 0x08) >> 3);
                        DigitalInputs[index].Mode = (InputMode)((data[1] & 0x06) >> 1);
                        DigitalInputs[index].DebounceTime = data[2] * 10;
                        DigitalInputs[index].Pull = (InputPull)(data[3] & 0x03);

                        key = (BaseId, (int)MessagePrefix.Input, index);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    break;

                case MessagePrefix.Output:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numOutputs)
                    {
                        Outputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Outputs[index].Input = (VarMap)(data[2]);
                        Outputs[index].CurrentLimit = data[3];
                        Outputs[index].ResetCountLimit = (data[4] & 0xF0) >> 4;
                        Outputs[index].ResetMode = (ResetMode)(data[4] & 0x0F);
                        Outputs[index].ResetTime = data[5] / 10.0;
                        Outputs[index].InrushCurrentLimit = data[6];
                        Outputs[index].InrushTime = data[7] / 10.0;

                        key = (BaseId, (int)MessagePrefix.Output, index);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    break;

                case MessagePrefix.VirtualInput:
                    index = data[2];
                    
                    if (index >= 0 && index < _numVirtualInputs)
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

                        key = (BaseId, (int)MessagePrefix.VirtualInput, index);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    break;

                case MessagePrefix.Flasher:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numFlashers)
                    {
                        Flashers[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Flashers[index].Single = Convert.ToBoolean((data[1] & 0x02) >> 1);
                        Flashers[index].Input = (VarMap)(data[2]);
                        Flashers[index].OnTime = data[4] / 10.0;
                        Flashers[index].OffTime = data[5] / 10.0;

                        key = (BaseId, (int)MessagePrefix.Flasher, index);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    break;

                case MessagePrefix.Wiper:
                    Wipers[0].Enabled = Convert.ToBoolean(data[1] & 0x01);
                    Wipers[0].Mode = (WiperMode)((data[1] & 0x03) >> 1);
                    Wipers[0].ParkStopLevel = Convert.ToBoolean((data[1] & 0x08) >> 3);
                    Wipers[0].WashWipeCycles = (data[1] & 0xF0) >> 4;
                    Wipers[0].SlowInput = (VarMap)data[2];
                    Wipers[0].FastInput = (VarMap)data[3];
                    Wipers[0].InterInput = (VarMap)data[4];
                    Wipers[0].OnInput = (VarMap)data[5];
                    Wipers[0].ParkInput = (VarMap)data[6];
                    Wipers[0].WashInput = (VarMap)data[7];

                    key = (BaseId, (int)MessagePrefix.Wiper, 0);
                    if (queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

                    break;

                case MessagePrefix.WiperSpeed:
                    Wipers[0].SwipeInput = (VarMap)data[1];
                    Wipers[0].SpeedInput = (VarMap)data[2];
                    Wipers[0].SpeedMap[0] = (WiperSpeed)(data[3] & 0x0F);
                    Wipers[0].SpeedMap[1] = (WiperSpeed)((data[3] & 0xF0) >> 4);
                    Wipers[0].SpeedMap[2] = (WiperSpeed)(data[4] & 0x0F);
                    Wipers[0].SpeedMap[3] = (WiperSpeed)((data[4] & 0xF0) >> 4);
                    Wipers[0].SpeedMap[4] = (WiperSpeed)(data[5] & 0x0F);
                    Wipers[0].SpeedMap[5] = (WiperSpeed)((data[5] & 0xF0) >> 4);
                    Wipers[0].SpeedMap[6] = (WiperSpeed)(data[6] & 0x0F);
                    Wipers[0].SpeedMap[7] = (WiperSpeed)((data[6] & 0xF0) >> 4);

                    key = (BaseId, (int)MessagePrefix.WiperSpeed, 0);
                    if (queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

                    break;

                case MessagePrefix.WiperDelay:
                    Wipers[0].IntermitTime[0] = data[1] / 10.0;
                    Wipers[0].IntermitTime[1] = data[2] / 10.0;
                    Wipers[0].IntermitTime[2] = data[3] / 10.0;
                    Wipers[0].IntermitTime[3] = data[4] / 10.0;
                    Wipers[0].IntermitTime[4] = data[5] / 10.0;
                    Wipers[0].IntermitTime[5] = data[6] / 10.0;

                    key = (BaseId, (int)MessagePrefix.WiperDelay, 0);
                    if (queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

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

                    key = (BaseId, (int)MessagePrefix.StarterDisable, 0);
                    if (queue.TryRemove(key, out response))
                    {
                        response.TimeSentTimer.Dispose();
                    }

                    break;

                case MessagePrefix.CANInput:
                    index = data[2];
                    if (index >= 0 && index < _numCanInputs)
                    {
                        CanInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        CanInputs[index].Mode = (InputMode)((data[1] & 0x06) >> 1);
                        CanInputs[index].Operator = (Operator)((data[1] & 0xF0) >> 4);
                        CanInputs[index].Id = (data[3] << 8) + data[4];
                        CanInputs[index].DLC = (data[5] & 0xF0) >> 4;
                        CanInputs[index].StartingByte = (data[5] & 0x0F);
                        CanInputs[index].OnVal = (data[6] << 8) + data[7];

                        key = (BaseId, (int)MessagePrefix.CANInput, index);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    break;

                case MessagePrefix.Burn:
                    if (data[1] == 1) //Successful burn
                    {
                        Logger.Info($"{Name} ID: {BaseId}, Burn Successful");

                        key = (BaseId, (int)MessagePrefix.Burn, 0);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    if (data[1] == 0) //Unsuccessful burn
                    {
                        Logger.Error($"{Name} ID: {BaseId}, Burn Failed");
                    }
                    break;

                case MessagePrefix.Sleep:
                    if (data[1] == 1) //Successful sleep
                    {
                        Logger.Info($"{Name} ID: {BaseId}, Sleep Successful");

                        key = (BaseId, (int)MessagePrefix.Sleep, 0);
                        if (queue.TryRemove(key, out response))
                        {
                            response.TimeSentTimer.Dispose();
                        }
                    }

                    if (data[1] == 0) //Unsuccessful sleep
                    {
                        Logger.Error($"{Name} ID: {BaseId}, Sleep Failed");
                    }
                    break;

                default:
                    break;
            }
        }

        protected void ReadInfoWarnErrorMessage(byte[] data)
        {
            //Response is lowercase version of set/get prefix
            MessagePrefix prefix = (MessagePrefix)Char.ToUpper(Convert.ToChar(data[0]));
            MessageSrc src = (MessageSrc)data[1];

            switch (prefix)
            {
                case MessagePrefix.Info:
                    Logger.Info($"{Name} ID: {BaseId}, Src: {src} {((data[3] << 8) + data[2])} {((data[5] << 8) + data[4])} {((data[7] << 8) + data[6])}");
                    break;
                case MessagePrefix.Warning:
                    Logger.Warn($"{Name} ID: {BaseId}, Src: {src} {((data[3] << 8) + data[2])} {((data[5] << 8) + data[4])} {((data[7] << 8) + data[6])}");
                    break;
                case MessagePrefix.Error:
                    Logger.Error($"{Name} ID: {BaseId}, Src: {src} {((data[3] << 8) + data[2])} {((data[5] << 8) + data[4])} {((data[7] << 8) + data[6])}");
                    break;
            }
        }

        public List<CanDeviceResponse> GetUploadMessages()
        {
            int id = BaseId - 1;

            List<CanDeviceResponse> msgs = new List<CanDeviceResponse>();

            //Request settings messages

            //Version
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.Version,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Version), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription="Version"
            });

            //CAN settings
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.CAN,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.CAN), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription="CANSettings"
            });

            //Inputs
            for (int i = 0; i < _numDigitalInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Input,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte(MessagePrefix.Input),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription=$"Input{i + 1}"
                });
            }

            //Outputs
            for (int i = 0; i < _numOutputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Output,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte(MessagePrefix.Output),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"Output{i + 1}"
                });
            }

            //Virtual inputs
            for (int i = 0; i < _numVirtualInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.VirtualInput,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte(MessagePrefix.VirtualInput),
                        Convert.ToByte(i),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"VirtualInput{i + 1}"
                });
            }

            //Flashers
            for (int i = 0; i < _numFlashers; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Flasher,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte(MessagePrefix.Flasher),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"Flasher{i + 1}"
                });
            }

            //CAN inputs
            for (int i = 0; i < _numCanInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.CANInput,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte(MessagePrefix.CANInput),
                        Convert.ToByte(i),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"CANInput{i + 1}"
                });
            }

            //Wiper
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.Wiper,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Wiper), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "Wiper"
            });

            //Wiper speeds
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.WiperSpeed,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.WiperSpeed), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "WiperSpeed"
            });

            //Wiper delays
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.WiperDelay,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.WiperDelay), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "WiperDelay"
            });

            //Starter disable
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.StarterDisable,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.StarterDisable), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "StarterDisable"
            });

            return msgs;
        }

        public List<CanDeviceResponse> GetDownloadMessages()
        {
            int id = BaseId - 1;

            List<CanDeviceResponse> msgs = new List<CanDeviceResponse>();

            //Send settings messages

            //CAN settings
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.CAN,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 5,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.CAN), //Byte 0
                    Convert.ToByte((Convert.ToByte(BaudRate) << 4) +
                    (0x03)), //Byte 1
                    Convert.ToByte((BaseId & 0xFF00) >> 8), //Byte 2
                    Convert.ToByte(BaseId & 0x00FF), //Byte 3
                    Convert.ToByte(5), //Byte 4
                    0, 0, 0 }
                },
                MsgDescription = "CANSettings"
            });

            //Inputs
            foreach(var input in DigitalInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Input,
                    Index = input.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 4,
                        Payload = new byte[] {
                        Convert.ToByte(MessagePrefix.Input), //Byte 0
                        Convert.ToByte((((input.Number - 1) & 0x0F) << 4) +
                        ((Convert.ToByte(input.InvertInput) & 0x01) << 3) +
                        ((Convert.ToByte(input.Mode) & 0x03) << 1) +
                        (Convert.ToByte(input.Enabled) & 0x01)), //Byte 1
                        (Convert.ToByte(input.DebounceTime / 10)), //Byte 2
                        Convert.ToByte(input.Pull), //Byte 3
                        0, 0, 0, 0 }
                    },
                    MsgDescription = $"Input{input.Number}"
                });
            }

            //Outputs
            foreach(var output in Outputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Output,
                    Index = output.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = new byte[] {
                        Convert.ToByte(MessagePrefix.Output), //Byte 0
                        Convert.ToByte((((output.Number - 1) & 0x0F) << 4) +
                        (Convert.ToByte(output.Enabled) & 0x01)), //Byte 1
                        Convert.ToByte(output.Input), //Byte 2
                        Convert.ToByte(output.CurrentLimit), //Byte 3 
                        Convert.ToByte((Convert.ToByte(output.ResetCountLimit) << 4) +
                        (Convert.ToByte(output.ResetMode) & 0x0F)), //Byte 4
                        Convert.ToByte(output.ResetTime * 10), //Byte 5
                        Convert.ToByte(output.InrushCurrentLimit), //Byte 6 
                        Convert.ToByte(output.InrushTime * 10) } //Byte 7
                    },
                    MsgDescription = $"Output{output.Number}"
                });
            }

            //Virtual inputs
            foreach(var virtInput in VirtualInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.VirtualInput,
                    Index = virtInput.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 7,
                        Payload = new byte[] {
                        Convert.ToByte(MessagePrefix.VirtualInput), //Byte 0
                        Convert.ToByte((Convert.ToByte(virtInput.Not2) << 3) +
                        (Convert.ToByte(virtInput.Not1) << 2) +
                        (Convert.ToByte(virtInput.Not0) << 1) +
                        Convert.ToByte(virtInput.Enabled)), //Byte 1
                        Convert.ToByte(virtInput.Number - 1), //Byte 2 
                        Convert.ToByte(virtInput.Var0), //Byte 3
                        Convert.ToByte(virtInput.Var1), //Byte 4
                        Convert.ToByte(virtInput.Var2), //Byte 5
                        Convert.ToByte(((Convert.ToByte(virtInput.Mode) & 0x03) << 0x06) +
                        ((Convert.ToByte(virtInput.Cond1) & 0x03) << 2) +
                        (Convert.ToByte(virtInput.Cond0) & 0x03)), //Byte 6
                        0 }
                    },
                    MsgDescription = $"VirtualInput{virtInput.Number}"
                });
            }

            //Flashers
            foreach(var flash in Flashers)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.Flasher,
                    Index = flash.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 6,
                        Payload = new byte[] {
                        Convert.ToByte(MessagePrefix.Flasher), //Byte 0
                        Convert.ToByte((((flash.Number - 1) & 0x0F) << 4) +
                        (Convert.ToByte(flash.Single) << 1) +
                        (Convert.ToByte(flash.Enabled))), //Byte 1
                        Convert.ToByte(flash.Input), //Byte 2
                        0,
                        Convert.ToByte(flash.OnTime * 10), //Byte 4
                        Convert.ToByte(flash.OffTime * 10), //Byte 5
                        0, 0 }
                    },
                    MsgDescription = $"Flasher{flash.Number}"
                });
            }

            //CAN inputs
            foreach(var canInput in CanInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Prefix = (int)MessagePrefix.CANInput,
                    Index = canInput.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = new byte[] {
                        Convert.ToByte(MessagePrefix.CANInput), //Byte 0
                        Convert.ToByte(((Convert.ToByte(canInput.Operator) & 0x0F) << 4) +
                        ((Convert.ToByte(canInput.Mode) & 0x03) << 1) +
                        (Convert.ToByte(canInput.Enabled) & 0x01)), //Byte 1
                        Convert.ToByte(canInput.Number - 1), //Byte 2
                        Convert.ToByte((canInput.Id & 0xFF00) >> 8), //Byte 3
                        Convert.ToByte(canInput.Id & 0x00FF), //Byte 4
                        Convert.ToByte(((canInput.DLC & 0x0F) << 4) +
                                        (canInput.StartingByte & 0x0F)), //Byte 5
                        Convert.ToByte((canInput.OnVal & 0xFF00) >> 8), //Byte 6 
                        Convert.ToByte(canInput.OnVal & 0x00FF) } //Byte 7
                    },
                    MsgDescription = $"CANInput{canInput.Number}"
                });
            }

            //Wiper
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.Wiper,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 8,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.Wiper), //Byte 0 
                    Convert.ToByte(((Convert.ToByte(Wipers[0].WashWipeCycles) & 0x0F) << 4) +
                    ((Convert.ToByte(Wipers[0].ParkStopLevel) & 0x01) << 3) +
                    ((Convert.ToByte(Wipers[0].Mode) & 0x03) << 1) +
                    (Convert.ToByte(Wipers[0].Enabled) & 0x01)), //Byte 1
                    Convert.ToByte(Wipers[0].SlowInput), //Byte 2
                    Convert.ToByte(Wipers[0].FastInput), //Byte 3
                    Convert.ToByte(Wipers[0].InterInput), //Byte 4
                    Convert.ToByte(Wipers[0].OnInput), //Byte 5
                    Convert.ToByte(Wipers[0].ParkInput), //Byte 6
                    Convert.ToByte(Wipers[0].WashInput) } //Byte 7
                },
                MsgDescription = "Wiper"
            });

            //Wiper speeds
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.WiperSpeed,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.WiperSpeed), //Byte 0 
                    Convert.ToByte(Wipers[0].SwipeInput), //Byte 1
                    Convert.ToByte(Wipers[0].SpeedInput), //Byte 2

                    Convert.ToByte(((Convert.ToByte(Wipers[0].SpeedMap[1]) & 0x0F) << 4) +
                    (Convert.ToByte(Wipers[0].SpeedMap[0]) & 0x0F)), //Byte 3
                    Convert.ToByte(((Convert.ToByte(Wipers[0].SpeedMap[3]) & 0x0F) << 4) +
                    (Convert.ToByte(Wipers[0].SpeedMap[2]) & 0x0F)), //Byte 4
                    Convert.ToByte(((Convert.ToByte(Wipers[0].SpeedMap[4]) & 0x0F) << 4) +
                    (Convert.ToByte(Wipers[0].SpeedMap[5]) & 0x0F)), //Byte 5
                    Convert.ToByte(((Convert.ToByte(Wipers[0].SpeedMap[7]) & 0x0F) << 4) +
                    (Convert.ToByte(Wipers[0].SpeedMap[6]) & 0x0F)), //Byte 6
                    
                    0 }
                },
                MsgDescription = "WiperSpeed"
            }) ;

            //Wiper delays
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.WiperDelay,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.WiperDelay), //Byte 0 
                    Convert.ToByte(Wipers[0].IntermitTime[0] * 10), //Byte 1
                    Convert.ToByte(Wipers[0].IntermitTime[1] * 10), //Byte 2
                    Convert.ToByte(Wipers[0].IntermitTime[2] * 10), //Byte 3
                    Convert.ToByte(Wipers[0].IntermitTime[3] * 10), //Byte 4
                    Convert.ToByte(Wipers[0].IntermitTime[4] * 10), //Byte 5
                    Convert.ToByte(Wipers[0].IntermitTime[5] * 10), //Byte 6
                    0 }
                },
                MsgDescription = "WiperDelay"
            });

            //Starter disable
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.StarterDisable,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 4,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.StarterDisable), //Byte 0 
                    Convert.ToByte(Convert.ToByte(StarterDisable[0].Enabled) & 0x01), //Byte 1
                    Convert.ToByte(StarterDisable[0].Input), //Byte 2
                    Convert.ToByte(((Convert.ToByte(StarterDisable[0].Output8) & 0x01) << 7) +
                    ((Convert.ToByte(StarterDisable[0].Output7) & 0x01) << 6) +
                    ((Convert.ToByte(StarterDisable[0].Output6) & 0x01) << 5) +
                    ((Convert.ToByte(StarterDisable[0].Output5) & 0x01) << 4) +
                    ((Convert.ToByte(StarterDisable[0].Output4) & 0x01) << 3) +
                    ((Convert.ToByte(StarterDisable[0].Output3) & 0x01) << 2) +
                    ((Convert.ToByte(StarterDisable[0].Output2) & 0x01) << 1) +
                    (Convert.ToByte(StarterDisable[0].Output1) & 0x01)), //Byte 3
                    0, 0, 0, 0 }
                },
                MsgDescription = "StarterDisable"
            });

            return msgs;
        }

        public List<CanDeviceResponse> GetUpdateMessages(int newId)
        {
            int id = BaseId - 1;

            List<CanDeviceResponse> msgs = new List<CanDeviceResponse>();

            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.CAN,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 5,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.CAN), //Byte 0
                    Convert.ToByte((Convert.ToByte(BaudRate) << 4) +
                    (0x03)), //Byte 1
                    Convert.ToByte((newId & 0xFF00) >> 8), //Byte 2
                    Convert.ToByte(newId & 0x00FF), //Byte 3
                    Convert.ToByte(5), //Byte 4
                    0, 0, 0 }
                },
                MsgDescription = "CANSettings"
            });

            return msgs;
        }

        public CanDeviceResponse GetBurnMessage()
        {
            return new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.Burn,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 4,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Burn), 1, 3, 8, 0, 0, 0, 0 }
                },
                MsgDescription = "Burn Settings"
            };
        }

        public CanDeviceResponse GetSleepMessage()
        {
            return new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Prefix = (int)MessagePrefix.Sleep,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 4,
                    Payload = new byte[] { Convert.ToByte('Q'), Convert.ToByte('U'), Convert.ToByte('I'), Convert.ToByte('T'), 0, 0, 0, 0 }
                },
                MsgDescription = "Sleep Request"
            };
        }

        protected bool CheckVersion(int major, int minor, int build)
        {
            if (major > _minMajorVersion)
                return true;

            if ((major == _minMajorVersion) && (minor > _minMinorVersion))
                return true;

            if ((major == _minMajorVersion) && (minor == _minMinorVersion) && (build >= _minBuildVersion))
                return true;
            
            return false;
        }

        public void Clear()
        {
            foreach(var input in DigitalInputs)
            {
                input.State = false;
            }

            foreach(var output in Outputs)
            {
                output.Current = 0;
                output.State = OutState.Off;
            }

            foreach(var input in VirtualInputs)
            {
                input.Value = 0;
            }

            foreach(var canInput in CanInputs)
            {
                canInput.Value = 0;
            }
        }
    }
}
