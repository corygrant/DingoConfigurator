using System;
using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class StarterDisable : NotifyPropertyChangedBase
    {
        private bool _enabled;
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }
        private VarMap _input;
        [JsonPropertyName("input")]
        public VarMap Input
        {
            get => _input;
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        private bool _output1;
        [JsonPropertyName("output1")]
        public bool Output1
        {
            get => _output1;
            set
            {
                if (_output1 != value)
                {
                    _output1 = value;
                    OnPropertyChanged(nameof(Output1));
                }
            }
        }

        private bool _output2;
        [JsonPropertyName("output2")]
        public bool Output2
        {
            get => _output2;
            set
            {
                if (_output2 != value)
                {
                    _output2 = value;
                    OnPropertyChanged(nameof(Output2));
                }
            }
        }

        private bool _output3;
        [JsonPropertyName("output3")]
        public bool Output3
        {
            get => _output3;
            set
            {
                if (_output3 != value)
                {
                    _output3 = value;
                    OnPropertyChanged(nameof(Output3));
                }
            }
        }

        private bool _output4;
        [JsonPropertyName("output4")]
        public bool Output4
        {
            get => _output4;
            set
            {
                if (_output4 != value)
                {
                    _output4 = value;
                    OnPropertyChanged(nameof(Output4));
                }
            }
        }

        private bool _output5;
        [JsonPropertyName("output5")]
        public bool Output5
        {
            get => _output5;
            set
            {
                if (_output5 != value)
                {
                    _output5 = value;
                    OnPropertyChanged(nameof(Output5));
                }
            }
        }

        private bool _output6;
        [JsonPropertyName("output6")]
        public bool Output6
        {
            get => _output6;
            set
            {
                if (_output6 != value)
                {
                    _output6 = value;
                    OnPropertyChanged(nameof(Output6));
                }
            }
        }

        private bool _output7;
        [JsonPropertyName("output7")]
        public bool Output7
        {
            get => _output7;
            set
            {
                if (_output7 != value)
                {
                    _output7 = value;
                    OnPropertyChanged(nameof(Output7));
                }
            }
        }

        private bool _output8;
        [JsonPropertyName("output8")]
        public bool Output8
        {
            get => _output8;
            set
            {
                if (_output8 != value)
                {
                    _output8 = value;
                    OnPropertyChanged(nameof(Output8));
                }
            }
        }

        public static byte[] Request()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.StarterDisable);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 4) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            Input = (VarMap)(data[2]);
            Output1 = Convert.ToBoolean(data[3] & 0x01);
            Output2 = Convert.ToBoolean((data[3] & 0x02) >> 1);
            Output3 = Convert.ToBoolean((data[3] & 0x04) >> 2);
            Output4 = Convert.ToBoolean((data[3] & 0x08) >> 3);
            Output5 = Convert.ToBoolean((data[3] & 0x10) >> 4);
            Output6 = Convert.ToBoolean((data[3] & 0x20) >> 5);
            Output7 = Convert.ToBoolean((data[3] & 0x40) >> 6);
            Output8 = Convert.ToBoolean((data[3] & 0x80) >> 7);

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.StarterDisable);
            data[1] = Convert.ToByte(Convert.ToByte(Enabled) & 0x01);
            data[2] = Convert.ToByte(Input);
            data[3] = Convert.ToByte(((Convert.ToByte(Output8) & 0x01) << 7) +
                      ((Convert.ToByte(Output7) & 0x01) << 6) +
                      ((Convert.ToByte(Output6) & 0x01) << 5) +
                      ((Convert.ToByte(Output5) & 0x01) << 4) +
                      ((Convert.ToByte(Output4) & 0x01) << 3) +
                      ((Convert.ToByte(Output3) & 0x01) << 2) +
                      ((Convert.ToByte(Output2) & 0x01) << 1) +
                      (Convert.ToByte(Output1) & 0x01));
            return data;
        }
    }
}
