using DingoConfigurator.Plots;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels.DingoPdm.Plots
{
    public class CurrentOutputPlot : PlotBase
    {
        public CurrentOutputPlot() : base("Current Output", true)
        {
            AddAxis("Value", AxisPosition.Left, "Current", "A", -0.001);
            AddAxis("Time", AxisPosition.Bottom, "Time", "s", 0);
        }
    }
}
