using CanDevices.CanMsgLog;
using CanDevices.SoftButtonBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DingoConfigurator.ViewModels
{
    public class SoftButtonBoxViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private SoftButtonBox _sbb;
        public SoftButtonBox SoftBtnBox { get { return _sbb; } }
        public ICommand ClearBtnCmd { get; set; }

        public IEnumerable<CanMsgLog.NumberFormat> IdFormat
        {
            get
            {
                return (IEnumerable<CanMsgLog.NumberFormat>)System.Enum.GetValues(typeof(CanMsgLog.NumberFormat));
            }
        }

        public SoftButtonBoxViewModel(MainViewModel vm)
        {
            _vm = vm;

            _sbb = (SoftButtonBox)_vm.SelectedCanDevice;

            _sbb.PropertyChanged += _sbb_PropertyChanged;

        }

        private void _sbb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

     

        public override void Dispose()
        {
            _sbb.PropertyChanged -= _sbb_PropertyChanged;
            base.Dispose();
        }
    }
}
