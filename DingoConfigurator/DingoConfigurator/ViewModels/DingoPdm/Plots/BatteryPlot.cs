using DingoConfigurator.Plots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels.DingoPdm.Plots
{
    public class BatteryPlot : PlotBase
    {
        public BatteryPlot() : base("Battery", true)
        {
            AddAxis("Value", OxyPlot.Axes.AxisPosition.Left, "Voltage", "V", 0.0, 16.0);
            AddAxis("Time", OxyPlot.Axes.AxisPosition.Bottom, "Time", "s", 0);
        }
    }
}
