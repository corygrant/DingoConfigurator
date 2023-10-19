using CanDevices;
using CanDevices.DingoPdm;
using CanDevices.CanBoard;
using CanDevices.DingoDash;
using CanInterfaces;
using DingoConfigurator.Config;
using DingoConfigurator.Properties;
using DingoConfigurator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Diagnostics;
using CommsHandler;
using CanDevices.CanMsgLog;

//Add another CanDevices list that holds the online value

namespace DingoConfigurator
{
    public class MainViewModel : ViewModelBase
    {
        private CanCommsHandler _canComms;
        public CanCommsHandler CanComms { get => _canComms; }

        private DevicesConfigHandler _configHandler;

        private string _settingsPath;

        private System.Timers.Timer _statusBarTimer;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainViewModel()
        {
            _canComms = new CanCommsHandler();

            _configHandler = new DevicesConfigHandler();

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
            AddDeviceBtnCmd = new RelayCommand(AddDevice, CanAddDevice);
            UpdateDeviceBtnCmd = new RelayCommand(UpdateDevice, CanUpdateDevice);
            RemoveDeviceBtnCmd = new RelayCommand(RemoveDevice, CanRemoveDevice);

            DeviceName = String.Empty;

            CanCans = true;
        }

        

        //Add devices from config file to comms handler
        private void AddCanDevicesToTree(DevicesConfig config)
        {
            _canComms.ResetCanDevices();

            foreach (var pdm in _configHandler.Config.pdm)
            {
                if (pdm != null)
                {
                    DingoPdmCan newPdm = (DingoPdmCan)_canComms.AddCanDevice(typeof(DingoPdmCan), pdm.label, pdm.canOutput.baseId);
                    PdmConfigHandler.ApplyConfig(ref newPdm, pdm); //Apply the config settings to the new device
                }
            }

            foreach (var cb in _configHandler.Config.canBoard)
            {
                if (cb != null)
                {
                    CanBoardCan newCb = (CanBoardCan)_canComms.AddCanDevice(typeof(CanBoardCan), cb.label, cb.baseCanId);
                    CanBoardConfigHandler.ApplyConfig(ref newCb, cb); //Apply the config settings to the new device
                }
            }

            foreach (var dash in _configHandler.Config.dash)
            {
                if (dash != null)
                {
                    DingoDashCan newDash = (DingoDashCan)_canComms.AddCanDevice(typeof(DingoDashCan), dash.label, dash.baseCanId);
                    DingoDashConfigHandler.ApplyConfig(ref newDash, dash); //Apply the config settings to the new device
                }
            }
        }

        public void WindowClosing()
        {
            _canComms.Disconnect();

            if (_statusBarTimer != null)
            {
                _statusBarTimer.Stop();
                _statusBarTimer.Dispose();
            }

            //Save user settings
            Settings.Default.CanInterface = SelectedCan.Name;
            Settings.Default.BaudRate = SelectedBaudRate;
            Settings.Default.ComPort = SelectedComPort;
            if (_settingsPath.Length > 0)
            {
                Settings.Default.SettingsPath = Path.GetDirectoryName(_settingsPath);
            }
            Settings.Default.Save();

            NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
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

        private bool _connected;
        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                OnPropertyChanged(nameof(Connected));
            }
        }

        private int _deviceBaseId;
        public int DeviceBaseId
        {
            get => _deviceBaseId;
            set
            {
                _deviceBaseId = value;
                OnPropertyChanged(nameof(DeviceBaseId));
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }

        public ICanDevice SelectedCanDevice { get; set; }

        #region TreeView
        

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
                SelectedDeviceToAdd = Devices.DingoPDM;
            }

            if(e.NewValue.GetType() == typeof(CanDeviceSub))
            {
                CanDeviceSub sub = (CanDeviceSub)e.NewValue;

                if(sub.CanDevice.GetType() == typeof(DingoPdmCan))
                {
                    if (sub.Name.Equals("Settings"))
                    {
                        SelectedCanDevice = (DingoPdmCan)sub.CanDevice;
                        CurrentViewModel = new DingoPdmSettingsViewModel(this);
                    }

                    SelectedDeviceToAdd = Devices.DingoPDM;
                }

            }

            if (e.NewValue.GetType() == typeof(CanBoardCan))
            {
                SelectedCanDevice = (CanBoardCan)e.NewValue;
                CurrentViewModel = new CanBoardViewModel(this);
                SelectedDeviceToAdd = Devices.CANBoard;
            }

            if (e.NewValue.GetType() == typeof(DingoDashCan))
            {
                SelectedCanDevice = (DingoDashCan)e.NewValue;
                CurrentViewModel = new DingoDashViewModel(this);
                SelectedDeviceToAdd = Devices.DingoDash;
            }

            if (e.NewValue.GetType() == typeof(CanMsgLog))
            {
                SelectedCanDevice =(CanMsgLog)e.NewValue;
                CurrentViewModel = new CanMsgLogViewModel(this);
                SelectedDeviceToAdd = Devices.CanMsgLog;
            }

            if (SelectedCanDevice != null)
            {
                DeviceName = SelectedCanDevice.Name;
                DeviceBaseId = SelectedCanDevice.BaseId;
            }
            else
            {
                DeviceName = String.Empty;
                DeviceBaseId = 0;
            }
        }
        #endregion

        #region Commands
        private void Connect(object parameter)
        {
            _canComms.Connect(SelectedCan.Name, SelectedComPort, SelectedBaudRate);

            CanCans = false;
            CanComPorts = false;
            CanBaudRates = false;

            // Create a timer to update status bar
            _statusBarTimer = new System.Timers.Timer(200);
            _statusBarTimer.Elapsed += UpdateStatusBar;
            _statusBarTimer.AutoReset = true;
            _statusBarTimer.Enabled = true;
        }

        
        private bool CanConnect(object parameter)
        {
            return !_canComms.Connected && 
                    (_canComms.CanDevices.Count > 0) &&
                    ((CanComPorts && (SelectedComPort != null)) ||
                    !CanComPorts);
        }

        private void Disconnect(object parameter)
        {
            _canComms.Disconnect();

            CanCans = true;
            CanComPorts = !SelectedCan.Name.Equals("PCAN");
            CanBaudRates = !SelectedCan.Name.Equals("USB");
        }

        private bool CanDisconnect(object parameter)
        {
            return _canComms.Connected;
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
            _canComms.Upload(SelectedCanDevice);
        }

        private bool CanUpload(object parameter)
        {
            return _canComms.Connected &&
                (SelectedCanDevice != null) &&
                (SelectedCanDevice.IsConnected) &&
                (SelectedCanDevice.GetType() != typeof(CanMsgLog));
        }

        private void Download(object parameter)
        {
            _canComms.Download(SelectedCanDevice);
        }

        private bool CanDownload(object parameter)
        {
            return _canComms.Connected && 
                (SelectedCanDevice != null) && 
                (SelectedCanDevice.IsConnected) &&
                (SelectedCanDevice.GetType() != typeof(CanMsgLog));
        }

        private void Burn(object parameter)
        {
            _canComms.Burn(SelectedCanDevice);
        }

        private bool CanBurn(object parameter)
        {
            return _canComms.Connected &&
                (SelectedCanDevice != null) &&
                (SelectedCanDevice.IsConnected) &&
                (SelectedCanDevice.GetType() != typeof(CanMsgLog));
        }

        private void NewConfigFile(object parameter)
        {
            //Clear devices
            _canComms.Disconnect();
            _canComms.CanDevices.Clear();
            _configHandler.Clear();
            CurrentViewModel = null;
            ConfigFileName = String.Empty;
        }

        private bool CanNewConfigFile(object parameter)
        {
            return (_canComms.CanDevices.Count > 0);
        }

        private void OpenConfigFile(object parameter)
        {
            _canComms.Disconnect();
            _canComms.CanDevices.Clear();
            CurrentViewModel = null;

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Config files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = _settingsPath;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".json") return;

            _settingsPath = openFileDialog.FileName;
            ConfigFileName = Path.GetFileName(_settingsPath);

            _configHandler = new DevicesConfigHandler();

            _configHandler.OpenFile(openFileDialog.FileName);

            AddCanDevicesToTree(_configHandler.Config);

        }

        private bool CanOpenConfigFile(object parameter)
        {
            return !_canComms.Connected;
        }

        private void SaveConfigFile(object parameter)
        {
            if (_configHandler.CheckDeviceCount(_canComms.CanDevices.ToArray()))
            {
                //Config file matches tree device count
                //Save existing device config
                _configHandler.UpdateSaveFile(_settingsPath, _canComms.CanDevices.ToArray());
                return;
            }

            //Config file doesn't match tree device count
            //Create a new DevicesConfigHandler and save
            _configHandler = new DevicesConfigHandler();
            _configHandler.NewSaveFile(_settingsPath, _canComms.CanDevices.ToArray());
        }

        private bool CanSaveConfigFile(object parameter)
        {
            return _configHandler.Opened;
        }

        private void SaveAsConfigFile(object parameter)
        {
            System.Windows.Forms.SaveFileDialog saveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveAsFileDialog.Filter = "Config files (*.json)|*.json|All files (*.*)|*.*";
            saveAsFileDialog.InitialDirectory = _settingsPath;
            if (saveAsFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(saveAsFileDialog.FileName).ToLower() != ".json") return;

            _settingsPath = saveAsFileDialog.FileName;
            ConfigFileName = Path.GetFileNameWithoutExtension(_settingsPath);

            if (_configHandler.Opened && _configHandler.CheckDeviceCount(_canComms.CanDevices.ToArray()))
            {
                //Config already exists, just save in the new location
                _configHandler.UpdateSaveFile(_settingsPath, _canComms.CanDevices.ToArray());
                return;
            }
  
            //New configuration or config file doesn't match tree device count
            //Create a new DevicesConfigHandler and save
            _configHandler = new DevicesConfigHandler();
            _configHandler.NewSaveFile(_settingsPath, _canComms.CanDevices.ToArray());
        }

        private bool CanSaveAsConfigFile(object parameter)
        {
            return (_canComms.CanDevices.Count > 0);
        }

        private void AddDevice(object parameter)
        {
            if (SelectedDeviceToAdd.Equals(Devices.CanMsgLog))
            {
                _canComms.AddCanDevice(typeof(CanMsgLog), "CAN Msg Log", 1);
            }

            if (DeviceBaseId < 1) return;
            if (DeviceBaseId > 2048) return;
            if (DeviceName.Length == 0) return;

            if (SelectedDeviceToAdd.Equals(Devices.DingoPDM))
            {
                _canComms.AddCanDevice(typeof(DingoPdmCan), DeviceName, DeviceBaseId);
            }

            if (SelectedDeviceToAdd.Equals(Devices.CANBoard))
            {
                _canComms.AddCanDevice(typeof(CanBoardCan), DeviceName, DeviceBaseId);
            }

            if (SelectedDeviceToAdd.Equals(Devices.DingoDash))
            {
                _canComms.AddCanDevice(typeof(DingoDashCan), DeviceName, DeviceBaseId);
            }
        }

        private bool CanAddDevice(object parameter)
        {
            if (SelectedDeviceToAdd.Equals(Devices.CanMsgLog))
            {
                var match = _canComms.CanDevices.FirstOrDefault<ICanDevice>(d => d.GetType() == typeof(CanMsgLog));

                return (match == null);
            }

            return (DeviceName.Length > 0) && (DeviceBaseId > 0) && (DeviceBaseId <= 2048);
        }

        private void UpdateDevice(object parameter)
        {
            if (SelectedCanDevice == null) return;
            if (SelectedCanDevice.GetType() == typeof(CanMsgLog)) return;

            SelectedCanDevice.Name = DeviceName;
            if (SelectedCanDevice.BaseId != DeviceBaseId)
            {
                _canComms.Update(SelectedCanDevice, DeviceBaseId);
            }
        }

        private bool CanUpdateDevice(object parameter)
        {
            if (SelectedCanDevice == null) return false;

            if (SelectedCanDevice.GetType() == typeof(CanMsgLog)) return false;

            return (DeviceName.Length > 0) && (DeviceBaseId > 0) && (DeviceBaseId <= 2048);
        }

        private void RemoveDevice(object parameter)
        {
            _canComms.RemoveCanDevice(SelectedCanDevice);
            CurrentViewModel = null;
        }

        private bool CanRemoveDevice(object parameter)
        {
            return SelectedCanDevice != null;
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

        private bool _canCans;
        public bool CanCans
        {
            get => _canCans;
            set
            {
                _canCans = value;
                OnPropertyChanged(nameof(CanCans));
            }
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

        public IEnumerable<Devices> DeviceToAdd
        {
            get
            {
                return (IEnumerable<Devices>)System.Enum.GetValues(typeof(Devices));
            }
        }

        private Devices _selectedDeviceToAdd;
        public Devices SelectedDeviceToAdd
        {
            get => _selectedDeviceToAdd;
            set
            {
                _selectedDeviceToAdd = value;
                OnPropertyChanged(nameof(SelectedDeviceToAdd));
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
        public ICommand AddDeviceBtnCmd { get; set; }
        public ICommand UpdateDeviceBtnCmd { get; set; }
        public ICommand RemoveDeviceBtnCmd { get; set; }
        #endregion

        #region StatusBar
        private void UpdateStatusBar(object sender, ElapsedEventArgs e)
        {
            CanInterfaceStatusText = $"{SelectedCan.Name} {(_canComms.Connected ? "Connected" : "Disconnected")}";

            int connectedCount = 0;

            if (_canComms.CanDevices == null) return;

            foreach (var cd in _canComms.CanDevices)
            {
                cd.UpdateIsConnected();

                //CanMsgLog is not a real device to be detected 
                if (cd.GetType().Equals(typeof(CanMsgLog))) continue;

                if (cd.IsConnected) connectedCount++;
            }

            QueueCount = _canComms.QueueCount;

            RxTimeDelta = _canComms.RxTimeDelta;

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

        private string _configFileName;
        public string ConfigFileName
        {
            get => _configFileName;
            set
            {
                _configFileName = value;
                OnPropertyChanged(nameof(ConfigFileName));
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

        private int _rxTimeDelta;
        public int RxTimeDelta
        {
            get => _rxTimeDelta;
            set
            {
                _rxTimeDelta = value;
                OnPropertyChanged(nameof(RxTimeDelta));
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

    public enum Devices
    {
        DingoPDM,
        CANBoard,
        DingoDash,
        CanMsgLog
    }
    #endregion
}
