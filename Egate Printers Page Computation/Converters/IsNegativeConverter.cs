using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Converters
{
    public class IsNegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;
            Type t = value.GetType();
            if (t == typeof(int) || t == typeof(long))
                return System.Convert.ToInt64(value) < 0;
            else if (t == typeof(decimal) || t == typeof(double))
                return System.Convert.ToDecimal(value) < 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
