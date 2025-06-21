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
using System.Runtime.InteropServices;
using CanDevices.SoftButtonBox;
using System.Management;
using System.Reflection;
using CanDevices.dingoPdmMax;
using System.Collections.Concurrent;

namespace CommsHandler
{
    public class CanCommsHandler : NotifyPropertyChangedBase, IDisposable
    {
        private ICanInterface _can;

        private ConcurrentDictionary<(int BaseId, int Prefix, int Index),CanDeviceResponse> _queue;

        public delegate void DataUpdatedHandler(object sender);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private string _port;
        private bool _isSerial = false;

        private System.Timers.Timer _checkConnectionTimer = new System.Timers.Timer(1000);
        private Dictionary<ICanDevice, System.Timers.Timer> _deviceTimers = new Dictionary<ICanDevice, System.Timers.Timer>();
        private readonly object _timerLock = new object();

        private int _sleepTime = 1;
        private int _msgTimeout = 10;
        private const int _maxReceiveAttempts = 20;
        private const bool _displayRetryWarnings = false;

        private bool _disposed;

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
            _queue = new ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse>();
            Connected = false;

            _checkConnectionTimer.Elapsed += (sender, e) =>
            {
                if (!IsPortConnected(_port) && _isSerial)
                {
                    Logger.Warn("CAN disconnected");
                    Disconnect();
                    _checkConnectionTimer.Stop();
                }
            };
            
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
                    _isSerial = true;
                    _sleepTime = 1; //ms
                    _msgTimeout = 10; //ms
                    break;

                case "PCAN":
                    _can = new CanInterfaces.PCAN();
                    _isSerial = false;
                    _sleepTime = 5; //ms
                    _msgTimeout = 10; //ms
                    break;

                case "USB":
                    _can = new CanInterfaces.USB();
                    _isSerial = true;
                    _sleepTime = 5; //ms
                    _msgTimeout = 10; //ms
                    break;
            }

            _port = port;

            if (!_can.Init(port, baud)) return;
            _can.DataReceived += CanDataReceived;
            if (!_can.Start()) return;
            Connected = true;

            Thread.Sleep(100); //Wait for devices to connect

            _checkConnectionTimer.Start();

            _ = GetVersion(null);
            StartDeviceTimers();
        }

        public void Disconnect()
        {
            StopDeviceTimers();
            if (_can != null)
            {
                _can.DataReceived -= CanDataReceived;
                _can.Stop();
            }
            foreach(var cd in CanDevices)
            {
                cd.Clear();
            }
            Connected = false;
        }

        public async Task Read(ICanDevice canDevice)
        {
            await Task.Run(() =>
            { 
                //If null, read all
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
                                msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout, _msgTimeout);
                                var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                                if (_queue.TryAdd(key, msg))
                                {
                                    _can.Write(msg.Data);
                                    ProcessMessage(msg.Data);//Catch with CanMsgLog

                                    Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
                                }
                                else
                                    Logger.Error($"Failed to add to queue: {msg.MsgDescription}");
                            }
                            msgs.Clear();
                        }
                    }
                }
            });
        }

        public async Task Write(ICanDevice canDevice)
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
                                msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout, _msgTimeout);
                                var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                                if (_queue.TryAdd(key, msg))
                                {
                                    if (!_can.Write(msg.Data))
                                    {
                                        Logger.Error("Failed to write to CAN");
                                        Disconnect();
                                    }
                                    ProcessMessage(msg.Data);//Catch with CanMsgLog

                                    Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
                                }
                                else
                                    Logger.Error($"Failed to add to queue: {msg.MsgDescription}");
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
                                    msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout, _msgTimeout);
                                    var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                                    if (_queue.TryAdd(key, msg))
                                    {
                                        if (!_can.Write(msg.Data))
                                        {
                                            Logger.Error("Failed to write to CAN");
                                            Disconnect();
                                        }
                                        ProcessMessage(msg.Data);//Catch with CanMsgLog

                                        Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
                                    }
                                    else
                                        Logger.Error($"Failed to add to queue: {msg.MsgDescription}");
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
                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout * 10, _msgTimeout * 10); //Check for burn slower
                            var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                            if (_queue.TryAdd(key, msg))
                            {
                                if (!_can.Write(msg.Data))
                                {
                                    Logger.Error("Failed to write to CAN");
                                    Disconnect();
                                }
                                ProcessMessage(msg.Data);//Catch with CanMsgLog


                                Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
                            }
                            else
                                Logger.Error($"Failed to add to queue: {msg.MsgDescription}");

                        }
                    }
                }
            });
        }

        public async Task Sleep(ICanDevice canDevice)
        {
            await Task.Run(() =>
            {
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
                    {
                        if (cd.IsConnected)
                        {
                            var msg = cd.GetSleepMessage();
                            if (msg == null) return;

                            msg.DeviceBaseId = cd.BaseId;
                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout, _msgTimeout);
                            var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                            if (_queue.TryAdd(key, msg))
                            {
                                if (!_can.Write(msg.Data))
                                {
                                    Logger.Error("Failed to write to CAN");
                                    Disconnect();
                                }
                                ProcessMessage(msg.Data);//Catch with CanMsgLog

                                Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
                            }
                            else
                                Logger.Error($"Failed to add to queue: {msg.MsgDescription}");
                        }
                    }
                }
            });
        }

        public async Task Wakeup(ICanDevice canDevice)
        {
            await Task.Run(() =>
            {
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
                    {
                        var msg = new CanDeviceResponse
                        {
                            Data = new CanInterfaceData
                            {
                                Id = canDevice.BaseId - 1,
                                Len = 1,
                                Payload = new byte[] { Convert.ToByte('!'), 0, 0, 0, 0, 0, 0, 0 }
                            },
                            MsgDescription = "Wake Request"
                        };

                        //No response needed, don't add to queue
                        msg.DeviceBaseId = cd.BaseId;
                        _can.Write(msg.Data);
                    }
                }
            });
        }

        public async Task FwUpdate(ICanDevice canDevice)
        {
            await Task.Run(() =>
            {
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
                    {
                        var msg = new CanDeviceResponse
                        {
                            Data = new CanInterfaceData
                            {
                                Id = canDevice.BaseId - 1,
                                Len = 6,
                                Payload = new byte[] { Convert.ToByte(MessagePrefix.Bootloader), (byte)'B', (byte)'O', (byte)'O', (byte)'T', (byte)'L', 0, 0 }
                            },
                            MsgDescription = "Fw Update Request"
                        };

                        //No response needed, don't add to queue
                        msg.DeviceBaseId = cd.BaseId;
                        _can.Write(msg.Data);
                    }
                }
            });
        }

        public async Task GetVersion(ICanDevice canDevice)
        {
            await Task.Run(() =>
            {
                foreach (var cd in _canDevices)
                {
                    if ((canDevice == null) || canDevice.Equals(cd))
                    {
                        if (cd.IsConnected)
                        {
                            var msg = cd.GetVersionMessage();
                            if (msg == null) return;

                            msg.DeviceBaseId = cd.BaseId;
                            msg.TimeSentTimer = new Timer(SentTimeElapsed, msg, _msgTimeout, _msgTimeout);
                            var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                            _queue.TryAdd(key, msg);
                            _can.Write(msg.Data);
                            ProcessMessage(msg.Data);//Catch with CanMsgLog

                            Thread.Sleep(_sleepTime); //Slow down, device can't respond fast enough
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
                msg.TimeSentTimer?.Dispose();
                var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                _queue.TryRemove(key, out _);
                return;
            }
            
            if(msg.ReceiveAttempts < _maxReceiveAttempts)
            {
                _can.Write(msg.Data);
                msg.ReceiveAttempts++;

                if (_displayRetryWarnings)
                    Logger.Warn($"No response {msg.MsgDescription}, {msg.ReceiveAttempts} attempts");
            }
            else
            {
                Logger.Error($"No response after {msg.ReceiveAttempts} attempts {msg.MsgDescription}");
                msg.TimeSentTimer?.Dispose();
                var key = (msg.DeviceBaseId, msg.Prefix, msg.Index);
                _queue.TryRemove(key, out _);
            }
        }

        public void ResetCanDevices()
        {
            StopDeviceTimers();
            _canDevices?.Clear();

            _canDevices = new ObservableCollection<ICanDevice>();
        }

        private void StartDeviceTimers()
        {
            lock (_timerLock)
            {
                foreach (var device in _canDevices)
                {
                    int intervalMs = device.TimerIntervalMs;
                    if (intervalMs > 0)
                    {
                        var timer = new System.Timers.Timer(intervalMs);
                        timer.Elapsed += (sender, e) => SendTimerMessages(device);
                        timer.AutoReset = true;
                        timer.Start();

                        _deviceTimers[device] = timer;
                    }
                }
            }
        }

        private void StopDeviceTimers()
        {
            lock (_timerLock)
            {
                foreach (var timer in _deviceTimers.Values)
                {
                    timer?.Stop();
                    timer?.Dispose();
                }
                _deviceTimers.Clear();
            }
        }

        private void SendTimerMessages(ICanDevice device)
        {
            if (!Connected) return;

            try
            {
                var messages = device.GetTimerMessages();
                if (messages != null)
                {
                    foreach (var msg in messages)
                    {
                        msg.DeviceBaseId = device.BaseId;
                        _can.Write(msg.Data);
                        ProcessMessage(msg.Data);
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to send timer messages: {ex.Message}");
            }
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

            if (type == typeof(dingoPdmMaxCan))
            {
                var newPdm = new dingoPdmMaxCan(name, baseId);
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
                var newMsgLog = new CanMsgLog(name, baseId);
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
            lock (_timerLock)
            {
                if (_deviceTimers.ContainsKey(canDevice))
                {
                    _deviceTimers[canDevice]?.Stop();
                    _deviceTimers[canDevice]?.Dispose();
                    _deviceTimers.Remove(canDevice);
                }
            }
            _canDevices.Remove(canDevice);
            OnPropertyChanged(nameof(CanDevices));
        }

        public bool IsPortConnected(string portName)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
            {
                var ports = searcher.Get();
                foreach (ManagementObject port in ports)
                {
                    if (port["DeviceID"].ToString().Equals(portName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Stop the timer
                    _checkConnectionTimer.Stop();
                    _checkConnectionTimer.Dispose();

                    // Stop device timers
                    StopDeviceTimers();

                    // Ensure no other threads are using the COM objects
                    lock (_queue)
                    {
                        if (_can != null)
                        {
                            _can.DataReceived -= CanDataReceived;
                            _can.Stop();
                            _can.Dispose();
                            _can = null;
                        }
                    }

                    // Dispose of other managed resources
                    foreach (var cd in _canDevices)
                    {
                        cd.Clear();
                    }
                    _canDevices.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
