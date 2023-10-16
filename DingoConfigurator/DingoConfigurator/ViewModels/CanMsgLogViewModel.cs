using CanDevices.CanMsgLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DingoConfigurator.ViewModels
{
    public class CanMsgLogViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private CanMsgLog _msgLog;
        public CanMsgLog MsgLog { get { return _msgLog; } }
        public ICommand ClearBtnCmd { get; set; }

        public  IEnumerable<CanMsgLog.NumberFormat> IdFormat
        {
            get
            {
                return (IEnumerable<CanMsgLog.NumberFormat>)System.Enum.GetValues(typeof(CanMsgLog.NumberFormat));
            }
        }

        private CanMsgLog.NumberFormat _selectedIdFormat;
        public CanMsgLog.NumberFormat SelectedIdFormat
        {
            get => _selectedIdFormat;
            set
            {
                _selectedIdFormat = value;
                OnPropertyChanged(nameof(SelectedIdFormat));
                _msgLog.IdFormat = value;
            }
        }

        public IEnumerable<CanMsgLog.NumberFormat> PayloadFormat
        {
            get
            {
                return (IEnumerable<CanMsgLog.NumberFormat>)System.Enum.GetValues(typeof(CanMsgLog.NumberFormat));
            }
        }

        private CanMsgLog.NumberFormat _selectedPayloadFormat;
        public CanMsgLog.NumberFormat SelectedPayloadFormat
        {
            get => _selectedPayloadFormat;
            set
            {
                _selectedPayloadFormat = value;
                OnPropertyChanged(nameof(SelectedPayloadFormat));
                _msgLog.PayloadFormat = value;
            }
        }

        public CanMsgLogViewModel(MainViewModel vm)
        {
            _vm = vm;

            _msgLog = (CanMsgLog)_vm.SelectedCanDevice;

            _msgLog.PropertyChanged += _msgLog_PropertyChanged;

            ClearBtnCmd = new RelayCommand(Clear, CanClear);
        }

        private void _msgLog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void Clear(object parameter)
        {
            _msgLog.ClearAll();
        }

        private bool CanClear(object parameter)
        {
            return true;
        }

        public override void Dispose()
        {
            _msgLog.PropertyChanged -= _msgLog_PropertyChanged;
            base.Dispose();
        }
    }
}
