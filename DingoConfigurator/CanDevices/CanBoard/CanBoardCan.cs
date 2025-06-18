using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices.CanBoard
{

    public class CanBoardCan : NotifyPropertyChangedBase, ICanDevice
    {
        private string _name;
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name= value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private int _baseId;
        [JsonPropertyName("baseId")]
        public int BaseId { 
            get => _baseId;
            set
            {
                if(_baseId != value)
                {
                    _baseId= value;
                    OnPropertyChanged(nameof(BaseId));
                }
            }
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

        private double _boardTempC;
        [JsonIgnore]
        public double BoardTempC { 
            get => _boardTempC; 
            private set
            {
                if(_boardTempC!= value)
                {
                    _boardTempC= value;
                    OnPropertyChanged(nameof(BoardTempC));
                }
            }
        }

        private double _boardTempF;
        [JsonIgnore]
        public double BoardTempF => _boardTempC * 1.8 + 32.0;

        private ObservableCollection<AnalogInput> _analogIn;
        [JsonPropertyName("analogIn")]
        public ObservableCollection<AnalogInput> AnalogIn
        {
            get => _analogIn;
            set
            {
                if (_analogIn!= value)
                {
                    _analogIn= value;
                    OnPropertyChanged(nameof(AnalogIn));
                }
            }
        }

        private ObservableCollection<DigitalInput> _digitalIn;
        [JsonPropertyName("digitalIn")]
        public ObservableCollection<DigitalInput> DigitalIn { 
            get => _digitalIn; 
            private set
            {
                if(_digitalIn!= value)
                {
                    _digitalIn= value;
                    OnPropertyChanged(nameof(DigitalIn));
                }
            }
        }

        private ObservableCollection<Output> _digitalOut;
        [JsonPropertyName("digitalOut")]
        public ObservableCollection<Output> DigitalOut { 
            get => _digitalOut; private set
            {
                if (_digitalOut != value)
                {
                    _digitalOut = value;
                    OnPropertyChanged(nameof(DigitalOut));
                }
            }
        }

        private int _heartbeat;
        [JsonIgnore]
        public int Heartbeat
        {
            get { return _heartbeat; }
            set { 
                if (_heartbeat != value)
                {
                    _heartbeat = value;
                    OnPropertyChanged(nameof(Heartbeat));
                }
            }
        }

        [JsonIgnore]
        public int TimerIntervalMs { get => 0; }

        public CanBoardCan(string name, int baseId)
        {
            Name = name;
            BaseId = baseId;

            AnalogIn = new ObservableCollection<AnalogInput>();
            for (int i=0; i < 5; i++)
            {
                AnalogIn.Add(new AnalogInput());
                AnalogIn[i].Number = i + 1;
            }

            DigitalIn = new ObservableCollection<DigitalInput>();
            for (int i = 0; i < 8; i++)
            {
                DigitalIn.Add(new DigitalInput());
                DigitalIn[i].Number = i + 1;
            }

            DigitalOut = new ObservableCollection<Output>();
            for (int i = 0; i < 4; i++)
            {
                DigitalOut.Add(new Output());
                DigitalOut[i].Number = i + 1;
            }
        }

        public void UpdateIsConnected()
        {
            TimeSpan timeSpan = DateTime.Now - _lastRxTime;
            IsConnected = timeSpan.TotalMilliseconds < 500;
        }


        public bool InIdRange(int id)
        {
            return (id >= BaseId) && (id <= BaseId + 2);
        }

        public void UpdateView()
        {
         
        }

        public void Clear()
        {
            foreach(var ai in AnalogIn)
            {
                ai.Millivolts = 0;
                ai.DigitalIn = false;
                ai.RotarySwitchPos  = 0;
            }

            foreach(var di in DigitalIn)
            {
                di.State = false;
            }

            foreach(var dout in DigitalOut)
            {
                dout.State = false;
            }
        }

        public bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue)
        {
            if ((id < BaseId) || (id > BaseId + 2)) return false;

            if (id == BaseId + 0) ReadMessage0(data);
            if (id == BaseId + 1) ReadMessage1(data);
            if (id == BaseId + 2) ReadMessage2(data);

            _lastRxTime = DateTime.Now;

            UpdateIsConnected();

            return true;
        }

        private void ReadMessage0(byte[] data)
        {
            AnalogIn[0].Millivolts = Convert.ToDouble((data[1] << 8) + data[0]);
            AnalogIn[1].Millivolts = Convert.ToDouble((data[3] << 8) + data[2]);
            AnalogIn[2].Millivolts = Convert.ToDouble((data[5] << 8) + data[4]);
            AnalogIn[3].Millivolts = Convert.ToDouble((data[7] << 8) + data[6]);
        }

        private void ReadMessage1(byte[] data)
        {
            AnalogIn[4].Millivolts = Convert.ToDouble((data[1] << 8) + data[0]);
            //Byte 2 empty
            //Byte 3 empty
            //Byte 4 empty
            //Byte 5 empty
            BoardTempC = Convert.ToDouble(((data[7] << 8) + data[6]) / 100.0);
        }

        private void ReadMessage2(byte[] data)
        {
            AnalogIn[0].RotarySwitchPos = Convert.ToInt16(data[0] & 0x0F);
            AnalogIn[1].RotarySwitchPos = Convert.ToInt16(data[0] & 0xF0);
            AnalogIn[2].RotarySwitchPos = Convert.ToInt16(data[1] & 0x0F);
            AnalogIn[3].RotarySwitchPos = Convert.ToInt16(data[1] & 0xF0);
            AnalogIn[4].RotarySwitchPos = Convert.ToInt16(data[2] & 0x0F);

            DigitalIn[0].State = Convert.ToBoolean(data[4] & 0x1);
            DigitalIn[1].State = Convert.ToBoolean((data[4] & 0x2) >> 1);
            DigitalIn[2].State = Convert.ToBoolean((data[4] & 0x4) >> 2);
            DigitalIn[3].State = Convert.ToBoolean((data[4] & 0x8) >> 3);
            DigitalIn[4].State = Convert.ToBoolean((data[4] & 0x10) >> 4);
            DigitalIn[5].State = Convert.ToBoolean((data[4] & 0x20) >> 5);
            DigitalIn[6].State = Convert.ToBoolean((data[4] & 0x40) >> 6);
            DigitalIn[7].State = Convert.ToBoolean((data[4] & 0x80) >> 7);

            AnalogIn[0].DigitalIn = Convert.ToBoolean(data[5] & 0x1);
            AnalogIn[1].DigitalIn = Convert.ToBoolean((data[5] & 0x2) >> 1);
            AnalogIn[2].DigitalIn = Convert.ToBoolean((data[5] & 0x4) >> 2);
            AnalogIn[3].DigitalIn = Convert.ToBoolean((data[5] & 0x8) >> 3);
            AnalogIn[4].DigitalIn = Convert.ToBoolean((data[5] & 0x10) >> 4);

            DigitalOut[0].State = Convert.ToBoolean(data[6] & 0x1);
            DigitalOut[1].State = Convert.ToBoolean((data[6] & 0x2) >> 1);
            DigitalOut[2].State = Convert.ToBoolean((data[6] & 0x4) >> 2);
            DigitalOut[3].State = Convert.ToBoolean((data[6] & 0x8) >> 3);

            Heartbeat = Convert.ToInt16(data[7]);
        }

        public List<CanDeviceResponse> GetUploadMessages()
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

        public CanDeviceResponse GetBurnMessage()
        {
            return null;
        }

        public CanDeviceResponse GetSleepMessage()
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
    }
}
