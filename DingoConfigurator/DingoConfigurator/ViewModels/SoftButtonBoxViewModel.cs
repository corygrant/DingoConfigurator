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
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Net.Http.Headers;

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
            UpdateLedColors(); // Initialize LED colors with current state
        }

        private void _sbb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            
            if (e.PropertyName == nameof(SoftButtonBox.KeypadModel))
            {
                // Use Dispatcher to delay the update until after the emulator is initialized
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    UpdateButtonStates();
                    UpdateLedColors(); // Refresh LED colors when model changes
                    OnPropertyChanged(nameof(KeypadEmulator));
                    OnPropertyChanged(nameof(NumButtons));
                    OnPropertyChanged(nameof(NumDials));
                    OnPropertyChanged(nameof(HasDials));
                    OnPropertyChanged(nameof(ColorsEnabled));
                    OnPropertyChanged(nameof(ButtonColumns));
                }));
            }
            else if (e.PropertyName == nameof(SoftButtonBox.LedStatesChanged))
            {
                // Update LED colors when LED control messages are received
                UpdateLedColors();
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

        public int ButtonColumns
        {
            get
            {
                if (NumButtons <= 6) return 6;
                if (NumButtons == 8) return 4;
                if (NumButtons == 10) return 5;
                if (NumButtons == 12) return 6;
                if (NumButtons == 13) return 6;
                if (NumButtons == 15) return 5;
                if (NumButtons == 20) return 5;

                return 6;
            }
        }

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

        private void UpdateLedColors()
        {
            for (int i = 0; i < _buttonStates.Count; i++)
            {
                string newColor = _sbb?.GetLedColorName(i) ?? "Off";
                string newBlinkColor = _sbb?.GetLedBlinkColorName(i) ?? "Off";
                _buttonStates[i].LedColor = newColor;
                _buttonStates[i].LedBlinkColor = newBlinkColor;
            }
        }


        public override void Dispose()
        {
            if (_sbb != null)
            {
                _sbb.PropertyChanged -= _sbb_PropertyChanged;
            }
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
                    OnPropertyChanged(nameof(ButtonBorder));
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
                    OnPropertyChanged(nameof(ButtonBackground));
                }
            }
        }
        
        private string _ledBlinkColor = "Off";
        public string LedBlinkColor 
        { 
            get => _ledBlinkColor;
            set
            {
                if (_ledBlinkColor != value)
                {
                    _ledBlinkColor = value;
                    OnPropertyChanged(nameof(LedBlinkColor));
                    OnPropertyChanged(nameof(ButtonBackground));
                    OnPropertyChanged(nameof(HasBlinkColor));
                }
            }
        }
        
        public bool HasBlinkColor => LedBlinkColor != "Off";

        public System.Windows.Media.Brush ButtonBackground
        {
            get
            {
                var mainColor = ConvertColorNameToBrush(LedColor);
                var blinkColor = ConvertColorNameToBrush(LedBlinkColor);
                
                // If no blink color, return main color
                if (LedBlinkColor == "Off")
                {
                    return mainColor;
                }
                
                // If there's a blink color, create a gradient brush with both colors
                var gradientBrush = new System.Windows.Media.LinearGradientBrush();
                gradientBrush.StartPoint = new System.Windows.Point(0, 0);
                gradientBrush.EndPoint = new System.Windows.Point(1, 0);
                gradientBrush.GradientStops.Add(new System.Windows.Media.GradientStop(
                    ((System.Windows.Media.SolidColorBrush)mainColor).Color, 0.5));
                gradientBrush.GradientStops.Add(new System.Windows.Media.GradientStop(
                    ((System.Windows.Media.SolidColorBrush)blinkColor).Color, 0.5));
                
                return gradientBrush;
            }
        }

        public System.Windows.Media.Brush ButtonBorder
        {
            get
            {
                if(IsPressed)
                    return System.Windows.Media.Brushes.Lime;

                return System.Windows.Media.Brushes.Gray;
            }
        }
        
        private System.Windows.Media.Brush ConvertColorNameToBrush(string colorName)
        {
            switch (colorName?.ToLower())
            {
                case "red": return System.Windows.Media.Brushes.Red;
                case "green": return System.Windows.Media.Brushes.Lime;
                case "blue": return System.Windows.Media.Brushes.Blue;
                case "orange": return System.Windows.Media.Brushes.Orange;
                case "violet": return System.Windows.Media.Brushes.DarkViolet;
                case "cyan": return System.Windows.Media.Brushes.Cyan;
                case "white": return System.Windows.Media.Brushes.White;
                case "yellow": return System.Windows.Media.Brushes.Yellow;
                default: return System.Windows.Media.Brushes.Gray;
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
