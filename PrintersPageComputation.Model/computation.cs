namespace PrintersPageComputation.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("computation")]
    public partial class computation
    {
        public long Id { get; set; }

        public long ContractId { get; set; }

        public long FirstPageCounterId { get; set; }

        public long LastPageCounterId { get; set; }

        public long ConsumptionMonth { get; set; }

        public long ConsumptionYear { get; set; }

        public long CreatedDate { get; set; }

        public string JustificationFile { get; set; }

        public virtual contract contract { get; set; }

        public virtual page_counter first_page_counter { get; set; }

        public virtual page_counter last_page_counter { get; set; }
    }
}
