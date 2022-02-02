namespace PrintersPageComputation.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("contract")]
    public partial class contract
    {
        public contract()
        {
            page_counters = new HashSet<page_counter>();
            computations = new HashSet<computation>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string ContractNumber { get; set; }

        public long CompanyId { get; set; }

        public string Description { get; set; }

        public string Department { get; set; }

        public string AssetModel { get; set; }

        public string AssetType { get; set; }

        public long? ExpirationDate { get; set; }

        public decimal? BasicPrice { get; set; }

        public int? PageLimit_Black_A4 { get; set; }
               
        public int? PageLimit_Black_A3 { get; set; }
               
        public int? PageLimit_Colored_A4 { get; set; }
               
        public int? PageLimit_Colored_A3 { get; set; }

        public decimal? ExcessPrice_Black_A4 { get; set; }

        public decimal? ExcessPrice_Black_A3 { get; set; }

        public decimal? ExcessPrice_Colored_A4 { get; set; }

        public decimal? ExcessPrice_Colored_A3 { get; set; }

        public virtual company company { get; set; }

        public virtual ICollection<page_counter> page_counters { get; set; }

        public virtual ICollection<computation> computations { get; set; }
    }
}
