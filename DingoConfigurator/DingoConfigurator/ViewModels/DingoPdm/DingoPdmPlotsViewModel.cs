using CanDevices.DingoPdm;
using DingoConfigurator.ViewModels.DingoPdm.Plots;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;

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


        private System.Timers.Timer _timer;
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

            ClearBtnCmd = new RelayCommand(ClearBtn, CanClearBtn);
            ExportBtnCmd = new RelayCommand(ExportBtn, CanExportBtn);

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

            foreach (var cond in _pdm.Conditions)
                cond.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var counter in _pdm.Counters)
                counter.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var flasher in _pdm.Flashers)
                flasher.PropertyChanged += PlotVar_PropertyChanged;

            foreach (var keypad in _pdm.Keypads)
            {
                foreach (var button in keypad.AllButtons)
                    button.PropertyChanged += PlotVar_PropertyChanged;

                foreach (var dial in keypad.Dials)
                    dial.PropertyChanged += PlotVar_PropertyChanged;
            }
        }

        private void PlotVar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(Output.Plot)) ||
                (e.PropertyName == nameof(Input.Plot)) ||
                (e.PropertyName == nameof(CanInput.Plot)) ||
                (e.PropertyName == nameof(VirtualInput.Plot)) ||
                (e.PropertyName == nameof(Condition.Plot)) ||
                (e.PropertyName == nameof(Counter.Plot)) ||
                (e.PropertyName == nameof(Flasher.Plot)) ||
                (e.PropertyName == nameof(CanDevices.DingoPdm.Button.Plot)) ||
                (e.PropertyName == nameof(Dial.Plot)))
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

            CurrentOutputPlot.UpdateTimeAxisZoom(0.0, 60.0);
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
            _timer = new System.Timers.Timer(100);
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
            _timer.Stop();

            CurrentOutputPlot.ClearData();
            StatePlot.ClearData();
            BatteryPlot.ClearData();

            CurrentOutputPlot.UpdateTimeAxisZoom(0.0, 60.0);
            StatePlot.UpdateTimeAxisZoom(0.0, 60.0);
            BatteryPlot.UpdateTimeAxisZoom(0.0, 60.0);

            BatteryPlot.ResetValueZoom();

            _zeroTime = DateTime.Now;

            _timer.Start();
        }

        private void InitSeries()
        {
            foreach(var output in _pdm.Outputs)
            {
                if (!output.Plot)
                {
                    CurrentOutputPlot.RemoveSeries($"O{output.Number}");
                    CurrentOutputPlot.RemoveSeries($"OC{output.Number}");
                    continue;
                }

                CurrentOutputPlot.AddSeries($"O{output.Number}", $"O{output.Number}: {output.Name}");
                StatePlot.AddSeries($"O{output.Number}", $"O{output.Number} :  {output.Name}");

                if (output.PwmEnabled)
                    CurrentOutputPlot.AddSeries($"OC{output.Number}", $"O{output.Number} Calc: {output.Name}", LineStyle.Dot);
            }

            foreach(var input in _pdm.DigitalInputs)
            {
                if (!input.Plot)
                {
                    StatePlot.RemoveSeries($"I{input.Number}");
                    continue;
                }
                
                StatePlot.AddSeries($"I{input.Number}", $"I{input.Number}: {input.Name}");
            }

            foreach(var input in _pdm.CanInputs)
            {
                if (!input.Plot)
                {
                    StatePlot.RemoveSeries($"CI{input.Number}");
                    continue;
                }

                StatePlot.AddSeries($"CI{input.Number}", $"CI{input.Number}: {input.Name}");
            }

            foreach (var input in _pdm.VirtualInputs)
            {
                if (!input.Plot)
                {
                    StatePlot.RemoveSeries($"VI{input.Number}");
                    continue;
                }

                StatePlot.AddSeries($"VI{input.Number}", $"VI{input.Number}: {input.Name}");
            }

            foreach (var cond in _pdm.Conditions)
            {
                if (!cond.Plot)
                {
                    StatePlot.RemoveSeries($"CON{cond.Number}");
                    continue;
                }
                
                StatePlot.AddSeries($"CON{cond.Number}", $"CON{cond.Number}: {cond.Name}");
            }

            foreach (var counter in _pdm.Counters)
            {
                if (!counter.Plot)
                {
                    StatePlot.RemoveSeries($"CNT{counter.Number}");
                    continue;
                }

                StatePlot.AddSeries($"CNT{counter.Number}", $"CNT{counter.Number}: {counter.Name}");
            }

            foreach (var flasher in _pdm.Flashers)
            {
                if (!flasher.Plot)
                {
                    StatePlot.RemoveSeries($"FLS{flasher.Number}");
                    continue;
                }
                
                StatePlot.AddSeries($"FLS{flasher.Number}", $"FLS{flasher.Number}: {flasher.Name}");
            }

            foreach (var keypad in _pdm.Keypads)
            {
                foreach (var button in keypad.AllButtons)
                {
                    if (!button.Plot)
                    {
                        StatePlot.RemoveSeries($"KB{keypad.Number}B{button.Number}");
                        continue;
                    }
                    StatePlot.AddSeries($"KB{keypad.Number}B{button.Number}", $"KB{keypad.Number}B{button.Number}: {button.Name}");
                }

                foreach (var dial in keypad.Dials)
                {
                    if(!dial.Plot)
                    {
                        StatePlot.RemoveSeries($"KB{keypad.Number}D{dial.Number}");
                        continue;
                    }
                    StatePlot.AddSeries($"KB{keypad.Number}D{dial.Number}", $"KB{keypad.Number}D{dial.Number}: {dial.Name}");
                }
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

            foreach(var output in _pdm.Outputs)
            {
                if(!output.Plot) continue;
                CurrentOutputPlot.AddPoint($"O{output.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? output.Current : 0);
                CurrentOutputPlot.AddPoint($"OC{output.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? output.CalculatedPower : 0);
                StatePlot.AddPoint($"O{output.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(output.State == OutState.On) : 0);
            }

            foreach(var input in _pdm.DigitalInputs)
            {
                if (!input.Plot) continue;
                StatePlot.AddPoint($"I{input.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(input.State) : 0);
            }

            foreach(var input in _pdm.CanInputs)
            {
                if (!input.Plot) continue;
                StatePlot.AddPoint($"CI{input.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? input.Value : 0);
            }

            foreach (var input in _pdm.VirtualInputs)
            {
                if (!input.Plot) continue;
                StatePlot.AddPoint($"VI{input.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(input.Value) : 0);
            }

            foreach(var cond in _pdm.Conditions)
            {
                if (!cond.Plot) continue;
                StatePlot.AddPoint($"CON{cond.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(cond.Value) : 0);
            }

            foreach (var counter in _pdm.Counters)
            {
                if (!counter.Plot) continue;
                StatePlot.AddPoint($"CNT{counter.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? counter.Value : 0);
            }

            foreach (var flasher in _pdm.Flashers)
            {
                if (!flasher.Plot) continue;
                StatePlot.AddPoint($"FLS{flasher.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(flasher.Value) : 0);
            }

            foreach (var keypad in _pdm.Keypads)
            {
                foreach (var button in keypad.AllButtons)
                {
                    if (!button.Plot) continue;
                    StatePlot.AddPoint($"KB{keypad.Number}B{button.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? Convert.ToInt16(button.State) : 0);
                }

                foreach (var dial in keypad.Dials)
                {
                    if (!dial.Plot) continue;
                    StatePlot.AddPoint($"KB{keypad.Number}D{dial.Number}", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.IsConnected ? dial.Ticks : 0);
                }
            }

            BatteryPlot.AddPoint("Battery", DateTimeAxis.ToDouble(DateTime.Now - _zeroTime), _pdm.BatteryVoltage);

            CurrentOutputPlot.Refresh();
            StatePlot.Refresh();
            BatteryPlot.Refresh();
        }

        public void ClearBtn(object parameter)
        {
            ClearData();
        }

        public bool CanClearBtn(object parameter)
        {
            return true;
        }

        public void ExportBtn(object parameter)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            //folderDialog.RootFolder = ;
            if (folderDialog.ShowDialog() != DialogResult.OK) return;

            var path = folderDialog.SelectedPath;
            CurrentOutputPlot.Export(Path.GetFullPath(path));
            StatePlot.Export(Path.GetFullPath(path));
            BatteryPlot.Export(Path.GetFullPath(path));
        }

        public bool CanExportBtn(object parameter)
        {
            return true;
        }

        public ICommand ClearBtnCmd { get; set; }
        public ICommand ExportBtnCmd { get; set; }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
