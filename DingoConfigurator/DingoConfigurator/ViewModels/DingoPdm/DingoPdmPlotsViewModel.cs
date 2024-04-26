using CanDevices.DingoPdm;
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
    public class DingoPdmPlotsViewModel : ViewModelBase
    {
        private MainViewModel _vm;
        private DingoPdmCan _pdm;
        public DingoPdmCan Pdm { get { return _pdm; } }

        public PlotModel CurrentOutput
        {
            get;
            private set;
        }

        private Timer _timer;

        public DingoPdmPlotsViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (DingoPdmCan)_vm.SelectedCanDevice;

            _pdm.PropertyChanged += _pdm_PropertyChanged;

            CurrentOutput = new PlotModel { 
                Title = "Current Output",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Current (A)",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.White,
                AbsoluteMinimum = -0.001
            };
            
            CurrentOutput.Axes.Add(yAxis);
            
             
            foreach (var output in _pdm.Outputs)
            {
                CurrentOutput.Series.Add(new LineSeries());
            }

            _timer = new Timer(100);
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += Timer_Elapsed;
        }

        private void _pdm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_pdm == null) return;
            if (!_pdm.IsConnected) return;

            for (int i=0; i<CurrentOutput.Series.Count; i++)
            {
                LineSeries series = (LineSeries) CurrentOutput.Series[i];
                if (series == null) return;

                series.Points.Add(new DataPoint(DateTime.Now.ToOADate(), _pdm.Outputs[i].Current));
                if (series.Points.Count > 10000)
                {
                    series.Points.RemoveAt(0);
                }
            }

            CurrentOutput.InvalidatePlot(true);
        }

        public override void Dispose()
        {
            _timer.Stop();
            _pdm.PropertyChanged -= _pdm_PropertyChanged;
            base.Dispose();
        }
    }
}
