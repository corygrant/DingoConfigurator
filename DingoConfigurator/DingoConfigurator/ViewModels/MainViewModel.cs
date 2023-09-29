using CanDevices;
using CanDevices.DingoPdm;
using CanDevices.CanBoard;
using CanDevices.DingoDash;
using CanInterfaces;
using DingoConfigurator.Config;
using DingoConfigurator.Properties;
using DingoConfigurator.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Concurrent;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Buffers.Text;
using System.Reflection;
using System.Windows.Interop;

//Add another CanDevices list that holds the online value

namespace DingoConfigurator
{
    public class MainViewModel : ViewModelBase
    {
        private ICanInterface _can;

        private DevicesConfig _config;

        private int _countSinceLast;

        private bool _canInterfaceConnected;
        public bool CanInterfaceConnected
        {
            get => _canInterfaceConnected;
            set {
                _canInterfaceConnected = value;
                OnPropertyChanged(nameof(CanInterfaceConnected));
            }
        }

        private bool _configFileOpened;

        private string _settingsPath;

        private System.Timers.Timer _statusBarTimer;
        private System.Timers.Timer _processQueueTimer;

        private ConcurrentQueue<CanDeviceResponse> _queue;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void DataUpdatedHandler(object sender);

        private CancellationTokenSource _cts;

        public MainViewModel()
        {
            RefreshComPorts(null);

            Cans = new ObservableCollection<ComboBoxCanInterfaces>()
            {
                new ComboBoxCanInterfaces(){ Id=1, Name="USB2CAN"},
                new ComboBoxCanInterfaces(){ Id=2, Name="PCAN"},
                new ComboBoxCanInterfaces(){ Id=3, Name="USB"}
            };

            //Settings-Last selected values
            //user.config file located in Users/AppData/Local/DingoConfigurator
            SelectedBaudRate = Settings.Default.BaudRate;
            string canName = Settings.Default.CanInterface;
            if (!String.IsNullOrEmpty(canName))
            {
                SelectedCan = Cans.First(x => x.Name == canName);
            }
            else
            {
                SelectedCan = Cans.First(x => x.Name == "USB2CAN");
            }
            string comPort = Settings.Default.ComPort;
            if(!String.IsNullOrEmpty(comPort))
            {
                SelectedComPort = ComPorts.FirstOrDefault(x => x.Equals(comPort));
            }
            _settingsPath = Settings.Default.SettingsPath;

            //Set Button Commands
            NewBtnCmd = new RelayCommand(NewConfigFile, CanNewConfigFile);
            OpenBtnCmd = new RelayCommand(OpenConfigFile, CanOpenConfigFile);
            SaveBtnCmd = new RelayCommand(SaveConfigFile, CanSaveConfigFile);
            SaveAsBtnCmd = new RelayCommand(SaveAsConfigFile, CanSaveAsConfigFile);
            ConnectBtnCmd = new RelayCommand(Connect, CanConnect);
            DisconnectBtnCmd = new RelayCommand(Disconnect, CanDisconnect);
            RefreshComPortsBtnCmd = new RelayCommand(RefreshComPorts, CanRefreshComPorts);
            UploadBtnCmd = new RelayCommand(Upload, CanUpload);
            DownloadBtnCmd = new RelayCommand(Download, CanDownload);
            BurnBtnCmd = new RelayCommand(Burn, CanBurn);

            _cts = new CancellationTokenSource();
        }

        private void NewConfigFile(object parameter)
        {
            System.Windows.Forms.SaveFileDialog newFileDialog = new System.Windows.Forms.SaveFileDialog();
            newFileDialog.Filter = "Config files (*.json)|*.json|All files (*.*)|*.*";
            newFileDialog.InitialDirectory = _settingsPath;
            if (newFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(newFileDialog.FileName).ToLower() != ".json") return;

            _settingsPath = newFileDialog.FileName;

            _config = new DevicesConfig();

            DevicesConfigHandler.Serialize(_config, _settingsPath);
        }

        private bool CanNewConfigFile(object parameter)
        {
            return true;
        }

        private void OpenConfigFile(object parameter)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Config files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = _settingsPath;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".json") return;

            _settingsPath = openFileDialog.FileName;

            _config = new DevicesConfig();

            _config = DevicesConfigHandler.Deserialize(openFileDialog.FileName);

            AddCanDevices(_config);

            _queue = new ConcurrentQueue<CanDeviceResponse>();

            // Create a timer to update status bar
            _statusBarTimer = new System.Timers.Timer(200);
            _statusBarTimer.Elapsed += UpdateStatusBar;
            _statusBarTimer.AutoReset = true;
            _statusBarTimer.Enabled = true;

            Task.Factory.StartNew(ProcessQueue, _cts.Token);

            _configFileOpened = true;
        }

        private bool CanOpenConfigFile(object parameter)
        {
            return !CanInterfaceConnected;
        }

        private void SaveConfigFile(object parameter)
        {
            int pdmNum = 0;
            int cbNum = 0;
            int dashNum = 0;
            foreach (var cd in CanDevices)
            {

                if(cd.GetType() == typeof(DingoPdmCan))
                {
                    var pdm = (DingoPdmCan) cd;

                    _config.pdm[pdmNum].name = $"PDM{pdmNum}";
                    _config.pdm[pdmNum].label = pdm.Name;

                    for(int i = 0; i< _config.pdm[pdmNum].input.Length; i++)
                    {
                        _config.pdm[pdmNum].input[i].name = $"DigitalInput{i}";
                        _config.pdm[pdmNum].input[i].label = pdm.DigitalInputs[i].Name;
                        _config.pdm[pdmNum].input[i].enabled = true;
                        _config.pdm[pdmNum].input[i].mode = 0;
                        _config.pdm[pdmNum].input[i].invertInput = pdm.DigitalInputs[i].InvertInput;
                        _config.pdm[pdmNum].input[i].debounceTime = 20;
                    }

                    for (int i = 0; i < _config.pdm[pdmNum].virtualInput.Length; i++)
                    {
                        _config.pdm[pdmNum].virtualInput[i].name = $"VirtualInput{i}";
                        _config.pdm[pdmNum].virtualInput[i].label = pdm.VirtualInputs[i].Name;
                        _config.pdm[pdmNum].virtualInput[i].enabled = pdm.VirtualInputs[i].Enabled;
                        _config.pdm[pdmNum].virtualInput[i].not0 = pdm.VirtualInputs[i].Not0;
                        _config.pdm[pdmNum].virtualInput[i].var0 = pdm.VirtualInputs[i].Var0;
                        _config.pdm[pdmNum].virtualInput[i].cond0 = pdm.VirtualInputs[i].Cond0;
                        _config.pdm[pdmNum].virtualInput[i].not1 = pdm.VirtualInputs[i].Not1;
                        _config.pdm[pdmNum].virtualInput[i].var1 = pdm.VirtualInputs[i].Var1;
                        _config.pdm[pdmNum].virtualInput[i].cond1 = pdm.VirtualInputs[i].Cond1;
                        _config.pdm[pdmNum].virtualInput[i].not2 = pdm.VirtualInputs[i].Not2;
                        _config.pdm[pdmNum].virtualInput[i].var2 = pdm.VirtualInputs[i].Var2;
                        _config.pdm[pdmNum].virtualInput[i].mode = pdm.VirtualInputs[i].Mode;
                    }

                    for (int i = 0; i < _config.pdm[pdmNum].output.Length; i++)
                    {
                        _config.pdm[pdmNum].output[i].name = $"Output{i}";
                        _config.pdm[pdmNum].output[i].label = pdm.Outputs[i].Name;
                        _config.pdm[pdmNum].output[i].enabled = true;
                        _config.pdm[pdmNum].output[i].input = 0;
                        _config.pdm[pdmNum].output[i].currentLimit = pdm.Outputs[i].CurrentLimit;
                        _config.pdm[pdmNum].output[i].inrushLimit = 0;
                        _config.pdm[pdmNum].output[i].inrushTime = 0;
                        _config.pdm[pdmNum].output[i].resetMode = 0;
                        _config.pdm[pdmNum].output[i].resetTime = 0;
                        _config.pdm[pdmNum].output[i].resetLimit = 0;
                    }

                    _config.pdm[pdmNum].wiper.enabled = pdm.Wipers[0].Enabled;
                    _config.pdm[pdmNum].wiper.lowSpeedInput = pdm.Wipers[0].SlowInput;
                    _config.pdm[pdmNum].wiper.highSpeedInput = pdm.Wipers[0].FastInput;
                    _config.pdm[pdmNum].wiper.parkInput = pdm.Wipers[0].ParkInput;
                    _config.pdm[pdmNum].wiper.parkStopLevel = pdm.Wipers[0].ParkStopLevel;
                    _config.pdm[pdmNum].wiper.washInput = pdm.Wipers[0].WashInput;
                    _config.pdm[pdmNum].wiper.washCycles = pdm.Wipers[0].WashWipeCycles;
                    _config.pdm[pdmNum].wiper.intermitInput = pdm.Wipers[0].InterInput;
                    _config.pdm[pdmNum].wiper.speedInput = pdm.Wipers[0].SpeedInput;
                    _config.pdm[pdmNum].wiper.intermitTime = pdm.Wipers[0].IntermitTime;
                    _config.pdm[pdmNum].wiper.speedMap = pdm.Wipers[0].SpeedMap;

                    for (int i = 0; i < _config.pdm[pdmNum].flasher.Length; i++)
                    {
                        _config.pdm[pdmNum].flasher[i].name = $"Flasher{i}";
                        _config.pdm[pdmNum].flasher[i].label = pdm.Flashers[i].Name;
                        _config.pdm[pdmNum].flasher[i].enabled = pdm.Flashers[i].Enabled;
                        _config.pdm[pdmNum].flasher[i].input = pdm.Flashers[i].Input;
                        _config.pdm[pdmNum].flasher[i].flashOnTime = pdm.Flashers[i].OnTime;
                        _config.pdm[pdmNum].flasher[i].flashOffTime = pdm.Flashers[i].OffTime;
                        _config.pdm[pdmNum].flasher[i].singleCycle = Convert.ToInt16(pdm.Flashers[i].Single);
                        _config.pdm[pdmNum].flasher[i].output = pdm.Flashers[i].Output;
                    }

                    _config.pdm[pdmNum].starter.enabled = pdm.StarterDisable[0].Enabled;
                    _config.pdm[pdmNum].starter.input = pdm.StarterDisable[0].Input;
                    _config.pdm[pdmNum].starter.disableOut[0] = pdm.StarterDisable[0].Output1;
                    _config.pdm[pdmNum].starter.disableOut[1] = pdm.StarterDisable[0].Output2;
                    _config.pdm[pdmNum].starter.disableOut[2] = pdm.StarterDisable[0].Output3;
                    _config.pdm[pdmNum].starter.disableOut[3] = pdm.StarterDisable[0].Output4;
                    _config.pdm[pdmNum].starter.disableOut[4] = pdm.StarterDisable[0].Output5;
                    _config.pdm[pdmNum].starter.disableOut[5] = pdm.StarterDisable[0].Output6;
                    _config.pdm[pdmNum].starter.disableOut[6] = pdm.StarterDisable[0].Output7;
                    _config.pdm[pdmNum].starter.disableOut[7] = pdm.StarterDisable[0].Output8;

                    for (int i = 0; i < _config.pdm[pdmNum].canInput.Length; i++)
                    {
                        _config.pdm[pdmNum].canInput[i].name = $"CanInput{i}";
                        _config.pdm[pdmNum].canInput[i].label = pdm.CanInputs[i].Name;
                        _config.pdm[pdmNum].canInput[i].enabled = pdm.CanInputs[i].Enabled;
                        _config.pdm[pdmNum].canInput[i].id = pdm.CanInputs[i].Id;
                        _config.pdm[pdmNum].canInput[i].lowByte = pdm.CanInputs[i].LowByte;
                        _config.pdm[pdmNum].canInput[i].highByte = pdm.CanInputs[i].HighByte;
                        _config.pdm[pdmNum].canInput[i].oper = pdm.CanInputs[i].Operator;
                        _config.pdm[pdmNum].canInput[i].onValue = pdm.CanInputs[i].OnVal;
                        _config.pdm[pdmNum].canInput[i].mode = pdm.CanInputs[i].Mode;
                    }

                    _config.pdm[pdmNum].canOutput.enable = true;
                    _config.pdm[pdmNum].canOutput.baseId = pdm.BaseId;
                    _config.pdm[pdmNum].canOutput.updateTime = 0;

                    pdmNum++;
                }
            }
            DevicesConfigHandler.Serialize(_config, _settingsPath);
        }

        private bool CanSaveConfigFile(object parameter)
        {
            return _configFileOpened;
        }

        private void SaveAsConfigFile(object parameter)
        {

        }

        private bool CanSaveAsConfigFile(object parameter)
        {
            return _configFileOpened;
        }

        private void AddCanDevices(DevicesConfig config)
        {
            _canDevices?.Clear();
            
            _canDevices = new ObservableCollection<ICanDevice>();

            foreach (var pdm in _config.pdm)
            {
                var newPdm = new DingoPdmCan(pdm.label, pdm.canOutput.baseId);
                newPdm.SetVars(pdm);
                _canDevices.Add(newPdm);
            }

            foreach (var cb in _config.canBoard)
            {
                var newCb = new CanBoardCan(cb.label, cb.baseCanId);
                _canDevices.Add(newCb);
            }

            foreach (var dash in _config.dash)
            {
                var newDash = new DingoDashCan(dash.label, dash.baseCanId);
                _canDevices.Add(newDash);
            }

            OnPropertyChanged(nameof(CanDevices));
        }

        public void WindowClosing()
        {
            _cts.Cancel();

            Disconnect(null);

            if (_statusBarTimer != null)
            {
                _statusBarTimer.Stop();
                _statusBarTimer.Dispose();
            }

            //Save user settings
            Settings.Default.CanInterface = SelectedCan.Name;
            Settings.Default.BaudRate = SelectedBaudRate;
            Settings.Default.ComPort = SelectedComPort;
            Settings.Default.SettingsPath = Path.GetDirectoryName(_settingsPath);
            Settings.Default.Save();

            NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
        }

        private void CanDataReceived(object sender, CanDataEventArgs e)
        {   
            foreach (var cd in _canDevices)
            {
                if (cd.InIdRange(e.canData.Id))
                {
                    cd.Read(e.canData.Id, e.canData.Payload, ref _queue);
                }
            }
        }

        private ViewModelBase _currentViewModel { get; set; }
        public ViewModelBase CurrentViewModel {
            get => _currentViewModel;
            set { 
                _currentViewModel?.Dispose();
                _currentViewModel= value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICanDevice SelectedCanDevice { get; set; }

        private void GetDeviceSettings(bool getAll)
        {
            _countSinceLast = 0;
            foreach (var cd in _canDevices)
            {
                if ((!getAll && SelectedCanDevice.Equals(cd)) ||
                    getAll)
                {
                    if (cd.IsConnected)
                    {
                        var msgs = cd.GetUploadMessages();
                        if (msgs == null) return;

                        foreach (var msg in msgs)
                        {
                            msg.DeviceBaseId = cd.BaseId;
                            _queue.Enqueue(msg);
                        }
                        msgs.Clear();
                    }
                }
            }
        }

        private void SendDeviceSettings(bool sendAll)
        {
            _countSinceLast = 0;
            foreach (var cd in _canDevices)
            {
                if ((!sendAll && SelectedCanDevice.Equals(cd)) ||
                        sendAll)
                {
                    if (cd.IsConnected)
                    {
                        var msgs = cd.GetDownloadMessages();
                        if (msgs == null) return;

                        foreach (var msg in msgs)
                        {
                            msg.DeviceBaseId = cd.BaseId;
                            _queue.Enqueue(msg);
                        }
                        msgs.Clear();
                    }
                }
            }
        }

        private void BurnDeviceSettings(bool burnAll)
        {
            _countSinceLast = 0;
            foreach (var cd in _canDevices)
            {
                if ((!burnAll && SelectedCanDevice.Equals(cd)) ||
                        burnAll)
                {
                    if (cd.IsConnected)
                    {
                        var msg = cd.GetBurnMessage();
                        if (msg == null) return;

                        msg.DeviceBaseId = cd.BaseId;
                        _queue.Enqueue(msg);
                    }
                }
            }
        }

        private void ProcessQueue(object taskState)
        {
            var token = (CancellationToken)taskState;
            CanDeviceResponse msg;

            while (!token.IsCancellationRequested)
            {
                if(_queue.TryDequeue(out msg))
                {
                    //Send message
                    if (!msg.Sent && CanInterfaceConnected)
                    {
                        _can.Write(msg.Data);
                        msg.TimeSent = DateTime.Now;
                        msg.Sent = true;
                        _queue.Enqueue(msg); //Sent, but not received
                        Thread.Sleep(10); //Wait a bit to ensure the device can respond
                    }

                    //Response received
                    if (msg.Sent && msg.Received)
                    {
                        _countSinceLast++;
                        //Received, don't add back to queue
                        //Logger.Info("Received {0} {1} {2} {3} {4} {5} {6} {7} {8}", msg.Data.Payload[0], msg.Data.Payload[1],
                        //Console.WriteLine("Received {0} {1} {2} {3} {4} {5} {6} {7} {8}", msg.Data.Payload[0], msg.Data.Payload[1],
                        //    msg.Data.Payload[2], msg.Data.Payload[3], msg.Data.Payload[4],
                        //    msg.Data.Payload[5], msg.Data.Payload[6], msg.Data.Payload[7], _countSinceLast);
                        Console.WriteLine(_countSinceLast);
                    }

                    TimeSpan timeSpan = DateTime.Now - msg.TimeSent;

                    if (msg.Sent && !msg.Received && (timeSpan.TotalSeconds > 2))
                    {
                        Logger.Info("No response {0} {1} {2} {3} {4} {5} {6} {7} {8}", msg.Data.Payload[0], msg.Data.Payload[1],
                            msg.Data.Payload[2], msg.Data.Payload[3], msg.Data.Payload[4],
                            msg.Data.Payload[5], msg.Data.Payload[6], msg.Data.Payload[7], _countSinceLast);
                        msg.Sent = false;
                        _queue.Enqueue(msg); //Sent, but not received. Resend
                    }
                }

                
            }
        }

        #region TreeView
        private ObservableCollection<ICanDevice> _canDevices;
        public ObservableCollection<ICanDevice> CanDevices
        {
            get => _canDevices;
            set
            {
                _canDevices = value;
                OnPropertyChanged(nameof(CanDevices));
            }
        }

        internal void Cans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null) return;

            CanComPorts = !SelectedCan.Name.Equals("PCAN");

            CanBaudRates = !SelectedCan.Name.Equals("USB");
        }

        internal void TreeView_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) return;

            if(e.NewValue.GetType() == typeof(DingoPdmCan))
            {
                SelectedCanDevice = (DingoPdmCan)e.NewValue;
                CurrentViewModel= new DingoPdmViewModel(this);
            }

            if(e.NewValue.GetType() == typeof(CanDeviceSub))
            {
                CanDeviceSub sub = (CanDeviceSub)e.NewValue;

                if(sub.CanDevice.GetType() == typeof(DingoPdmCan))
                {
                    if (sub.Name.Equals("States"))
                    {
                        SelectedCanDevice = (DingoPdmCan)sub.CanDevice;
                        CurrentViewModel = new DingoPdmStatesViewModel(this);
                    }

                    if (sub.Name.Equals("Settings"))
                    {
                        SelectedCanDevice = (DingoPdmCan)sub.CanDevice;
                        CurrentViewModel = new DingoPdmSettingsViewModel(this);
                    }
                }

            }

            if (e.NewValue.GetType() == typeof(CanBoardCan))
            {
                SelectedCanDevice = (CanBoardCan)e.NewValue;
                CurrentViewModel = new CanBoardViewModel(this);
            }

            if (e.NewValue.GetType() == typeof(DingoDashCan))
            {
                SelectedCanDevice = (DingoDashCan)e.NewValue;
                CurrentViewModel = new DingoDashViewModel(this);
            }
        }
        #endregion

        #region Commands
        private void Connect(object parameter)
        {
            string port = " ";
            CanInterfaceBaudRate baud = CanInterfaceBaudRate.BAUD_500K;

            switch (SelectedCan.Name)
            {
                case "USB2CAN":
                    _can = new CanInterfaces.USB2CAN();
                    port = SelectedComPort;
                    baud = SelectedBaudRate;
                    break;

                case "PCAN":
                    _can = new CanInterfaces.PCAN();
                    port = "USBBUS1";
                    baud = SelectedBaudRate;
                    break;

                case "USB":
                    _can = new CanInterfaces.USB();
                    port = SelectedComPort;
                    baud = SelectedBaudRate;//Not used
                    break;
            }

            if(!_can.Init(port, baud)) return;
            _can.DataReceived += CanDataReceived;
            if(!_can.Start()) return;
            CanInterfaceConnected = true;
            Thread.Sleep(100); //Wait for devices to connect
            GetDeviceSettings(true);
        }

        
        private bool CanConnect(object parameter)
        {
            return !CanInterfaceConnected && 
                    _configFileOpened &&
                    ((CanComPorts && (SelectedComPort != null)) ||
                    !CanComPorts);
        }

        private void Disconnect(object parameter)
        {
            if(_can != null) _can.Stop();
            CanInterfaceConnected = false;
        }

        private bool CanDisconnect(object parameter)
        {
            return CanInterfaceConnected;
        }

        private void RefreshComPorts(object parameter)
        {
            if(ComPorts != null) ComPorts.Clear();
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
        }

        private bool CanRefreshComPorts(object parameter)
        {
            return CanComPorts;
        }

        private void Upload(object parameter)
        {
            GetDeviceSettings(false);
        }

        private bool CanUpload(object parameter)
        {
            return CanInterfaceConnected && (SelectedCanDevice != null);
        }

        private void Download(object parameter)
        {
            SendDeviceSettings(false);
        }

        private bool CanDownload(object parameter)
        {
            return CanInterfaceConnected && (SelectedCanDevice != null);
        }

        private void Burn(object parameter)
        {
            BurnDeviceSettings(false);
        }

        private bool CanBurn(object parameter)
        {
            return CanInterfaceConnected && (SelectedCanDevice != null);
        }
        #endregion

        #region Combobox
        private ObservableCollection<ComboBoxCanInterfaces> _cans;
        public ObservableCollection<ComboBoxCanInterfaces> Cans
        {
            get { return _cans; }
            set { _cans = value; }
        }
        private ComboBoxCanInterfaces _selectedCan;
        public ComboBoxCanInterfaces SelectedCan
        {
            get { return _selectedCan; }
            set { _selectedCan = value; }
        }

        private ObservableCollection<string> _comPorts;
        public ObservableCollection<string> ComPorts
        {
            get { return _comPorts; }
            set 
            { 
                _comPorts = value; 
                OnPropertyChanged(nameof(ComPorts));
            }
        }

        private string _selectedComPort;
        public string SelectedComPort
        {
            get { return _selectedComPort; }
            set { _selectedComPort = value; }
        }

        private bool _canComPorts;
        public bool CanComPorts
        {
            get { return _canComPorts; }
            set
            {
                _canComPorts = value;
                OnPropertyChanged(nameof(CanComPorts));
            }
        }

        private CanInterfaceBaudRate _selectedBaudRate;
        public CanInterfaceBaudRate SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set { 
                _selectedBaudRate = value;
                OnPropertyChanged(nameof(SelectedBaudRate));
            }
        }

        public IEnumerable<CanInterfaceBaudRate> BaudRates
        {
            get
            {
                return (IEnumerable<CanInterfaceBaudRate>)System.Enum.GetValues(typeof(CanInterfaceBaudRate));
            }
        }

        private bool _canBaudRates;
        public bool CanBaudRates
        {
            get => _canBaudRates;
            set
            {
                _canBaudRates = value;
                OnPropertyChanged(nameof(CanBaudRates));
            }
        }
        #endregion

        #region Buttons
        public ICommand NewBtnCmd { get; set; }
        public ICommand OpenBtnCmd { get; set; }
        public ICommand SaveBtnCmd { get; set; }
        public ICommand SaveAsBtnCmd { get; set; }
        public ICommand ConnectBtnCmd { get; set; }
        public ICommand DisconnectBtnCmd { get; set; }
        public ICommand RefreshComPortsBtnCmd { get; set; }
        public ICommand UploadBtnCmd { get; set; }
        public ICommand DownloadBtnCmd { get; set; }
        public ICommand BurnBtnCmd { get; set; }
        #endregion

        #region StatusBar
        private void UpdateStatusBar(object sender, ElapsedEventArgs e)
        {
            CanInterfaceStatusText = $"{SelectedCan.Name} {(CanInterfaceConnected ? "Connected" : "Disconnected")}";

            int connectedCount = 0;

            if (_canDevices == null) return;

            foreach (var cd in _canDevices)
            {
                cd.UpdateIsConnected();
                if (cd.IsConnected) connectedCount++;
            }

            QueueCount = _queue.Count;

            DeviceCountText = $"Detected Devices: {connectedCount}";
        }

        private Brush _canInterfaceStatusFill;
        public Brush CanInterfaceStatusFill
        {
            get => _canInterfaceStatusFill;
            set
            {
                _canInterfaceStatusFill = value;
                OnPropertyChanged(nameof(CanInterfaceStatusFill));
            }
        }

        private string _canInterfaceStatusText;
        public string CanInterfaceStatusText
        {
            get => _canInterfaceStatusText;
            set
            {
                _canInterfaceStatusText = value;
                OnPropertyChanged(nameof(CanInterfaceStatusText));
            }
        }

        private string _deviceCountText;
        public string DeviceCountText
        {
            get => _deviceCountText;
            set
            {
                _deviceCountText = value;
                OnPropertyChanged(nameof(DeviceCountText));
            }
        }

        private int _queueCount;
        public int QueueCount
        {
            get => _queueCount;
            set
            {
                _queueCount = value;
                OnPropertyChanged(nameof(QueueCount));
            }
        }
        #endregion

    }

    #region ComboBox Types
    public class ComboBoxCanInterfaces
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
    #endregion
}
