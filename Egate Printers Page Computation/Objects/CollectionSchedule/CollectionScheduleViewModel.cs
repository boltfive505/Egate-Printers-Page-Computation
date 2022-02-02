using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionSchedule.Model;

namespace Egate_Printers_Page_Computation.Objects.CollectionSchedule
{
    public class CollectionScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public CollectionCompanyViewModel Company { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public CollectionEmployeeViewModel UpdateByEmployee { get; set; }
        public string ReceiptNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public CollectionBankViewModel Bank { get; set; }
        public string CheckNumber { get; set; }
        public decimal? CheckAmount { get; set; }
        public decimal? CashAmount { get; set; }
        public DateTime? CollectedDate { get; set; }
        public CollectionEmployeeViewModel CollectedByEmployee { get; set; }
        public bool NotScheduled { get; set; } = false;
        public ScheduleNotesType ScheduleNotesType { get; set; } = ScheduleNotesType.None;
        public string ScheduleNotes { get; set; }
        public string CollectionNotes { get; set; }
        public string File2307Attachment { get; set; }
        public decimal? WhtAmount { get; set; }

        public bool? IsCheckAmount
        {
            get
            {
                if (CheckAmount != null) return true;
                else if (CashAmount != null) return false;
                else return null;
            }
        }

        public decimal? NetWhtAmount
        {
            get
            {
                if (IsCheckAmount == null || WhtAmount == null) return null;
                else return (IsCheckAmount.Value ? CheckAmount : CashAmount) - WhtAmount;
            }
        }

        public bool IsCollected
        {
            get { return CollectedDate != null; }
        }

        public string ScheduleNotesDisplay
        {
            get
            {
                switch (ScheduleNotesType)
                {
                    case ScheduleNotesType.Others:
                        return ScheduleNotes;
                    case ScheduleNotesType.None:
                        return null;
                    default:
                        return TypeDescriptor.GetConverter(typeof(ScheduleNotesType)).ConvertToString(ScheduleNotesType);
                }
            }
        }

        public CollectionScheduleViewModel()
        { }

        public CollectionScheduleViewModel(collection entity, bank bankEntity, company companyEntity, location locationEntity, employee employeeUpdatedEntity, employee employeeCollectedEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.Company = new CollectionCompanyViewModel(companyEntity, locationEntity);
            this.ScheduleDate = entity.ScheduleDate.ToUnixDate();
            this.UpdatedDate = entity.UpdatedDate.ToUnixDate();
            this.UpdateByEmployee = employeeUpdatedEntity == null ? null : new CollectionEmployeeViewModel(employeeUpdatedEntity);
            this.ReceiptNumber = entity.ReceiptNumber;
            this.InvoiceNumber = entity.InvoiceNumber;
            this.Bank = bankEntity == null ? null : new CollectionBankViewModel(bankEntity);
            //this.BankName = entity.BankName;
            this.CheckNumber = entity.CheckNumber;
            this.CheckAmount = entity.CheckAmount;
            this.CashAmount = entity.CashAmount;
            this.CollectedDate = entity.CollectedDate.ToUnixDate();
            this.CollectedByEmployee = employeeCollectedEntity == null ? null : new CollectionEmployeeViewModel(employeeCollectedEntity);
            this.NotScheduled = entity.NotScheduled.ToBool();
            this.ScheduleNotesType = entity.ScheduleNotesType.ToEnum<ScheduleNotesType>();
            this.ScheduleNotes = entity.ScheduleNotes;
            this.CollectionNotes = entity.CollectionNotes;
            this.File2307Attachment = entity.File2307Attachment;
            this.WhtAmount = entity.WhtAmount;
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionScheduleViewModel)
            {
                var o = obj as CollectionScheduleViewModel;
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
