namespace CollectionSchedule.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("invoice")]
    public partial class invoice
    {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public int? InvoiceMonth { get; set; }

        public int? InvoiceYear { get; set; }

        [StringLength(2147483647)]
        public string InvoiceNumber { get; set; }

        public long? UpdatedDate { get; set; }

        public decimal? Amount { get; set; }

        public string FileAttachment { get; set; }

        public string Notes { get; set; }

        public string ComparisonNotes { get; set; }

        public virtual company company { get; set; }
    }
}
