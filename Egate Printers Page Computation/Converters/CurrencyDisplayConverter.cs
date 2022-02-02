using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Converters
{
    [ValueConversion(typeof(decimal?), typeof(string))]
    public class CurrencyDisplayConverter : IValueConverter
    {
        public bool DisplayCurrency { get; set; } = true;
        public bool DisplayIfZero { get; set; }
        public string FormatString { get; set; } = "N2";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            decimal num = System.Convert.ToDecimal(value);
            if (!DisplayIfZero && num == 0)
                return null;
            else
                return string.Format("{0}{1}", (DisplayCurrency ? "₱" : string.Empty), num.ToString(FormatString));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
