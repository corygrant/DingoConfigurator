using CanDevices.DingoPdm;
using DingoConfigurator.ViewModels.DingoPdm.Plots;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public CurrentOutputPlot CurrentOutputPlot { get; private set; }
        public StatePlot StatePlot { get; private set; }
        public BatteryPlot BatteryPlot { get; private set; }


        private Timer _timer;
        private DateTime _zeroTime;
        private bool _isZoomUpdating;

        public DingoPdmPlotsViewModel(MainViewModel vm)
        {
            _vm = vm;

            _pdm = (DingoPdmCan)_vm.SelectedCanDevice;
       
            _pdm.PropertyChanged += _pdm_PropertyChanged;

            CurrentOutputPlot = new CurrentOutputPlot();
            StatePlot = new StatePlot();
            BatteryPlot = new BatteryPlot();

            InitPlotEvents();
            InitSeries();
            InitPlots();
            InitTimer();
        }

        private void InitPlotEvents()
        {
            foreach(var output in _pdm.Outputs)
                output.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var input in _pdm.DigitalInputs)
                input.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var input in _pdm.CanInputs)
                input.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var input in _pdm.VirtualInputs)
                input.PropertyChanged += PlotVar_PropertyChanged;

        }

        private void PlotVar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(Output.Plot)) ||
                (e.PropertyName == nameof(Input.Plot)) ||
                (e.PropertyName == nameof(CanInput.Plot)) ||
                (e.PropertyName == nameof(VirtualInput.Plot)))
            {
                InitSeries();
            }
        }

        private void InitPlots()
        {
            //AxisChanged event to be deprecated, but it hasn't been yet
            //https://github.com/oxyplot/oxyplot/issues/111

            var currentTimeAxis = CurrentOutputPlot.PlotModel.Axes.FirstOrDefault(a => a.Key == "Time");
            if (currentTimeAxis != null)
            {
                currentTimeAxis.AxisChanged += TimeAxis_AxisChanged;
            }

            var stateTimeAxis = StatePlot.PlotModel.Axes.FirstOrDefault(a => a.Key == "Time");
            if (stateTimeAxis != null)
            {
                stateTimeAxis.AxisChanged += TimeAxis_AxisChanged;
            }

            var batteryTimeAxis = BatteryPlot.PlotModel.Axes.FirstOrDefault(a => a.Key == "Time");
            if (batteryTimeAxis != null)
            {
                batteryTimeAxis.AxisChanged += TimeAxis_AxisChanged;
            }
        }

        private void TimeAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            if(_isZoomUpdating) return;

            _isZoomUpdating = true;

            if (sender is Axis axis)
            {
                double minimum = axis.ActualMinimum;
                double maximum = axis.ActualMaximum;

                if (axis.PlotModel == CurrentOutputPlot.PlotModel)
                {
                    StatePlot.UpdateTimeAxisZoom(minimum, maximum);
                    BatteryPlot.UpdateTimeAxisZoom(minimum, maximum);
                }
                else if (axis.PlotModel == StatePlot.PlotModel)
                {
                    CurrentOutputPlot.UpdateTimeAxisZoom(minimum, maximum);
                    BatteryPlot.UpdateTimeAxisZoom(minimum, maximum);
                }
                else if (axis.PlotModel == BatteryPlot.PlotModel)
                {
                    CurrentOutputPlot.UpdateTimeAxisZoom(minimum, maximum);
                    StatePlot.UpdateTimeAxisZoom(minimum, maximum);
                }
            }

            _isZoomUpdating = false;
        }

        private void InitTimer()
        {
            _zeroTime = DateTime.Now;
            _timer = new Timer(100);
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += Timer_Elapsed;
        }

        private void OnPdmConnected()
        {
            if(_timer.Enabled == false)
            {
                _timer.Start();
            }
        }

        private void OnPdmDisconnected()
        {
            _timer.Stop();
        }

        private void ClearData()
        {
            CurrentOutputPlot.ClearData();
            StatePlot.ClearData();
            BatteryPlot.ClearData();

            CurrentOutputPlot.UpdateTimeAxisZoom(0.0, 60.0);
            StatePlot.UpdateTimeAxisZoom(0.0, 60.0);
            BatteryPlot.UpdateTimeAxisZoom(0.0, 60.0);
        }

        private void InitSeries()
        {
            for (int i = 0; i < _pdm.Outputs.Count; i++)
            {
                if (!_pdm.Outputs[i].Plot) continue;

                CurrentOutputPlot.AddSeries($"O{i}", $"O{i}: {_pdm.Outputs[i].Name}");
                StatePlot.AddSeries($"O{i}", $"O{i}: {_pdm.Outputs[i].Name}");

                if (_pdm.Outputs[i].PwmEnabled)
                    CurrentOutputPlot.AddSeries($"OC{i}", $"O{i} Calc: {_pdm.Outputs[i].Name}", LineStyle.Dot);
            }

            for (int i = 0; i < _pdm.DigitalInputs.Count; i++)
            {
                if (!_pdm.DigitalInputs[i].Plot) continue;
                StatePlot.AddSeries($"I{i}", $"I{i}: {_pdm.DigitalInputs[i].Name}");
            }

            for (int i = 0; i < _pdm.CanInputs.Count; i++)
            {
                if (!_pdm.CanInputs[i].Plot) continue;
                StatePlot.AddSeries($"CI{i}", $"CI{i}: {_pdm.CanInputs[i].Name}");
            }

            for (int i = 0; i < _pdm.VirtualInputs.Count; i++)
            {
                if (!_pdm.VirtualInputs[i].Plot) continue;
                StatePlot.AddSeries($"VI{i}", $"VI{i}: {_pdm.VirtualInputs[i].Name}");
            }

            BatteryPlot.AddSeries("Battery", "Battery Voltage");
        }

        private void _pdm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DingoPdmCan.IsConnected))
            {
                if (!_pdm.IsConnected)
                {
                    OnPdmDisconnected();
                }
                else
                {
                    OnPdmConnected();
                }
            }

            OnPropertyChanged(e.PropertyName);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_pdm == null) return;

            for (int i = 0; i < _pdm.Outputs.Count; i++)
            {
                if(!_pdm.Outputs[i].Plot) continue;
                CurrentOutputPlot.AddPoint($"O{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? _pdm.Outputs[i].Current : 0);
                CurrentOutputPlot.AddPoint($"OC{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? _pdm.Outputs[i].CalculatedPower : 0);
                StatePlot.AddPoint($"O{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(_pdm.Outputs[i].State == OutState.On) : 0);
            }

            for (int i = 0; i < _pdm.DigitalInputs.Count; i++)
            {
                if(!_pdm.DigitalInputs[i].Plot) continue;
                StatePlot.AddPoint($"I{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(_pdm.DigitalInputs[i].State) : 0);
            }

            for (int i = 0; i < _pdm.CanInputs.Count; i++)
            {
                if (!_pdm.CanInputs[i].Plot) continue;
                StatePlot.AddPoint($"CI{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? _pdm.CanInputs[i].Value : 0);
            }

            for (int i = 0; i < _pdm.VirtualInputs.Count; i++)
            {
                if (!_pdm.VirtualInputs[i].Plot) continue;
                StatePlot.AddPoint($"VI{i}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(_pdm.VirtualInputs[i].Value) : 0);
            }

            BatteryPlot.AddPoint("Battery", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.BatteryVoltage);

            CurrentOutputPlot.Refresh();
            StatePlot.Refresh();
            BatteryPlot.Refresh();
        }

        public override void Dispose()
        {
            //_timer.Stop();
            //_pdm.PropertyChanged -= _pdm_PropertyChanged;
            base.Dispose();
        }
    }
}
