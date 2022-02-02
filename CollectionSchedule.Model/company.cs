namespace CollectionSchedule.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("company")]
    public partial class company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public company()
        {
            collections = new HashSet<collection>();
            invoices = new HashSet<invoice>();
            IsActive = 1;
        }

        public long Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public long? LocationId { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string TinNumber { get; set; }

        public string Description { get; set; }

        public long IsActive { get; set; }

        public string OverdueNotes { get; set; }

        public string ScheduleNotes { get; set; }

        public long Is2307 { get; set; }

        public long IsBankTransfer { get; set; }

        public string ToDoNotes { get; set; }

        public string ClientType { get; set; }

        public virtual location location { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<collection> collections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invoice> invoices { get; set; }
    }
}
