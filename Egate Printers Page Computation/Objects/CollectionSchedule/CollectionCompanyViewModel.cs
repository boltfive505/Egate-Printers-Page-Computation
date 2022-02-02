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
    public class CollectionCompanyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public CollectionLocationViewModel Location { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string TinNumber { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string OverdueNotes { get; set; }
        public string ScheduleNotes { get; set; }
        public bool Is2307 { get; set; } = true;
        public bool IsBankTransfer { get; set; }
        public string ToDoNotes { get; set; }
        public CompanyClientType ClientType { get; set; } = CompanyClientType.Regular;

        [CloneCopyIgnore]
        public CollectionScheduleViewModel CurrentScheduledCollection { get; set; }
        [CloneCopyIgnore]
        public CollectionScheduleViewModel LatestCollected { get; set; }
        [CloneCopyIgnore]
        public int? ScheduleCount { get; set; }
        [CloneCopyIgnore]
        public bool HasFollowUp { get; set; }
        public bool HasOverdue { get { return !string.IsNullOrEmpty(OverdueNotes); } }

        //for to do
        [CloneCopyIgnore]
        public bool HasFollowupCollection { get; set; }
        [CloneCopyIgnore]
        public bool HasFollowup2307 { get; set; }
        public List<string> InvoiceNumberMissingList { get; set; } = new List<string>();

        //for comparison
        [CloneCopyIgnore]
        public bool IsMissingMark { get; set; }

        public DateTime? UpdatedDate
        {
            get
            {
                if (CurrentScheduledCollection == null)
                    return LatestCollected?.UpdatedDate;
                else
                    return CurrentScheduledCollection?.UpdatedDate;
            }
        }

        public CollectionEmployeeViewModel UpdatedByEmployee
        {
            get
            {
                if (CurrentScheduledCollection == null)
                    return LatestCollected?.UpdateByEmployee;
                else
                    return CurrentScheduledCollection?.UpdateByEmployee;
            }
        }

        public CollectionCompanyViewModel()
        { }

        public CollectionCompanyViewModel(company entity, location locationEntity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.CompanyName = entity.CompanyName;
            this.ContactPerson = entity.ContactPerson;
            this.ContactNumber = entity.ContactNumber;
            this.Email = entity.Email;
            this.Address = entity.Address;
            this.PhoneNumber = entity.PhoneNumber;
            this.TinNumber = entity.TinNumber;
            this.Description = entity.Description;
            this.IsActive = entity.IsActive.ToBool();
            this.OverdueNotes = entity.OverdueNotes;
            this.ScheduleNotes = entity.ScheduleNotes;
            this.Is2307 = entity.Is2307.ToBool();
            this.IsBankTransfer = entity.IsBankTransfer.ToBool();
            this.ToDoNotes = entity.ToDoNotes;
            this.ClientType = entity.ClientType.ToEnum<CompanyClientType>(CompanyClientType.Regular);
            this.Location = locationEntity == null ? null : new CollectionLocationViewModel(locationEntity);
        }

        public override bool Equals(object obj)
        {
            if (obj is CollectionCompanyViewModel)
            {
                var o = obj as CollectionCompanyViewModel;
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
