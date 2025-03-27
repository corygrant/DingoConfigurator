using DingoConfigurator.Plots;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoConfigurator.ViewModels.DingoPdm.Plots
{
    public class StatePlot : PlotBase
    {
        public StatePlot() : base("State", true)
        {
            AddAxis("Value", AxisPosition.Left, "State", "", 0.0, 1.2);
            AddAxis("Time", AxisPosition.Bottom, "Time", "s", 0);
        }
    }
}
