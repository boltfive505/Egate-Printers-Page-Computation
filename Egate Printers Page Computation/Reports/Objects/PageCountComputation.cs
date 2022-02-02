using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Reports.Objects
{
    [Serializable]
    public class PageCountComputation
    {
        public string PrintType { get; set; }
        public string PaperSize { get; set; }
        public DateTime? LastPeriod { get; set; }
        public DateTime? FirstPeriod { get; set; }
        public int? LastPageCount { get; set; }
        public int? FirstPageCount { get; set; }
        public int? TotalCount { get; set; }
        public int? BasicCount { get; set; }
        public int? ExcessCount { get; set; }
        public decimal? ExcessCost { get; set; }
        public decimal? ExcessCharge { get; set; }
    }
}
