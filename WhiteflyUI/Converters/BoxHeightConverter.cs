using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WhiteflyUI.Converters
{
    // Calcula la altura de la caja: y2 - y1
    public class BoxHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Se esperan dos valores: y1 y y2
            if (values.Length < 2)
                return 0.0;

            if (double.TryParse(values[0]?.ToString(), out double y1) &&
                double.TryParse(values[1]?.ToString(), out double y2))
            {
                return Math.Abs(y2 - y1);
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
