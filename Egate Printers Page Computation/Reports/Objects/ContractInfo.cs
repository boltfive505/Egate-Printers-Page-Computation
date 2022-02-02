using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Reports.Objects
{
    [Serializable]
    public class ContractInfo
    {
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string Model { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? ConsumptionPeriod { get; set; }

        public decimal? BasicPrice { get; set; }
        public int? PageLimit_Black_A4 { get; set; }
        public int? PageLimit_Black_A3 { get; set; }
        public int? PageLimit_Colored_A4 { get; set; }
        public int? PageLimit_Colored_A3 { get; set; }
        public decimal? ExcessPrice_Black_A4 { get; set; }
        public decimal? ExcessPrice_Black_A3 { get; set; }
        public decimal? ExcessPrice_Colored_A4 { get; set; }
        public decimal? ExcessPrice_Colored_A3 { get; set; }

        public decimal TotalExcessCost { get; set; }
        public decimal GrandTotal { get { return TotalExcessCost + (BasicPrice ?? 0); } }
    }
}
