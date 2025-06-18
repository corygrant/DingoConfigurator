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
        protected virtual int _minBuildVersion { get; } = 12;

        protected virtual int _numDigitalInputs { get; } = 2;
        protected virtual int _numOutputs { get; } = 8;
        protected virtual int _numCanInputs { get; } = 32;
        protected virtual int _numVirtualInputs { get; } = 16;
        protected virtual int _numFlashers { get; } = 4;
		protected virtual int _numCounters { get; } = 4;
		protected virtual int _numConditions { get; } = 32;
        protected virtual int _numKeypads { get; } = 2;
        protected virtual int _numKeypadButtons { get; } = 20;
        protected virtual int _numKeypadDials { get; } = 4;

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
                    OnPropertyChanged(nameof(VisibleSubPages));
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<CanDeviceSub> VisibleSubPages
        {
            get
            {
                return _subPages?.Where(subPage => 
                    !(subPage is Keypad keypad) || keypad.Visible) ?? new List<CanDeviceSub>();
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

        protected bool _sleepEnabled;
        [JsonPropertyName("sleepEnabled")]
        public bool SleepEnabled
        {
            get => _sleepEnabled;
            set
            {
                if (_sleepEnabled != value)
                {
                    _sleepEnabled = value;
                    OnPropertyChanged(nameof(SleepEnabled));
                }
            }
        }

        protected bool _canFiltersEnabled;
        [JsonPropertyName("canFiltersEnabled")]
        public bool CanFiltersEnabled
        {
            get => _canFiltersEnabled;
            set
            {
                if (_canFiltersEnabled != value)
                {
                    _canFiltersEnabled = value;
                    OnPropertyChanged(nameof(CanFiltersEnabled));
                }
            }
        }

        protected CanSpeed _baudRate;
        [JsonPropertyName("baudRate")]
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

		protected ObservableCollection<Counter> _counters;
		[JsonPropertyName("counters")]
		public ObservableCollection<Counter> Counters
		{
			get => _counters;
			set
			{
				if (value != _counters)
				{
					_counters = value;
					OnPropertyChanged(nameof(Counters));
				}
			}
		}

		protected ObservableCollection<Condition> _conditions;
		[JsonPropertyName("conditions")]
		public ObservableCollection<Condition> Conditions
		{
			get => _conditions;
			set
			{
				if (value != _conditions)
				{
					_conditions = value;
					OnPropertyChanged(nameof(Conditions));
				}
			}
		}

        protected ObservableCollection<Keypad> _keypads;
        [JsonPropertyName("keypads")]
        public ObservableCollection<Keypad> Keypads
        {
            get => _keypads;
            set
            {
                if (_keypads != value)
                {
                    _keypads = value;
                    OnPropertyChanged(nameof(Keypads));
                }
            }
        }

        [JsonIgnore]
        public int TimerIntervalMs { get => 0; }

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

			Counters = new ObservableCollection<Counter>();
			for (int i = 0; i < _numCounters; i++)
			{
				Counters.Add(new Counter());
				Counters[i].Number = i + 1;
			}

			Conditions = new ObservableCollection<Condition>();
			for (int i = 0; i < _numConditions; i++)
			{
				Conditions.Add(new Condition());
				Conditions[i].Number = i + 1;
			}

            Keypads = new ObservableCollection<Keypad>();
            for (int i = 0; i < _numKeypads; i++)
            {
                var keypad = new Keypad(i + 1, $"Keypad{i + 1}", this);
                keypad.PropertyChanged += Keypad_PropertyChanged;
                Keypads.Add(keypad);
            }

            SubPages.Add(new CanDeviceSub("Settings", this));
            SubPages.Add(new DingoPdmPlot("Plots", this));

            for(int i= 0; i < _numKeypads; i++)
            {
                SubPages.Add(Keypads[i]);
            }
        }

        public void InitializeAfterDeserialization()
        {
            // Set up parent references for keypads and ensure 20 buttons per keypad
            foreach (var keypad in Keypads)
            {
                keypad.CanDevice = this;
                keypad.Name = keypad.Name ?? $"Keypad{keypad.Number}";
                keypad.PropertyChanged += Keypad_PropertyChanged;
                
                // Ensure each keypad has exactly 20 buttons
                var existingButtons = keypad.AllButtons?.ToList() ?? new List<Button>();
                keypad.AllButtons = new ObservableCollection<Button>();
                
                // Create exactly 20 buttons
                for (int i = 0; i < 20; i++)
                {
                    Button button;
                    if (i < existingButtons.Count)
                    {
                        // Use existing button from config if available
                        button = existingButtons[i];
                        // Ensure button properties are correct
                        button.Number = i + 1;
                        button.KeypadNumber = keypad.Number;
                    }
                    else
                    {
                        // Create new button for missing entries
                        button = new Button(keypad.Number, i + 1);
                    }
                    keypad.AllButtons.Add(button);
                }
            }

            // Set up SubPages
            SubPages.Clear();
            SubPages.Add(new CanDeviceSub("Settings", this));
            SubPages.Add(new DingoPdmPlot("Plots", this));
            foreach (var keypad in Keypads)
            {
                SubPages.Add(keypad);
            }
        }

        public void UpdateIsConnected()
        {
            //Have to use a property set to get OnPropertyChanged to fire
            //Otherwise could be directly in the getter
            TimeSpan timeSpan = DateTime.Now - LastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;

            foreach (var subPage in _subPages)
            {
                subPage.UpdateIsConnected();
            }
        }

        public bool InIdRange(int id)
        {
            foreach(var kp in Keypads)
            {
                if (kp.InIdRange(id))
                    return true;
            }

            return (id >= BaseId) && (id <= BaseId + 31) ;
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            foreach (var kp in Keypads)
            {
                kp.Read(id, data, ref queue);
            }

            if ((id < BaseId) || (id > BaseId + 31)) 
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
            if (id == BaseId + 18) ReadMessage18(data);

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
            BoardTempC = (Math.Round(Convert.ToDouble((data[7] << 8) + data[6]))) / 10.0;
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

        protected virtual void ReadMessage5(byte[] data)
        {
            CanInputs[0].Output = Convert.ToBoolean(data[0] & 0x01);
            CanInputs[1].Output = Convert.ToBoolean((data[0] >> 1) & 0x01);
            CanInputs[2].Output = Convert.ToBoolean((data[0] >> 2) & 0x01);
            CanInputs[3].Output = Convert.ToBoolean((data[0] >> 3) & 0x01);
            CanInputs[4].Output = Convert.ToBoolean((data[0] >> 4) & 0x01);
            CanInputs[5].Output = Convert.ToBoolean((data[0] >> 5) & 0x01);
            CanInputs[6].Output = Convert.ToBoolean((data[0] >> 6) & 0x01);
            CanInputs[7].Output = Convert.ToBoolean((data[0] >> 7) & 0x01);

            CanInputs[8].Output  = Convert.ToBoolean(data[1] & 0x01);
            CanInputs[9].Output  = Convert.ToBoolean((data[1] >> 1) & 0x01);
            CanInputs[10].Output = Convert.ToBoolean((data[1] >> 2) & 0x01);
            CanInputs[11].Output = Convert.ToBoolean((data[1] >> 3) & 0x01);
            CanInputs[12].Output = Convert.ToBoolean((data[1] >> 4) & 0x01);
            CanInputs[13].Output = Convert.ToBoolean((data[1] >> 5) & 0x01);
            CanInputs[14].Output = Convert.ToBoolean((data[1] >> 6) & 0x01);
            CanInputs[15].Output = Convert.ToBoolean((data[1] >> 7) & 0x01);
                                   
            CanInputs[16].Output = Convert.ToBoolean(data[2] & 0x01);
            CanInputs[17].Output = Convert.ToBoolean((data[2] >> 1) & 0x01);
            CanInputs[18].Output = Convert.ToBoolean((data[2] >> 2) & 0x01);
            CanInputs[19].Output = Convert.ToBoolean((data[2] >> 3) & 0x01);
            CanInputs[20].Output = Convert.ToBoolean((data[2] >> 4) & 0x01);
            CanInputs[21].Output = Convert.ToBoolean((data[2] >> 5) & 0x01);
            CanInputs[22].Output = Convert.ToBoolean((data[2] >> 6) & 0x01);
            CanInputs[23].Output = Convert.ToBoolean((data[2] >> 7) & 0x01);
                                  
            CanInputs[24].Output = Convert.ToBoolean(data[3] & 0x01);
            CanInputs[25].Output = Convert.ToBoolean((data[3] >> 1) & 0x01);
            CanInputs[26].Output = Convert.ToBoolean((data[3] >> 2) & 0x01);
            CanInputs[27].Output = Convert.ToBoolean((data[3] >> 3) & 0x01);
            CanInputs[28].Output = Convert.ToBoolean((data[3] >> 4) & 0x01);
            CanInputs[29].Output = Convert.ToBoolean((data[3] >> 5) & 0x01);
            CanInputs[30].Output = Convert.ToBoolean((data[3] >> 6) & 0x01);
            CanInputs[31].Output = Convert.ToBoolean((data[3] >> 7) & 0x01);

            VirtualInputs[0].Value = Convert.ToBoolean(data[4] & 0x01);
            VirtualInputs[1].Value = Convert.ToBoolean((data[4] >> 1) & 0x01);
            VirtualInputs[2].Value = Convert.ToBoolean((data[4] >> 2) & 0x01);
            VirtualInputs[3].Value = Convert.ToBoolean((data[4] >> 3) & 0x01);
            VirtualInputs[4].Value = Convert.ToBoolean((data[4] >> 4) & 0x01);
            VirtualInputs[5].Value = Convert.ToBoolean((data[4] >> 5) & 0x01);
            VirtualInputs[6].Value = Convert.ToBoolean((data[4] >> 6) & 0x01);
            VirtualInputs[7].Value = Convert.ToBoolean((data[4] >> 7) & 0x01);

            VirtualInputs[8].Value = Convert.ToBoolean(data[5] & 0x01);
            VirtualInputs[9].Value = Convert.ToBoolean((data[5] >> 1) & 0x01);
            VirtualInputs[10].Value = Convert.ToBoolean((data[5] >> 2) & 0x01);
            VirtualInputs[11].Value = Convert.ToBoolean((data[5] >> 3) & 0x01);
            VirtualInputs[12].Value = Convert.ToBoolean((data[5] >> 4) & 0x01);
            VirtualInputs[13].Value = Convert.ToBoolean((data[5] >> 5) & 0x01);
            VirtualInputs[14].Value = Convert.ToBoolean((data[5] >> 6) & 0x01);
            VirtualInputs[15].Value = Convert.ToBoolean((data[5] >> 7) & 0x01);
        }

        protected virtual void ReadMessage6(byte[] data)
		{
			Counters[0].Value = data[0];
			Counters[1].Value = data[1];
			Counters[2].Value = data[2];
			Counters[3].Value = data[3];

			Conditions[0].Value = data[4] & 0x01;
			Conditions[1].Value = (data[4] >> 1) & 0x01;
			Conditions[2].Value = (data[4] >> 2) & 0x01;
			Conditions[3].Value = (data[4] >> 3) & 0x01;
			Conditions[4].Value = (data[4] >> 4) & 0x01;
			Conditions[5].Value = (data[4] >> 5) & 0x01;
			Conditions[6].Value = (data[4] >> 6) & 0x01;
			Conditions[7].Value = (data[4] >> 7) & 0x01;

			Conditions[8].Value = data[5] & 0x01;
			Conditions[9].Value = (data[5] >> 1) & 0x01;
			Conditions[10].Value = (data[5] >> 2) & 0x01;
			Conditions[11].Value = (data[5] >> 3) & 0x01;
			Conditions[12].Value = (data[5] >> 4) & 0x01;
			Conditions[13].Value = (data[5] >> 5) & 0x01;
			Conditions[14].Value = (data[5] >> 6) & 0x01;
			Conditions[15].Value = (data[5] >> 7) & 0x01;

			Conditions[16].Value = data[6] & 0x01;
			Conditions[17].Value = (data[6] >> 1) & 0x01;
			Conditions[18].Value = (data[6] >> 2) & 0x01;
			Conditions[19].Value = (data[6] >> 3) & 0x01;
			Conditions[20].Value = (data[6] >> 4) & 0x01;
			Conditions[21].Value = (data[6] >> 5) & 0x01;
			Conditions[22].Value = (data[6] >> 6) & 0x01;
			Conditions[23].Value = (data[6] >> 7) & 0x01;

			Conditions[24].Value = data[7] & 0x01;
			Conditions[25].Value = (data[7] >> 1) & 0x01;
			Conditions[26].Value = (data[7] >> 2) & 0x01;
			Conditions[27].Value = (data[7] >> 3) & 0x01;
			Conditions[28].Value = (data[7] >> 4) & 0x01;
			Conditions[29].Value = (data[7] >> 5) & 0x01;
			Conditions[30].Value = (data[7] >> 6) & 0x01;
			Conditions[31].Value = (data[7] >> 7) & 0x01;
		}

        protected virtual void ReadMessage7(byte[] data)
        {
            CanInputs[0].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[1].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[2].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[3].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage8(byte[] data)
        {
            CanInputs[4].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[5].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[6].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[7].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage9(byte[] data)
        {
            CanInputs[8].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[9].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[10].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[11].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage10(byte[] data)
        {
            CanInputs[12].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[13].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[14].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[15].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage11(byte[] data)
        {
            CanInputs[16].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[17].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[18].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[19].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage12(byte[] data)
        {
            CanInputs[20].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[21].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[22].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[23].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage13(byte[] data)
        {
            CanInputs[24].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[25].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[26].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[27].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage14(byte[] data)
        {
            CanInputs[28].Value = Convert.ToInt16((data[1] << 8) + data[0]);
            CanInputs[29].Value = Convert.ToInt16((data[3] << 8) + data[2]);
            CanInputs[30].Value = Convert.ToInt16((data[5] << 8) + data[4]);
            CanInputs[31].Value = Convert.ToInt16((data[7] << 8) + data[6]);
        }

        protected virtual void ReadMessage15(byte[] data)
        {
            Outputs[0].CurrentDutyCycle = data[0];
            Outputs[1].CurrentDutyCycle = data[1];
            Outputs[2].CurrentDutyCycle = data[2];
            Outputs[3].CurrentDutyCycle = data[3];
            Outputs[4].CurrentDutyCycle = data[4];
            Outputs[5].CurrentDutyCycle = data[5];
            Outputs[6].CurrentDutyCycle = data[6];
            Outputs[7].CurrentDutyCycle = data[7];
        }

        protected virtual void ReadMessage16(byte[] data)
        {
            Keypads[0].SetButtonState(0, Convert.ToBoolean(data[0] & 0x01));
            Keypads[0].SetButtonState(1, Convert.ToBoolean((data[0] >> 1) & 0x01));
            Keypads[0].SetButtonState(2, Convert.ToBoolean((data[0] >> 2) & 0x01));
            Keypads[0].SetButtonState(3, Convert.ToBoolean((data[0] >> 3) & 0x01));
            Keypads[0].SetButtonState(4, Convert.ToBoolean((data[0] >> 4) & 0x01));
            Keypads[0].SetButtonState(5, Convert.ToBoolean((data[0] >> 5) & 0x01));
            Keypads[0].SetButtonState(6, Convert.ToBoolean((data[0] >> 6) & 0x01));
            Keypads[0].SetButtonState(7, Convert.ToBoolean((data[0] >> 7) & 0x01));
            Keypads[0].SetButtonState(8, Convert.ToBoolean(data[1] & 0x01));
            Keypads[0].SetButtonState(9, Convert.ToBoolean((data[1] >> 1) & 0x01));
            Keypads[0].SetButtonState(10, Convert.ToBoolean((data[1] >> 2) & 0x01));
            Keypads[0].SetButtonState(11, Convert.ToBoolean((data[1] >> 3) & 0x01));
            Keypads[0].SetButtonState(12, Convert.ToBoolean((data[1] >> 4) & 0x01));
            Keypads[0].SetButtonState(13, Convert.ToBoolean((data[1] >> 5) & 0x01));
            Keypads[0].SetButtonState(14, Convert.ToBoolean((data[1] >> 6) & 0x01));
            Keypads[0].SetButtonState(15, Convert.ToBoolean((data[1] >> 7) & 0x01));
            Keypads[0].SetButtonState(16, Convert.ToBoolean(data[2] & 0x01));
            Keypads[0].SetButtonState(17, Convert.ToBoolean((data[2] >> 1) & 0x01));
            Keypads[0].SetButtonState(18, Convert.ToBoolean((data[2] >> 2) & 0x01));
            Keypads[0].SetButtonState(19, Convert.ToBoolean((data[2] >> 3) & 0x01));

            Keypads[1].SetButtonState(0, Convert.ToBoolean(data[3] & 0x01));
            Keypads[1].SetButtonState(1, Convert.ToBoolean((data[3] >> 1) & 0x01));
            Keypads[1].SetButtonState(2, Convert.ToBoolean((data[3] >> 2) & 0x01));
            Keypads[1].SetButtonState(3, Convert.ToBoolean((data[3] >> 3) & 0x01));
            Keypads[1].SetButtonState(4, Convert.ToBoolean((data[3] >> 4) & 0x01));
            Keypads[1].SetButtonState(5, Convert.ToBoolean((data[3] >> 5) & 0x01));
            Keypads[1].SetButtonState(6, Convert.ToBoolean((data[3] >> 6) & 0x01));
            Keypads[1].SetButtonState(7, Convert.ToBoolean((data[3] >> 7) & 0x01));
            Keypads[1].SetButtonState(8, Convert.ToBoolean(data[4] & 0x01));
            Keypads[1].SetButtonState(9, Convert.ToBoolean((data[4] >> 1) & 0x01));
            Keypads[1].SetButtonState(10, Convert.ToBoolean((data[4] >> 2) & 0x01));
            Keypads[1].SetButtonState(11, Convert.ToBoolean((data[4] >> 3) & 0x01));
            Keypads[1].SetButtonState(12, Convert.ToBoolean((data[4] >> 4) & 0x01));
            Keypads[1].SetButtonState(13, Convert.ToBoolean((data[4] >> 5) & 0x01));
            Keypads[1].SetButtonState(14, Convert.ToBoolean((data[4] >> 6) & 0x01));
            Keypads[1].SetButtonState(15, Convert.ToBoolean((data[4] >> 7) & 0x01));
            Keypads[1].SetButtonState(16, Convert.ToBoolean(data[5] & 0x01));
            Keypads[1].SetButtonState(17, Convert.ToBoolean((data[5] >> 1) & 0x01));
            Keypads[1].SetButtonState(18, Convert.ToBoolean((data[5] >> 2) & 0x01));
            Keypads[1].SetButtonState(19, Convert.ToBoolean((data[5] >> 3) & 0x01));
        }

        protected virtual void ReadMessage17(byte[] data)
        {
            Keypads[0].SetDialTicks(0, data[0] + (data[1] << 8));
            Keypads[0].SetDialTicks(1, data[2] + (data[3] << 8));
            Keypads[0].SetDialTicks(2, data[4] + (data[5] << 8));
            Keypads[0].SetDialTicks(3, data[6] + (data[7] << 8));
        }

        protected virtual void ReadMessage18(byte[] data)
        {
            Keypads[1].SetDialTicks(0, data[0] + (data[1] << 8));
            Keypads[1].SetDialTicks(1, data[2] + (data[3] << 8));
            Keypads[1].SetDialTicks(2, data[4] + (data[5] << 8));
            Keypads[1].SetDialTicks(3, data[6] + (data[7] << 8));
        }

        protected void ReadSettingsResponse(byte[] data, ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            //Response is prefix + 128
            if (data[0] < 128)
                return;
                
            var prefix = (MessagePrefix)(data[0] - 128);

            int index = 0;
            int buttonIndex = 0;

            //Vars used below
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
                    if (queue.TryGetValue(key, out response))
                    {
                        response.TimeSentTimer?.Dispose();
                        queue.TryRemove(key, out _);
                    }

                    break;

                case MessagePrefix.Can:
                    SleepEnabled = Convert.ToBoolean(data[1] & 0x01);
                    CanFiltersEnabled = Convert.ToBoolean((data[1] >> 1) & 0x01);
                    BaseId = (data[2] << 8) + data[3];
                    BaudRate = (CanSpeed)((data[1] & 0xF0)>>4);

                    key = (BaseId, (int)MessagePrefix.Can, 0);
                    if (queue.TryGetValue(key, out response))
                    {
                        response.TimeSentTimer?.Dispose();
                        queue.TryRemove(key, out _);
                    }

                    break;

                case MessagePrefix.Inputs:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numDigitalInputs)
                    {
                        if (DigitalInputs[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.Inputs, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.Outputs:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numOutputs)
                    {
                        if (Outputs[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.Outputs, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.OutputsPwm:
                    index = (data[1] & 0xF0) >> 4;

                    if (index >= 0 && index < _numOutputs)
                    {
                        if (Outputs[index].ReceivePwm(data))
                        {
                            key = (BaseId, (int)MessagePrefix.OutputsPwm, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.VirtualInputs:
                    index = data[2];
                    
                    if (index >= 0 && index < _numVirtualInputs)
                    {
                        if (VirtualInputs[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.VirtualInputs, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.Flashers:
                    index = (data[1] & 0xF0) >> 4;
                    
                    if (index >= 0 && index < _numFlashers)
                    {
                        if (Flashers[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.Flashers, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.Wiper:
                    if (Wipers[0].Receive(data))
                    {
                        key = (BaseId, (int)MessagePrefix.Wiper, 0);
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
                        }
                    }

                    break;

                case MessagePrefix.WiperSpeed:
                    if (Wipers[0].ReceiveSpeed(data))
                    {
                        key = (BaseId, (int)MessagePrefix.WiperSpeed, 0);
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
                        }
                    }

                    break;

                case MessagePrefix.WiperDelays:
                    if (Wipers[0].ReceiveDelays(data))
                    {
                        key = (BaseId, (int)MessagePrefix.WiperDelays, 0);
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
                        }
                    }

                    break;

                case MessagePrefix.StarterDisable:
                    
                    if (StarterDisable[0].Receive(data))
                    {
                        key = (BaseId, (int)MessagePrefix.StarterDisable, 0);
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
                        }
                    }

                    break;

                case MessagePrefix.CanInputs:
                    index = data[1];
                    if (index >= 0 && index < _numCanInputs)
                    {
                        if(CanInputs[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.CanInputs, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.CanInputsId:
                    index = data[1];
                    if (index >= 0 && index < _numCanInputs)
                    {
                        if (CanInputs[index].ReceiveId(data))
                        {
                            key = (BaseId, (int)MessagePrefix.CanInputsId, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.Counter:
					index = data[1];
					if (index >= 0 && index < _numCounters)
					{
						if (Counters[index].Receive(data))
						{
							key = (BaseId, (int)MessagePrefix.Counter, index);
							if (queue.TryGetValue(key, out response))
							{
								response.TimeSentTimer?.Dispose();
								queue.TryRemove(key, out _);
							}
						}
					}

					break;

				case MessagePrefix.Conditions:
					index = data[1];
					if (index >= 0 && index < _numConditions)
					{
						if (Conditions[index].Receive(data))
						{
							key = (BaseId, (int)MessagePrefix.Conditions, index);
							if (queue.TryGetValue(key, out response))
							{
								response.TimeSentTimer?.Dispose();
								queue.TryRemove(key, out _);
							}
						}
					}

					break;

                case MessagePrefix.Keypad:
                    index = data[1];
                    if (index >= 0 && index < _numKeypads)
                    {
                        if (Keypads[index].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.Keypad, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.KeypadLed:
                    index = data[1];
                    if (index >= 0 && index < _numKeypads)
                    {
                        if (Keypads[index].ReceiveLed(data))
                        {
                            key = (BaseId, (int)MessagePrefix.KeypadLed, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.KeypadButton:
                    index = data[1] & 0x07;
                    buttonIndex = data[1] >> 3;
                    if ((index >= 0 && index < _numKeypads) && (buttonIndex >= 0 && buttonIndex < Keypads[index].NumButtons))
                    {
                        if (Keypads[index].AllButtons[buttonIndex].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.KeypadButton, (index & 0x07) + (buttonIndex << 3));
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.KeypadButtonLed:
                    index = data[1] & 0x07;
                    buttonIndex = data[1] >> 3;
                    if ((index >= 0 && index < _numKeypads) && (buttonIndex >= 0 && buttonIndex < Keypads[index].NumButtons))
                    {
                        if (Keypads[index].AllButtons[buttonIndex].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.KeypadButtonLed, (index & 0x07) + (buttonIndex << 3));
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.KeypadDial:
                    index = data[1] & 0x07;
                    buttonIndex = data[1] >> 3;
                    if ((index >= 0 && index < _numKeypads) && (buttonIndex >= 0 && buttonIndex < Keypads[index].Dials.Count))
                    {
                        if (Keypads[index].AllButtons[buttonIndex].Receive(data))
                        {
                            key = (BaseId, (int)MessagePrefix.KeypadDial, index);
                            if (queue.TryGetValue(key, out response))
                            {
                                response.TimeSentTimer?.Dispose();
                                queue.TryRemove(key, out _);
                            }
                        }
                    }

                    break;

                case MessagePrefix.BurnSettings:
                    if (data[1] == 1) //Successful burn
                    {
                        Logger.Info($"{Name} ID: {BaseId}, Burn Successful");

                        key = (BaseId, (int)MessagePrefix.BurnSettings, 0);
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
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
                        if (queue.TryGetValue(key, out response))
                        {
                            response.TimeSentTimer?.Dispose();
                            queue.TryRemove(key, out _);
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
            MessageType type = (MessageType)Char.ToUpper(Convert.ToChar(data[0]));
            MessageSrc src = (MessageSrc)data[1];

            switch (type)
            {
                case MessageType.Info:
                    Logger.Info($"{Name} ID: {BaseId}, Src: {src} {((data[3] << 8) + data[2])} {((data[5] << 8) + data[4])} {((data[7] << 8) + data[6])}");
                    break;
                case MessageType.Warning:
                    Logger.Warn($"{Name} ID: {BaseId}, Src: {src} {((data[3] << 8) + data[2])} {((data[5] << 8) + data[4])} {((data[7] << 8) + data[6])}");
                    break;
                case MessageType.Error:
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
                Prefix = (int)MessagePrefix.Can,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Can), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription="CANSettings"
            });

            //Inputs
            for (int i = 0; i < _numDigitalInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Inputs,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Input.Request(i)
                    },
                    MsgDescription=$"Input{i + 1}"
                });
            }

            //Outputs
            for (int i = 0; i < _numOutputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Outputs,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Output.Request(i)
                    },
                    MsgDescription = $"Output{i + 1}"
                });
            }

            //Outputs PWM
            for (int i = 0; i < _numOutputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.OutputsPwm,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Output.RequestPwm(i)
                    },
                    MsgDescription = $"OutputPwm{i + 1}"
                });
            }

            //Virtual inputs
            for (int i = 0; i < _numVirtualInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.VirtualInputs,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = VirtualInput.Request(i)
                    },
                    MsgDescription = $"VirtualInput{i + 1}"
                });
            }

            //Flashers
            for (int i = 0; i < _numFlashers; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Flashers,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = Flasher.Request(i)
                    },
                    MsgDescription = $"Flasher{i + 1}"
                });
            }

            //CAN inputs
            for (int i = 0; i < _numCanInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.CanInputs,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = CanInput.Request(i)
                    },
                    MsgDescription = $"CANInput{i + 1}"
                });
            }

            //CAN inputs ID
            for (int i = 0; i < _numCanInputs; i++)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.CanInputsId,
                    Index = i,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 2,
                        Payload = CanInput.RequestId(i)
                    },
                    MsgDescription = $"CANInputId{i + 1}"
                });
            }

            //Wiper
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.Wiper,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = Wiper.Request()
                },
                MsgDescription = "Wiper"
            });

            //Wiper speeds
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.WiperSpeed,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = Wiper.RequestSpeed()
                },
                MsgDescription = "WiperSpeed"
            });

            //Wiper delays
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.WiperDelays,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = Wiper.RequestDelays()
                },
                MsgDescription = "WiperDelay"
            });

            //Starter disable
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.StarterDisable,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 1,
                    Payload = CanDevices.DingoPdm.StarterDisable.Request()
                },
                MsgDescription = "StarterDisable"
            });

			//Counter
			for (int i = 0; i < _numCounters; i++)
			{
				msgs.Add(new CanDeviceResponse
				{
					Prefix = (int)MessagePrefix.Counter,
					Index = i,
					Data = new CanInterfaceData
					{
						Id = id,
						Len = 2,
						Payload = Counter.Request(i)
					},
					MsgDescription = $"Counter{i + 1}"
				});
			}

			//Condition
			for (int i = 0; i < _numConditions; i++)
			{
				msgs.Add(new CanDeviceResponse
				{
					Prefix = (int)MessagePrefix.Conditions,
					Index = i,
					Data = new CanInterfaceData
					{
						Id = id,
						Len = 2,
						Payload = Condition.Request(i)
					},
					MsgDescription = $"Condition{i + 1}"
				});
			}

            //Keypads
            for (int i = 0; i < _numKeypads; i++)
            {
                if (Keypads[i].Enabled)
                {
                    foreach (var msg in Keypads[i].RequestMsgs(id))
                    {
                        msgs.Add(msg);
                    }
                }
            }

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
                Prefix = (int)MessagePrefix.Can,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 4,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.Can), //Byte 0
                    Convert.ToByte(Convert.ToByte(SleepEnabled) + 
                    (Convert.ToByte(CanFiltersEnabled) << 1) + 
                    ((Convert.ToByte(BaudRate) & 0x0F) << 4)),
                    Convert.ToByte((BaseId & 0xFF00) >> 8), //Byte 2
                    Convert.ToByte(BaseId & 0x00FF), //Byte 3
                    0, 0, 0, 0 }
                },
                MsgDescription = "CANSettings"
            });

            //Inputs
            foreach(var input in DigitalInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Inputs,
                    Index = input.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 4,
                        Payload = DigitalInputs[input.Number - 1].Write()
                    },
                    MsgDescription = $"Input{input.Number}"
                });
            }

            //Outputs
            foreach(var output in Outputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Outputs,
                    Index = output.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = Outputs[output.Number - 1].Write()
                    },
                    MsgDescription = $"Output{output.Number}"
                });
            }

            //Outputs PWM
            foreach (var output in Outputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.OutputsPwm,
                    Index = output.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = Outputs[output.Number - 1].WritePwm()
                    },
                    MsgDescription = $"OutputPwm{output.Number}"
                });
            }

            //Virtual inputs
            foreach (var virtInput in VirtualInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.VirtualInputs,
                    Index = virtInput.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 7,
                        Payload = VirtualInputs[virtInput.Number - 1].Write()
                    },
                    MsgDescription = $"VirtualInput{virtInput.Number}"
                });
            }

            //Flashers
            foreach(var flash in Flashers)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.Flashers,
                    Index = flash.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 6,
                        Payload = Flashers[flash.Number - 1].Write()
                    },
                    MsgDescription = $"Flasher{flash.Number}"
                });
            }

            //CAN inputs
            foreach(var canInput in CanInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.CanInputs,
                    Index = canInput.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 7,
                        Payload = CanInputs[canInput.Number - 1].Write()

                    },
                    MsgDescription = $"CANInput{canInput.Number}"
                });
            }

            //CAN inputs ID
            foreach (var canInput in CanInputs)
            {
                msgs.Add(new CanDeviceResponse
                {
                    Prefix = (int)MessagePrefix.CanInputsId,
                    Index = canInput.Number - 1,
                    Data = new CanInterfaceData
                    {
                        Id = id,
                        Len = 8,
                        Payload = CanInputs[canInput.Number - 1].WriteId()

                    },
                    MsgDescription = $"CANInputId{canInput.Number}"
                });
            }

            //Wiper
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.Wiper,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 8,
                    Payload = Wipers[0].Write()
                },
                MsgDescription = "Wiper"
            });

            //Wiper speeds
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.WiperSpeed,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = Wipers[0].WriteSpeed()
                },
                MsgDescription = "WiperSpeed"
            }) ;

            //Wiper delays
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.WiperDelays,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 7,
                    Payload = Wipers[0].WriteDelays()
                },
                MsgDescription = "WiperDelay"
            });

            //Starter disable
            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.StarterDisable,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 4,
                    Payload = StarterDisable[0].Write()
                },
                MsgDescription = "StarterDisable"
            });

			//Counter
			foreach (var counter in Counters)
			{
				msgs.Add(new CanDeviceResponse
				{
					Prefix = (int)MessagePrefix.Counter,
					Index = counter.Number - 1,
					Data = new CanInterfaceData
					{
						Id = id,
						Len = 8,
						Payload = Counters[counter.Number - 1].Write()

					},
					MsgDescription = $"Counter{counter.Number}"
				});
			}

			//Condition
			foreach (var condition in Conditions)
			{
				msgs.Add(new CanDeviceResponse
				{
					Prefix = (int)MessagePrefix.Conditions,
					Index = condition.Number - 1,
					Data = new CanInterfaceData
					{
						Id = id,
						Len = 6,
						Payload = Conditions[condition.Number - 1].Write()

					},
					MsgDescription = $"Condition{condition.Number}"
				});
			}

            //Keypads
            foreach(var kp in Keypads)
            {
                if (kp.Enabled)
                {
                    foreach (var msg in kp.WriteMsgs(id))
                    {
                        msgs.Add(msg);
                    }
                }
            }

            return msgs;
        }

        public List<CanDeviceResponse> GetUpdateMessages(int newId)
        {
            int id = BaseId - 1;

            List<CanDeviceResponse> msgs = new List<CanDeviceResponse>();

            msgs.Add(new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.Can,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = id,
                    Len = 4,
                    Payload = new byte[] {
                    Convert.ToByte(MessagePrefix.Can), //Byte 0
                    Convert.ToByte(Convert.ToByte(SleepEnabled) +
                    (Convert.ToByte(CanFiltersEnabled) << 1) +
                    ((Convert.ToByte(BaudRate) & 0x0F) << 4)),
                    Convert.ToByte((newId & 0xFF00) >> 8), //Byte 2
                    Convert.ToByte(newId & 0x00FF), //Byte 3
                    0, 0, 0, 0 }
                },
                MsgDescription = "CANSettings"
            });

            return msgs;
        }

        public CanDeviceResponse GetBurnMessage()
        {
            return new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.BurnSettings,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 4,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.BurnSettings), 1, 3, 8, 0, 0, 0, 0 }
                },
                MsgDescription = "Burn Settings"
            };
        }

        public CanDeviceResponse GetSleepMessage()
        {
            return new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.Sleep,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 5,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Sleep), Convert.ToByte('Q'), Convert.ToByte('U'), Convert.ToByte('I'), Convert.ToByte('T'), 0, 0, 0 }
                },
                MsgDescription = "Sleep Request"
            };
        }

        public CanDeviceResponse GetVersionMessage()
        {
            return new CanDeviceResponse
            {
                Prefix = (int)MessagePrefix.Version,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = BaseId - 1,
                    Len = 1,
                    Payload = new byte[] { Convert.ToByte(MessagePrefix.Version), 0, 0, 0, 0, 0, 0, 0 }
                },
                MsgDescription = "Version"
            };
        }

        public virtual List<CanDeviceResponse> GetTimerMessages()
        {
            return new List<CanDeviceResponse>();
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

        private void Keypad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Keypad.Visible))
            {
                OnPropertyChanged(nameof(VisibleSubPages));
            }
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
                input.Value = false;
            }

            foreach(var canInput in CanInputs)
            {
                canInput.Output = false;
            }
        }
    }
}
