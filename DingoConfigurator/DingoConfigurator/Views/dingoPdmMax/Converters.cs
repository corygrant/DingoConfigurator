using CanDevices.DingoPdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DingoConfigurator.Views
{
    public class VarMapFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Ensure that 'value' is an array of enum values (VarMap[])
            if (value is Array enumValues)
            {
                return enumValues
                    .Cast<VarMap>()
                    .Where(e => e != VarMap.Output5 && e != VarMap.Output6 && e != VarMap.Output7 && e != VarMap.Output8)
                    .ToList();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
