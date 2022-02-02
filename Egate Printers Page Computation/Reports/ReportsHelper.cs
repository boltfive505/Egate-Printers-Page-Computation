using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WinForms;
using Egate_Printers_Page_Computation.Objects;
using Egate_Printers_Page_Computation.Reports.Objects;

namespace Egate_Printers_Page_Computation.Reports
{
    public static class ReportsHelper
    {
        public static void GeneratePageCountComputationReport(ComputationGroupViewModel computations)
        {
            //period list
            List<PageCountPeriod> periodList = computations.ComputationPeriods.Select(i => new PageCountPeriod()
            {
                FirstPeriod = i.FirstCounter?.Date,
                LastPeriod = i.LastCounter?.Date,
                UnitName = i.UnitName,
                UnitSerialNumber = i.UnitSerialNumber
            }).ToList();
            //totals list
            List<PageCountTotals> totalsList = new List<PageCountTotals>();
            foreach (var per in computations.ComputationPeriods)
            {
                foreach (var comp in per.Computations)
                {
                    totalsList.Add(new PageCountTotals()
                    {
                        FirstPeriod = per.FirstCounter?.Date,
                        LastPeriod = per.LastCounter?.Date,
                        PrintType = comp.PrintType,
                        PaperSize = comp.PaperSize,
                        TotalCount = comp.TotalCount,
                        BasicCount = comp.CountLimit,
                        ExcessCharge = comp.ExcessCharge
                    });
                }
            }
            //contract info
            List<ContractInfo> contractInfo = new List<ContractInfo>();
            contractInfo.Add(new ContractInfo()
            {
                CompanyName = computations.Contract?.Company?.CompanyName,
                Department = computations.Contract?.Department,
                Model = computations.Contract?.AssetModel,
                PeriodFrom = computations.FirstCounter?.Date,
                PeriodTo = computations.LastCounter?.Date,
                ConsumptionPeriod = new DateTime(computations.ConsumptionYear, computations.ConsumptionMonth, 1),
                BasicPrice = computations.Contract?.BasicPrice,
                PageLimit_Black_A4 = computations.Contract.PageLimit_Black_A4,
                PageLimit_Black_A3 = computations.Contract.PageLimit_Black_A3,
                PageLimit_Colored_A4 = computations.Contract.PageLimit_Colored_A4,
                PageLimit_Colored_A3 = computations.Contract.PageLimit_Colored_A3,
                ExcessPrice_Black_A4 = computations.Contract.ExcessPrice_Black_A4,
                ExcessPrice_Black_A3 = computations.Contract.ExcessPrice_Black_A3,
                ExcessPrice_Colored_A4 = computations.Contract.ExcessPrice_Colored_A4,
                ExcessPrice_Colored_A3 = computations.Contract.ExcessPrice_Colored_A3,
                TotalExcessCost = computations.TotalExcessCost
            });

            report_form.ShowReport("page count report", new Dictionary<string, object>()
            {
                { "PageCountPeriodDataset", periodList },
                { "ContractInfoDataset", contractInfo },
                { "PageCountTotalsDataset", totalsList }
            }, 
            new Action<SubreportProcessingEventArgs>((e) => 
            {
                DateTime? firstPeriod = (DateTime?)Convert.ToDateTime(e.Parameters["FirstPeriod"].Values[0]);
                DateTime? lastPeriod = (DateTime?)Convert.ToDateTime(e.Parameters["LastPeriod"].Values[0]);
                List<PageCountComputation> list = new List<PageCountComputation>();
                ComputationPeriodViewModel computationPeriod = computations.ComputationPeriods.FirstOrDefault(i => i.FirstCounter?.Date == firstPeriod && i.LastCounter?.Date == lastPeriod);
                if (computationPeriod != null)
                {
                    list = computationPeriod.Computations
                        .Select(i => new PageCountComputation()
                        {
                            PrintType = i.PrintType,
                            PaperSize = i.PaperSize,
                            LastPageCount = i.LastPeriodCount,
                            FirstPageCount = i.FirstPeriodCount,
                            TotalCount = i.TotalCount,
                            ExcessCount = i.ExcessCount,
                            ExcessCost = i.ExcessCost,
                            BasicCount = i.CountLimit,
                            ExcessCharge = i.ExcessCharge
                        }).ToList();
                }
                e.DataSources.Add(new ReportDataSource("PageCountComputationDataset", list));
            }));
        }
    }
}
