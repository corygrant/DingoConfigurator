using CanDevices.CanBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels
{
    public class CanBoardViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private CanBoardCan _cb;
        public CanBoardCan Cb { get { return _cb; } }

        public CanBoardViewModel(MainViewModel vm)
        {
            _vm = vm;
            _cb = (CanBoardCan)_vm.SelectedCanDevice;
            _cb.PropertyChanged += _cb_PropertyChanged;
        }

        private void _cb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);  
        }

        public override void Dispose()
        {
            _cb.PropertyChanged -= _cb_PropertyChanged;
            base.Dispose();
        }
    }
}
