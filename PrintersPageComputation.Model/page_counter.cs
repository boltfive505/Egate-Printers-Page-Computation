namespace PrintersPageComputation.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("page_counter")]
    public partial class page_counter
    {
        public page_counter()
        {

        }

        public long Id { get; set; }

        public long ContractId { get; set; }

        public long Date { get; set; }

        public int? PageCount_Black_A4 { get; set; }

        public int? PageCount_Black_A3 { get; set; }

        public int? PageCount_Colored_A4 { get; set; }

        public int? PageCount_Colored_A3 { get; set; }

        public string FileAttachment { get; set; }

        public int IsReplaced { get; set; }

        public string UnitName { get; set; }

        public string UnitSerialNumber { get; set; }

        public virtual contract contract { get; set; }

        public virtual ICollection<computation> first_page_counter_computations { get; set; }

        public virtual ICollection<computation> last_page_counter_computations { get; set; }
    }
}
