using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CanDevices.DingoPdm
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

        public void SetVars(PdmConfig config)
        {
            Name = config.label;
            BaseId = config.canOutput.baseId;

            int index = 0;

            foreach(var di in DigitalInputs)
            {
                di.Name = config.input[index].label;
                di.Enabled = config.input[index].enabled;
                di.Mode = config.input[index].mode;
                di.InvertInput = config.input[index].invertInput;
                di.DebounceTime = config.input[index].debounceTime;

                index++;
            }

            index = 0;

            foreach(var output in Outputs)
            {
                output.Name = config.output[index].label;
                output.Enabled = config.output[index].enabled;
                output.Input = config.output[index].input;
                output.CurrentLimit = config.output[index].currentLimit;
                output.InrushCurrentLimit = config.output[index].inrushLimit;
                output.InrushTime = config.output[index].inrushTime;
                output.ResetMode = config.output[index].resetMode;
                output.ResetTime = config.output[index].resetTime;
                output.ResetCountLimit = config.output[index].resetLimit;

                index++;
            }

            index = 0;

            foreach(var canIn in CanInputs)
            {
                canIn.Name = config.canInput[index].label;
                canIn.Enabled = config.canInput[index].enabled;
                canIn.Id = config.canInput[index].id;
                canIn.LowByte = config.canInput[index].lowByte;
                canIn.HighByte = config.canInput[index].highByte;
                canIn.Operator = config.canInput[index].oper;
                canIn.OnVal = config.canInput[index].onValue;
                canIn.Mode = config.canInput[index].mode;

                index++;
            }

            index = 0;

            foreach(var virtIn in VirtualInputs)
            {
                virtIn.Name = config.virtualInput[index].label;
                virtIn.Enabled = config.virtualInput[index].enabled;
                virtIn.Not0 = config.virtualInput[index].not0;
                virtIn.Var0 = config.virtualInput[index].var0;
                virtIn.Cond0 = config.virtualInput[index].cond0;
                virtIn.Not1 = config.virtualInput[index].not1;
                virtIn.Var1 = config.virtualInput[index].var1;
                virtIn.Not2 = config.virtualInput[index].not2;
                virtIn.Var2 = config.virtualInput[index].var2;
                virtIn.Mode = config.virtualInput[index].mode;

                index++;
            }

            index = 0;

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

            foreach(var flash in Flashers)
            {
                flash.Name = config.flasher[index].label;
                flash.Enabled = config.flasher[index].enabled;
                flash.Input = config.flasher[index].input;
                flash.OnTime = config.flasher[index].flashOnTime;
                flash.OffTime = config.flasher[index].flashOffTime;
                flash.Single = config.flasher[index].singleCycle == 1;
                flash.Output = config.flasher[index].output;

                index++;
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
    }
}
