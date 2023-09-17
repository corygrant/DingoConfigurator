using CanDevices.DingoDash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels
{
    public class DingoDashViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private DingoDashCan _dash;
        public DingoDashCan Dash { get => _dash; }

        public DingoDashViewModel(MainViewModel vm)
        {
            _vm = vm;
            _dash = (DingoDashCan)_vm.SelectedCanDevice;
            _dash.PropertyChanged += _dash_PropertyChanged;
        }

        private void _dash_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public override void Dispose()
        {
            _dash.PropertyChanged -= _dash_PropertyChanged;
            base.Dispose();
        }
    }
}
