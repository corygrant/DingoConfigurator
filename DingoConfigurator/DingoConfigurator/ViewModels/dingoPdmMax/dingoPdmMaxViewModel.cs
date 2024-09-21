using CanDevices.DingoPdm;
using CanDevices.dingoPdmMax;
using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace DingoConfigurator.ViewModels
{
    public class dingoPdmMaxViewModel : DingoPdmViewModel
    {

        public dingoPdmMaxViewModel(MainViewModel vm) : base(vm)
        {
        }
    }
}

