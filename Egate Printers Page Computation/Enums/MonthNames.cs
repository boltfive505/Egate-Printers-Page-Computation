
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MonthNames : int
    {
        January = 1,
        Febuary = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}
