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

        private double _lastCurrentOutputTimeZoomMin = 0;
        private double _lastCurrentOutputTimeZoomMax = 60;

        public PlotModel StatePlotModel
        {
            get;
            private set;
        }

        private double _lastStateTimeZoomMin = 0;
        private double _lastStateTimeZoomMax = 60;

        public PlotModel BatteryPlotModel
        {
            get;
            private set;
        }

        private double _lastBatteryTimeZoomMin = 0;
        private double _lastBatteryTimeZoomMax = 60;

        private Timer _timer;

        private DateTime _zeroTime;

        public DingoPdmPlotsViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (DingoPdmCan)_vm.SelectedCanDevice;

            _pdm.PropertyChanged += _pdm_PropertyChanged;

            CurrentOutputPlotModel = new PlotModel
            {
                Title = "Current Output",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White
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

            var currentTimeAxis = new LinearAxis
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

            currentTimeAxis.Zoom(0.0, 60.0);

            CurrentOutputPlotModel.Axes.Add(currentAxis);
            CurrentOutputPlotModel.Axes.Add(currentTimeAxis);

            foreach (var output in _pdm.Outputs)
            {
                CurrentOutputPlotModel.Series.Add(new LineSeries());
            }

            for (int i = 0; i < _pdm.Outputs.Count; i++)
            {
                LineSeries series = (LineSeries)CurrentOutputPlotModel.Series[i];
                series.Title = $"O{i}: {_pdm.Outputs[i].Name} Current";
            }


            StatePlotModel = new PlotModel
            {
                Title = "States",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,

            };

            var stateAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "State",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.Transparent,
                TicklineColor = OxyColors.Transparent,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.Transparent,
                AbsoluteMinimum = 0.0,
                AbsoluteMaximum = 1.2,
            };
            stateAxis.Zoom(0.0, 1.2);
            stateAxis.IsZoomEnabled = false;

            var stateTimeAxis = new LinearAxis
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
            stateTimeAxis.Zoom(0.0, 60.0);

            StatePlotModel.Axes.Add(stateAxis);
            StatePlotModel.Axes.Add(stateTimeAxis);

            foreach (var output in _pdm.Outputs)
            {
                StatePlotModel.Series.Add(new LineSeries());
            }

            for (int i = 0; i < _pdm.Outputs.Count; i++)
            {
                LineSeries series = (LineSeries)StatePlotModel.Series[i];
                series.Title = $"O{i}: {_pdm.Outputs[i].Name} State";
            }

            foreach (var digInput in _pdm.DigitalInputs)
            {
                StatePlotModel.Series.Add(new LineSeries());
            }

            for (int i = 0; i < _pdm.DigitalInputs.Count; i++)
            {
                LineSeries series = (LineSeries)StatePlotModel.Series[8 + i];
                series.Title = $"I{i}: {_pdm.DigitalInputs[i].Name}";
            }

            foreach (var canInput in _pdm.CanInputs)
            {
                StatePlotModel.Series.Add(new LineSeries());
            }

            for (int i = 0; i < _pdm.CanInputs.Count; i++)
            {
                LineSeries series = (LineSeries)StatePlotModel.Series[10 + i];
                series.Title = $"CI{i}: {_pdm.CanInputs[i].Name}";
            }

            foreach (var virtInput in _pdm.VirtualInputs)
            {
                StatePlotModel.Series.Add(new LineSeries());
            }

            for (int i = 0; i < _pdm.VirtualInputs.Count; i++)
            {
                LineSeries series = (LineSeries)StatePlotModel.Series[42 + i];
                series.Title = $"VI{i}: {_pdm.VirtualInputs[i].Name}";
            }

            BatteryPlotModel = new PlotModel
            {
                Title = "Battery Voltage",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,

            };

            var batteryAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Battery Voltage",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.White,
                AbsoluteMinimum = 0.0,
                Unit = "V"
            };
            batteryAxis.Zoom(0.0, 20.0);

            var batteryTimeAxis = new LinearAxis
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
            batteryTimeAxis.Zoom(0.0, 60.0);

            BatteryPlotModel.Axes.Add(batteryAxis);
            BatteryPlotModel.Axes.Add(batteryTimeAxis);

            BatteryPlotModel.Series.Add(new LineSeries());
            BatteryPlotModel.Series[0].Title = "Battery Voltage";

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

            if ((_lastCurrentOutputTimeZoomMin != CurrentOutputPlotModel.Axes[1].ActualMinimum) ||
                (_lastCurrentOutputTimeZoomMax != CurrentOutputPlotModel.Axes[1].ActualMaximum))
            {
                StatePlotModel.Axes[1].Zoom(CurrentOutputPlotModel.Axes[1].ActualMinimum, CurrentOutputPlotModel.Axes[1].ActualMaximum);
                BatteryPlotModel.Axes[1].Zoom(CurrentOutputPlotModel.Axes[1].ActualMinimum, CurrentOutputPlotModel.Axes[1].ActualMaximum);
            }

            if ((_lastStateTimeZoomMin != StatePlotModel.Axes[1].ActualMinimum) ||
                (_lastStateTimeZoomMax != StatePlotModel.Axes[1].ActualMaximum))
            {
                CurrentOutputPlotModel.Axes[1].Zoom(StatePlotModel.Axes[1].ActualMinimum, StatePlotModel.Axes[1].ActualMaximum);
                BatteryPlotModel.Axes[1].Zoom(StatePlotModel.Axes[1].ActualMinimum, StatePlotModel.Axes[1].ActualMaximum);
            }

            if ((_lastBatteryTimeZoomMin != BatteryPlotModel.Axes[1].ActualMinimum) ||
                (_lastBatteryTimeZoomMax != BatteryPlotModel.Axes[1].ActualMaximum))
            {
                CurrentOutputPlotModel.Axes[1].Zoom(BatteryPlotModel.Axes[1].ActualMinimum, BatteryPlotModel.Axes[1].ActualMaximum);
                StatePlotModel.Axes[1].Zoom(BatteryPlotModel.Axes[1].ActualMinimum, BatteryPlotModel.Axes[1].ActualMaximum);
            }

            _lastCurrentOutputTimeZoomMin = CurrentOutputPlotModel.Axes[1].ActualMinimum;
            _lastCurrentOutputTimeZoomMax = CurrentOutputPlotModel.Axes[1].ActualMaximum;

            _lastStateTimeZoomMin = StatePlotModel.Axes[1].ActualMinimum;
            _lastStateTimeZoomMax = StatePlotModel.Axes[1].ActualMaximum;

            _lastBatteryTimeZoomMin = BatteryPlotModel.Axes[1].ActualMinimum;
            _lastBatteryTimeZoomMax = BatteryPlotModel.Axes[1].ActualMaximum;

            for (int i = 0; i < CurrentOutputPlotModel.Series.Count; i++)
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
                //if (series.Points.Count > 10000)
                // {
                //     series.Points.RemoveAt(0);
                // }
            }

            for (int i = 0; i < StatePlotModel.Series.Count; i++)
            {
                LineSeries series = (LineSeries)StatePlotModel.Series[i];
                if (series == null) return;

                int val = 0;
                if (!_pdm.IsConnected)
                {
                    val = 0;
                }
                else
                {
                    if ((i >= 0) && (i < 8))
                    {
                        val = Convert.ToInt16(_pdm.Outputs[i].State == OutState.On);
                    }

                    if ((i >= 8) && (i < 10))
                    {
                        val = Convert.ToInt16(_pdm.DigitalInputs[i - 8].State);
                    }

                    if ((i >= 10) && (i < 42))
                    {
                        val = Convert.ToInt16(_pdm.CanInputs[i - 10].Value);
                    }

                    if ((i >= 42) && (i < 58))
                    {
                        val = Convert.ToInt16(_pdm.CanInputs[i - 42].Value);
                    }
                }

                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), val));
                // if (series.Points.Count > 10000)
                // {
                //      series.Points.RemoveAt(0);
                // }
            }

            LineSeries battSeries = (LineSeries)BatteryPlotModel.Series[0];
            if (battSeries != null)
            {
                battSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.BatteryVoltage));
                // if (battSeries.Points.Count > 10000)
                //  {
                //      battSeries.Points.RemoveAt(0);
                //  }
            }

            CurrentOutputPlotModel.InvalidatePlot(true);
            StatePlotModel.InvalidatePlot(true);
            BatteryPlotModel.InvalidatePlot(true);
        }

        public override void Dispose()
        {
            _timer.Stop();
            _pdm.PropertyChanged -= _pdm_PropertyChanged;
            base.Dispose();
        }
    }
}
