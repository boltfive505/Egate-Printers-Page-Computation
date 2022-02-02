namespace CollectionSchedule.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("collection")]
    public partial class collection
    {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public long? ScheduleDate { get; set; }

        public long? UpdatedDate { get; set; }

        public long? UpdatedByEmployeeId { get; set; }

        [StringLength(2147483647)]
        public string ReceiptNumber { get; set; }

        [StringLength(2147483647)]
        public string InvoiceNumber { get; set; }

        public long? BankId { get; set; }

        [StringLength(2147483647)]
        public string CheckNumber { get; set; }

        public decimal? CheckAmount { get; set; }

        public decimal? CashAmount { get; set; }

        public long? CollectedDate { get; set; }

        public long? CollectedByEmployeeId { get; set; }

        public long NotScheduled { get; set; }

        public string ScheduleNotes { get; set; }

        public string ScheduleNotesType { get; set; }

        public string CollectionNotes { get; set; }

        public string File2307Attachment { get; set; }

        public decimal? WhtAmount { get; set; }

        public virtual employee employee_collected_by { get; set; }

        public virtual employee employee_updated_by { get; set; }

        public virtual company company { get; set; }

        public virtual bank bank { get; set; }
    }
}
