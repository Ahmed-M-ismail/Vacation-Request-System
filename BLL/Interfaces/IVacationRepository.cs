using System;
using System.Collections.Generic;
using BOL;

namespace BLL.Interfaces  
{
    public  interface IVacationRepository : IDisposable
    {


        Request Get(string id, string userId);

         Request GetById(string id);

        int AddVacation(Request request);



        IEnumerable<Request> ShowVactionHistory(string employeeId);


        IEnumerable<Request> ManagerShowRequests(string employeeId);


        IEnumerable<Notifications> ManagerShownotifications(string employeeId);


        int  numOfNotifications(string employeeId);


        void ApproveVacation(RequestApproval requestapproval, string employeeId, string requestid);


        void RejctVacation(RequestApproval requestapproval, string employeeId, string requestid);



        bool UpdateVacation(Request request);

        void CancelVacation(Request request);


    








    }
}
