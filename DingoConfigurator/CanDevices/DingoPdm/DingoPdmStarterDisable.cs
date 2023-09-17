namespace CanDevices.DingoPdm
{
    public class DingoPdmStarterDisable : NotifyPropertyChangedBase
    {
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
        private VarMap _input;
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

    }
}
