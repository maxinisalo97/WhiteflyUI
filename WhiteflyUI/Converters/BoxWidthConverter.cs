using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WhiteflyUI.Converters
{
    public class BoxWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Se esperan dos valores: x1 y x2
            if (values.Length < 2)
                return 0.0;

            if (double.TryParse(values[0]?.ToString(), out double x1) &&
                double.TryParse(values[1]?.ToString(), out double x2))
            {
                return Math.Abs(x2 - x1);
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
