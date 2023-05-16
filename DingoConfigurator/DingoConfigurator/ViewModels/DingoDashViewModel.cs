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

        public DingoDashViewModel(MainViewModel vm)
        {
            _vm = vm;
        }
    }
}
