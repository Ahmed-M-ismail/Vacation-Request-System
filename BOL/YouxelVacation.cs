namespace BOL
{
    using System.Data.Entity;

    public partial class YouxelVacation : DbContext
    {
        public YouxelVacation()
            : base("name=YouxelVacation")
        {
        }

        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeRole> EmployeeRole { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<RequestApproval> RequestApproval { get; set; }
        public virtual DbSet<RequestStatus> RequestStatus { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<VacationType> VacationType { get; set; }
        public virtual DbSet<Workflow> Workflow { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Configuration>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.LName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.JobTittle)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.UserRoleID)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Notifications)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Request)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.RequestApproval)
                .WithRequired(e => e.Employee)
                .HasForeignKey(e => e.ApprovalBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Team)
                .WithRequired(e => e.Employee)
                .HasForeignKey(e => e.ManagerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Workflow)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmployeeRole>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<EmployeeRole>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<EmployeeRole>()
                .HasMany(e => e.Employee)
                .WithRequired(e => e.EmployeeRole)
                .HasForeignKey(e => e.UserRoleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notifications>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Notifications>()
                .Property(e => e.EmployeeID)
                .IsUnicode(false);

            modelBuilder.Entity<Notifications>()
                .Property(e => e.text)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.EmployeeID)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.VacationTypeID)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.RequestStatusID)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .Property(e => e.Comment)
                .IsUnicode(false);

            modelBuilder.Entity<Request>()
                .HasMany(e => e.RequestApproval)
                .WithRequired(e => e.Request)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.WorkflowID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.RequestID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.StatusID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.Comment)
                .IsUnicode(false);

            modelBuilder.Entity<RequestApproval>()
                .Property(e => e.ApprovalBy)
                .IsUnicode(false);

            modelBuilder.Entity<RequestStatus>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestStatus>()
                .Property(e => e.StatusName)
                .IsUnicode(false);

            modelBuilder.Entity<RequestStatus>()
                .HasMany(e => e.Request)
                .WithRequired(e => e.RequestStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RequestStatus>()
                .HasMany(e => e.RequestApproval)
                .WithRequired(e => e.RequestStatus)
                .HasForeignKey(e => e.StatusID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.ManagerID)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Workflow)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VacationType>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<VacationType>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .Property(e => e.ID)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .Property(e => e.TeamID)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .Property(e => e.EmployeeID)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .HasMany(e => e.RequestApproval)
                .WithRequired(e => e.Workflow)
                .WillCascadeOnDelete(false);
        }
    }
}
