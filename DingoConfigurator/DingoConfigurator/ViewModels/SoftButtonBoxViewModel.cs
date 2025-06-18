using CanDevices.CanMsgLog;
using CanDevices.SoftButtonBox;
using CanDevices.DingoPdm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CanDevices;

namespace DingoConfigurator.ViewModels
{
    public class SoftButtonBoxViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private SoftButtonBox _sbb;
        public SoftButtonBox SoftBtnBox { get { return _sbb; } }

        public SoftButtonBoxViewModel(MainViewModel vm)
        {
            _vm = vm;

            _sbb = (SoftButtonBox)_vm.SelectedCanDevice;

            _sbb.PropertyChanged += _sbb_PropertyChanged;

            ButtonPressCommand = new RelayCommand(OnButtonPress, CanExecuteButton);
            ButtonReleaseCommand = new RelayCommand(OnButtonRelease, CanExecuteButton);
            DialValueChangedCommand = new RelayCommand(OnDialValueChanged, CanExecuteCommand);

            UpdateButtonStates();
        }

        private void _sbb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            
            if (e.PropertyName == nameof(SoftButtonBox.KeypadModel))
            {
                UpdateButtonStates();
                OnPropertyChanged(nameof(KeypadEmulator));
                OnPropertyChanged(nameof(NumButtons));
                OnPropertyChanged(nameof(NumDials));
                OnPropertyChanged(nameof(HasDials));
                OnPropertyChanged(nameof(ColorsEnabled));
            }
        }

        public IKeypadEmulator KeypadEmulator => _sbb?.KeypadEmulator;

        public IEnumerable<KeypadModel> KeypadModels => 
            Enum.GetValues(typeof(KeypadModel)).Cast<KeypadModel>();

        public KeypadModel SelectedKeypadModel
        {
            get => _sbb?.KeypadModel ?? KeypadModel.Blink12Key;
            set
            {
                if (_sbb != null && _sbb.KeypadModel != value)
                {
                    _sbb.KeypadModel = value;
                    OnPropertyChanged(nameof(SelectedKeypadModel));
                }
            }
        }

        public int NumButtons => KeypadEmulator?.NumButtons ?? 0;
        public int NumDials => KeypadEmulator?.NumDials ?? 0;
        public bool HasDials => NumDials > 0;
        public bool ColorsEnabled => KeypadEmulator?.ColorsEnabled ?? false;

        private ObservableCollection<ButtonState> _buttonStates = new ObservableCollection<ButtonState>();
        public ObservableCollection<ButtonState> ButtonStates => _buttonStates;

        private ObservableCollection<DialState> _dialStates = new ObservableCollection<DialState>();
        public ObservableCollection<DialState> DialStates => _dialStates;

        public ICommand ButtonPressCommand { get; }
        public ICommand ButtonReleaseCommand { get; }
        public ICommand DialValueChangedCommand { get; }

        private void OnButtonPress(object parameter)
        {
            if (parameter is int buttonIndex && buttonIndex >= 0 && buttonIndex < _buttonStates.Count)
            {
                bool newState = !_buttonStates[buttonIndex].IsPressed;
                _sbb?.SetButtonState(buttonIndex, newState);
                _buttonStates[buttonIndex].IsPressed = newState;
            }
        }

        private void OnButtonRelease(object parameter)
        {
            // Not used in toggle mode
        }

        private void OnDialValueChanged(object parameter)
        {
            if (parameter is object[] args && args.Length == 2 &&
                args[0] is int dialIndex && args[1] is int value)
            {
                _sbb?.SetDialValue(dialIndex, value);
                UpdateDialState(dialIndex);
            }
        }

        private bool CanExecuteButton(object parameter)
        {
            return _sbb != null && parameter is int;
        }

        private bool CanExecuteCommand(object parameter)
        {
            return _sbb != null;
        }

        private void UpdateButtonStates()
        {
            _buttonStates.Clear();
            
            for (int i = 0; i < NumButtons; i++)
            {
                _buttonStates.Add(new ButtonState
                {
                    Index = i,
                    Number = i + 1,
                    IsPressed = false,
                    LedColor = "Off"
                });
            }

            _dialStates.Clear();
            
            for (int i = 0; i < NumDials; i++)
            {
                var dialState = new DialState
                {
                    Index = i,
                    Number = i + 1,
                    Value = 0,
                    MinValue = -32768,
                    MaxValue = 32767
                };
                
                // Subscribe to value changes
                dialState.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(DialState.Value) && s is DialState dial)
                    {
                        _sbb?.SetDialValue(dial.Index, dial.Value);
                    }
                };
                
                _dialStates.Add(dialState);
            }

            OnPropertyChanged(nameof(ButtonStates));
            OnPropertyChanged(nameof(DialStates));
        }

        private void UpdateButtonState(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < _buttonStates.Count)
            {
                var states = _sbb?.GetButtonStates();
                if (states != null && buttonIndex < states.Length)
                {
                    _buttonStates[buttonIndex].IsPressed = states[buttonIndex];
                }
            }
        }

        private void UpdateDialState(int dialIndex)
        {
            if (dialIndex >= 0 && dialIndex < _dialStates.Count)
            {
                var values = _sbb?.GetDialValues();
                if (values != null && dialIndex < values.Length)
                {
                    _dialStates[dialIndex].Value = values[dialIndex];
                }
            }
        }

        public override void Dispose()
        {
            _sbb.PropertyChanged -= _sbb_PropertyChanged;
            base.Dispose();
        }
    }

    public class ButtonState : NotifyPropertyChangedBase
    {
        public int Index { get; set; }
        public int Number { get; set; }
        
        private bool _isPressed;
        public bool IsPressed 
        { 
            get => _isPressed;
            set
            {
                if (_isPressed != value)
                {
                    _isPressed = value;
                    OnPropertyChanged(nameof(IsPressed));
                }
            }
        }
        
        private string _ledColor = "Off";
        public string LedColor 
        { 
            get => _ledColor;
            set
            {
                if (_ledColor != value)
                {
                    _ledColor = value;
                    OnPropertyChanged(nameof(LedColor));
                }
            }
        }
    }

    public class DialState : NotifyPropertyChangedBase
    {
        public int Index { get; set; }
        public int Number { get; set; }
        
        private int _value;
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
        
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
