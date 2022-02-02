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
    [ValueConversion(typeof(IEnumerable<string>), typeof(string))]
    public class StringListJoinConverter : IValueConverter
    {
        public string Separator { get; set; } = ", ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            IEnumerable<string> list = (IEnumerable<string>)value;
            return string.Join(Separator, list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
