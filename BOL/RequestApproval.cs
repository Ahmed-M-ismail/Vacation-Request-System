namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RequestApproval")]
    public partial class RequestApproval
    {
        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkflowID { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusID { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        [Required]
        [StringLength(50)]
        public string ApprovalBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? ApprovalAT { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LastModified { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Request Request { get; set; }

        public virtual RequestStatus RequestStatus { get; set; }

        public virtual Workflow Workflow { get; set; }
    }
}
