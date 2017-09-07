namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Workflow")]
    public partial class Workflow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workflow()
        {
            RequestApproval = new HashSet<RequestApproval>();
        }

        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TeamID { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        public int LevelNum { get; set; }

        public virtual Employee Employee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestApproval> RequestApproval { get; set; }

        public virtual Team Team { get; set; }
    }
}
