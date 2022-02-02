using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class ColorConditionConverter : IValueConverter
    {
        public Color TrueValue { get; set; } = Color.Black;
        public Color FalseValue { get; set; } = Color.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return FalseValue;
            bool flag = (bool)value;
            return flag ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
