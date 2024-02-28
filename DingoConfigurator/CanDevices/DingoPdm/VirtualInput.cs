using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class VirtualInput : NotifyPropertyChangedBase
    {
        private string _name;
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
    }
}
