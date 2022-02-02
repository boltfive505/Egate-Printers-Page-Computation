using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Converters
{
    [ValueConversion(typeof(bool), typeof(FontWeight))]
    public class FontWeightCondition : IValueConverter
    {
        public FontWeight TrueValue { get; set; } = FontWeight.FromOpenTypeWeight(400);
        public FontWeight FalseValue { get; set; } = FontWeight.FromOpenTypeWeight(400);

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
