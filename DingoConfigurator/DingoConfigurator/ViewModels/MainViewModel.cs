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
using CanDevices.SoftButtonBox;
using Microsoft.Win32;
using System.Management;
using System.Text.RegularExpressions;
using System.Reflection;

//Add another CanDevices list that holds the online value

namespace DingoConfigurator
{
    public class MainViewModel : ViewModelBase
    {
        private CanCommsHandler _canComms;
        public CanCommsHandler CanComms { get => _canComms; }

        private string _settingsPath;

        private System.Timers.Timer _statusBarTimer;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();


            _canComms = new CanCommsHandler();
            _canComms.PropertyChanged += _canComms_PropertyChanged  ;


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
            NewBtnConfirmCmd = new RelayCommand(NewConfigFileConfirm, CanNewConfigFile);
            OpenBtnCmd = new RelayCommand(OpenConfigFile, CanOpenConfigFile);
            SaveBtnCmd = new RelayCommand(SaveConfigFile, CanSaveConfigFile);
            SaveAsBtnCmd = new RelayCommand(SaveAsConfigFile, CanSaveAsConfigFile);
            ConnectBtnCmd = new RelayCommand(Connect, CanConnect);
            DisconnectBtnCmd = new RelayCommand(Disconnect, CanDisconnect);
            RefreshComPortsBtnCmd = new RelayCommand(RefreshComPorts, CanRefreshComPorts);
            ReadBtnCmd = new RelayCommand(Read, CanRead);
            ReadBtnConfirmCmd = new RelayCommand(ReadConfirm, CanRead);
            WriteBtnCmd = new RelayCommand(Write, CanWrite);
            BurnBtnCmd = new RelayCommand(Burn, CanBurn);
            SleepBtnCmd = new RelayCommand(Sleep, CanSleep);
            AddDeviceBtnCmd = new RelayCommand(AddDevice, CanAddDevice);
            UpdateDeviceBtnCmd = new RelayCommand(UpdateDevice, CanUpdateDevice);
            RemoveDeviceBtnCmd = new RelayCommand(RemoveDevice, CanRemoveDevice);
            CancelConfirmCmd = new RelayCommand(CancelConfirm, CanCancelConfirm);

            DeviceName = String.Empty;

            DeviceCountText = "Detected Devices: 0";

            CanCans = true;

        }

        private void _canComms_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Connected")
            {
                if(!_canComms.Connected)
                {
                    CanCans = true;
                    CanComPorts = !SelectedCan.Name.Equals("PCAN");
                    CanBaudRates = !SelectedCan.Name.Equals("USB");

                    CommandManager.InvalidateRequerySuggested();
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

        private string _version;
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
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
                SelectedDeviceToAdd = Devices.dingoPDM;
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

                    SelectedDeviceToAdd = Devices.dingoPDM;
                }

            }

            if (e.NewValue.GetType() == typeof(DingoPdmPlot))
            {
                DingoPdmPlot plot = (DingoPdmPlot)e.NewValue;

                if (plot.CanDevice.GetType() == typeof(DingoPdmCan))
                {
                    if (plot.Name.Equals("Plots"))
                    {
                        SelectedCanDevice = (DingoPdmCan)plot.CanDevice;
                        CurrentViewModel = new DingoPdmPlotsViewModel(this);
                    }

                    SelectedDeviceToAdd = Devices.dingoPDM;
                }

            }

            if (e.NewValue.GetType() == typeof(CanBoardCan))
            {
                SelectedCanDevice = (CanBoardCan)e.NewValue;
                CurrentViewModel = new CanBoardViewModel(this);
                SelectedDeviceToAdd = Devices.CANBoard;
            }

            /*
            if (e.NewValue.GetType() == typeof(DingoDashCan))
            {
                SelectedCanDevice = (DingoDashCan)e.NewValue;
                CurrentViewModel = new DingoDashViewModel(this);
                SelectedDeviceToAdd = Devices.DingoDash;
            }
            */

            if (e.NewValue.GetType() == typeof(CanMsgLog))
            {
                SelectedCanDevice =(CanMsgLog)e.NewValue;
                CurrentViewModel = new CanMsgLogViewModel(this);
                SelectedDeviceToAdd = Devices.CanMsgLog;
            }

            if (e.NewValue.GetType() == typeof(SoftButtonBox))
            {
                SelectedCanDevice = (SoftButtonBox)e.NewValue;
                CurrentViewModel = new SoftButtonBoxViewModel(this);
                SelectedDeviceToAdd = Devices.SoftButtonBox;
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
            if (SelectedCan.Name.Equals("PCAN"))
            {
                _canComms.Connect(SelectedCan.Name, string.Empty, SelectedBaudRate);
            }
            else
            {
                _canComms.Connect(SelectedCan.Name, ExtractComPort(SelectedComPort), SelectedBaudRate);
            }

            foreach(var cd in _canComms.CanDevices)
            {
                _canComms.Wakeup(cd);
            }
            
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
            if (ComPorts != null) ComPorts.Clear();

            var detailedPortNames = new ObservableCollection<string>();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0"))
            {
                foreach (var device in searcher.Get())
                {
                    string caption = device["Caption"]?.ToString();
                    string deviceId = device["DeviceID"]?.ToString();

                    if (caption != null && caption.Contains("(COM"))
                    {
                        detailedPortNames.Add(caption);
                    }
                }
            }

            ComPorts = detailedPortNames;
        }

        private bool CanRefreshComPorts(object parameter)
        {
            return CanComPorts;
        }

        private string ExtractComPort(string fullName)
        {
            var match = Regex.Match(fullName, @"\((COM\d+)\)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private void Read(object parameter)
        {
            IsReadConfirmVisible = true;
        }

        private bool CanRead(object parameter)
        {
            return _canComms.Connected &&
                (SelectedCanDevice != null) &&
                (SelectedCanDevice.IsConnected) &&
                (SelectedCanDevice.GetType() == typeof(DingoPdmCan));
        }

        private void ReadConfirm(object parameter)
        {
            IsReadConfirmVisible = false;
            _canComms.Read(SelectedCanDevice);
        }

        private bool _isReadConfirmVisible;
        public bool IsReadConfirmVisible
        {
            get => _isReadConfirmVisible;
            set
            {
                _isReadConfirmVisible = value;
                OnPropertyChanged(nameof(IsReadConfirmVisible));
            }
        }

        private void Write(object parameter)
        {
            _canComms.Write(SelectedCanDevice);
        }

        private bool CanWrite(object parameter)
        {
            return _canComms.Connected && 
                (SelectedCanDevice != null) && 
                (SelectedCanDevice.IsConnected) &&
                (SelectedCanDevice.GetType() == typeof(DingoPdmCan));
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
                (SelectedCanDevice.GetType() == typeof(DingoPdmCan));
        }

        private void Sleep(object parameter)
        {
            if(SelectedCanDevice.IsConnected)
                _canComms.Sleep(SelectedCanDevice);
            else
                _canComms.Wakeup(SelectedCanDevice);

        }

        private bool CanSleep(object parameter)
        {
            return _canComms.Connected &&
                (SelectedCan.Name != "USB") &&
                (SelectedCanDevice != null) &&
                (SelectedCanDevice.GetType() == typeof(DingoPdmCan));
        }

        private bool ConfigFileOpen { get; set; }

        private void NewConfigFile(object parameter)
        {
            IsConfirmNewConfigFileVisible = true;
        }

        private bool CanNewConfigFile(object parameter)
        {
            return (_canComms.CanDevices.Count > 0);
        }

        private bool _isConfirmNewConfigFileVisible;
        public bool IsConfirmNewConfigFileVisible
        {
            get => _isConfirmNewConfigFileVisible;
            set
            {
                _isConfirmNewConfigFileVisible = value;
                OnPropertyChanged(nameof(IsConfirmNewConfigFileVisible));
            }
        }

        private void NewConfigFileConfirm(object parameter)
        {
            IsConfirmNewConfigFileVisible = false;

            //Clear devices
            _canComms.Disconnect();
            _canComms.CanDevices.Clear();
            CurrentViewModel = null;
            ConfigFileName = String.Empty;

            ConfigFileOpen = false;
        }

        private void CancelConfirm(object parameter)
        {
            IsConfirmNewConfigFileVisible = false;
            IsReadConfirmVisible = false;
        }

        private bool CanCancelConfirm(object parameter)
        {
            return true;
        }

        private void OpenConfigFile(object parameter)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Config files (*.dco)|*.dco|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = _settingsPath;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".dco") return;

            _canComms.Disconnect();
            _canComms.CanDevices.Clear();
            CurrentViewModel = null;

            
            OpenFile(openFileDialog.FileName);
        }

        public void OpenFile(string fileName)
        {
            _settingsPath = fileName;
            ConfigFileName = Path.GetFullPath(fileName);

            DevicesConfig devices = new DevicesConfig();
            ConfigFileOpen = ConfigFile.Open(fileName, out devices);
            _canComms.CanDevices = ConfigFile.ConfigToCollection(devices);
        }   

        private bool CanOpenConfigFile(object parameter)
        {
            return !_canComms.Connected;
        }

        private void SaveConfigFile(object parameter)
        {
            ConfigFile.Save(_settingsPath, ConfigFile.CollectionToConfig(_canComms.CanDevices));
        }

        private bool CanSaveConfigFile(object parameter)
        {
            return ConfigFileOpen;
        }

        private void SaveAsConfigFile(object parameter)
        {
            System.Windows.Forms.SaveFileDialog saveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveAsFileDialog.Filter = "Config files (*.dco)|*.dco|All files (*.*)|*.*";
            saveAsFileDialog.InitialDirectory = _settingsPath;
            if (saveAsFileDialog.ShowDialog() != DialogResult.OK) return;

            if (Path.GetExtension(saveAsFileDialog.FileName).ToLower() != ".dco") return;

            _settingsPath = saveAsFileDialog.FileName;
            ConfigFileName = Path.GetFullPath(_settingsPath);

            ConfigFile.Save(_settingsPath, ConfigFile.CollectionToConfig(_canComms.CanDevices));
        }

        private bool CanSaveAsConfigFile(object parameter)
        {
            return (_canComms.CanDevices.Count > 0);
        }

        private void AddDevice(object parameter)
        {
            if (SelectedDeviceToAdd.Equals(Devices.CanMsgLog))
            {
                if(DeviceName.Length == 0)
                {
                    DeviceName = "CAN Msg Log";
                }
                _canComms.AddCanDevice(typeof(CanMsgLog), DeviceName, 9999);
            }

            if (DeviceBaseId < 1) return;
            if (DeviceBaseId > 2048) return;
            if (DeviceName.Length == 0) return;

            if (SelectedDeviceToAdd.Equals(Devices.dingoPDM))
            {
                if (DeviceName.Length == 0)
                {
                    DeviceName = "Dingo PDM";
                }
                _canComms.AddCanDevice(typeof(DingoPdmCan), DeviceName, DeviceBaseId);
            }

            if (SelectedDeviceToAdd.Equals(Devices.CANBoard))
            {
                if (DeviceName.Length == 0)
                {
                    DeviceName = "CAN Board";
                }
                _canComms.AddCanDevice(typeof(CanBoardCan), DeviceName, DeviceBaseId);
            }

            /*
            if (SelectedDeviceToAdd.Equals(Devices.DingoDash))
            {
                _canComms.AddCanDevice(typeof(DingoDashCan), DeviceName, DeviceBaseId);
            }
            */

            if (SelectedDeviceToAdd.Equals(Devices.SoftButtonBox))
            {
                if (DeviceName.Length == 0)
                {
                    DeviceName = "Soft Button Box";
                }
                _canComms.AddCanDevice(typeof(SoftButtonBox), DeviceName, DeviceBaseId);
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
            
            SelectedCanDevice.Name = DeviceName;
            if (SelectedCanDevice.GetType() == typeof(CanMsgLog)) return;
            if (SelectedCanDevice.BaseId != DeviceBaseId)
            {
                _canComms.Update(SelectedCanDevice, DeviceBaseId);
            }
        }

        private bool CanUpdateDevice(object parameter)
        {
            if (SelectedCanDevice == null) return false;

            if (SelectedCanDevice.GetType() == typeof(CanMsgLog)) return true;

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
        public ICommand NewBtnConfirmCmd { get; set; }
        public ICommand OpenBtnCmd { get; set; }
        public ICommand SaveBtnCmd { get; set; }
        public ICommand SaveAsBtnCmd { get; set; }
        public ICommand ConnectBtnCmd { get; set; }
        public ICommand DisconnectBtnCmd { get; set; }
        public ICommand RefreshComPortsBtnCmd { get; set; }
        public ICommand ReadBtnCmd { get; set; }
        public ICommand ReadBtnConfirmCmd { get; set; }
        public ICommand WriteBtnCmd { get; set; }
        public ICommand BurnBtnCmd { get; set; }
        public ICommand SleepBtnCmd { get; set;}
        public ICommand CancelConfirmCmd { get; set; }
        private string _sleepBtnContent;
        public string SleepBtnContent
        {
            get => _sleepBtnContent;
            set
            {
                _sleepBtnContent = value;
                OnPropertyChanged(nameof(SleepBtnContent));
            }
        }
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

            //Not in the status bar...but whatever
            SleepBtnContent = SelectedCanDevice?.IsConnected == true ? "Sleep" : "Wake";
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
        dingoPDM,
        CANBoard,
        //DingoDash,
        CanMsgLog,
        SoftButtonBox
    }
    #endregion
}
