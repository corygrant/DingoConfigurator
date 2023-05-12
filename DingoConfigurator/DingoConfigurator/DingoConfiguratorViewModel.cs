using CanDevices;
using CanInterfaces;
using DingoConfigurator.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace DingoConfigurator
{
    internal class DingoConfiguratorViewModel : ViewModelBase
    {
        private ICanInterface _can;

        private ObservableCollection<ICanDevice> _canDevices { get; set; }

        private bool _canInterfaceConnected;
        public bool CanInterfaceConnected
        {
            get => _canInterfaceConnected;
            set {
                _canInterfaceConnected = value;
                UpdateBrushes();
                UpdateStatusBar();
            }
        }

        private System.Timers.Timer updateTimer;

        public DingoConfiguratorViewModel()
        {
            _canDevices = new ObservableCollection<ICanDevice>
            {
                new DingoPdmCan("Engine PDM", 2000),
                new CanBoardCan("Steering Wheel", 1600)
            };

            Cans = new ObservableCollection<ComboBoxCanInterfaces>()
            {
                new ComboBoxCanInterfaces(){ Id=1, Name="USB2CAN"},
                new ComboBoxCanInterfaces(){ Id=2, Name="PCAN"}
            };

            //TODO: Setting-Last selected interface
            SelectedCan = Cans.First(x => x.Name == "PCAN");
            
            ConnectBtnCmd = new RelayCommand(Connect, CanConnect);
            DisconnectBtnCmd = new RelayCommand(Disconnect, CanDisconnect);

            UpdateBrushes();
            UpdateStatusBar();

            // Create a timer to update status bar
            updateTimer = new System.Timers.Timer(1000);
            updateTimer.Elapsed += UpdateView;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
        }

        private void UpdateView(object sender, ElapsedEventArgs e)
        {
            UpdateBrushes();
            UpdateStatusBar();
        }

        public void WindowClosing()
        {
            Disconnect(null);
            updateTimer.Stop();
            updateTimer.Dispose();
        }

        private void CanDataReceived(object sender, CanDataEventArgs e)
        {
            Console.WriteLine(e.canData.Id);

            foreach (var cd in _canDevices)
                cd.Read(e.canData.Id, e.canData.Payload);

            UpdateStatusBar();
        }

        #region TreeView
        public ObservableCollection<ICanDevice> CanDevices
        {
            get => _canDevices;
        }
        #endregion

        #region Commands
        private void Connect(object parameter)
        {
            switch (SelectedCan.Name)
            {
                case "USB2CAN":
                    _can = new CanInterfaces.USB2CAN();
                    break;

                case "PCAN":
                    _can = new CanInterfaces.PCAN();
                    break;
            }

            _can.Init();
            _can.DataReceived += CanDataReceived;
            _can.Start();
            CanInterfaceConnected = true;
            UpdateBrushes();
            UpdateStatusBar();
        }

        
        private bool CanConnect(object parameter)
        {
            return !CanInterfaceConnected;
        }

        private void Disconnect(object parameter)
        {
            if(_can != null) _can.Stop();
            CanInterfaceConnected = false;
            UpdateBrushes();
            UpdateStatusBar();
        }

        private bool CanDisconnect(object parameter)
        {
            return CanInterfaceConnected;
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
        #endregion

        #region Buttons
        public ICommand ConnectBtnCmd { get; set; }
        public ICommand DisconnectBtnCmd { get; set; }
        #endregion

        #region StatusBar
        private void UpdateStatusBar()
        {
            CanInterfaceStatusText = $"{SelectedCan.Name} Connected: {CanInterfaceConnected}";

            int connectedCount = 0;
            foreach(var cd in _canDevices)
                if(cd.IsConnected) connectedCount++;

            DeviceCountText = $"{connectedCount}";
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

        #region Brushes
        private void UpdateBrushes()
        {
            CanInterfaceStatusFill = (CanInterfaceConnected ? Brushes.LightGreen : Brushes.LightGray);
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
