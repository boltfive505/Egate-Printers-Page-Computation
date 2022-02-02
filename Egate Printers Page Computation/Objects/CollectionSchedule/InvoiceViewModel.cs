using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionSchedule.Model;
using bolt5.CloneCopy;

namespace Egate_Printers_Page_Computation.Objects.CollectionSchedule
{
    public class InvoiceViewModel : INotifyPropertyChanged, IEditableObject
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public CollectionCompanyViewModel Company { get; set; }
        public int? InvoiceMonth { get; set; }
        public int? InvoiceYear { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? Amount { get; set; }
        public string FileAttachment { get; set; }
        public string Notes { get; set; }
        public string ComparisonNotes { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        [CloneCopyIgnore]
        public bool ComparisonMark { get; set; }
        public bool IsVoid { get { return (Amount ?? 0) == 0; } }
        [CloneCopyIgnore]
        public bool IsPaid { get; set; }

        public DateTime InvoiceDate
        {
            get { return new DateTime(InvoiceYear.Value, InvoiceMonth.Value, 1); }
        }

        public InvoiceViewModel()
        { }

        public InvoiceViewModel(invoice entity, company companyEntity, location locationEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.Company = new CollectionCompanyViewModel(companyEntity, locationEntity);
            this.InvoiceMonth = entity.InvoiceMonth;
            this.InvoiceYear = entity.InvoiceYear;
            this.InvoiceNumber = entity.InvoiceNumber;
            this.Amount = entity.Amount;
            this.FileAttachment = entity.FileAttachment;
            this.Notes = entity.Notes;
            this.ComparisonNotes = entity.ComparisonNotes;
            this.UpdatedDate = entity.UpdatedDate.ToUnixDate();
        }

        public void BeginEdit()
        {
            
        }

        public void EndEdit()
        {
            _ = Helpers.CollectionScheduleHelper.UpdateInvoiceComparisonNotesAsync(this);
        }

        public void CancelEdit()
        {
            
        }
    }
}
