using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintersPageComputation.Model;

namespace Egate_Printers_Page_Computation.Objects
{
    public class PageCounterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public ContractViewModel Contract { get; set; }
        public DateTime Date { get; set; }
        public int? PageCount_Black_A4 { get; set; }
        public int? PageCount_Black_A3 { get; set; }
        public int? PageCount_Colored_A4 { get; set; }
        public int? PageCount_Colored_A3 { get; set; }
        public string FileAttachment { get; set; }
        public bool IsReplaced { get; set; }
        public string UnitName { get; set; }
        public string UnitSerialNumber { get; set; }

        //page count difference
        public PageCounterViewModel LastPageCount { get; set; }

        public int? PageCountDifference_Black_A4 { get { return LastPageCount == null ? null : GetDifferenceFromLastPageCount(this.PageCount_Black_A4, LastPageCount?.PageCount_Black_A4); } }
        public int? PageCountDifference_Black_A3 { get { return LastPageCount == null ? null : GetDifferenceFromLastPageCount(this.PageCount_Black_A3, LastPageCount?.PageCount_Black_A3); } }
        public int? PageCountDifference_Colored_A4 { get { return LastPageCount == null ? null : GetDifferenceFromLastPageCount(this.PageCount_Colored_A4, LastPageCount?.PageCount_Colored_A4); } }
        public int? PageCountDifference_Colored_A3 { get { return LastPageCount == null ? null : GetDifferenceFromLastPageCount(this.PageCount_Colored_A3, LastPageCount?.PageCount_Colored_A3); } }

        public bool IsNew { get { return this.Id == 0; } }

        public PageCounterViewModel()
        { }

        public PageCounterViewModel(page_counter entity, contract contractEntity, company companyEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.Contract = new ContractViewModel(contractEntity, companyEntity);
            this.Date = entity.Date.ToUnixDate();
            this.PageCount_Black_A4 = entity.PageCount_Black_A4;
            this.PageCount_Black_A3 = entity.PageCount_Black_A3;
            this.PageCount_Colored_A4 = entity.PageCount_Colored_A4;
            this.PageCount_Colored_A3 = entity.PageCount_Colored_A3;
            this.FileAttachment = entity.FileAttachment;
            this.IsReplaced = entity.IsReplaced.ToBool();
            this.UnitName = entity.UnitName;
            this.UnitSerialNumber = entity.UnitSerialNumber;
        }

        private int? GetDifferenceFromLastPageCount(int? count1, int? count2)
        {
            if (IsLastPageCountOldUnit || (count1 == null && count2 == null)) return null;
            return (count1 ?? 0) - (count2 ?? 0);
        }

        private bool IsLastPageCountOldUnit //if previous page count is for replacement
        {
            get
            {
                return LastPageCount?.IsReplaced ?? false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PageCounterViewModel)
            {
                var o = obj as PageCounterViewModel;
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
