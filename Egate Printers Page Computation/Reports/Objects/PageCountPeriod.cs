using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Reports.Objects
{
    [Serializable]
    public class PageCountPeriod
    {
        public DateTime? LastPeriod { get; set; }
        public DateTime? FirstPeriod { get; set; }
        public string UnitName { get; set; }
        public string UnitSerialNumber { get; set; }
    }
}
