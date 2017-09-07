using BLL;

namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Request")]
    public  class Request : IValidatableObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request()
        {
            RequestApproval = new HashSet<RequestApproval>();
        }

        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        [FutureDate]
        public DateTime? DurationFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
      
    [FutureDate]
        public DateTime? DurationTo { get; set; }

        [StringLength(50)]
        public string VacationTypeID { get; set; }

        public bool VacationORWorkHome { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestStatusID { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? CreatedAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? LastModified { get; set; }

        public int? CountDays { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual RequestStatus RequestStatus { get; set; }

        public virtual VacationType VacationType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestApproval> RequestApproval { get; set; }




        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (DurationTo < DurationFrom)
            {
                yield return new ValidationResult("EndDate must be greater than StartDate");
            }
        }


    }
}
