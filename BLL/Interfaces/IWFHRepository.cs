using System;
using System.Collections.Generic;
using BOL;

namespace BLL.Interfaces
{
     public  interface  IWfhRepository : IDisposable
    {
       bool AddWfh(Request request);



        IEnumerable<Request> ShowWfhHistory(string employeeId);

   

        IEnumerable<Request> ManagerShowRequests(string employeeId);

        void ApproveWfh(RequestApproval requestapproval, string employeeId, string requestid);
        void RejctWfh(RequestApproval requestapproval, string employeeId, string requestid);

        void UpdateWfh(Request request);

        void CancelWfh(Request request);






    }



}
