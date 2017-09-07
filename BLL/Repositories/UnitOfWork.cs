using BLL.Interfaces;
using BLL.Services;
using BOL;

namespace BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {


        private readonly YouxelVacation _context;
        public IEmployeeRepository Employee { get; private set; }

        public IVacationRepository Vacation { get; private set; }

        public IAdminRepository Admin { get; private set; }

        public IWfhRepository Wfh { get; private set; }

        public ITeamRepository  Team  { get; private set; }

        public IWorkingDaysCalculator WorkingDaysCalculator{ get; private set; }

        public ISendEmail SendEmail { get; private set; }


        public IGetMd5Hash GetMd5Hash { get; private set; }

        public INotificationRepository Notification { get; private set; }

        public IWorkflowRepository Workflow { get; private set; }

   

    public UnitOfWork(YouxelVacation context)/*constructor*/
        {/*
            
             */


            /*UOW hwa el central lkol el reositories*/
            _context = context;
           WorkingDaysCalculator = new WorkingDaysCalculator();
            GetMd5Hash = new GetMd5Hash();
            SendEmail = new SendEmail();
            Employee = new EmployeeRepository(context , SendEmail , GetMd5Hash);
            Admin = new AdminRepository(context , GetMd5Hash);
            Team = new TeamRepository(context);
            Vacation = new VacationRepository(context  , WorkingDaysCalculator , SendEmail);
            Wfh = new  WfhRepository(context , SendEmail);
            Workflow = new WorkflowRepository(context);
            Notification = new NotificationRepository(context);

        }


        public void Complete()
        {
            _context.SaveChanges();
        }


    }
}
