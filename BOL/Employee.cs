using System.Web.Mvc;

namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employee")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            Notifications = new HashSet<Notifications>();
            Request = new HashSet<Request>();
            RequestApproval = new HashSet<RequestApproval>();
            Team = new HashSet<Team>();
            Workflow = new HashSet<Workflow>();
        }

        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FName { get; set; }

        [Required]
        [StringLength(50)]
        public string LName { get; set; }

        [Required]
        [StringLength(255)]
        public string JobTittle { get; set; }

        public DateTime DateHired { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        [Remote("IsAlreadySigned", "Home", HttpMethod = "POST", ErrorMessage = "EmailId already exists in database.")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string UserRoleID { get; set; }

        public int? VacationBalance { get; set; }

        public int? BalanceLimit { get; set; }

        public bool AccountActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? CreatedAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? LastUpdated { get; set; }

        public virtual EmployeeRole EmployeeRole { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notifications> Notifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Request { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestApproval> RequestApproval { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Team { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow> Workflow { get; set; }
    }
}
