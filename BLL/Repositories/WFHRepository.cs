using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BLL.Interfaces;
using BLL.Services;
using BOL;

namespace BLL.Repositories
{
    public class WfhRepository : IWfhRepository
    {

        private readonly YouxelVacation _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly ISendEmail _SendEmail;



        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>


        public WfhRepository(YouxelVacation context, ISendEmail sendEmail)
        {



            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _notificationRepository = new NotificationRepository(_context);
            _SendEmail = sendEmail;
        }








        /// <summary>
        /// Adds the WFH.
        /// </summary>
        /// <param name="request">The request.</param>
        public bool AddWfh(Request request)
        {



            var empID =
            (string)HttpContext.Current.Session["LogedUserID"];



            var fromvacations =
           _context.Request.Where(x => x.EmployeeID.Equals(empID) &&
           (x.RequestStatusID.Equals("1") || x.RequestStatusID.Equals("2"))).Select(x => x.DurationFrom).ToList();




            var tovacations =
                          _context.Request.Where(x => x.EmployeeID.Equals(empID) && (x.RequestStatusID.Equals("1") || x.RequestStatusID.Equals("2"))).Select(x => x.DurationTo).ToList();



            if (request.DurationTo <= fromvacations.Max() || request.DurationTo <= tovacations.Max())
            {
                return false;

            }



            var teamId =
         _context.Workflow.Where(x => x.EmployeeID == empID).Select(x => x.TeamID).ToList();

            int levelnum = /*level el logined user */
               _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.EmployeeID == empID).Select(x => x.LevelNum).FirstOrDefault();








            int numlevel = /*level el team's user */
               _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();






            var teamMembers /*all members in my teams */ = _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();





            var lowermanager /*the first managers bottom of logined user */  = _context.Workflow.Where(x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 1)).Select(x => x.Employee.Email).FirstOrDefault();




            var lowerlowermanager =
        _context.Workflow.Where(
            x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 2))
            .Select(x => x.EmployeeID)
            .FirstOrDefault();


            var hisDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
_context.Request
 .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
     x.Employee.Email.Equals(lowermanager));



            var hisSecondDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
           _context.Request
               .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                   x.EmployeeID.Equals(lowerlowermanager));



            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();
            Request vacation = new Request();
            var listofvc = _context.Request.ToList().OrderByDescending(x => x.ID);




            if (!listofvc.Any())
            {
                vacation.ID = "Re-000001";
            }
            else
            {
                var originalValue = listofvc.Select(x => x.ID).FirstOrDefault();

                // Get the numeric part (0002)
                var stringValue = Regex.Match(originalValue, @"\d+").Value;
                // Convert to int
                var intValue = Int32.Parse(stringValue);
                // Increase
                intValue++;
                // Convert back to string
                originalValue = "Re-" + intValue.ToString("D6");




                vacation.ID = originalValue;

            }






            string comment = (string)HttpContext.Current.Session["LogedUserFname"] + "  " + (string)HttpContext.Current.Session["LogedUserLname"] + " Comment : " + request.Comment + "  ,            ";
            vacation.Comment = comment;




            vacation.DurationTo = request.DurationTo;



            if (levelnum == 1 || (levelnum == 2 && hisDirectManagetHasVacation != null) || (levelnum == 3 && hisDirectManagetHasVacation != null && hisSecondDirectManagetHasVacation != null))
            {
                vacation.RequestStatusID = "2";

            }

            else
            {

                vacation.RequestStatusID = "1";
            }





            vacation.VacationORWorkHome = false;

            vacation.EmployeeID = (string)HttpContext.Current.Session["LogedUserID"];
            vacation.CreatedAT = DateTime.Now;
            vacation.LastModified = DateTime.Now;





            _context.Request.Add(vacation);
            _context.SaveChanges();



            _SendEmail.SendingEmail("Work from home  request", string.Format(body, "Youxel", "Admin", "" + (string)HttpContext.Current.Session["LogedUserFname"] + "  " + (string)HttpContext.Current.Session["LogedUserLname"] + "   request a  new WFH in     " + vacation.DurationTo + ", navigate to  the system for more details "), lowermanager, adminEmail, adminPass);

            return true;


        }

        /// <summary>
        /// Shows the WFH history.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns></returns>
        public IEnumerable<Request> ShowWfhHistory(string employeeId)
        {

            return _context.Request
              .Where(g =>
                  g.EmployeeID == employeeId && g.VacationORWorkHome == false)
              .ToList();
        }


        /// <summary>
        /// Updates the WFH.
        /// </summary>
        /// <param name="request">The request.</param>
        public void UpdateWfh(Request request)
        {
            Request vacation = new Request();






            vacation.ID = request.ID;




            vacation.DurationTo = request.DurationTo;

            vacation.VacationORWorkHome = false;



            vacation.LastModified = DateTime.Now;


            string comment =
                (string)HttpContext.Current.Session["LogedUserLname"] + "  " + "Comment : " +
                request.Comment + "  ,            ";
            vacation.Comment = comment;





            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            {



                var CreatedAT = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.CreatedAT).FirstOrDefault();
                var EmployeeID = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.EmployeeID).FirstOrDefault();


                var EmployeeEmail = _context.Employee.Where(x => x.ID.Equals(EmployeeID)).Select(x => x.Email).FirstOrDefault();
                var RequestStatusID = _context.Request.Where(x => x.ID.Equals(request.ID)).Select(x => x.RequestStatusID).FirstOrDefault();

                vacation.CreatedAT = CreatedAT;
                vacation.EmployeeID = EmployeeID;
                vacation.RequestStatusID = RequestStatusID;




                _context.RequestApproval.RemoveRange(_context.RequestApproval
                  .Where(g =>
                      g.RequestID == request.ID)
                  .ToList());


                _context.Entry<Request>(vacation)
           .State = EntityState.Modified;









                try
                {

                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

                    var adminEmail =
                        _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

                    var adminPass =
                        _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();








                    _SendEmail.SendingEmail("Vacation  request",
                        string.Format(body, "Youxel", "Admin",
                            "Your work from home  request   has been updated by admin  to be in" + vacation.DurationTo.Value.ToString("yyyy-MM-dd") +
                            ", navigate to the system for more details  "), EmployeeEmail,
                        adminEmail, adminPass);





                    var IDs =
                               _context.RequestApproval.Where(g => g.RequestID == request.ID)
                                   .Select(x => x.ApprovalBy)
                                   .ToList();

                    foreach (string id in IDs)
                    {

                        var email =
                            _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                        _SendEmail.SendingEmail("Vacation  request",
                            string.Format(body, "Youxel", "Admin",
                                "WFH request from " + request.Employee.FName + "  " +
                                request.Employee.LName + "     in    " + request.DurationTo.Value.ToString("yyyy-MM-dd") +
                                " has been  Updated  , navigate to the system for more details  " +
                                request.ID), email, adminEmail, adminPass);
                    }


                    _context.SaveChanges();
                }

                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }




            }



            else
            {




                vacation.EmployeeID = (string)HttpContext.Current.Session["LogedUserID"];


                vacation.RequestStatusID = "1";





                var seenrequests = _context.RequestApproval.Where(x => x.RequestID.Equals(request.ID)).ToList();

                if (seenrequests.Count != 0)
                {


                    var text = "Employee " + (string)HttpContext.Current.Session["LogedUserFname"] + "  " +
                               (string)HttpContext.Current.Session["LogedUserLname"] +
                               "      has updated his Work From Home  request  , request ID :  " + request.ID + "";

                    _notificationRepository.AddNotification(text, request.ID);

                }



                _context.RequestApproval.RemoveRange(_context.RequestApproval
                    .Where(g =>
                        g.RequestID == request.ID)
                    .ToList());












                _context.Entry<Request>(vacation)
                    .State = EntityState.Modified;

                _context.Entry(vacation).Property(x => x.CreatedAT).IsModified = false;



                _context.SaveChanges();


                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

                var adminEmail =
                    _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

                var adminPass =
                    _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();

                var IDs =
                           _context.RequestApproval.Where(g => g.RequestID == request.ID)
                               .Select(x => x.ApprovalBy)
                               .ToList();

                foreach (string id in IDs)
                {
                    var email =
                        _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                    _SendEmail.SendingEmail("Vacation  request",
                        string.Format(body, "Youxel", "Admin",
                            "WFH request from " + request.Employee.FName + "  " +
                            request.Employee.LName + "     in    " + request.DurationTo.Value.ToString("yyyy-MM-dd") +
                            " has been  Updated, navigate to the system for more details  " +
                            request.ID), email, adminEmail, adminPass);
                }

            }
        }



        /// <summary>
        /// Cancels the WFH.
        /// </summary>
        /// <param name="request">The request.</param>
        public void CancelWfh(Request request)


        {




            var canceledrequest = _context.Request.FirstOrDefault(x => x.ID.Equals(request.ID));
            var seenrequests = _context.RequestApproval.Where(x => x.RequestID.Equals(request.ID)).ToList();



            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail =
                _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass =
                _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();


            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            {




                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "Your work from home  request in  " + canceledrequest.DurationTo.Value.ToString("yyyy-MM-dd") +
                        " has been cancled by admin, navigate to the system for more details  "), canceledrequest.Employee.Email,
                    adminEmail, adminPass);
            }



            if (seenrequests.Count != 0)
            {

                var text = "Work from home  request from " + canceledrequest.Employee.FName + "  " +
                           canceledrequest.Employee.LName + " has been canceled    , request ID :  " +
                           canceledrequest.ID;
                _notificationRepository.AddNotification(text, canceledrequest.ID);













                _context.RequestApproval.RemoveRange(_context.RequestApproval
                    .Where(g =>
                        g.RequestID == request.ID)
                    .ToList());



            }


            _context.Request.Remove(_context.Request
                .FirstOrDefault(g => g.ID == request.ID));





            _context.SaveChanges();







            var IDs =
                           _context.RequestApproval.Where(g => g.RequestID == request.ID)
                               .Select(x => x.ApprovalBy)
                               .ToList();

            foreach (string id in IDs)
            {

                var email =
                    _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                _SendEmail.SendingEmail("Vacation  request",
                    string.Format(body, "Youxel", "Admin",
                        "WFH request from " + request.Employee.FName + "  " +
                        request.Employee.LName + "     in    " + request.DurationTo.Value.ToString("yyyy-MM-dd") +
                        " has been  Canceled, navigate to the system for more details  " +
                        request.ID), email, adminEmail, adminPass);
            }

        }




        /// <summary>
        /// Managers the show requests.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns></returns>
        public IEnumerable<Request> ManagerShowRequests(string employeeId)
        {

            if ((string)HttpContext.Current.Session["UserTypeID"] == "2")
            {


                return
                    _context.Request
                        .Where(x => x.VacationORWorkHome.Equals(false))
                        .ToList();

            }




            if ((string)HttpContext.Current.Session["UserTypeID"] == "3")
            {


                return _context.Request.Where(x => x.RequestStatusID == "2" || x.RequestStatusID == "3").Where(x => x.VacationORWorkHome.Equals(false)).ToList();

            }






            var teamId =
               _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.TeamID).ToList();

            int levelnum = /*level el logined user */
               _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.EmployeeID == employeeId).Select(x => x.LevelNum).FirstOrDefault();


            int numlevel = /*level el team's user */
               _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();

            var requestsHaveSeen /*requests which i took decion on it */ =
              _context.RequestApproval.Where(x => x.ApprovalBy == employeeId).Select(x => x.RequestID).ToList();


            var teamMembers /*all members in my teams */ = _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();

            var teamworkflow /*my team members and bottom of me  */ = _context.Workflow.Where(x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum > levelnum).Select(x => x.EmployeeID).ToList();



            var lowermanager /*the first managers bottom of logined user */  =
           _context.Workflow.Where(
               x => teamMembers.Contains(x.EmployeeID) && x.LevelNum != numlevel && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum + 1))
               .Select(x => x.EmployeeID)
               .FirstOrDefault();



            var lowerlowermanager =
            _context.Workflow.Where(
                x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum + 2))
                .Select(x => x.EmployeeID)
                .FirstOrDefault();



            var hisDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
              _context.Request
                  .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                      x.EmployeeID.Equals(lowermanager));



            var hisSecondDirectManagetHasVacation = /*if direct manager for logined user has a vacation*/
           _context.Request
               .FirstOrDefault(x => x.DurationFrom.Value < DateTime.Now && DateTime.Now < x.DurationTo.Value && x.VacationORWorkHome.Equals(true) && x.RequestStatusID.Equals("2") &&
                   x.EmployeeID.Equals(lowerlowermanager));





            if (hisDirectManagetHasVacation != null)
            {


                var firstapproval = _context.Request.Where(x => /*approvalforlastmanager */
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).ToList();


                var used = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).Select(x => x.ID).ToList();



                var lowerlist /*all request from first managers bottom of logined user */ = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID) && lowerlowermanager.Contains(x.EmployeeID)).ToList();




                var /*list of the IDS of  requests approved by all bottom managers*/
                    requestapprovedbybottommanagers =
                        _context.RequestApproval.Where(
                            x =>
                                used.Contains(x.RequestID) && x.Workflow.LevelNum.Equals(levelnum + 2) &&
                                x.StatusID == "2").Select(x => x.RequestID).ToList();


                var /*list of all requests approved by all bottom managers*/
                    totalrequests = _context.Request.Where(x => requestapprovedbybottommanagers.Contains(x.ID));

                var result = Enumerable.Concat(totalrequests, lowerlist);
                /*merge all request by lower manager and all request approved by lower manager */


                if (levelnum == numlevel - 1 || hisSecondDirectManagetHasVacation != null)
                {

                    return firstapproval;

                }

                return result;

            }

            else
            {

                var firstapproval = _context.Request.Where(x => /*approvalforlastmanager */
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).ToList();


                var used = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID)).Select(x => x.ID).ToList();



                var lowerlist /*all request from first managers bottom of logined user */ = _context.Request.Where(x =>
                    teamworkflow.Contains(x.EmployeeID) && x.VacationORWorkHome.Equals(false) &&
                    x.RequestStatusID == "1" &&
                    !requestsHaveSeen.Contains(x.ID) && lowermanager.Contains(x.EmployeeID)).ToList();




                var /*list of the IDS of  requests approved by all bottom managers*/
                    requestapprovedbybottommanagers =
                        _context.RequestApproval.Where(
                            x =>
                                used.Contains(x.RequestID) && x.Workflow.LevelNum.Equals(levelnum + 1) &&
                                x.StatusID == "2").Select(x => x.RequestID).ToList();


                var /*list of all requests approved by all bottom managers*/
                    totalrequests = _context.Request.Where(x => requestapprovedbybottommanagers.Contains(x.ID));

                var result = Enumerable.Concat(totalrequests, lowerlist);
                /*merge all request by lower manager and all request approved by lower manager */


                return levelnum == numlevel - 1 ? firstapproval : result;


            }

        }




        /// <summary>
        /// Approves the vacation.
        /// </summary>
        /// <param name="requestapproval">The requestapproval.</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="requestid">The requestid.</param>
        public void ApproveWfh(RequestApproval requestapproval, string employeeId, string requestid)
        {


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();




            string teamId =
              _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.TeamID).FirstOrDefault();

            int levelnum =
               _context.Workflow.Where(x => x.TeamID == teamId && x.EmployeeID == employeeId).Select(x => x.LevelNum).FirstOrDefault();




            int numlevel = /*level el team's user */
                               _context.Team.Where(x => teamId.Contains(x.ID)).Select(x => x.NumLevel).FirstOrDefault();






            var teamMembers /*all members in my teams */ = _context.Workflow.Where(x => teamId.Contains(x.TeamID)).Select(x => x.EmployeeID).ToList();

            Request acceptedRequest = _context.Request.FirstOrDefault(x => x.ID == requestid);



            var topmanager /*the first managers bottom of logined user */  =
          _context.Workflow.Where(
              x => teamMembers.Contains(x.EmployeeID) && x.LevelNum == 1 && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 1))
              .Select(x => x.EmployeeID)
              .FirstOrDefault();

            var adminhasvacation = _context.Request.Where(x => x.EmployeeID.Equals(topmanager) &&
                         x.DurationFrom.Value < acceptedRequest.DurationTo.Value && acceptedRequest.DurationTo.Value < x.DurationTo.Value).FirstOrDefault();



            if (levelnum == 1 || adminhasvacation != null ||
                Convert.ToString(HttpContext.Current.Session["UserTypeID"]) == "2")
            {


                if (acceptedRequest != null)
                {
                    acceptedRequest.RequestStatusID = "2";






                    acceptedRequest.LastModified = DateTime.Now;


                    _context.Entry(acceptedRequest).Property(x => x.EmployeeID).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.DurationFrom).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.VacationORWorkHome).IsModified = false;
                    _context.Entry(acceptedRequest).Property(x => x.CreatedAT).IsModified = false;
                    _context.Entry(acceptedRequest).State = EntityState.Modified;

                }


                _context.SaveChanges();


                if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) != "2")
                {

                    var text = "Work from home  request from " + acceptedRequest.Employee.FName + "  " +
                               acceptedRequest.Employee.LName +
                               " completely approved by all his  team hierarchy Employee " + "Request ID" +
                               acceptedRequest.ID;


                    _notificationRepository.AddNotification(text, acceptedRequest.ID);


                    var allmanagers /*all managers  in my teams */ =
                                          _context.Workflow.Where(x => teamId.Contains(x.TeamID) && x.LevelNum > levelnum).Select(x => x.EmployeeID).ToList();

                    foreach (string id in allmanagers)
                    {


                        var email = _context.Employee.Where(x => x.ID.Equals(id)).Select(x => x.Email).FirstOrDefault();
                        _SendEmail.SendingEmail("Vacation  request",
                            string.Format(body, "Youxel", "Admin",
                                "WFH request from " + acceptedRequest.Employee.FName + "  " +
                                acceptedRequest.Employee.LName + "  in " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") +

                                " completely approved " +
                                acceptedRequest.ID), email, adminEmail, adminPass);
                    }


                }
                _SendEmail.SendingEmail("Vacation  request", string.Format(body, "Youxel", "Admin", "  your  Work from home  request in  " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") + " has been completely approved, navigate to  the system for more details "), acceptedRequest.Employee.Email, adminEmail, adminPass);
                /*send to user how send vacation that a manger approve his request */



            }


            else
            {






                var lowermanager /*the first managers bottom of logined user */  = _context.Workflow.FirstOrDefault(x => teamMembers.Contains(x.EmployeeID) && x.LevelNum <= numlevel && x.LevelNum.Equals(levelnum - 1));






                _SendEmail.SendingEmail("work from home   request", string.Format(body, "Youxel", "Admin", "" + acceptedRequest.Employee.FName + "      " + acceptedRequest.Employee.LName + "   request a  new WFH in " + acceptedRequest.DurationTo.Value.ToString("yyyy-MM-dd") + "      ,navigate to  the system for more details "), lowermanager.Employee.Email, adminEmail, adminPass);




                _SendEmail.SendingEmail("Vacation  request",
               string.Format(body, "Youxel", "Admin",
                   "" + HttpContext.Current.Session["LogedUserFname"] + "      " +
                   (string)HttpContext.Current.Session["LogedUserLname"] +
                   "  approved your work from home  request ,it is now pending for approval from " + "   " + lowermanager.Employee.FName + "  " + lowermanager.Employee.LName + ",navigate to  the system for more details "),
               acceptedRequest.Employee.Email, adminEmail, adminPass);
                /*send to user how send vacation that a manger approve his request */
            }







            if (Convert.ToString(HttpContext.Current.Session["UserTypeID"]) != "2")
            {


                RequestApproval vacation = new RequestApproval();

                var listofapprovals = _context.RequestApproval.ToList().OrderByDescending(x => x.ID);
                string workflowId =
                    _context.Workflow.Where(x => x.EmployeeID == employeeId).Select(x => x.ID).FirstOrDefault();


                if (!listofapprovals.Any())
                {

                    vacation.ID = "Approval-000001";

                }



                else
                {

                    var originalValue = listofapprovals.Select(x => x.ID).FirstOrDefault();

                    // Get the numeric part (0002)
                    var stringValue = Regex.Match(originalValue, @"\d+").Value;
                    // Convert to int
                    var intValue = Int32.Parse(stringValue);
                    // Increase
                    intValue++;
                    // Convert back to string
                    originalValue = "Approval-" + intValue.ToString("D6");










                    vacation.ID = originalValue;

                }
                vacation.WorkflowID = workflowId;
                vacation.RequestID = requestid;
                vacation.StatusID = "2";
                vacation.Comment = requestapproval.Comment;





                





                vacation.ApprovalBy = (string)HttpContext.Current.Session["LogedUserID"];
                vacation.ApprovalAT = DateTime.Now;
                vacation.LastModified = DateTime.Now;



                _context.RequestApproval.Add(vacation);



                Request requestcomment = _context.Request.FirstOrDefault(x => x.ID == requestid);


                if (requestcomment != null)
                {
                    requestcomment.Comment = requestcomment.Comment +
                                             "   \n " + HttpContext.Current.Session["LogedUserFname"] + "  " +
                                             (string)HttpContext.Current.Session["LogedUserLname"] + "  " +
                                             "Comment : " +
                                             requestapproval.Comment + "  \n ";
                    requestcomment.LastModified = DateTime.Now;


                    _context.Entry(requestcomment).Property(x => x.EmployeeID).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.DurationFrom).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.VacationORWorkHome).IsModified = false;
                    _context.Entry(requestcomment).Property(x => x.CreatedAT).IsModified = false;
                    _context.Entry(requestcomment).State = EntityState.Modified;
                }


                _context.SaveChanges();






            }











        }




        public void RejctWfh(RequestApproval requestapproval, string employeeId, string requestid)
        {


            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

            var adminEmail = _context.Configuration.Where(x => x.ID.Equals("AdminEmail")).Select(x => x.Value).FirstOrDefault();

            var adminPass = _context.Configuration.Where(x => x.ID.Equals("AdminPass")).Select(x => x.Value).FirstOrDefault();

            Request rejectedRequest = _context.Request.FirstOrDefault(x => x.ID == requestid);
            if (rejectedRequest != null)
            {
                rejectedRequest.RequestStatusID = "3";


                rejectedRequest.Comment = rejectedRequest.Comment +
                                          HttpContext.Current.Session["LogedUserFname"] + "  " +
                                          (string)HttpContext.Current.Session["LogedUserLname"] + "Comment: " +
                                          requestapproval.Comment + "";



                rejectedRequest.LastModified = DateTime.Now;


                _context.Entry(rejectedRequest).Property(x => x.EmployeeID).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.DurationFrom).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.VacationORWorkHome).IsModified = false;
                _context.Entry(rejectedRequest).Property(x => x.CreatedAT).IsModified = false;
                _context.Entry(rejectedRequest).State = EntityState.Modified;

                _context.SaveChanges();

                var text = "Work From Home  request from " + rejectedRequest.Employee.FName + "  " +
                              rejectedRequest.Employee.LName +
                              " completely  rejected  by  his  team hierarchy    , request ID :  " +
                              rejectedRequest.ID;

                _notificationRepository.AddNotification(text, rejectedRequest.ID);



                _SendEmail.SendingEmail("Vacation  request", string.Format(body, "Youxel", "Admin", "" + "your WFH request in  " + rejectedRequest.DurationTo + "    has been completely rejected   ,navigate to  the system for more details "), rejectedRequest.Employee.Email, adminEmail, adminPass);


            }
        }

























        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose all resource
        /// </summary>
        /// <param name="disposing">Dispose managed resources check</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }




















    }
}
