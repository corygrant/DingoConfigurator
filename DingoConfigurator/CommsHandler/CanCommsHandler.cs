using CanInterfaces;
using CanDevices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private ConcurrentQueue<CanDeviceResponse> _queue;

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
            _queue = new ConcurrentQueue<CanDeviceResponse>();
            Connected = false;
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

            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning, _cts.Token);

            Thread.Sleep(100); //Wait for devices to connect
            Upload(null);
        }

        public async Task Disconnect()
        {
            _cts.Cancel();

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
                            _queue.Enqueue(msg);
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
                            _queue.Enqueue(msg);
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
                        _queue.Enqueue(msg);
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

        private void ProcessQueue(object taskState)
        {
            CanDeviceResponse msg;
            bool reQueue;

            while (!_cts.Token.IsCancellationRequested)
            {
                //Always have to Enqueue again, unless it was received
                if (_queue.TryDequeue(out msg))
                {
                    reQueue = true;

                    //Send message
                    if (!msg.Sent && Connected)
                    {
                        _can.Write(msg.Data);
                        msg.TimeSentStopwatch = Stopwatch.StartNew();
                        msg.Sent = true;
                        msg.Received = false;
                        Task.Delay(100); //Wait a bit to ensure the device can respond
                        //CANTx task runs every 50ms, so must be longer than that
                    }
                    else
                    {
                        if (msg.Sent && !msg.Received)
                        {
                            if (msg.TimeSentStopwatch.ElapsedMilliseconds > 1000)
                            {
                                if (msg.ReceiveAttempts <= 4)
                                {
                                    Logger.Warn($"No response {msg.MsgDescription}");
                                    msg.Sent = false; //Resend request
                                    msg.ReceiveAttempts++;
                                }
                                else
                                {
                                    Logger.Error($"No response after 4 attempts {msg.MsgDescription}");
                                    //Dont put back on queue
                                    reQueue = false;
                                }
                            }
                        }
                    }

                    if (msg.Sent && msg.Received)
                    {
                        reQueue = false;
                    }

                    if (reQueue)
                    {
                        _queue.Enqueue(msg); //Add back to queue, at the end
                    }
                }
                Task.Delay(10);
            }
            //Dump queue
            while (_queue.TryDequeue(out msg)) ;

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
