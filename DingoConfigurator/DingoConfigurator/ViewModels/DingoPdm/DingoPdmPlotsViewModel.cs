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

        public PlotModel CurrentOutputPlotModel
        {
            get;
            private set;
        }

        private Timer _timer;

        private DateTime _zeroTime;

        public DingoPdmPlotsViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (DingoPdmCan)_vm.SelectedCanDevice;

            _pdm.PropertyChanged += _pdm_PropertyChanged;

            CurrentOutputPlotModel = new PlotModel { 
                Title = "Current Output",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
                
            };

            var currentAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Current",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.White,
                AbsoluteMinimum = -0.001,
                Unit = "A",
            };

            currentAxis.Zoom(0.0, 25.0);

            var timeAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.White,
                AbsoluteMinimum = 0,
                Unit = "s"
            };

            timeAxis.Zoom(0.0, 60.0);

            CurrentOutputPlotModel.Axes.Add(currentAxis);
            CurrentOutputPlotModel.Axes.Add(timeAxis);
             
            foreach (var output in _pdm.Outputs)
            {
                CurrentOutputPlotModel.Series.Add(new LineSeries());
            }

            _zeroTime = DateTime.Now;

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

            for (int i=0; i< CurrentOutputPlotModel.Series.Count; i++)
            {
               
                LineSeries series = (LineSeries)CurrentOutputPlotModel.Series[i];
                if (series == null) return;

                double val;
                if (!_pdm.IsConnected)
                {
                    val = 0;
                }
                else
                {
                    val = _pdm.Outputs[i].Current;
                }

                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), val));
                if (series.Points.Count > 10000)
                {
                    series.Points.RemoveAt(0);
                }
            }

            CurrentOutputPlotModel.InvalidatePlot(true);
        }

        public void ResetZoom()
        {
            CurrentOutputPlotModel.ResetAllAxes();
        }

        public override void Dispose()
        {
            _timer.Stop();
            _pdm.PropertyChanged -= _pdm_PropertyChanged;
            base.Dispose();
        }
    }
}
