using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels
{
    public class DingoPdmViewModel : ViewModelBase
    {
        private MainViewModel _vm;

        public DingoPdmViewModel(MainViewModel vm)
        {
            _vm = vm;
            _vm.DataUpdated += VmDataUpdated;
        }

        private bool _isConnected { get; set; }
        public bool IsConnected {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        public string BaseId
        {
            get => _vm.SelectedCanDevice?.BaseId.ToString();
        }

        private void VmDataUpdated(object sender)
        {
            IsConnected = _vm.SelectedCanDevice.IsConnected;
        }
    }
}
