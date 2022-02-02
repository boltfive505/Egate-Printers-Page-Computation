using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CompanyClientType
    {
        [Description("All")]
        None,
        Regular,        
        Rental,
        //Old
    }
}
