using CanInterfaces;
using CanDevices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CanDevices.DingoPdm;
using CanDevices.CanBoard;
using CanDevices.DingoDash;

namespace CommsHandler
{
    public class CanCommsHandler : NotifyPropertyChangedBase
    {
        private ICanInterface _can;

        private List<CanDeviceResponse> _queue;

        public delegate void DataUpdatedHandler(object sender);

        private CancellationTokenSource _cts;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ObservableCollection<ICanDevice> _canDevices;
        public ObservableCollection<ICanDevice> CanDevices
        {
            get => _canDevices;
            private set
            {
                _canDevices = value;
                OnPropertyChanged(nameof(CanDevices));
            }
        }

        public CanCommsHandler()
        {
            _canDevices = new ObservableCollection<ICanDevice>();
            _cts = new CancellationTokenSource();
            _queue = new List<CanDeviceResponse>();
            Connected = false;
        }

        ~CanCommsHandler()
        {
            _cts.Cancel();
        }

        private bool _connected;
        public bool Connected
        {
            get => _connected;
            private set
            {
                _connected = value;
                OnPropertyChanged(nameof(Connected));
            }
        }

        public int QueueCount { get => _queue.Count; }
        public int RxTimeDelta {
            get
            {
                if (_can == null) return 0;
                return _can.RxTimeDelta;
            }
        }

        public async Task Connect(string interfaceName, string port, CanInterfaceBaudRate baud)
        {

            switch (interfaceName)
            {
                case "USB2CAN":
                    _can = new CanInterfaces.USB2CAN();
                    break;

                case "PCAN":
                    _can = new CanInterfaces.PCAN();
                    port = "USBBUS1";
                    break;

                case "USB":
                    _can = new CanInterfaces.USB();
                    break;
            }

            if (!_can.Init(port, baud)) return;
            _can.DataReceived += CanDataReceived;
            if (!_can.Start()) return;
            Connected = true;

            

            Thread.Sleep(100); //Wait for devices to connect
            Upload(null);
        }

        public async Task Disconnect()
        {
            if (_can != null) _can.Stop();
            Connected = false;
        }

        public async Task Upload(ICanDevice canDevice)
        {
            //If null, upload all
            foreach (var cd in _canDevices)
            {
                if((canDevice == null) || canDevice.Equals(cd))
                {
                    if (cd.IsConnected)
                    {
                        var msgs = cd.GetUploadMessages();
                        if (msgs == null) return;

                        foreach (var msg in msgs)
                        {
                            msg.DeviceBaseId = cd.BaseId;
                            _queue.Add(msg);
                            _can.Write(msg.Data);

                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);
                        }
                        msgs.Clear();
                    }
                }
            }
        }

        public async Task Download(ICanDevice canDevice)
        {
            foreach (var cd in _canDevices)
            {
                if ((canDevice == null) || canDevice.Equals(cd))
                {
                    if (cd.IsConnected)
                    {
                        var msgs = cd.GetDownloadMessages();
                        if (msgs == null) return;

                        foreach (var msg in msgs)
                        {
                            msg.DeviceBaseId = cd.BaseId;
                            _queue.Add(msg);
                            _can.Write(msg.Data);

                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);
                        }
                        msgs.Clear();
                    }
                }
            }
        }

        public async Task Burn(ICanDevice canDevice)
        {
            foreach (var cd in _canDevices)
            {
                if ((canDevice == null) || canDevice.Equals(cd))
                {
                    if (cd.IsConnected)
                    {
                        var msg = cd.GetBurnMessage();
                        if (msg == null) return;

                        msg.DeviceBaseId = cd.BaseId;
                        _queue.Add(msg);
                        _can.Write(msg.Data);

                        msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);
                    }
                }
            }
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

        private void SentTimeElapsed(Object response)
        {
            CanDeviceResponse msg = (CanDeviceResponse)response;
            if(msg.ReceiveAttempts < 4)
            {
                Logger.Warn($"No response {msg.MsgDescription}");
                _can.Write(msg.Data);
                msg.ReceiveAttempts++;
            }
            else
            {
                Logger.Error($"No response after 4 attempts {msg.MsgDescription}");
                msg.TimeSentTimer.Dispose();
                _queue.Remove(msg);
            }
        }

        public void ResetCanDevices()
        {
            _canDevices?.Clear();

            _canDevices = new ObservableCollection<ICanDevice>();
        }

        public ICanDevice AddCanDevice(Type type, string name, int baseId)
        {
            if (!(typeof(ICanDevice).IsAssignableFrom(type))) return null;

            if(type == typeof(DingoPdmCan))
            {
                var newPdm = new DingoPdmCan(name, baseId);
                _canDevices.Add(newPdm);
                OnPropertyChanged(nameof(CanDevices));
                return newPdm;
            }

            if (type == typeof(CanBoardCan))
            {
                var newCb = new CanBoardCan(name, baseId);
                _canDevices.Add(newCb);
                OnPropertyChanged(nameof(CanDevices));
                return newCb;
            }

            if (type == typeof(DingoDashCan))
            {
                var newDash = new DingoDashCan(name, baseId);
                _canDevices.Add(newDash);
                OnPropertyChanged(nameof(CanDevices));
                return newDash;
            }

            return null;
        }
    }
}
