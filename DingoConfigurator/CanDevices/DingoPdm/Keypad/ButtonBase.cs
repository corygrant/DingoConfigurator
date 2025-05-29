using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CanDevices.DingoPdm;

namespace CanDevices.DingoPdm
{
    public class ButtonBase : NotifyPropertyChangedBase
    {
        private bool _state;
        [JsonIgnore]
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
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

        private int _keypadNumber;
        [JsonPropertyName("keypadNumber")]
        public int KeypadNumber
        {
            get => _keypadNumber;
            set
            {
                if (_keypadNumber != value)
                {
                    _keypadNumber = value;
                    OnPropertyChanged(nameof(KeypadNumber));
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

        private bool _hasDial;
        [JsonPropertyName("hasDial")]
        public bool HasDial
        {
            get => _hasDial;
            set
            {
                if (_hasDial != value)
                {
                    _hasDial = value;
                    OnPropertyChanged(nameof(HasDial));
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

        private VarMap[] _valVars;
        [JsonPropertyName("valVars")]
        public VarMap[] ValVars
        {
            get => _valVars;
            set
            {
                if (_valVars != value)
                {
                    _valVars = value;
                    OnPropertyChanged(nameof(ValVars));
                }
            }
        }

        private VarMap _faultVar;
        [JsonPropertyName("faultVar")]
        public VarMap FaultVar
        {
            get => _faultVar;
            set
            {
                if (_faultVar != value)
                {
                    _faultVar = value;
                    OnPropertyChanged(nameof(FaultVar));
                }
            }
        }

        public ButtonBase(int keypadNum, int buttonNum)
        {
            _valVars = new VarMap[4];
            KeypadNumber = keypadNum;
            Number = buttonNum;
        }
        public virtual List<CanDeviceResponse> RequestMsgs(int id)
        {
            throw new NotImplementedException();
        }

        public virtual bool Receive(byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual List<CanDeviceResponse> WriteMsgs(int id)
        {
            throw new NotImplementedException();
        }
    }
}
