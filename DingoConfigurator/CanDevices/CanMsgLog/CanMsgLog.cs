using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CanDevices.CanMsgLog
{
    public class CanMsgLog : NotifyPropertyChangedBase, ICanDevice
    {
        private bool _logToFile;
        [JsonIgnore]
        public bool LogToFile
        {
            get => _logToFile;
            set => _logToFile = value;
        }

        private string _name;
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

        private int _baseId;
        [JsonPropertyName("baseId")]
        public int BaseId
        {
            get => _baseId;
            set => _baseId = value;
        }

        private DateTime _lastRxTime;
        [JsonIgnore]
        public DateTime LastRxTime { get => _lastRxTime; }

        private bool _isConnected;
        [JsonIgnore]
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        private NumberFormat _idFormat;
        [JsonPropertyName("idFormat")]
        public NumberFormat IdFormat
        {
            get => _idFormat;
            set 
            {
                if (_idFormat != value)
                {
                    _idFormat = value;
                    OnPropertyChanged(nameof(IdFormat));

                    foreach (var msg in AllData)
                    {
                        switch (IdFormat)
                        {
                            case NumberFormat.Hex:
                                msg.IdString = String.Format("{0:X}", msg.Id);
                                break;

                            case NumberFormat.Dec:
                                msg.IdString = String.Format("{0}", msg.Id);
                                break;
                        }
                    }
                }
            }
        }

        private NumberFormat _payloadFormat;
        [JsonPropertyName("payloadFormat")]
        public NumberFormat PayloadFormat
        {
            get => _payloadFormat;
            set
            {
                if (value != _payloadFormat)
                {
                    _payloadFormat = value;
                    OnPropertyChanged(nameof(PayloadFormat));

                    foreach (var msg in AllData)
                    {
                        msg.PayloadString = String.Empty;
                        for (int i = 0; i < msg.Len; i++)
                        {
                            switch (PayloadFormat)
                            {
                                case NumberFormat.Hex:
                                    msg.PayloadString += String.Format("{0:X} ", msg.Payload[i]);
                                    break;

                                case NumberFormat.Dec:
                                    msg.PayloadString += String.Format("{0} ", msg.Payload[i]);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private readonly object _allDataLock;
        private ObservableCollection<CanMsgLogData> _allData;
        [JsonIgnore]
        public ObservableCollection<CanMsgLogData> AllData
        {
            get => _allData;
            set
            {
                _allData = value;
                OnPropertyChanged(nameof(AllData));
            }
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [JsonIgnore]
        public int TimerIntervalMs { get => 0; }

        public CanMsgLog(string name, int baseId)
        {
            _name = name;
            _baseId = baseId;
            _allDataLock = new object();
            AllData = new ObservableCollection<CanMsgLogData>();
            BindingOperations.EnableCollectionSynchronization(_allData, _allDataLock);
        }

        public CanDeviceResponse GetBurnMessage()
        {
            return null;
        }

        public CanDeviceResponse GetSleepMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetDownloadMessages()
        {
            return null;
        }
        public List<CanDeviceResponse> GetUpdateMessages(int newId)
        {
            return null;
        }

        public List<CanDeviceResponse> GetUploadMessages()
        {
            return null;
        }

        public CanDeviceResponse GetVersionMessage()
        {
            return null;
        }

        public List<CanDeviceResponse> GetTimerMessages()
        {
            return new List<CanDeviceResponse>();
        }

        public bool InIdRange(int id)
        {
            return true;
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            _lastRxTime = DateTime.Now;
            UpdateIsConnected();

            bool addNew = true;

            if (AllData.Count > 0)
            {
                foreach (var msg in AllData)
                {
                    if (msg.Id == id)
                    {
                        switch (IdFormat)
                        {
                            case NumberFormat.Hex:
                                msg.IdString = String.Format("{0:X}", msg.Id);
                                break;

                            case NumberFormat.Dec:
                                msg.IdString = String.Format("{0}", msg.Id);
                                break;
                        }
                        msg.Len = data.Length;
                        msg.Payload = data;
                        msg.PayloadString = String.Empty;
                        for (int i = 0; i < msg.Len; i++)
                        {
                            switch (PayloadFormat)
                            {
                                case NumberFormat.Hex:
                                    msg.PayloadString += String.Format("{0:X} ", msg.Payload[i]);
                                    break;

                                case NumberFormat.Dec:
                                    msg.PayloadString += String.Format("{0} ", msg.Payload[i]);
                                    break;
                            }
                        }
                        msg.Count++;
                        OnPropertyChanged(nameof(AllData));
                        addNew = false;

                        if (LogToFile)
                        {
                            Logger.Debug($"ID:{msg.Id} Len:{msg.Len} Data:{msg.PayloadString}");
                        }

                        break;
                    }
                }
            }

            if (addNew)
            {
                lock (_allDataLock)
                {
                    string _idString = string.Empty;
                    string _payloadString = string.Empty;
                    switch (IdFormat)
                    {
                        case NumberFormat.Hex:
                            _idString = String.Format("{0:X}", id);
                            break;

                        case NumberFormat.Dec:
                            _idString = String.Format("{0}", id);
                            break;
                    }

                    for (int i = 0; i < data.Length; i++)
                    {
                        switch (PayloadFormat)
                        {
                            case NumberFormat.Hex:
                                _payloadString += String.Format("{0:X} ", data[i]);
                                break;

                            case NumberFormat.Dec:
                                _payloadString += String.Format("{0} ", data[i]);
                                break;
                        }
                    }

                    AllData.Add(new CanMsgLogData { 
                        Id = id, 
                        IdString = _idString,
                        Payload = data, 
                        PayloadString = _payloadString,
                        Len = data.Length, 
                        Count = 1 });
                }
            }

            

            return true;
        }

        public void Clear()
        {

        }

        public void ClearAll()
        {
            AllData.Clear();
        }

        public void UpdateIsConnected()
        {
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }

        public enum NumberFormat
        {
            Hex,
            Dec
        }
    }
}
