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

        public CanBoardViewModel(MainViewModel vm)
        {
            _vm = vm;
        }
    }
}
