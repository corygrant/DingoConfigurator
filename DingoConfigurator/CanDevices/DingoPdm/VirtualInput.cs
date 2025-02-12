using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class VirtualInput : NotifyPropertyChangedBase
    {
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

        private int _number;
        [JsonPropertyName("number")]
        public int Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private int _value;
        [JsonIgnore]
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

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

        private bool _not0;
        [JsonPropertyName("not0")]
        public bool Not0
        {
            get => _not0;
            set
            {
                if (_not0 != value)
                {
                    _not0 = value;
                    OnPropertyChanged(nameof(Not0));
                }
            }
        }

        private VarMap _var0;
        [JsonPropertyName("var0")]
        public VarMap Var0
        {
            get => _var0;
            set
            {
                if (_var0 != value)
                {
                    _var0 = value;
                    OnPropertyChanged(nameof(Var0));
                }
            }
        }

        private Conditional _cond0;
        [JsonPropertyName("cond0")]
        public Conditional Cond0
        {
            get => _cond0;
            set
            {
                if (_cond0 != value)
                {
                    _cond0 = value;
                    OnPropertyChanged(nameof(Cond0));
                }
            }
        }

        private bool _not1;
        [JsonPropertyName("not1")]
        public bool Not1
        {
            get => _not1;
            set
            {
                if (_not1 != value)
                {
                    _not1 = value;
                    OnPropertyChanged(nameof(Not1));
                }
            }
        }

        private VarMap _var1;
        [JsonPropertyName("var1")]
        public VarMap Var1
        {
            get => _var1;
            set
            {
                if (_var1 != value)
                {
                    _var1 = value;
                    OnPropertyChanged(nameof(Var1));
                }
            }
        }

        private Conditional _cond1;
        [JsonPropertyName("cond1")]
        public Conditional Cond1
        {
            get => _cond1;
            set
            {
                if (_cond1 != value)
                {
                    _cond1 = value;
                    OnPropertyChanged(nameof(Cond1));
                }
            }
        }

        private bool _not2;
        [JsonPropertyName("not2")]
        public bool Not2
        {
            get => _not2;
            set
            {
                if (_not2 != value)
                {
                    _not2 = value;
                    OnPropertyChanged(nameof(Not2));
                }
            }
        }

        private VarMap _var2;
        [JsonPropertyName("var2")]
        public VarMap Var2
        {
            get => _var2;
            set
            {
                if (_var2 != value)
                {
                    _var2 = value;
                    OnPropertyChanged(nameof(Var2));
                }
            }
        }

        private InputMode _mode;
        [JsonPropertyName("mode")]
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        public static byte[] Request(int index)
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.VirtualInputs);
            data[1] = Convert.ToByte(index);
            return data;
        }

        public bool Receive(byte[] data)
        {
            if (data.Length != 7) return false;

            Enabled = Convert.ToBoolean(data[1] & 0x01);
            Not0 = Convert.ToBoolean((data[1] & 0x02) >> 1);
            Not1 = Convert.ToBoolean((data[1] & 0x04) >> 2);
            Not2 = Convert.ToBoolean((data[1] & 0x08) >> 3);
            Var0 = (VarMap)data[3];
            Var1 = (VarMap)data[4];
            Var2 = (VarMap)data[5];
            Mode = (InputMode)((data[6] & 0xC0) >> 6);
            Cond0 = (Conditional)(data[6] & 0x03);
            Cond1 = (Conditional)((data[6] & 0x0C) >> 2);

            return true;
        }

        public byte[] Write()
        {
            byte[] data = new byte[8];
            data[0] = Convert.ToByte(MessagePrefix.VirtualInputs);
            data[1] = Convert.ToByte((Convert.ToByte(Not2) << 3) +
                      (Convert.ToByte(Not1) << 2) +
                      (Convert.ToByte(Not0) << 1) +
                      Convert.ToByte(Enabled));
            data[2] = Convert.ToByte(Number - 1);
            data[3] = Convert.ToByte(Var0);
            data[4] = Convert.ToByte(Var1);
            data[5] = Convert.ToByte(Var2);
            data[6] = Convert.ToByte(((Convert.ToByte(Mode) & 0x03) << 0x06) +
                      ((Convert.ToByte(Cond1) & 0x03) << 2) +
                      (Convert.ToByte(Cond0) & 0x03));
            return data;
        }
    }
}
