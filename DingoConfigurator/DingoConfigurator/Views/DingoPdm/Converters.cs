using CanDevices;
using CanDevices.DingoPdm;
using CanDevices.dingoPdmMax;
using DingoConfigurator.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DingoConfigurator.Views
{
    public class VarMapFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure that 'value' is an array of enum values (VarMap[])
            if (values[0] is Array enumValues && values[1] is ViewModelBase viewModel)
            {
                ICanDevice dev = null;

                if (viewModel is DingoPdmViewModel dingoPdmViewModel)
                {
                    dev = dingoPdmViewModel.Pdm;
                }
                else if (viewModel is DingoPdmSettingsViewModel dingoPdmSettingsViewModel)
                {
                    dev = dingoPdmSettingsViewModel.Pdm;
                }

                if (dev is dingoPdmMaxCan)
                {
                    return enumValues
                        .Cast<VarMap>()
                        .Where(e => e != VarMap.Output5_On && e != VarMap.Output5_OC && e != VarMap.Output5_Fault &&
                                    e != VarMap.Output6_On && e != VarMap.Output6_OC && e != VarMap.Output6_Fault && 
                                    e != VarMap.Output7_On && e != VarMap.Output7_OC && e != VarMap.Output7_Fault && 
                                    e != VarMap.Output8_On && e != VarMap.Output8_OC && e != VarMap.Output8_Fault)
                        .ToList();
                }
                else
                {
                    return enumValues.Cast<VarMap>().ToList();
                }
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
