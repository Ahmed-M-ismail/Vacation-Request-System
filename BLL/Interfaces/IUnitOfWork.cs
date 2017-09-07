

using BLL.Services;

namespace BLL.Interfaces
{
  public  interface IUnitOfWork
    {
        IEmployeeRepository Employee { get; }

         IVacationRepository Vacation { get;  }

        IAdminRepository Admin { get;  }

        IWfhRepository Wfh { get; }

        ITeamRepository Team { get;}

        IWorkflowRepository Workflow { get; }

        IWorkingDaysCalculator WorkingDaysCalculator { get; }

        INotificationRepository Notification { get; }

        ISendEmail SendEmail { get; }


    }
}
