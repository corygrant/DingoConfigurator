using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels.Keypad
{
    public class GrayhillViewModel : ViewModelBase
    {
        private MainViewModel _vm;

        public GrayhillViewModel(MainViewModel vm)
        {
            _vm = vm;
        }
    }
}
