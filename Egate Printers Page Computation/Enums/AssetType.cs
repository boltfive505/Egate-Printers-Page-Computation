using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AssetType
    {
        [Description("A4-Printer")]
        A4_Printer,
        [Description("A4-MPF")]
        A4_MPF,
        [Description("A3-Printer")]
        A3_Printer,
        [Description("A3-MPF")]
        A3_MPF,
        Scanner,
        [Description("Copier-Colored")]
        Copier_Colored,
        [Description("Copier-Black")]
        Copier_Black
    }
}
