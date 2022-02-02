using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Printers_Page_Computation.Objects
{
    public class ComputationPeriodViewModel
    {
        //public DateTime? FirstPeriod { get; set; }
        //public DateTime? LastPeriod { get; set; }
        public ContractViewModel Contract { get; set; }
        public PageCounterViewModel LastCounter { get; set; }
        public PageCounterViewModel FirstCounter { get; set; }

        public string UnitName { get; set; }
        public string UnitSerialNumber { get; set; }
        public ComputationRowViewModel[] Computations { get; set; }

        //public decimal TotalExcessCost { get; set; }
        //public decimal GrandTotal
        //{
        //    get { return (Contract?.BasicPrice ?? 0) + TotalExcessCost; }
        //}

        public ComputationPeriodViewModel()
        {
            Computations = new ComputationRowViewModel[4] { new ComputationRowViewModel(), new ComputationRowViewModel(), new ComputationRowViewModel(), new ComputationRowViewModel() };
        }

        public ComputationPeriodViewModel(ContractViewModel contract, PageCounterViewModel firstCounter, PageCounterViewModel lastCounter) : this()
        {
            this.Contract = contract;
            this.FirstCounter = firstCounter;
            this.LastCounter = lastCounter;
            this.Computations[0] = new ComputationRowViewModel("Black", "A4", LastCounter?.PageCount_Black_A4, FirstCounter?.PageCount_Black_A4, Contract?.PageLimit_Black_A4, Contract?.BasicPrice, Contract?.ExcessPrice_Black_A4);
            this.Computations[1] = new ComputationRowViewModel("Black", "A3", LastCounter?.PageCount_Black_A3, FirstCounter?.PageCount_Black_A3, Contract?.PageLimit_Black_A3, Contract?.BasicPrice, Contract?.ExcessPrice_Black_A3);
            this.Computations[2] = new ComputationRowViewModel("Color", "A4", LastCounter?.PageCount_Colored_A4, FirstCounter?.PageCount_Colored_A4, Contract?.PageLimit_Colored_A4, Contract?.BasicPrice, Contract?.ExcessPrice_Colored_A4);
            this.Computations[3] = new ComputationRowViewModel("Color", "A3", LastCounter?.PageCount_Colored_A3, FirstCounter?.PageCount_Colored_A3, Contract?.PageLimit_Colored_A3, Contract?.BasicPrice, Contract?.ExcessPrice_Colored_A3);
            //TotalExcessCost = Computations.Sum(i => i.ExcessCost ?? 0);
        }
    }
}
