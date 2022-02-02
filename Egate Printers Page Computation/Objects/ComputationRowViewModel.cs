using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Objects
{
    public class ComputationRowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string PrintType { get; set; }
        public string PaperSize { get; set; }
        public int? LastPeriodCount { get; set; }
        public int? FirstPeriodCount { get; set; }
        public int? CountLimit { get; set; }
        public decimal? ExcessCharge { get; set; }

        public int? TotalCount
        {
            get
            {
                if (LastPeriodCount == null || FirstPeriodCount == null)
                    return null;
                return (LastPeriodCount ?? 0) - (FirstPeriodCount ?? 0);
            }
        }

        public int? ExcessCount
        {
            get
            {
                if (TotalCount == null || CountLimit == null)
                    return null;
                int excess = (TotalCount ?? 0) - (CountLimit ?? 0);
                if (excess > 0)
                    return excess;
                else
                    return null;
            }
        }

        public decimal? ExcessCost
        {
            get
            {
                if (ExcessCount == null || ExcessCharge == null)
                    return null;
                return Math.Round((ExcessCount ?? 0) * (ExcessCharge ?? 0), 2);
            }
        }

        public ComputationRowViewModel()
        { }

        public ComputationRowViewModel(string printType, string paperSize, int? lastCount, int? firstCount, int? countLimit, decimal? basicCharge, decimal? excessCharge)
        {
            this.PrintType = printType;
            this.PaperSize = paperSize;
            this.LastPeriodCount = lastCount;
            this.FirstPeriodCount = firstCount;
            this.CountLimit = countLimit;
            this.ExcessCharge = excessCharge;
        }
    }
}
