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
        private const int _minMajorVersion = 0;
        private const int _minMinorVersion = 3;
        private const int _minBuildVersion = 2;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private string _name;
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

        private int _baseId;
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

        private List<CanDeviceSub> _subPages = new List<CanDeviceSub>();
        [JsonIgnore]
        public List<CanDeviceSub> SubPages
        {
            get => _subPages;
            private set
            {
                if (_subPages != value)
                {
                    _subPages = value;
                    OnPropertyChanged(nameof(SubPages));
                }
            }
        }

        private DateTime _lastRxTime { get; set; }
        [JsonIgnore]
        public DateTime LastRxTime { get => _lastRxTime; }

        private bool _isConnected;
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

        private ObservableCollection<Input> _digitalInputs { get; set; }
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

        private DeviceState _deviceState;
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

        private double _totalCurrent;
        [JsonIgnore]
        public double TotalCurrent
        {
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
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
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

        private CanSpeed _baudRate;
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

        private ObservableCollection<Output> _outputs;
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

        private ObservableCollection<CanInput> _canInputs;
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

        private ObservableCollection<VirtualInput> _virtualInputs;
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

        private ObservableCollection<Wiper> _wipers;
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

        private ObservableCollection<Flasher> _flashers;
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

        private ObservableCollection<StarterDisable> _starterDisable;
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
            for (int i = 0; i < 2; i++)
            {
                DigitalInputs.Add(new Input());
                DigitalInputs[i].Number = i + 1;
            }

            TotalCurrent = 0;
            BatteryVoltage = 0;
            BoardTempC = 0;

            Outputs = new ObservableCollection<Output>();
            for (int i = 0; i < 8; i++)
            {
                Outputs.Add(new Output());
                Outputs[i].Number = i + 1;
            }

            CanInputs = new ObservableCollection<CanInput>();
            for (int i = 0; i < 32; i++)
            {
                CanInputs.Add(new CanInput());
                CanInputs[i].Number = i + 1;
            }

            VirtualInputs = new ObservableCollection<VirtualInput>();
            for (int i = 0; i < 16; i++)
            {
                VirtualInputs.Add(new VirtualInput());
                VirtualInputs[i].Number = i + 1;
            }

            Wipers = new ObservableCollection<Wiper>
            {
                new Wiper()
            };

            Flashers = new ObservableCollection<Flasher>();
            for (int i = 0; i < 4; i++)
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

        public bool Read(int id, byte[] data, ref List<CanDeviceResponse> queue)
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

        private void ReadMessage0(byte[] data)
        {
            DigitalInputs[0].State = Convert.ToBoolean(data[0] & 0x01);
            DigitalInputs[1].State = Convert.ToBoolean((data[0] >> 1) & 0x01);

            DeviceState = (DeviceState)(data[1]);

            TotalCurrent = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            BatteryVoltage = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            BoardTempC = Math.Round(Convert.ToDouble((data[6] << 8) + data[7]));
            BoardTempF = Math.Round(BoardTempC * 1.8 + 32);

        }

        private void ReadMessage1(byte[] data)
        {
            Outputs[0].Current = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[1].Current = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[2].Current = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[3].Current = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage2(byte[] data)
        {
            Outputs[4].Current = Convert.ToDouble(((data[0] << 8) + data[1]) / 10.0);
            Outputs[5].Current = Convert.ToDouble(((data[2] << 8) + data[3]) / 10.0);
            Outputs[6].Current = Convert.ToDouble(((data[4] << 8) + data[5]) / 10.0);
            Outputs[7].Current = Convert.ToDouble(((data[6] << 8) + data[7]) / 10.0);
        }

        private void ReadMessage3(byte[] data)
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

            Flashers[0].Value = Convert.ToBoolean(data[6] & 0x01) && Flashers[0].Enabled && Flashers[0].InputValue;
            Flashers[1].Value = Convert.ToBoolean((data[6] >> 1) & 0x01) && Flashers[1].Enabled && Flashers[1].InputValue;
            Flashers[2].Value = Convert.ToBoolean((data[6] >> 2) & 0x01) && Flashers[2].Enabled && Flashers[2].InputValue;
            Flashers[3].Value = Convert.ToBoolean((data[6] >> 3) & 0x01) && Flashers[3].Enabled && Flashers[3].InputValue;

            Flashers[0].InputValue = Convert.ToBoolean((data[6] >> 4) & 0x01);
            Flashers[1].InputValue = Convert.ToBoolean((data[6] >> 5) & 0x01);
            Flashers[2].InputValue = Convert.ToBoolean((data[6] >> 6) & 0x01);
            Flashers[3].InputValue = Convert.ToBoolean((data[6] >> 7) & 0x01);
        }

        private void ReadMessage4(byte[] data)
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

        private void ReadMessage5(byte[] data)
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


        private void ReadSettingsResponse(byte[] data, List<CanDeviceResponse> queue)
        {
            //Response is lowercase version of set/get prefix
            MessagePrefix prefix = (MessagePrefix)Char.ToUpper(Convert.ToChar(data[0]));

            int index = 0;

            switch (prefix)
            {
                case MessagePrefix.Version:
                    Version = $"V{data[1]}.{data[2]}.{(data[3] << 8) + (data[4])}";

                    if (!CheckVersion(data[1], data[2], (data[3] << 8) + (data[4])))
                    {
                        Logger.Error($"{Name} ID: {BaseId}, Firmware needs to be updated. V{_minMajorVersion}.{_minMinorVersion}.{_minBuildVersion} or greater");
                    }

                    foreach(var msg in queue)
                    {
                        if((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Version))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
                    }
                    break;

                case MessagePrefix.CAN:
                    BaseId = (data[2] << 8) + data[3];
                    BaudRate = (CanSpeed)((data[1] & 0xF0)>>4);

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.CAN))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
                    }
                    break;

                case MessagePrefix.Input:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < 2)
                    {
                        DigitalInputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        DigitalInputs[index].InvertInput = Convert.ToBoolean((data[1] & 0x08) >> 3);
                        DigitalInputs[index].Mode = (InputMode)((data[1] & 0x06) >> 1);
                        DigitalInputs[index].DebounceTime = data[2] * 10;
                        DigitalInputs[index].Pull = (InputPull)(data[3] & 0x03);
                    }

                    foreach (var msg in queue)
                    {
                        if((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Input) &&
                                        ((msg.Data.Payload[1] & 0xF0) >> 4) == index)
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
                    }
                    break;

                case MessagePrefix.Output:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < 8)
                    {
                        Outputs[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Outputs[index].Input = (VarMap)(data[2]);
                        Outputs[index].CurrentLimit = data[3];
                        Outputs[index].ResetCountLimit = (data[4] & 0xF0) >> 4;
                        Outputs[index].ResetMode = (ResetMode)(data[4] & 0x0F);
                        Outputs[index].ResetTime = data[5] / 10.0;
                        Outputs[index].InrushCurrentLimit = data[6];
                        Outputs[index].InrushTime = data[7] / 10.0;
                    }

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Output) &&
                                        ((msg.Data.Payload[1] & 0xF0) >> 4) == index)
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
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

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.VirtualInput))
                        {
                            if (((msg.Data.Len == 7) && (msg.Data.Payload[2] == index)) ||
                                    ((msg.Data.Len == 2) && (msg.Data.Payload[1] == index)))
                            {
                                msg.TimeSentTimer.Dispose();
                                queue.Remove(msg);
                                break;
                            }
                        }
                    }
                    break;

                case MessagePrefix.Flasher:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < 16)
                    {
                        Flashers[index].Enabled = Convert.ToBoolean(data[1] & 0x01);
                        Flashers[index].Single = Convert.ToBoolean((data[1] & 0x02) >> 1);
                        Flashers[index].Input = (VarMap)(data[2]);
                        Flashers[index].OnTime = data[4] / 10.0;
                        Flashers[index].OffTime = data[5] / 10.0;
                    }

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Flasher) &&
                                        ((msg.Data.Payload[1] & 0xF0) >> 4) == index)
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
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

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Wiper))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
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

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.WiperSpeed))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
                    }
                    break;

                case MessagePrefix.WiperDelay:
                    Wipers[0].IntermitTime[0] = data[1] / 10.0;
                    Wipers[0].IntermitTime[1] = data[2] / 10.0;
                    Wipers[0].IntermitTime[2] = data[3] / 10.0;
                    Wipers[0].IntermitTime[3] = data[4] / 10.0;
                    Wipers[0].IntermitTime[4] = data[5] / 10.0;
                    Wipers[0].IntermitTime[5] = data[6] / 10.0;

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.WiperDelay))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
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

                    foreach (var msg in queue)
                    {
                        if ((msg.DeviceBaseId == BaseId) &&
                                        ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.StarterDisable))
                        {
                            msg.TimeSentTimer.Dispose();
                            queue.Remove(msg);
                            break;
                        }
                    }
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

                        foreach (var msg in queue)
                        {
                            if ((msg.DeviceBaseId == BaseId) &&
                                            ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.CANInput))
                            {
                                if (((msg.Data.Len == 7) && (msg.Data.Payload[2] == index)) ||
                                    ((msg.Data.Len == 2) && (msg.Data.Payload[1] == index)))
                                {
                                    msg.TimeSentTimer.Dispose();
                                    queue.Remove(msg);
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case MessagePrefix.Burn:
                    if (data[1] == 1) //Successful burn
                    {
                        Logger.Info($"{Name} ID: {BaseId}, Burn Successful");

                        foreach (var msg in queue)
                        {
                            if ((msg.DeviceBaseId == BaseId) &&
                                            ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Burn))
                            {
                                msg.TimeSentTimer.Dispose();
                                queue.Remove(msg);
                                break;
                            }
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

                        foreach (var msg in queue)
                        {
                            if ((msg.DeviceBaseId == BaseId) &&
                                            ((MessagePrefix)Convert.ToChar(msg.Data.Payload[0]) == MessagePrefix.Sleep))
                            {
                                msg.TimeSentTimer.Dispose();
                                queue.Remove(msg);
                                break;
                            }
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

        private void ReadInfoWarnErrorMessage(byte[] data)
        {
            //Response is lowercase version of set/get prefix
            MessagePrefix prefix = (MessagePrefix)Char.ToUpper(Convert.ToChar(data[0]));
            MessageSrc src = (MessageSrc)data[1];

            int index = 0;

            switch (prefix)
            {
                case MessagePrefix.Info:
                    Logger.Info($"{Name} ID: {BaseId}, Src: {src} {data[2]} {data[3]} {data[4]}");
                    break;
                case MessagePrefix.Warning:
                    Logger.Warn($"{Name} ID: {BaseId}, Src: {src} {data[2]} {data[3]} {data[4]}");
                    break;
                case MessagePrefix.Error:
                    Logger.Error($"{Name} ID: {BaseId}, Src: {src} {data[2]} {data[3]} {data[4]}");
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('V'), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription="Version"
            });

            //CAN settings
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('C'), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription="CANSettings"
            });

            //Inputs
            for (int i = 0; i < 2; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte('I'),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription=$"Input{i + 1}"
                });
            }

            //Outputs
            for (int i = 0; i < 8; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte('O'),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"Output{i + 1}"
                });
            }

            //Virtual inputs
            for (int i = 0; i < 16; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte('U'),
                        Convert.ToByte(i),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"VirtualInput{i + 1}"
                });
            }

            //Flashers
            for (int i = 0; i < 4; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte('H'),
                        Convert.ToByte((i & 0x0F) << 4),
                        0, 0, 0, 0, 0, 0 }
                    },
                    MsgDescription = $"Flasher{i + 1}"
                });
            }

            //CAN inputs
            for (int i = 0; i < 32; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Sent = false,
                    Received = false,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = new byte[] { Convert.ToByte('N'),
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('W'), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "Wiper"
            });

            //Wiper speeds
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('P'), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "WiperSpeed"
            });

            //Wiper delays
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('Y'), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "WiperDelay"
            });

            //Starter disable
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte('D'), 0, 0, 0, 0, 0, 0, 0 }
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 5,
                    Payload = new byte[] {
                    Convert.ToByte('C'), //Byte 0
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
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 4,
                        Payload = new byte[] {
                        Convert.ToByte('I'), //Byte 0
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
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = new byte[] {
                        Convert.ToByte('O'), //Byte 0
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
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 7,
                        Payload = new byte[] {
                        Convert.ToByte('U'), //Byte 0
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
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 6,
                        Payload = new byte[] {
                        Convert.ToByte('H'), //Byte 0
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
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 7,
                        Payload = new byte[] {
                        Convert.ToByte('N'), //Byte 0
                        Convert.ToByte(((Convert.ToByte(canInput.Operator) & 0x0F) << 4) +
                        ((Convert.ToByte(canInput.Mode) & 0x03) << 1) +
                        (Convert.ToByte(canInput.Enabled) & 0x01)), //Byte 1
                        Convert.ToByte(canInput.Number - 1), //Byte 2
                        Convert.ToByte((canInput.Id & 0xFF00) >> 8), //Byte 3
                        Convert.ToByte(canInput.Id & 0x00FF), //Byte 4
                        Convert.ToByte(((canInput.HighByte & 0x0F) << 4) +
                        (canInput.LowByte & 0x0F)), //Byte 5
                        Convert.ToByte(canInput.OnVal), //Byte 6 
                        0 }
                    },
                    MsgDescription = $"CANInput{canInput.Number}"
                });
            }

            //Wiper
            msgs.Add(new CanDeviceResponse
            {
                Sent = false,
                Received = false,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 8,
                    Payload = new byte[] {
                    Convert.ToByte('W'), //Byte 0 
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = new byte[] {
                    Convert.ToByte('P'), //Byte 0 
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = new byte[] {
                    Convert.ToByte('Y'), //Byte 0 
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 4,
                    Payload = new byte[] {
                    Convert.ToByte('D'), //Byte 0 
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
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 5,
                    Payload = new byte[] {
                    Convert.ToByte('C'), //Byte 0
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
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 4,
                    Payload = new byte[] { Convert.ToByte('B'), 1, 3, 8, 0, 0, 0, 0 }
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
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 4,
                    Payload = new byte[] { Convert.ToByte('Q'), Convert.ToByte('U'), Convert.ToByte('I'), Convert.ToByte('T'), 0, 0, 0, 0 }
                },
                MsgDescription = "Sleep Request"
            };
        }

        private bool CheckVersion(int major, int minor, int build)
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
