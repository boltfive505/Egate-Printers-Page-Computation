using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Reports.Objects
{
    [Serializable]
    public class PageCountTotals
    {
        public DateTime? FirstPeriod { get; set; }
        public DateTime? LastPeriod { get; set; }
        public string PrintType { get; set; }
        public string PaperSize { get; set; }
        public int? TotalCount { get; set; }

        public int? BasicCount { get; set; }
        public decimal? ExcessCharge { get; set; }

        //public int? ExcessCount
        //{
        //    get
        //    {
        //        if (TotalCount == null || BasicCount == null) return null;
        //        else if (TotalCount <= BasicCount) return null;
        //        else return TotalCount - BasicCount;
        //    }
        //}

        //public decimal? ExcessCost
        //{
        //    get
        //    {
        //        if (ExcessCharge == null || ExcessCount == null) return null;
        //        else return Math.Round((ExcessCharge ?? 0) * ((decimal?)ExcessCount ?? 0), 2);
        //    }
        //}
        
        public string TotalsKey
        {
            get { return string.Format("{0} - {1}", FirstPeriod, LastPeriod); }
        }
    }
}
