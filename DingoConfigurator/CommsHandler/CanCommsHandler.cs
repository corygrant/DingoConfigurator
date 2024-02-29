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
using CanDevices.CanMsgLog;
using PCAN;
using System.Runtime.InteropServices;
using CanDevices.SoftButtonBox;

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
            set
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

        public void Connect(string interfaceName, string port, CanInterfaceBaudRate baud)
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

        public void Disconnect()
        {
            if (_can != null) _can.Stop();
            foreach(var cd in CanDevices)
            {
                cd.Clear();
            }
            Connected = false;
        }

        public async Task Upload(ICanDevice canDevice)
        {
            await Task.Run(() =>
            { 
                //If null, upload all
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
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
                                ProcessMessage(msg.Data);//Catch with CanMsgLog

                                msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);

                                Thread.Sleep(20); //Slow down, device can't respond fast enough
                            }
                            msgs.Clear();
                        }
                    }
                }
            });
        }

        public async Task Download(ICanDevice canDevice)
        {
            await Task.Run(() =>
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
                                ProcessMessage(msg.Data);//Catch with CanMsgLog

                                msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);

                                Thread.Sleep(20); //Slow down, device can't respond fast enough
                            }
                            msgs.Clear();
                        }
                    }
                }
            });
        }

        public async Task Update(ICanDevice canDevice, int newId)
        {
            await Task.Run(() =>
            {
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
                    {
                        if (cd.IsConnected)
                        {
                            var msgs = cd.GetUpdateMessages(newId);

                            if (msgs != null)
                            {
                                foreach (var msg in msgs)
                                {
                                    msg.DeviceBaseId = newId; //Set msg ID to new ID so response is processed properly
                                    _queue.Add(msg);
                                    _can.Write(msg.Data);
                                    ProcessMessage(msg.Data);//Catch with CanMsgLog

                                    msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);

                                    Thread.Sleep(20); //Slow down, device can't respond fast enough
                                }
                                msgs.Clear();
                            }

                            //After sending updated ID, set local base ID
                            cd.BaseId = newId;
                        }
                        else
                        {
                            cd.BaseId = newId;
                        }
                    }
                }
            });
        }

        public async Task Burn(ICanDevice canDevice)
        {
            await Task.Run(() =>
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
                            ProcessMessage(msg.Data);//Catch with CanMsgLog

                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, 1000, 1000);

                            Thread.Sleep(20); //Slow down, device can't respond fast enough
                        }
                    }
                }
            });
        }

        private void CanDataReceived(object sender, CanDataEventArgs e)
        {
            ProcessMessage(e.canData);
        }

        private void ProcessMessage(CanInterfaceData data)
        {
            foreach (var cd in _canDevices)
            {
                if (cd.InIdRange(data.Id))
                {
                    cd.Read(data.Id, data.Payload, ref _queue);
                }
            }
        }

        private void SentTimeElapsed(Object response)
        {
            CanDeviceResponse msg = (CanDeviceResponse)response;

            if (!Connected)
            {
                msg.TimeSentTimer.Dispose();
                _queue.Remove(msg);
                return;
            }
            
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

            if (type == typeof(CanMsgLog))
            {
                var newMsgLog = new CanMsgLog();
                _canDevices.Add(newMsgLog);
                OnPropertyChanged(nameof(CanDevices));
                return newMsgLog;
            }

            if (type == typeof(SoftButtonBox))
            {
                var newSbb = new SoftButtonBox(name, baseId);
                _canDevices.Add(newSbb);
                OnPropertyChanged(nameof(CanDevices));
                return newSbb;
            }

            return null;
        }

        public void RemoveCanDevice(ICanDevice canDevice)
        {
            _canDevices.Remove(canDevice);
            OnPropertyChanged(nameof(CanDevices));
        }
    }
}
