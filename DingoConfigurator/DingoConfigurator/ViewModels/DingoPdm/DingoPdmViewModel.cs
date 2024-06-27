using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DingoConfigurator.ViewModels
{
    public class DingoPdmViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private DingoPdmCan _pdm;
        public DingoPdmCan Pdm { get { return _pdm; } }

        public DingoPdmViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (DingoPdmCan)_vm.SelectedCanDevice;

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
