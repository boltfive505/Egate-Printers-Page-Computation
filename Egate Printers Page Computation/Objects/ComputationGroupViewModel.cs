using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Egate_Printers_Page_Computation.Helpers;
using Egate_Printers_Page_Computation.Objects;
using PrintersPageComputation.Model;
using PropertyChanged;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Objects
{
    public class ComputationGroupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public ContractViewModel Contract { get; set; }
        public PageCounterViewModel LastCounter { get; set; }
        public PageCounterViewModel FirstCounter { get; set; }
        [AlsoNotifyFor("ConsumptionDate")]
        public int ConsumptionMonth { get; set; }
        [AlsoNotifyFor("ConsumptionDate")]
        public int ConsumptionYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public string JustificationFile { get; set; }

        public DateTime ConsumptionDate { get { return new DateTime(ConsumptionYear, ConsumptionMonth, 1); } }

        [CloneCopyIgnore]
        public List<ComputationPeriodViewModel> ComputationPeriods { get; set; } = new List<ComputationPeriodViewModel>() { new ComputationPeriodViewModel() };

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public ComputationPeriodViewModel FirstComputation
        {
            get { return ComputationPeriods == null || ComputationPeriods.Count == 0 ? new ComputationPeriodViewModel() : ComputationPeriods[0]; }
        }

        public decimal TotalExcessCost
        {
            get
            {
                decimal[] results = new decimal[4];
                //get total counts
                ComputationPeriods.ForEach(i =>
                {
                    results[0] += (i.Computations[0].TotalCount ?? 0);
                    results[1] += (i.Computations[1].TotalCount ?? 0);
                    results[2] += (i.Computations[2].TotalCount ?? 0);
                    results[3] += (i.Computations[3].TotalCount ?? 0);
                });
                //get page total difference
                results[0] -= (ComputationPeriods.FirstOrDefault()?.Computations[0].CountLimit ?? 0);
                results[1] -= (ComputationPeriods.FirstOrDefault()?.Computations[1].CountLimit ?? 0);
                results[2] -= (ComputationPeriods.FirstOrDefault()?.Computations[2].CountLimit ?? 0);
                results[3] -= (ComputationPeriods.FirstOrDefault()?.Computations[3].CountLimit ?? 0);
                //get excess cost
                results[0] = Math.Max(results[0], 0) * (ComputationPeriods.FirstOrDefault()?.Computations[0].ExcessCharge ?? 0);
                results[1] = Math.Max(results[1], 0) * (ComputationPeriods.FirstOrDefault()?.Computations[1].ExcessCharge ?? 0);
                results[2] = Math.Max(results[2], 0) * (ComputationPeriods.FirstOrDefault()?.Computations[2].ExcessCharge ?? 0);
                results[3] = Math.Max(results[3], 0) * (ComputationPeriods.FirstOrDefault()?.Computations[3].ExcessCharge ?? 0);
                return results.Sum(x => x);
            }
        }

        public decimal GrandTotal { get { return TotalExcessCost + (Contract?.BasicPrice ?? 0); } }

        public ComputationGroupViewModel()
        { }

        public ComputationGroupViewModel(computation entity, contract contractEntity, company companyEntity, page_counter firstCounterEntity, page_counter lastCounterEntity) : this()
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.Contract = new ContractViewModel(contractEntity, companyEntity);
            this.FirstCounter = new PageCounterViewModel(firstCounterEntity, null, null);
            this.LastCounter = new PageCounterViewModel(lastCounterEntity, null, null);
            this.ConsumptionMonth = (int)entity.ConsumptionMonth;
            this.ConsumptionYear = (int)entity.ConsumptionYear;
            this.CreatedDate = entity.CreatedDate.ToUnixDate();
            this.JustificationFile = entity.JustificationFile;
        }

        private void OnContractChanged()
        {
            Compute();
        }

        private void OnLastCounterChanged()
        {
            Compute();
        }

        private void OnFirstCounterChanged()
        {
            Compute();
        }

        public void Compute()
        {
            ComputationPeriods.Clear();
            _tokenSource.Cancel(true);
            _tokenSource = new CancellationTokenSource();
            var task = GetComputationPeriodsList(this.Contract, this.FirstCounter, this.LastCounter, _tokenSource.Token);
            task.ContinueWith(t =>
            {
                if (t.IsCanceled) return;
                this.ComputationPeriods.AddRange(t.Result);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComputationPeriods)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstComputation)));
            });
        }

        private static async Task<IEnumerable<ComputationPeriodViewModel>> GetComputationPeriodsList(ContractViewModel contract, PageCounterViewModel firstCounter, PageCounterViewModel lastCounter, CancellationToken token)
        {
            List<ComputationPeriodViewModel> list = new List<ComputationPeriodViewModel>();
            if (contract != null && firstCounter != null && lastCounter != null)
            {
                if (token.IsCancellationRequested) token.ThrowIfCancellationRequested(); //cancel task
                var countersList = await PrintersPageComputationHelper.GetPageCountListFromPeriodAsync(firstCounter.Date, lastCounter.Date, contract.Id);
                if (token.IsCancellationRequested) token.ThrowIfCancellationRequested(); //cancel task
                if (countersList.Count() > 0)
                {
                    PageCounterViewModel first = null;
                    foreach (var counter in countersList)
                    {
                        if (token.IsCancellationRequested) token.ThrowIfCancellationRequested(); //cancel task
                        if (first == null)
                            first = counter;
                        if (counter.IsReplaced) //is end point of unit being replaced
                        {
                            list.Add(new ComputationPeriodViewModel(contract, first, counter) { UnitName = first.UnitName, UnitSerialNumber = first.UnitSerialNumber });
                            first = null;
                        }
                    }
                    if (token.IsCancellationRequested) token.ThrowIfCancellationRequested(); //cancel task
                    if (first != null)
                    {
                        //still has first period
                        PageCounterViewModel last = countersList.Last();
                        if (first.Equals(last)) //first and last counters are same, so it now be first counter only, leaving last counter as null
                            last = null;
                        list.Add(new ComputationPeriodViewModel(contract, first, last) { UnitName = first.UnitName, UnitSerialNumber = first.UnitSerialNumber });
                    }
                }
            }
            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj is ComputationGroupViewModel)
            {
                var o = obj as ComputationGroupViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
