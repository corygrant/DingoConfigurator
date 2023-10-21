namespace CanDevices.CanBoard
{
    public class AnalogInput : NotifyPropertyChangedBase
    {
        private int _number { get; set; }
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

        private double _millivolts;
        public double Millivolts
        {
            get => _millivolts;
            set
            {
                if (_millivolts != value)
                {
                    _millivolts = value;
                    OnPropertyChanged(nameof(Millivolts));
                }
            }
        }

        private int _rotarySwitchPos;
        public int RotarySwitchPos
        {
            get => _rotarySwitchPos;
            set
            {
                if (_rotarySwitchPos != value)
                {
                    _rotarySwitchPos = value;
                    OnPropertyChanged(nameof(RotarySwitchPos));
                }
            }
        }

        private bool _digitalIn;
        public bool DigitalIn
        {
            get => _digitalIn;
            set
            {
                if (_digitalIn != value)
                {
                    _digitalIn = value;
                    OnPropertyChanged(nameof(DigitalIn));
                }
            }
        }
    }
}
