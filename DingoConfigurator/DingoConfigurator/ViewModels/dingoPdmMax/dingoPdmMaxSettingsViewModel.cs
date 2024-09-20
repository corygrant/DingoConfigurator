using CanDevices.dingoPdmMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels
{
    public class dingoPdmMaxSettingsViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private dingoPdmMaxCan _pdm;
        public dingoPdmMaxCan Pdm { get { return _pdm; } }

        public dingoPdmMaxSettingsViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (dingoPdmMaxCan)_vm.SelectedCanDevice;

            _pdm.PropertyChanged += _pdm_PropertyChanged;
        }

        private void _pdm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public override void Dispose()
        {
            _pdm.PropertyChanged -= _pdm_PropertyChanged;
            base.Dispose();
        }
    }
}
