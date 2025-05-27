using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanDevices.DingoPdm;

namespace DingoConfigurator.ViewModels
{
    public class KeypadViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private Keypad _kp;
        public Keypad Kp {get { return _kp; }}

        public KeypadViewModel(MainViewModel vm)
        {
            _vm = vm;

            if (_vm.SelectedItem == null)
            {
                throw new InvalidOperationException("No item is selected.");
            }

            if (_vm.SelectedItem.GetType() == typeof(Keypad))
            {
                _kp = (Keypad)_vm.SelectedItem;
            }
            else if (_vm.SelectedCanDevice.GetType() == typeof(DingoPdmCan))
            {
                _kp = ((DingoPdmCan)_vm.SelectedCanDevice).Keypads.FirstOrDefault();
            }
            else
            {
                throw new InvalidOperationException("Selected item is not a Keypad or DingoPdmCan.");
            }
        }

    }
}
