namespace PrintersPageComputation.Model
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
            contracts = new HashSet<contract>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string CompanyName { get; set; }

        public long IsActive { get; set; } = 1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<contract> contracts { get; set; }
    }
}
