using CanDevices;
using CanInterfaces;
using DingoConfigurator.Config;
using DingoConfigurator.Properties;
using DingoConfigurator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DingoConfigurator
{
    public class MainViewModel : ViewModelBase
    {
        private ICanInterface _can;

        private DevicesConfig _config;

        private ObservableCollection<ICanDevice> _canDevices { get; set; }

        private bool _canInterfaceConnected;
        public bool CanInterfaceConnected
        {
            get => _canInterfaceConnected;
            set {
                _canInterfaceConnected = value;
                UpdateStatusBar();
                OnPropertyChanged(nameof(CanInterfaceConnected));
            }
        }

        private System.Timers.Timer updateTimer;

        public delegate void DataUpdatedHandler(object sender);

        public MainViewModel()
        {
            _config = new DevicesConfig();
            _config = DevicesConfigHandler.Deserialize("D:\\GitHub\\DingoConfigurator\\RallyCar.json");

            UpdateCanDevices();

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
                SelectedComPort = ComPorts.First(x => x.Equals(comPort));
            }

            //Set Button Commands
            ConnectBtnCmd = new RelayCommand(Connect, CanConnect);
            DisconnectBtnCmd = new RelayCommand(Disconnect, CanDisconnect);
            RefreshComPortsBtnCmd = new RelayCommand(RefreshComPorts, CanRefreshComPorts);

            UpdateStatusBar();

            // Create a timer to update status bar
            updateTimer = new System.Timers.Timer(200);
            updateTimer.Elapsed += UpdateView;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
        }

        private void UpdateCanDevices()
        {
            _canDevices?.Clear();
            
            _canDevices = new ObservableCollection<ICanDevice>();
            //            {
            //               new DingoPdmCan("Engine PDM", 2000),
            //              new CanBoardCan("Steering Wheel", 1600),
            //             new DingoDashCan("Dash", 2200)
            //        };
            foreach (var pdm in _config.pdm)
                _canDevices.Add(new DingoPdmCan(pdm.label, 2000));

            foreach (var cb in _config.canBoard)
                _canDevices.Add(new CanBoardCan(cb.label, 1600));

            foreach (var dash in _config.dash)
                _canDevices.Add(new DingoDashCan(dash.label, 2200));
        }

        private void UpdateView(object sender, ElapsedEventArgs e)
        {
            UpdateStatusBar();
        }

        public void WindowClosing()
        {
            Disconnect(null);
            updateTimer.Stop();
            updateTimer.Dispose();

            //Save user settings
            Settings.Default.CanInterface = SelectedCan.Name;
            Settings.Default.BaudRate = SelectedBaudRate;
            Settings.Default.ComPort = SelectedComPort;
            Settings.Default.Save();
        }

        private void CanDataReceived(object sender, CanDataEventArgs e)
        {
            foreach (var cd in _canDevices)
                cd.Read(e.canData.Id, e.canData.Payload);

            UpdateStatusBar();
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

        #region TreeView
        public ObservableCollection<ICanDevice> CanDevices
        {
            get => _canDevices;
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
                    baud = CanInterfaceBaudRate.BAUD_500K;
                    break;

                case "PCAN":
                    _can = new CanInterfaces.PCAN();
                    port = "USBBUS1";
                    baud = CanInterfaceBaudRate.BAUD_500K;
                    break;

                case "USB":
                    _can = new CanInterfaces.USB();
                    port = SelectedComPort;
                    baud = CanInterfaceBaudRate.BAUD_500K;//Not used
                    break;
            }

            if(!_can.Init(port, baud)) return;
            _can.DataReceived += CanDataReceived;
            if(!_can.Start()) return;
            CanInterfaceConnected = true;
            UpdateStatusBar();
        }

        
        private bool CanConnect(object parameter)
        {
            return !CanInterfaceConnected && 
                    ((CanComPorts && (SelectedComPort != null)) ||
                    !CanComPorts);
        }

        private void Disconnect(object parameter)
        {
            if(_can != null) _can.Stop();
            CanInterfaceConnected = false;
            UpdateStatusBar();
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
        public ICommand ConnectBtnCmd { get; set; }
        public ICommand DisconnectBtnCmd { get; set; }
        public ICommand RefreshComPortsBtnCmd { get; set; }
        #endregion

        #region StatusBar
        private void UpdateStatusBar()
        {
            CanInterfaceStatusText = $"{SelectedCan.Name} {(CanInterfaceConnected ? "Connected" : "Disconnected")}";

            int connectedCount = 0;
            foreach (var cd in _canDevices)
            {
                cd.UpdateIsConnected();
                if (cd.IsConnected) connectedCount++;
            }

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
