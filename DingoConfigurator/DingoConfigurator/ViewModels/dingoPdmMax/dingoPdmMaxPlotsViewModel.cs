using CanDevices.dingoPdmMax;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DingoConfigurator.ViewModels
{
    public class dingoPdmMaxPlotsViewModel : DingoPdmPlotsViewModel
    {
        public dingoPdmMaxPlotsViewModel(MainViewModel vm) : base(vm)
        {
        }
    }
}
