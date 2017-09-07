namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Configuration")]
    public partial class Configuration
    {
        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Value { get; set; }
    }
}
