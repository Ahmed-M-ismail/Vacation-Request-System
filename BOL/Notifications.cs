namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notifications
    {
        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        public DateTime CreatedAT { get; set; }

        public bool seen { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(500)]
        public string text { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
